/*
 *  C# interface for commanding the Arduinos controlling the TDT sandwiches and recording the data.
 *
 *  NOTE: For best debugging purposes (especially exceptions with code 999), always include the .pdb
 *  file (generated when compiling program) in the same location as the .exe file. This allows the
 *  program to identify the line number when handling exceptions.
 *
 *  created 2017
 *  by Soon Kiat Lau
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.IO.Ports;

namespace Agenator
{
    public partial class AgenatorForm : Form
    {
        // Program settings
        private uint chamberCount_                          = 12;   // Default number of chambers, if no default config file was found
        private const float stepRowHeight                   = 33F;  // Height of the table row for a humidity step
        private static int CONNECTION_CHECK_INTERVAL        = 3000; // How often (ms) to check the connection with the Arduino
        private static int RAMP_INTERVAL                    = 10;   // Time interval between each temporary setpoint during a ramp step (s)
        private const int MAX_COMMAND_ID                    = 99;   // Upper limit of the IDs for commands sent/received
        private const string autosaveFolder_                = "autosaves/"; // Relative folder location to save autosaves
        private const string errorLogFilePath_              = @".\errorLog.txt"; // Relative path to error log file. @ is to escape forward slashes
        private const string commandLogFilePath_            = @".\commandLog.txt"; // Relative path to command log file. @ is to escape forward slashes
        private const string defaultConfigFile_             = "defaultConfig.csv"; // Name of the default configuration file, assumed to be in same directory as program
        private const double HUMIDITY_RESETPID_THRESHOLD    = 0.1;  // Threshold between the setpoint of new step versus previous step that determines whether to reset PID or not.
        public int humidityDecimalPlaces                    = 1;    // Number of decimal places for weighing scale offset
        public int scaleFactorDecimalPlaces                 = 3;    // Number of decimal places for scale factor
        public int scaleOffsetDecimalPlaces                 = 0;    // Number of decimal places for weighing scale offset

        // General vars
        private bool receivedConnectionResponse_            = false;
        private bool arduinoConnected_                      = false;
        private bool controlsUpdating_                      = false;  // Used when performing cross-thread control updating
        private bool autosaving_                            = false;
        private int activeChamberID_;
        private uint autosaveInterval_                      = 5;
        private List<Chamber> chambers;
        private ErrorLogger errorLogger_;
        private CommandLogger commandLogger_;
        private Commands commands_;
        private System.Timers.Timer connectionCheckTimer_   = new System.Timers.Timer();
        private System.Timers.Timer autosaverTimer_         = new System.Timers.Timer();

        // Other threads
        private bool threadsStarted = false;
        private Thread commandOutWorker_;
        private Thread commandInWorker_;
        private Thread errorLogWorker_;
        private Thread commandLogWorker_;

        // Chart vars
        private int chartSpacingMinute_                     = 2;    // Spacing between ticks in the chart (min)
        private const uint CHART_INTERVAL_MAX               = 60;   // Maximum time interval on the x-axis of the graph (min)
        
        // Display messages for the status box
        private const string STATUS_OK = "OK";
        private const string STATUS_CONTROL_DC = "Arduino disconnected";
        private const string STATUS_CHAMBER_DC = "Chamber disconnected";

        // Used for updating the controls for time remaining
        private static TimeSpan OneSecInterval = new TimeSpan(0, 0, 1);

        // Serial port settings. Must match Arduino
        private SerialPort port_;
        private const int baudRate_ = 19200;
        private const Parity parity_ = Parity.None;
        private const int dataBits_ = 8;
        private const StopBits stopBits_ = StopBits.One;
        private const int portTimeout_ = 500; // Timeout for communication with Arduino, in ms

        // Delegate functions for thread-safe control updating
        private delegate void deleteControlCallback(Control refControl);
        private delegate void refreshStepsCallback(int chamberID);
        private delegate void suspendLayoutControlCallback(Control refControl);
        private delegate void resumeLayoutControlCallback(Control refControl);
        private delegate void updateFlowVisibilityCallback(FlowLayoutPanel flowControl, bool visible);
        private delegate void updateTextboxCallback(TextBox boxControl, string data);
        private delegate void updateTextBoxEnabledCallback(TextBox textBoxControl, bool enabled);
        private delegate void updateNumericValueCallback(NumericUpDown refControl, decimal data);
        private delegate void updateNumericEnabledCallback(NumericUpDown numericControl, bool enabled);
        private delegate void updateButtonEnabledCallback(Button buttonControl, bool enabled);
        private delegate void updateButtonVisibilityCallback(Button buttonControl, bool visible);
        private delegate void updateLabelVisibilityCallback(Label labelControl, bool visible);
        private delegate void changeControlBackColorCallback(Control refControl, Color color);
        private delegate void changeControlForeColorCallback(Control refControl, Color color);
        private delegate void changeTextboxFontColorCallback(TextBox textBoxControl, Color color);
        private delegate void removeChartPointCallback(Chart chartControl, string seriesName, int pointIndex);
        private delegate void addChartXYCallback(Chart chartControl, string seriesName, double xVal, double yVal);
        private delegate void updateChartPointEmptyCallback(Chart chartControl, string seriesName, int pointIndex, bool empty);
        private delegate void redrawDisplaychartCallback(Chart chartControl);

        public AgenatorForm()
        {
            InitializeComponent();
            Graph_interval_value.Maximum = CHART_INTERVAL_MAX;
            Data_DAQ_interval_value.MouseWheel += chill_MouseWheel;
            WeighScale_flow_cal_value.MouseWheel += chill_MouseWheel;
            Advanced_humidity_RH1_std_value.MouseWheel += chill_MouseWheel;
            Advanced_humidity_RH2_std_value.MouseWheel += chill_MouseWheel;
            errorLogger_ = new ErrorLogger(errorLogFilePath_);
            commandLogger_ = new CommandLogger(commandLogFilePath_, errorLogger_);


            ////////////////////////////////////
            // Initialize the chamber classes //
            ////////////////////////////////////
            chambers = new List<Chamber>();

            for (int i = 0; i < chamberCount_; i++)
            {
                chambers.Add(new Chamber(i, CHART_INTERVAL_MAX));

                // Create dummy steps for each chamber
                chambers[i].steps.Add(new HumidityStep(0, HumidityStep.stepType.TYPE_HOLD, 40, 0, 0, 0, 0, 0, HumidityStep.stepStatus.STATUS_WAITING));
            }

            //Chart.Series[0].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Primary;
            //Chart.Series[1].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;

            // Initialize connection with Arduino
            initArduinoConnection();

            // Find for a default config file and load it if it exists
            bool defaultConfigExists = false;

            try
            {
                FileStream openConfigStream = new FileStream(defaultConfigFile_, FileMode.Open, FileAccess.Read, FileShare.None);
                defaultConfigExists = true;
                openConfigStream.Close();
            }
            catch (Exception)
            {
                defaultConfigExists = false;
            }

            if (defaultConfigExists)
            {
                // Note that loadConfigFile sets activeChamberId_ to the id of the first chamber
                loadConfigFile(false, false, defaultConfigFile_);
            }
            else
            {
                // Choose the first chamber to display upon opening the program
                activeChamberID_ = 0;
            }

            updateForm(activeChamberID_);

            // Now check the connection of each chamber to the Arduino.
            // Do not need to explicitly handle the results of this step because the response sent back from the control
            // board will be handled by handleCommandStatuses().
            if (arduinoConnected_)
            {
                // Get status on chamber connections
                commands_.queueCommandConnectionChamber();
            }
            else
            {
                // Inform user that Arduino was not detected
                MessageBox.Show("The Arduino was not detected by the computer. Please reconnect the USB cable to the computer and restart the program.\n\n If that doesn't work, try restarting the computer.\n\n If that still doesn't work, contact Kiat at Lskiat@huskers.unl.edu", "Arduino not detected");
            }
        }

        private void initArduinoConnection()
        {
            // Close port, if opened
            if (port_ != null && port_.IsOpen)
            {
                port_.Close();

                // From documentation for SerialPort Close() function:
                // "The best practice for any application is to wait for some amount of time after calling
                // the Close method before attempting to call the Open method, as the port may not be closed instantly."
                System.Threading.Thread.Sleep(5000);
            }

            ////////////////////////////////////////////////////
            // Get the port at which the Arduino is connected //
            ////////////////////////////////////////////////////
            arduinoConnected_ = false;

            // Combination of https://stackoverflow.com/a/46683622 and https://stackoverflow.com/a/6017027
            // Selecting from Win32_PnPEntity returns ALL of plug n play devices (as opposed to Win32_SerialPort);
            // adding restriction for Name such as '(COM' narrows down to serial devices
            ManagementObjectSearcher arduinoSearch = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name like '%Arduino Uno (COM%'");
            ManagementObjectCollection arduinoList = arduinoSearch.Get();

            // If at least one Arduino is found, check if it's the right one via SERIAL_REPLY_CONNECTION
            if (arduinoList.Count > 0)
            {
                foreach (ManagementObject arduino in arduinoList)
                {
                    string caption = arduino["Caption"].ToString();
                    int COMStart = caption.IndexOf("(COM") + 1;  // +1 to go past the opening bracket
                    int COMEnd = caption.IndexOf(")", COMStart);
                    string portName = caption.Substring(COMStart, COMEnd - COMStart);
                    port_ = new SerialPort(portName, baudRate_, parity_, dataBits_, stopBits_);
                    port_.Handshake = Handshake.None;
                    port_.Encoding = System.Text.Encoding.ASCII;
                    port_.ReadTimeout = portTimeout_;
                    port_.Open();
                    
                    // Initialize command handler because port_ is set to a new instance
                    commands_ = new Commands(MAX_COMMAND_ID, commandLogFilePath_, this, errorLogger_, commandLogger_, port_,
                                                    handleArduinoConnection,
                                                    handleChamberConnection,
                                                    updateScaleFactor,
                                                    updateScaleOffset,
                                                    updateReadings,
                                                    startControl,
                                                    updateControlsIdleSuccess,
                                                    updateControlsIdleFail,
                                                    updateControlsDAQSuccess,
                                                    updateControlsDAQFail,
                                                    updateControlsControlSuccess,
                                                    updateControlsControlFail,
                                                    updateControlsSetpointSuccess,
                                                    updateControlsSetpointFail,
                                                    updateControlsRHCalibrateSuccess,
                                                    updateControlsRHCalibrateFail,
                                                    updateControlsScaleTareSuccess,
                                                    updateControlsScaleTareFail,
                                                    updateControlsScaleCalibrateSuccess,
                                                    updateControlsScaleCalibrateFail);

                    // This should be caught by Arduino as a SERIAL_CMD_CONNECTION_CONTROL command
                    // The chamber should respond with SERIAL_REPLY_CONNECTION if it's the correct Arduino
                    // Also note that if this jumpstarts the loop for checking the connection because the event handler
                    // for elapsing connection check intervals calls sendCommandConnectionControl, thus forming a loop
                    commands_.sendCommandConnectionArduino(false);

                    // Give time for Arduino to respond
                    System.Threading.Thread.Sleep(200);

                    // Clear all serial port buffers (to prepare for the second call to sendCommandConnectionControl())
                    port_.DiscardInBuffer();
                    port_.DiscardOutBuffer();

                    // Strangely, the first sent command ocassionally doesn't get a response. Forcing a second command
                    // would get a response though
                    commands_.sendCommandConnectionArduino(false);

                    try
                    {
                        // Since we set ReadTimeout to a finite number, ReadTo will return a TimeoutException if no response is received within
                        // that time limit. This is done on purpose because the thread would be blocked forever by ReadLine if ReadTimeout is not a
                        // finite number. If no response is received after the timeout or the response isn't equal to REPLY_WHO_YES, then we assume
                        // that this port is not occupied by the Arduino controlling the chambers.
                        //
                        // Also, trial and error showed that even if the Arduino is the correct one, we still need to sleep for a while before a reply is received.
                        System.Threading.Thread.Sleep(200); // Increase this if Arduino does not respond quick enough
                        string readBuffer = port_.ReadTo(Commands.SERIAL_REPLY_EOL);

                        // Check if response from Arduino follows the standardized format
                        if (readBuffer.Substring(0, 1) == Commands.SERIAL_REPLY_START
                            && readBuffer.Substring(1, 1) == Commands.SERIAL_REPLY_CONNECTION
                            && readBuffer.Substring(2, 1) == Commands.SERIAL_REPLY_END)
                        {
                            // Identified the port at which the Arduino is located. Keep the port as it is.
                            receivedConnectionResponse_ = true;
                            arduinoConnected_ = true;

                            // Clear all serial port buffers (just in case for the double call to sendCommandConnectionControl())
                            port_.DiscardInBuffer();
                            port_.DiscardOutBuffer();

                            break;
                        }
                        else
                        {
                            port_.Close();
                        }
                    }
                    catch (TimeoutException)
                    {
                        port_.Close();
                    }
                }
            }

            if (arduinoConnected_)
            {   // Port is already open from the scouting routine. So now just add the received event handler to this port.
                port_.DataReceived += new SerialDataReceivedEventHandler(commands_.handleSerialDataReceived);

                // Assign each thread and start them
                commandLogWorker_ = new Thread(commandLogger_.logCommands);
                errorLogWorker_ = new Thread(errorLogger_.logErrors);
                commandInWorker_ = new Thread(commands_.handleCommandStatuses);
                commandOutWorker_ = new Thread(commands_.sendCommands);
                commandLogWorker_.Start();
                errorLogWorker_.Start();
                commandInWorker_.Start();
                commandOutWorker_.Start();
                threadsStarted = true;

                // Set up timer for checking connection with Arduino
                connectionCheckTimer_.Interval = CONNECTION_CHECK_INTERVAL;
                connectionCheckTimer_.AutoReset = true;
                connectionCheckTimer_.Elapsed += connectionCheckTimerElapsed;
                connectionCheckTimer_.Start();
            }
        }

        // Resets Arduino by sending the reset signal thru DTR pin
        private void resetArduino()
        {
            // Stop the connection timer first, because we are resetting Arduino
            connectionCheckTimer_.Stop();

            // Log this as attempt to reset Arduino
            errorLogger_.logCProgError(001, "resetArduino");

            // Tell Arduino to stop sending serial messages. Note that this will be kinda
            // useless if we really lost connection with Arduino, but on the off chance that we are actually
            // still connected but somehow this function is called, this ensures the serial line can be put
            // to idle state.
            commands_.sendCommandDisconnect(true);

            // Pause sending out commands
            commands_.pauseCommandOut();

            // Clear the outgoing command queue
            commands_.clearCommandOut();

            // Wait until we have handled all incoming command responses
            while (commands_.getCommandInTotal() > 0)
            {
                System.Threading.Thread.Sleep(10);
            }

            // Reset Arduino
            port_.DtrEnable = true;

            // Sleep for a while to ensure Arduino resets
            System.Threading.Thread.Sleep(300);

            // Clear serial buffers
            port_.DiscardInBuffer();
            port_.DiscardOutBuffer();

            // Now allow Arduino to start up
            port_.DtrEnable = false;

            // Sleep for a while to give Arduino time to load up
            System.Threading.Thread.Sleep(5000);

            // Send connection check for Arduino
            commands_.sendCommandConnectionArduino(true);
            
            // Give time for response
            System.Threading.Thread.Sleep(100);

            // Send another connection check for Arduino just to ensure the serial line is cleaned of any garbage
            commands_.sendCommandConnectionArduino(true);

            // Give time for response
            System.Threading.Thread.Sleep(100);

            // Send connection check for chambers. Upon receiving notification that chamber
            // is connected, the program will call handleChamberConnection() to resume chamber operations.
            commands_.queueCommandConnectionChamber();

            // Resume sending out commands. This is necessary for the chamber connection check to be sent out
            commands_.resumeCommandOut();

            // Resume connection check timer
            connectionCheckTimer_.Start();
        }

        // Update the controls to reflect the currently selected chamber
        private void updateForm(int selectedChamberID)
        {
            controlsUpdating_ = true;

            this.Other.SuspendLayout();
            this.WeighScale.SuspendLayout();
            this.WeighScale_flow.SuspendLayout();
            this.Graph.SuspendLayout();
            this.Graph_flow.SuspendLayout();
            this.Data_save.SuspendLayout();
            this.Data_save_flow.SuspendLayout();
            this.Readings.SuspendLayout();
            this.Readings_humidity.SuspendLayout();
            this.Readings_humidity_flow.SuspendLayout();
            this.Readings_temperature.SuspendLayout();
            this.Readings_temperature_flow.SuspendLayout();
            this.Readings_weight.SuspendLayout();
            this.Readings_weight_flow.SuspendLayout();
            this.Readings_velocity.SuspendLayout();
            this.Readings_velocity_flow.SuspendLayout();
            this.Chamber_flow.SuspendLayout();
            this.Body_panel_top.SuspendLayout();
            this.HumidityControl.SuspendLayout();
            this.HumidityControl_heading.SuspendLayout();
            this.SuspendLayout();


            // The list of chambers were implicitly sorted during initialization of the list, so we can access the correct chamber directly

            if (selectedChamberID == 0)
            {
                Chamber_prev.Enabled = false;
            }
            else
            {
                Chamber_prev.Enabled = true;
            }

            Chamber_title.Text = "Chamber " + Convert.ToString(selectedChamberID + 1);

            if (selectedChamberID == (chamberCount_ - 1))
            {
                Chamber_next.Enabled = false;
            }
            else
            {
                Chamber_next.Enabled = true;
            }


            // Chart
            try
            {
                Displaychart.Series["Relative humidity"].Points.Clear();
                Displaychart.Series["Weight"].Points.Clear();

                if (chambers[selectedChamberID].plottedPoints <= 0)
                {
                    Displaychart.Series["Relative humidity"].Points.AddXY(0, 0);
                    Displaychart.Series["Relative humidity"].Points[0].IsEmpty = true;

                    Displaychart.Series["Weight"].Points.AddXY(0, 0);
                    Displaychart.Series["Weight"].Points[0].IsEmpty = true;
                }
                else
                {
                    for (int i = 0; i < chambers[selectedChamberID].plottedPoints; i++)
                    {
                        Displaychart.Series["Relative humidity"].Points.AddXY(chambers[selectedChamberID].chartTime[i], chambers[selectedChamberID].chartRH[i]);
                        Displaychart.Series["Weight"].Points.AddXY(chambers[selectedChamberID].chartTime[i], chambers[selectedChamberID].chartWeight[i]);

                        // Check if the value is close to -100000, which is considered as a datapoint with error
                        if (Math.Abs(chambers[selectedChamberID].chartRH[i] + 100000) < 0.001)
                        {
                            Displaychart.Series["Relative humidity"].Points[i].IsEmpty = true;
                        }

                        if (Math.Abs(chambers[selectedChamberID].chartWeight[i] + 100000) < 0.001)
                        {
                            Displaychart.Series["Weight"].Points[i].IsEmpty = true;
                        }
                    }
                }

                redrawDisplaychart(Displaychart);
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);
            }

            /*******************************************************************
            * Middle section (DAQ, readings, record, weighing scale functions)
            *******************************************************************/
            // Readings
            updateTextBox(Readings_humidity_flow_value, Convert.ToString(chambers[selectedChamberID].humidity));
            updateTextBox(Readings_temperature_flow_value, Convert.ToString(chambers[selectedChamberID].temperature));
            updateTextBox(Readings_weight_flow_value, Convert.ToString(chambers[selectedChamberID].weight));
            updateTextBox(Readings_velocity_flow_value, Convert.ToString(chambers[selectedChamberID].velocity));
            updateTextBox(Readings_fan1Speed_flow_value, Convert.ToString(chambers[selectedChamberID].fan1Speed));
            updateTextBox(Readings_fan2Speed_flow_value, Convert.ToString(chambers[selectedChamberID].fan2Speed));
            Data_save_flow_path.Text = chambers[selectedChamberID].recordFilePath;

            // If the event handler wasn't removed beforehand, then the manual change of value would trigger the handler
            Data_DAQ_interval_value.ValueChanged -= DAQ_interval_value_ValueChanged;
            Data_DAQ_interval_value.Value = chambers[selectedChamberID].DAQInterval;
            Data_DAQ_interval_value.ValueChanged += DAQ_interval_value_ValueChanged;

            // If the event handler wasn't removed beforehand, then the manual change of value would trigger the handler
            WeighScale_flow_cal_value.ValueChanged -= OtherFunc_scale_flow_calValue_ValueChanged;
            WeighScale_flow_cal_value.Value = chambers[selectedChamberID].calibrationWeight;
            WeighScale_flow_cal_value.ValueChanged += OtherFunc_scale_flow_calValue_ValueChanged;

            // Scale factor
            updateNumericValue(Advanced_weigh_factor_value, Convert.ToDecimal(Math.Truncate(chambers[selectedChamberID].scaleFactor * Math.Pow(10, scaleFactorDecimalPlaces)) / Math.Pow(10, scaleFactorDecimalPlaces)));
            updateNumericValue(Advanced_weigh_offset_value, Convert.ToDecimal(Math.Truncate(chambers[selectedChamberID].scaleOffset * Math.Pow(10, scaleOffsetDecimalPlaces)) / Math.Pow(10, scaleOffsetDecimalPlaces)));

            Advanced_humidity_RH1_raw_value.Value = Convert.ToDecimal(Math.Truncate(chambers[selectedChamberID].RHRaw1 * Math.Pow(10, humidityDecimalPlaces)) / Math.Pow(10, humidityDecimalPlaces));
            Advanced_humidity_RH1_std_value.Value = Convert.ToDecimal(Math.Truncate(chambers[selectedChamberID].RHStd1 * Math.Pow(10, humidityDecimalPlaces)) / Math.Pow(10, humidityDecimalPlaces));
            Advanced_humidity_RH2_raw_value.Value = Convert.ToDecimal(Math.Truncate(chambers[selectedChamberID].RHRaw2 * Math.Pow(10, humidityDecimalPlaces)) / Math.Pow(10, humidityDecimalPlaces));
            Advanced_humidity_RH2_std_value.Value = Convert.ToDecimal(Math.Truncate(chambers[selectedChamberID].RHStd2 * Math.Pow(10, humidityDecimalPlaces)) / Math.Pow(10, humidityDecimalPlaces));

            // Modify appearance of the various sections depending on the state of the chamber
            if (chambers[selectedChamberID].DAQRunning)
            {
                // DAQ
                Data_DAQ_interval_value.Enabled = false;

                Data_DAQ_start.Enabled = false;
                Data_DAQ_start.Visible = false;

                changeControlBackColor(Data_DAQ_flow, Color.LightGreen);

                // Recording
                if (chambers[selectedChamberID].recording)
                {
                    Data_save_flow_browse.Enabled = false;

                    Data_save_flow_start.Visible = false;
                    Data_save_flow_start.Enabled = false;

                    Data_save_flow_stop.Visible = true;
                    Data_save_flow_stop.Enabled = true;

                    changeControlBackColor(Data_save_flow, Color.LightGreen);
                }
                else
                {
                    Data_save_flow_browse.Enabled = true;

                    Data_save_flow_start.Visible = true;
                    Data_save_flow_start.Enabled = true;

                    Data_save_flow_stop.Visible = false;
                    Data_save_flow_stop.Enabled = false;

                    changeControlBackColor(Data_save_flow, SystemColors.Control);
                }


                if (chambers[selectedChamberID].controlRunning)
                {
                    // DAQ
                    Data_DAQ_stop.Enabled = false;
                    Data_DAQ_stop.Visible = true;

                    // Weighing functions
                    WeighScale_flow_tare.Enabled = false;
                    WeighScale_flow_cal_value.Enabled = false;
                    WeighScale_flow_cal_button.Enabled = false;

                    // Humidity sensor calibration
                    Advanced_humidity_RH1_std_apply.Enabled = false;
                    Advanced_humidity_RH1_std_value.Enabled = false;
                    Advanced_humidity_RH2_std_apply.Enabled = false;
                    Advanced_humidity_RH2_std_value.Enabled = false;

                    // Control
                    if (chambers[selectedChamberID].pauseControlWaiting || chambers[selectedChamberID].endControlWaiting)
                    {// If waiting for pause control or end contorl command, disable pause control button
                        HumidityControl_pause.Enabled = false;
                    }
                    else
                    {
                        HumidityControl_pause.Enabled = true;
                    }
                    
                    HumidityControl_pause.Visible = true;

                    HumidityControl_start.Enabled = false;
                    HumidityControl_start.Visible = false;

                    changeControlBackColor(HumidityControl, Color.LightGreen);
                }
                else
                {
                    // DAQ
                    if (chambers[selectedChamberID].stopDAQWaiting)
                    {// If waiting for stop DAQ command, disable stop DAQ button
                        Data_DAQ_stop.Enabled = false;
                    }
                    else
                    {
                        Data_DAQ_stop.Enabled = true;
                    }

                    Data_DAQ_stop.Visible = true;

                    // Weighing scale functions
                    if (chambers[selectedChamberID].scaleTareWaiting)
                    {// If waiting for tare command, disable tare button
                        WeighScale_flow_tare.Enabled = false;
                    }
                    else
                    {
                        WeighScale_flow_tare.Enabled = true;
                    }

                    if (chambers[selectedChamberID].scaleCalibrateWaiting)
                    {// If waiting for calibrate command, disable calibration controls
                        WeighScale_flow_cal_value.Enabled = false;
                        WeighScale_flow_cal_button.Enabled = false;
                    }
                    else
                    {
                        WeighScale_flow_cal_value.Enabled = true;
                        WeighScale_flow_cal_button.Enabled = true;
                    }

                    if (chambers[selectedChamberID].startControlWaiting)
                    {// If waiting for start control command, disable start control and other sections
                        HumidityControl_start.Enabled       = false;
                        Data_DAQ_stop.Enabled               = false;

                        // Weighing functions
                        WeighScale_flow_tare.Enabled       = false;
                        WeighScale_flow_cal_value.Enabled  = false;
                        WeighScale_flow_cal_button.Enabled = false;
                    }
                    else
                    {
                        HumidityControl_start.Enabled = true;
                    }

                    // Humidity sensor calibration
                    if (chambers[selectedChamberID].RH1CalibrateWaiting || chambers[selectedChamberID].RH2CalibrateWaiting)
                    {
                        Advanced_humidity_RH1_std_apply.Enabled = false;
                        Advanced_humidity_RH1_std_value.Enabled = false;
                        Advanced_humidity_RH2_std_apply.Enabled = false;
                        Advanced_humidity_RH2_std_value.Enabled = false;
                    }
                    else
                    {
                        Advanced_humidity_RH1_std_apply.Enabled = true;
                        Advanced_humidity_RH1_std_value.Enabled = true;
                        Advanced_humidity_RH2_std_apply.Enabled = true;
                        Advanced_humidity_RH2_std_value.Enabled = true;
                    }

                    // Control
                    HumidityControl_start.Visible = true;
                    HumidityControl_pause.Enabled = false;
                    HumidityControl_pause.Visible = false;

                    changeControlBackColor(HumidityControl, SystemColors.Control);
                }
            }
            else
            {
                // DAQ
                Data_DAQ_stop.Enabled = false;
                Data_DAQ_stop.Visible = false;
                Data_DAQ_start.Visible = true;
                changeControlBackColor(Data_DAQ_flow, SystemColors.Control);

                if (chambers[selectedChamberID].startDAQWaiting)
                {// Waiting for the start DAQ command to complete, so disable some controls
                    Data_DAQ_interval_value.Enabled = false;
                    Data_DAQ_start.Enabled = false;
                }
                else
                {
                    Data_DAQ_interval_value.Enabled = true;
                    Data_DAQ_start.Enabled = true;
                }

                // Recording
                Data_save_flow_browse.Enabled = true;

                Data_save_flow_start.Visible = true;
                Data_save_flow_start.Enabled = false;

                Data_save_flow_stop.Visible = false;
                Data_save_flow_stop.Enabled = false;

                changeControlBackColor(Data_save_flow, SystemColors.Control);

                // Weighing functions
                WeighScale_flow_tare.Enabled = false;
                WeighScale_flow_cal_value.Enabled = true;
                WeighScale_flow_cal_button.Enabled = false;

                // Humidity sensor calibration
                Advanced_humidity_RH1_std_apply.Enabled = false;
                Advanced_humidity_RH1_std_value.Enabled = true;
                Advanced_humidity_RH2_std_apply.Enabled = false;
                Advanced_humidity_RH2_std_value.Enabled = true;

                // Control
                HumidityControl_start.Visible = true;
                HumidityControl_start.Enabled = false;

                HumidityControl_pause.Visible = false;
                HumidityControl_pause.Enabled = false;

                changeControlBackColor(HumidityControl, SystemColors.Control);
            }

            // Update controls to reflect connection status
            if (!chambers[selectedChamberID].chamberConnected || !arduinoConnected_)
            {// Disable the appropriate buttons (updateControlsChamberDisconnect() will also update the status textbox)
                updateControlsChamberDisconnect(selectedChamberID);
            }
            else
            {// Simply update the status textbox
                updateConnectionStatusTextbox(selectedChamberID);
            }

            // Refresh the list of humidity control steps
            refreshSteps(selectedChamberID);

            // Restart controls updating because refreshSteps killed it
            controlsUpdating_ = true;

            this.Other.ResumeLayout(false);
            this.WeighScale.ResumeLayout(false);
            this.WeighScale.PerformLayout();
            this.WeighScale_flow.ResumeLayout(false);
            this.WeighScale_flow.PerformLayout();
            this.Graph.ResumeLayout(false);
            this.Graph.PerformLayout();
            this.Graph_flow.ResumeLayout(false);
            this.Graph_flow.PerformLayout();
            this.Data_save.ResumeLayout(false);
            this.Data_save.PerformLayout();
            this.Data_save_flow.ResumeLayout(false);
            this.Data_save_flow.PerformLayout();
            this.Readings.ResumeLayout(false);
            this.Readings_humidity.ResumeLayout(false);
            this.Readings_humidity.PerformLayout();
            this.Readings_humidity_flow.ResumeLayout(false);
            this.Readings_humidity_flow.PerformLayout();
            this.Readings_temperature.ResumeLayout(false);
            this.Readings_temperature.PerformLayout();
            this.Readings_temperature_flow.ResumeLayout(false);
            this.Readings_temperature_flow.PerformLayout();
            this.Readings_weight.ResumeLayout(false);
            this.Readings_weight.PerformLayout();
            this.Readings_weight_flow.ResumeLayout(false);
            this.Readings_weight_flow.PerformLayout();
            this.Readings_velocity.ResumeLayout(false);
            this.Readings_velocity.PerformLayout();
            this.Readings_velocity_flow.ResumeLayout(false);
            this.Readings_velocity_flow.PerformLayout();
            this.Chamber_flow.ResumeLayout(false);
            this.Chamber_flow.PerformLayout();
            this.Body_panel_top.ResumeLayout(false);
            this.Body_panel_top.PerformLayout();
            this.HumidityControl.ResumeLayout(false);
            this.HumidityControl.PerformLayout();
            this.HumidityControl_heading.ResumeLayout(false);
            this.HumidityControl_heading.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            // Give time for all the form controls to load up.
            System.Threading.Thread.Sleep(10);
            controlsUpdating_ = false;
        }

        // This repaints the controls for the humidity control steps section.
        // It accounts for whether the control for the chamber actively displayed is running or not.
        //
        // A modified appearance will be applied onto all steps that were in "Completed" status. The ordering buttons for the completed and running steps will be disabled.
        // If a step was marked as completed but it comes after the determined running step (referred to as "completed but not-yet-reached" steps), they will
        // have the same appearance as waiting steps. When reaching that step, it will be instantly pushed to the completed stack anyway.
        // Delete buttons are removed for completed steps. Delete buttons are replaced by skip buttons for running, waiting, and "completed but not-yet-reached" steps.
        private void refreshSteps(int chamberID)
        {
            if (chamberID == activeChamberID_)
            {
                if (HumidityControl_steps.InvokeRequired)
                {
                    refreshStepsCallback d = new refreshStepsCallback(refreshSteps);
                    this.Invoke(d, new object[] { chamberID });
                }
                else
                {
                    controlsUpdating_ = true;

                    // Removing rows from TableLayoutPanel causes a lot of problems. Better to just recreate the entire table
                    HumidityControl_steps.SuspendLayout();
                    deleteControl(HumidityControl_steps_table);
                    HumidityControl_steps.ResumeLayout();
                    HumidityControl_steps_table = new System.Windows.Forms.TableLayoutPanel();
                    HumidityControl_steps.Controls.Add(HumidityControl_steps_table);
                    HumidityControl_steps.SuspendLayout();
                    HumidityControl_steps_table.SuspendLayout();


                    HumidityControl_steps_table.Anchor = System.Windows.Forms.AnchorStyles.Top;
                    HumidityControl_steps_table.AutoSize = true;
                    HumidityControl_steps_table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                    HumidityControl_steps_table.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
                    HumidityControl_steps_table.ColumnCount = 1;
                    HumidityControl_steps_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
                    HumidityControl_steps_table.Location = new System.Drawing.Point(0, 0);
                    HumidityControl_steps_table.Margin = new System.Windows.Forms.Padding(0);
                    HumidityControl_steps_table.Name = "HumidityControl_steps_table";
                    HumidityControl_steps_table.Size = new System.Drawing.Size(1116, 33);
                    HumidityControl_steps_table.TabIndex = 4;



                    // Prepare to add the steps
                    chambers[chamberID].sortSteps();
                    HumidityControl_steps_table.RowCount = 0;

                    for (int i = 0; i < chambers[chamberID].steps.Count; i++)
                    {
                        UInt16 stepID = Convert.ToUInt16(i);

                        HumidityStep_Control createdStep = new HumidityStep_Control(stepID,
                                                                                    chambers[chamberID].activeStep,
                                                                                    chambers[chamberID].steps.Count,
                                                                                    chambers[chamberID].controlRunning,
                                                                                    chambers[chamberID].controlPaused,
                                                                                    chambers[chamberID].skipControlStepWaiting,
                                                                                    chambers[chamberID].nextControlStepWaiting,
                                                                                    chambers[chamberID].pauseControlWaiting,
                                                                                    chambers[chamberID].endControlWaiting,
                                                                                    chambers[chamberID].steps[i].type,
                                                                                    chambers[chamberID].steps[i].targetHumidity,
                                                                                    chambers[chamberID].steps[i].timeRemaining.Days,
                                                                                    chambers[chamberID].steps[i].timeRemaining.Hours,
                                                                                    chambers[chamberID].steps[i].timeRemaining.Minutes,
                                                                                    chambers[chamberID].steps[i].timeRemaining.Seconds,
                                                                                    chambers[chamberID].steps[i].targetFanSpeed,
                                                                                    chambers[chamberID].steps[i].status);

                        // Assign the duration remaining controls that will be updated everytime the timer elapsed event is raised
                        chambers[chamberID].steps[i].dayRemainingControl = createdStep.duration_day_value;
                        chambers[chamberID].steps[i].hourRemainingControl = createdStep.duration_hour_value;
                        chambers[chamberID].steps[i].minuteRemainingControl = createdStep.duration_minute_value;
                        chambers[chamberID].steps[i].secondRemainingControl = createdStep.duration_second_value;

                        // Assign event handlers
                        createdStep.order_up.Click += (sender, e) => { changeStepOrder_Click(sender, e, stepID, true); };
                        createdStep.order_down.Click += (sender, e) => { changeStepOrder_Click(sender, e, stepID, false); };
                        createdStep.stepType.SelectedIndexChanged += (sender, e) => { stepType_SelectedIndexChanged(sender, e, stepID); };
                        createdStep.humidity.ValueChanged += (sender, e) => { targetHumidity_ValueChanged(sender, e, stepID); };
                        createdStep.duration_day_value.ValueChanged += (sender, e) => { duration_day_value_ValueChanged(sender, e, stepID); };
                        createdStep.duration_hour_value.ValueChanged += (sender, e) => { duration_hour_value_ValueChanged(sender, e, stepID); };
                        createdStep.duration_minute_value.ValueChanged += (sender, e) => { duration_minute_value_ValueChanged(sender, e, stepID); };
                        createdStep.duration_second_value.ValueChanged += (sender, e) => { duration_second_value_ValueChanged(sender, e, stepID); };
                        createdStep.fanSpeed.ValueChanged += (sender, e) => { targetVelocity_ValueChanged(sender, e, stepID); };
                        createdStep.stepStatus.SelectedIndexChanged += (sender, e) => { stepStatus_SelectedIndexChanged(sender, e, stepID); };
                        createdStep.deleteButton.Click += (sender, e) => { deleteButton_Click(sender, e, stepID); };
                        createdStep.skipButton.Click += (sender, e) => { skipButton_Click(sender, e, stepID); };

                        HumidityControl_steps_table.RowCount++;
                        HumidityControl_steps_table.RowStyles.Add(new RowStyle(SizeType.Absolute, stepRowHeight));
                        HumidityControl_steps_table.Controls.Add(createdStep, 0, i);
                    }

                    HumidityControl_steps_table.Size = new System.Drawing.Size(HumidityControl_steps_table.Size.Width, Convert.ToInt16((stepRowHeight + 2) * HumidityControl_steps_table.RowCount));

                    HumidityControl_steps_table.ResumeLayout(false);
                    HumidityControl_steps_table.PerformLayout();
                    HumidityControl_steps.ResumeLayout(false);
                    HumidityControl_steps.PerformLayout();

                    controlsUpdating_ = false;
                }
            }
        }

        private void deleteControl(Control refControl)
        {
            if (refControl.InvokeRequired)
            {
                deleteControlCallback d = new deleteControlCallback(deleteControl);
                this.Invoke(d, new object[] { refControl });
            }
            else
            {
                controlsUpdating_ = true;

                if (!refControl.HasChildren)
                {
                    refControl.Dispose();
                }
                else
                {
                    refControl.SuspendLayout();

                    for (int i = (refControl.Controls.Count - 1); i >= 0; i--)
                    {
                        deleteControl(refControl.Controls[i]);
                    }

                    refControl.ResumeLayout();
                    refControl.Dispose();
                }

                controlsUpdating_ = false;
            }
        }

        // Save the current program state into a config file
        private void saveConfigFile(string filePath, bool manualSave)
        {
            try
            {
                FileStream saveConfigStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                StreamWriter saveConfigWriter = new StreamWriter(saveConfigStream);

                foreach (Chamber chamber in chambers)
                {
                    // Settings for a chamber
                    string configuration = "";
                    configuration += Convert.ToString(chamber.id);                               // chamber id
                    configuration += ",";
                    configuration += "TRUE";                                            // writing configuration for chamber
                    configuration += ",";
                    configuration += Convert.ToString(chamber.DAQRunning);          // DAQ running?
                    configuration += ",";
                    configuration += Convert.ToString(chamber.recording);           // Recording data?
                    configuration += ",";
                    configuration += Convert.ToString(chamber.controlRunning);      // Control running?
                    configuration += ",";
                    configuration += Convert.ToString(chamber.DAQInterval);         // interval between each DAQ
                    configuration += ",";
                    configuration += Convert.ToString(chamber.recordFilePath);      // recording file path
                    configuration += ",";
                    configuration += Convert.ToString(chamber.calibrationWeight);   // Calibration weight
                    configuration += ",";
                    configuration += Convert.ToString(chamber.scaleOffset);         // Scale offset
                    configuration += ",";
                    configuration += Convert.ToString(chamber.scaleFactor);         // Scale factor
                    configuration += ",";
                    configuration += Convert.ToString(chamber.RHRaw1);              // Humidity sensor calibration point 1 raw reading
                    configuration += ",";
                    configuration += Convert.ToString(chamber.RHStd1);              // Humidity sensor calibration point 1 standard reading
                    configuration += ",";
                    configuration += Convert.ToString(chamber.RHRaw2);              // Humidity sensor calibration point 2 raw reading
                    configuration += ",";
                    configuration += Convert.ToString(chamber.RHStd2);              // Humidity sensor calibration point 2 standard reading
                    configuration += ",";
                    configuration += Convert.ToString(chamber.rampTotalSeconds);    // Total time (seconds) for this ramp step
                    configuration += ",";
                    configuration += Convert.ToString(chamber.RHCurSetpoint);       // Current setpoint for relative humidity
                    configuration += ",";
                    configuration += Convert.ToString(chamber.RHInitial);           // The initial RH for this step
                    configuration += ",";
                    configuration += Convert.ToString(chamber.fanSpeedCurSetpoint); // Current setpoint for velocity
                    configuration += ",";
                    configuration += Convert.ToString(chamber.fanSpeedInitial);     // The initial velocity for this step
                    configuration += ",";
                    configuration += Convert.ToString(chamber.nextRampTime.TotalSeconds);  // When to change to the next ramp setpoint
                    configuration += "\n";

                    saveConfigWriter.Write(configuration);

                    for (int j = 0; j < chamber.steps.Count; j++)
                    {
                        // Steps for a chamber
                        configuration = "";
                        configuration += Convert.ToString(chamber.id);                              // chamber id
                        configuration += ",";
                        configuration += "FALSE";                                                   // writing steps for chamber
                        configuration += ",";
                        configuration += Convert.ToString(j);                                       // step id
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].type);                   // step type
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].targetHumidity);         // target humidity
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].timeRemaining.Days);     // duration: days
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].timeRemaining.Hours);    // duration: hours
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].timeRemaining.Minutes);  // duration: minutes
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].timeRemaining.Seconds);  // duration: seconds
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].targetFanSpeed);         // target velocity
                        configuration += ",";
                        configuration += Convert.ToString(chamber.steps[j].status);                 // step status
                        configuration += "\n";

                        saveConfigWriter.Write(configuration);
                    }
                }

                saveConfigWriter.Flush();
                saveConfigWriter.Close();
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);

                if (manualSave)
                {
                    MessageBox.Show("Error saving configuration file. Please choose a different configuration file name or check if you appropriate privileges (e.g. administrator privileges) to save file to this location. The error is: \n\n" + err.Message, "Error");
                }
            }
        }

        // Load the given configuration file. Returns true if success; false if not.
        private bool loadConfigFile(bool loadAutoSave, bool promptUser = true, string filePath = "")
        {
            bool systemIdle = true;

            // First, check if any of the chambers are running (DAQ, recording, or control)
            foreach (Chamber chamber in chambers)
            {
                if (chamber.DAQRunning || chamber.recording || chamber.controlRunning)
                {
                    systemIdle = false;
                    break;
                }
            }

            if (systemIdle)
            {
                try
                {
                    // If promptUser is false, first condition evaluates to true and skips check for second condition
                    if (!promptUser || openConfigFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePathName;

                        if (promptUser)
                        {// A dialog box appeared asking user for file path
                            filePathName = openConfigFileDialog.FileName;
                        }
                        else
                        {// No dialog box; file path is from function argument
                            filePathName = filePath;
                        }

                        FileStream openConfigStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.None);
                        StreamReader openConfigReader = new StreamReader(openConfigStream);

                        openConfigFileDialog.FileName = "";

                        // Use a temporary list so that the original list is not affected if the loading fails
                        List<Chamber> tempChambers = new List<Chamber>(); ;

                        string configuration = "";

                        while (openConfigReader.Peek() >= 0)
                        {
                            configuration = openConfigReader.ReadLine();

                            configuration = configuration.TrimEnd('\n');

                            if (configuration != "")
                            {
                                string[] configurationParts = configuration.Split(',');

                                if (Convert.ToBoolean(configurationParts[1]))
                                {// Create a chamber
                                    int chamberID = Convert.ToInt16(configurationParts[0]);

                                    tempChambers.Add(new Chamber(chamberID, CHART_INTERVAL_MAX));

                                    if (loadAutoSave)
                                    {
                                        // Load up variables that can be used to resume chamber operation
                                        tempChambers[chamberID].DAQRunning          = Convert.ToBoolean(configurationParts[2]);
                                        tempChambers[chamberID].recording           = Convert.ToBoolean(configurationParts[3]);
                                        tempChambers[chamberID].controlRunning      = Convert.ToBoolean(configurationParts[4]);
                                        tempChambers[chamberID].rampTotalSeconds    = Convert.ToDouble(configurationParts[14]);
                                        tempChambers[chamberID].RHCurSetpoint       = Convert.ToDouble(configurationParts[15]);
                                        tempChambers[chamberID].RHInitial           = Convert.ToDouble(configurationParts[16]);
                                        tempChambers[chamberID].fanSpeedCurSetpoint = Convert.ToDouble(configurationParts[17]);
                                        tempChambers[chamberID].fanSpeedInitial     = Convert.ToDouble(configurationParts[18]);
                                        tempChambers[chamberID].nextRampTime        = new TimeSpan(0, 0, Convert.ToInt32(configurationParts[19]));
                                    }

                                    tempChambers[chamberID].DAQInterval         = Convert.ToUInt16(configurationParts[5]);
                                    tempChambers[chamberID].recordFilePath      = configurationParts[6];
                                    tempChambers[chamberID].calibrationWeight   = Convert.ToDecimal(configurationParts[7]);
                                    tempChambers[chamberID].scaleOffset         = Convert.ToInt32(configurationParts[8]);
                                    tempChambers[chamberID].scaleFactor         = Math.Round(Convert.ToDouble(configurationParts[9]), scaleFactorDecimalPlaces);
                                    tempChambers[chamberID].RHRaw1              = Math.Round(Convert.ToDouble(configurationParts[10]), humidityDecimalPlaces);
                                    tempChambers[chamberID].RHStd1              = Math.Round(Convert.ToDouble(configurationParts[11]), humidityDecimalPlaces);
                                    tempChambers[chamberID].RHRaw2              = Math.Round(Convert.ToDouble(configurationParts[12]), humidityDecimalPlaces);
                                    tempChambers[chamberID].RHStd2              = Math.Round(Convert.ToDouble(configurationParts[13]), humidityDecimalPlaces);
                                }
                                else
                                {// Create a step
                                    int chamberID = Convert.ToInt16(configurationParts[0]);
                                    HumidityStep.stepType stepType;
                                    HumidityStep.stepStatus stepStatus;

                                    switch (configurationParts[3])
                                    {
                                        case "TYPE_RAMP":
                                            stepType = HumidityStep.stepType.TYPE_RAMP;
                                            break;
                                        case "TYPE_HOLD":
                                        default:
                                            stepType = HumidityStep.stepType.TYPE_HOLD;
                                            break;
                                    }

                                    switch (configurationParts[10])
                                    {
                                        case "STATUS_COMPLETED":
                                            stepStatus = HumidityStep.stepStatus.STATUS_COMPLETED;
                                            break;
                                        case "STATUS_RUNNING":
                                        case "STATUS_WAITING":
                                        default:
                                            // Even if the step was running when the config/autosave was saved, force
                                            // it to be waiting, because upon starting the control, the runNextControlStep function
                                            // would run the first non-completed step anyway (which would be the first waiting step)
                                            stepStatus = HumidityStep.stepStatus.STATUS_WAITING;
                                            break;
                                    }

                                    tempChambers[chamberID].steps.Add(new HumidityStep(Convert.ToUInt16(configurationParts[2]),
                                                                                    stepType,
                                                                                    Convert.ToDouble(configurationParts[4]),
                                                                                    Convert.ToInt16(configurationParts[5]),
                                                                                    Convert.ToInt16(configurationParts[6]),
                                                                                    Convert.ToInt16(configurationParts[7]),
                                                                                    Convert.ToInt16(configurationParts[8]),
                                                                                    Convert.ToDouble(configurationParts[9]),
                                                                                    stepStatus));
                                }
                            }
                        }

                        openConfigReader.Close();

                        // Now replace original list with new list
                        chambers = tempChambers;
                        chamberCount_ = Convert.ToUInt32(chambers.Count);

                        // Set to the id of the first chamber
                        activeChamberID_ = chambers[0].id;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Error opening configuration file. Please check if the file exists or if you have chosen the correct location. The error is: \n\n" + err.Message, "Error");
                    errorLogger_.logUnknownError(err);
                    return false;
                }
            }
            else
            {
                // At least one of the chambers are operational; tell the user to stop that first
                MessageBox.Show("At least one of the chambers is operational.\nStop all chamber operations before loading a\nconfiguration or autosave file.", "Error");
                return false;
            }
        }

        // Start DAQ for a given chamber
        // showError determines if a Messagebox should be shown to the user if this command wasn't successful.
        // controlAfterDAQ determines if a command to start control mode should be immediately sent after DAQ is successfully started.
        private void startDAQ(int chamberID, bool showError, bool controlAfterDAQ)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected)
            {
                if (chamberID == activeChamberID_)
                {
                    // This is the actively-displayed chamber, so disable the buttons
                    controlsUpdating_ = true;
                    updateButtonEnabled(Data_DAQ_start, false);
                    updateNumericEnabled(Data_DAQ_interval_value, false);
                    controlsUpdating_ = false;
                }

                // Ensure updateForm() apply appropriate changes when switching to this chamber
                chambers[chamberID].startDAQWaiting = true;
                
                // Put a flag determining if we should show a Messagebox error or not
                chambers[chamberID].showCommandError = showError;

                // Send command and wait for confirmation
                commands_.queueCommandDAQ(  chamberID,
                                            chambers[chamberID].DAQInterval,
                                            chambers[chamberID].scaleOffset,
                                            chambers[chamberID].scaleFactor,
                                            chambers[chamberID].RHRaw1,
                                            chambers[chamberID].RHStd1,
                                            chambers[chamberID].RHRaw2,
                                            chambers[chamberID].RHStd2,
                                            controlAfterDAQ);
            }
        }

        // Stop DAQ for a given chamber
        // showError determines if a Messagebox should be shown to the user if this command wasn't successful.
        private void stopDAQ(int chamberID, bool showError)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected)
            {
                if (chamberID == activeChamberID_)
                {
                    // This is the actively-displayed chamber, so disable the buttons
                    controlsUpdating_ = true;
                    updateButtonEnabled(Data_DAQ_stop, false);
                    controlsUpdating_ = false;
                }

                // Ensure updateForm() apply appropriate changes when switching to this chamber
                chambers[chamberID].stopDAQWaiting = true;

                // Put a flag determining if we should show a Messagebox error or not
                chambers[chamberID].showCommandError = showError;

                // Send command and wait for confirmation
                commands_.queueCommandIdle(chamberID, true);
            }
        }

        // Start recording for a given chamber
        private void startRecording(int chamberID, bool activeChamber)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected && chambers[chamberID].DAQRunning)
            {
                chambers[chamberID].recordPaused = false;

                if (activeChamber)
                {
                    updateButtonEnabled(Data_save_flow_start, false);
                    updateButtonEnabled(Data_save_flow_browse, false);
                    suspendLayoutControl(Data_save_flow);
                }

                try
                {
                    bool fileExists = false;

                    // Check if the file is already created. Will be used for adding the header at first line
                    if (File.Exists(chambers[chamberID].recordFilePath))
                    {
                        fileExists = true;
                    }

                    // append or create new file; write-only; allow read-only access to other processes.
                    // The read-only access allows one to copy the data file to somewhere else before making modifications.
                    // Disable auto-flush so that the program does not slow down from writing long files (maybe?)
                    chambers[chamberID].recordStream = new FileStream(chambers[chamberID].recordFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    chambers[chamberID].CSVWriter = new StreamWriter(chambers[chamberID].recordStream); // default encoding is UTF-8
                    chambers[chamberID].CSVWriter.AutoFlush = true;
                    chambers[chamberID].legitFilePath = true;
                    chambers[chamberID].recording = true;

                    // Write header if we are creating a new file
                    if (!fileExists)
                    {
                        chambers[chamberID].CSVWriter.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}",
                                                            new Object[] {  "Date and time",
                                                                            ",",
                                                                            "Step no.",
                                                                            ",",
                                                                            "Step type",
                                                                            ",",
                                                                            "Relative humidity setpoint (%)",
                                                                            ",",
                                                                            "Air velocity setpoint (m/s)",
                                                                            ",",
                                                                            "Relative humidity (%)",
                                                                            ",",
                                                                            "Temperature (degC)",
                                                                            ",",
                                                                            "Weight (g)",
                                                                            ",",
                                                                            "Air velocity (m/s)",
                                                                            ",",
                                                                            "Fan 1 speed (rpm)",
                                                                            ",",
                                                                            "Fan 2 speed (rpm)",
                                                                            Environment.NewLine });
                    }
                }
                catch (Exception err)
                {
                    if (activeChamber)
                    {
                        updateButtonEnabled(Data_save_flow_start, true);
                        updateButtonEnabled(Data_save_flow_browse, true);
                    }

                    chambers[chamberID].legitFilePath = false;
                    chambers[chamberID].recording = false;
                    errorLogger_.logCProgError(031, err, err.Message);
                    MessageBox.Show("The file path\n\n" + chambers[chamberID].recordFilePath + "\n\nfor recording data is invalid. \n\nIf the file already exists, close any programs that are opening that file.", "Unable to create/read file");
                }

                if (chambers[chamberID].legitFilePath)
                {
                    if (activeChamber)
                    {
                        updateButtonVisibility(Data_save_flow_start, false);
                        updateButtonVisibility(Data_save_flow_stop, true);
                        updateButtonEnabled(Data_save_flow_stop, true);
                        changeControlBackColor(Data_save_flow, Color.LightGreen);
                    }

                    // If the program is notified that the chamber is disconnected, the DAQPaused var will be set to true.
                    // We check this flag so we don't send a read command for a disconnected chamber.
                    if (!chambers[chamberID].DAQPaused)
                    {
                        commands_.queueCommandRead(chamberID);
                    }
                }

                if (activeChamber)
                {
                    Data_save_flow.Controls.SetChildIndex(Data_save_flow_start, Data_save_flow.Controls.GetChildIndex(Data_save_flow_start) + 1);
                    resumeLayoutControl(Data_save_flow);
                    Data_save_flow.PerformLayout();
                }
            }
        }

        // Stop recording data for the given chamber
        private void stopRecording(int chamberID, bool activeChamber)
        {
            chambers[chamberID].recordPaused = false;

            if (activeChamber)
            {
                updateButtonEnabled(Data_save_flow_stop, false);
                suspendLayoutControl(Data_save_flow);
            }

            try
            {
                // Regardless of whether stopping recording succeeds or not, force the recording state var to be false
                chambers[activeChamberID_].recording = false;
                chambers[chamberID].CSVWriter.Flush();
                chambers[chamberID].CSVWriter.Close();
            }
            catch (ArgumentNullException err)
            {
                errorLogger_.logCProgError(041, err);
            }
            catch (ObjectDisposedException err)
            {
                errorLogger_.logCProgError(042, err);
            }
            catch (FormatException err)
            {
                errorLogger_.logCProgError(043, err);
            }
            catch (IOException err)
            {
                errorLogger_.logCProgError(044, err);
            }
            catch (EncoderFallbackException err)
            {
                errorLogger_.logCProgError(045, err);
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);
            }


            if (activeChamber)
            {
                // Regardless of whether we succeeded or failed in closing the stream, reset the layout to the non-recording style
                updateButtonVisibility(Data_save_flow_stop, false);
                updateButtonVisibility(Data_save_flow_start, true);
                updateButtonEnabled(Data_save_flow_start, true);
                updateButtonEnabled(Data_save_flow_browse, true);
                changeControlBackColor(Data_save_flow, SystemColors.Control);

                Data_save_flow.Controls.SetChildIndex(Data_save_flow_stop, Data_save_flow.Controls.GetChildIndex(Data_save_flow_stop) + 1);
                resumeLayoutControl(Data_save_flow);
                Data_save_flow.PerformLayout();
            }
        }

        // Send command to update the first RH calibration point for the given chamber
        // showError determines if a Messagebox should be shown to the user if this command wasn't successful.
        private void startRHCalibrate(int chamberID, bool point1, bool showError)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected)
            {
                if (chamberID == activeChamberID_)
                {
                    // This is the actively-displayed chamber, so disable the buttons
                    controlsUpdating_ = true;
                    updateNumericEnabled(Advanced_humidity_RH1_std_value, false);
                    updateButtonEnabled(Advanced_humidity_RH1_std_apply, false);
                    updateNumericEnabled(Advanced_humidity_RH2_std_value, false);
                    updateButtonEnabled(Advanced_humidity_RH2_std_apply, false);
                    controlsUpdating_ = false;
                }

                if (point1)
                {
                    chambers[chamberID].RHRaw1 = Convert.ToDouble(Advanced_humidity_RH1_raw_value.Value);
                    chambers[chamberID].RHStd1 = Convert.ToDouble(Advanced_humidity_RH1_std_value.Value);
                    // Ensure updateForm() doesn't re-enable the buttons when switching to this chamber
                    chambers[chamberID].RH1CalibrateWaiting = true;

                    // Put a flag determining if we should show a Messagebox error or not
                    chambers[chamberID].showCommandError = showError;

                    // Send command and wait for confirmation
                    commands_.queueCommandCalibrateRH(activeChamberID_, point1, chambers[chamberID].RHRaw1, chambers[chamberID].RHStd1);
                }
                else
                {
                    chambers[chamberID].RHRaw2 = Convert.ToDouble(Advanced_humidity_RH2_raw_value.Value);
                    chambers[chamberID].RHStd2 = Convert.ToDouble(Advanced_humidity_RH2_std_value.Value);
                    // Ensure updateForm() doesn't re-enable the buttons when switching to this chamber
                    chambers[chamberID].RH2CalibrateWaiting = true;

                    // Put a flag determining if we should show a Messagebox error or not
                    chambers[chamberID].showCommandError = showError;

                    // Send command and wait for confirmation
                    commands_.queueCommandCalibrateRH(activeChamberID_, point1, chambers[chamberID].RHRaw2, chambers[chamberID].RHStd2);
                }
            }
        }

        // Send command to tare the weighing scale for the given chamber.
        // showError determines if a Messagebox should be shown to the user if this command wasn't successful.
        private void startScaleTare(int chamberID, bool showError)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected)
            {
                if (chamberID == activeChamberID_)
                {
                    // This is the actively-displayed chamber, so disable the buttons
                    controlsUpdating_ = true;
                    updateButtonEnabled(WeighScale_flow_tare, false);
                    controlsUpdating_ = false;
                }
                
                // Ensure updateForm() doesn't re-enable the buttons when switching to this chamber
                chambers[chamberID].scaleTareWaiting = true;

                // Put a flag determining if we should show a Messagebox error or not
                chambers[chamberID].showCommandError = showError;

                // Send command and wait for confirmation
                commands_.queueCommandTareScale(activeChamberID_);
            }
        }

        // Send command to calibrate the weighing scale for the given chamber.
        // showError determines if a Messagebox should be shown to the user if this command wasn't successful.
        private void startScaleCalibrate(int chamberID, Int16 calibrationWeight, bool showError)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected)
            {
                if (chamberID == activeChamberID_)
                {
                    // This is the actively-displayed chamber, so disable the buttons
                    controlsUpdating_ = true;
                    updateButtonEnabled(WeighScale_flow_cal_button, false);
                    updateNumericEnabled(WeighScale_flow_cal_value, false);
                    controlsUpdating_ = false;
                }

                // Ensure updateForm() doesn't re-enable the buttons when switching to this chamber
                chambers[chamberID].scaleCalibrateWaiting = true;

                // Put a flag determining if we should show a Messagebox error or not
                chambers[chamberID].showCommandError = showError;

                // Send command and wait for confirmation
                commands_.queueCommandCalibrateScale(activeChamberID_, calibrationWeight);
            }
        }
        

        // Start the control program for the given chamber.
        // showError determines if a Messagebox should be shown to the user if this command wasn't successful.
        private void startControl(int chamberID, bool showError, bool resumeOperation)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected && chambers[chamberID].DAQRunning)
            {
                if (chamberID == activeChamberID_)
                {
                    controlsUpdating_ = true;

                    updateButtonEnabled(HumidityControl_start, false);

                    // Disable other sections while waiting for confirmation on the command
                    updateButtonEnabled(Data_DAQ_stop, false);
                    updateButtonEnabled(WeighScale_flow_tare, false);
                    updateNumericEnabled(WeighScale_flow_cal_value, false);
                    updateButtonEnabled(WeighScale_flow_cal_button, false);
                    updateButtonEnabled(Advanced_humidity_RH1_std_apply, false);
                    updateNumericEnabled(Advanced_humidity_RH1_std_value, false);
                    updateButtonEnabled(Advanced_humidity_RH2_std_apply, false);
                    updateNumericEnabled(Advanced_humidity_RH2_std_value, false);

                    controlsUpdating_ = false;
                }
                //System.Diagnostics.Debug.WriteLine("Clicked start control for chamber " + Convert.ToString(chamberID));
                // Begin the first available control step. This function 
                // sends the commands and sets the appropriate flags for the chamber.
                runNextControlStep(chamberID, true, showError, resumeOperation);
            }
        }


        // Pauses the control program for the given chamber.
        // showError determines if a Messagebox should be shown to the user if this command wasn't successful.
        private void pauseControl(int chamberID, bool activeChamber)
        {
            if (arduinoConnected_ && chambers[chamberID].chamberConnected)
            {
                if (activeChamber)
                {
                    updateButtonEnabled(HumidityControl_pause, false);
                }
                
                stopControlStep(chamberID, false);
            }
        }
        
        // This function browses through the list of steps (starting from 0) in the given chamber and determines which step should be run.
        // It will choose the first step encountered that is in "Waiting" status and run that.
        // Returns true if there is a next control step, false if there are no remaining control steps to run or if encountered an error sending command to Arduino
        private void runNextControlStep(int chamberID, bool initialize, bool showError, bool resumeOperation)
        {
            // Pause the step timer. This will be started upon receiving confirmation from Arduino 
            // that a new control step is successfully started.
            chambers[chamberID].stepTimer.Stop();

            for (int i = 0; i < chambers[chamberID].steps.Count; i++)
            {
                // Only consider starting this step if it is waiting or running (happens when loading autosaves)
                if (chambers[chamberID].steps[i].status == HumidityStep.stepStatus.STATUS_WAITING || chambers[chamberID].steps[i].status == HumidityStep.stepStatus.STATUS_RUNNING)
                {
                    // If it has more than 0 seconds remaining, then execute the starting instructions. Otherwise, set it to completed right away
                    if (chambers[chamberID].steps[i].timeRemaining.TotalSeconds > 0.1)
                    {
                        chambers[chamberID].tempActiveStep = i;

                        // If the control is initialized (i.e. it wasn't running before), tell Arduino to begin control...
                        if (initialize)
                        {
                            // Ensure updateForm() doesn't revert control changes when moving from one chamber back to this one
                            chambers[chamberID].startControlWaiting = true;

                            // Put a flag determining if we should show a Messagebox error or not
                            chambers[chamberID].showCommandError = showError;

                            // Calculate the new setpoint, if we are not resuming from an autosave.
                            // The autosave file would already have all the setpoints necessary for the current step.
                            if (!resumeOperation)
                            {
                                calculateAndStoreSetpoint(chamberID, i, true);
                            }
                            //System.Diagnostics.Debug.WriteLine("Qeueuing start control command for chamber " + Convert.ToString(chamberID));
                            // Send command to begin using the calculated setpoints
                            commands_.queueCommandControl(  chamberID,
                                                            chambers[chamberID].RHCurSetpoint,
                                                            chambers[chamberID].fanSpeedCurSetpoint);
                        }
                        else
                        {// ...otherwise the control is just continuing from one step to another, so just change the setpoint.
                            // The flag for nextControlStepWaiting and skipControlStepWaiting are set in the calling functions
                            
                            // Put a flag determining if we should show a Messagebox error or not
                            chambers[chamberID].showCommandError = showError;

                            // Calculate the new setpoint, if we are not resuming from an autosave.
                            // The autosave file would already have all the setpoints necessary for the current step.
                            if (!resumeOperation)
                            {
                                calculateAndStoreSetpoint(chamberID, i, true);
                            }

                            // Send command to begin using the calculated setpoints
                            commands_.queueCommandSetpoint( chamberID,
                                                            chambers[chamberID].RHCurSetpoint,
                                                            chambers[chamberID].fanSpeedCurSetpoint);
                        }

                        // If the chamber is starting from the first step or the difference in setpoint between new step and old step is above a threshold, reset the PID params
                        // This helps to speed up the change in setpoints.
                        // NOTE: THIS FEATURE IS CURRENTLY DISABLED because resetting the PID sometimes causes the system to respond slower
                        /*
                        if (chambers[chamberID].activeStep == 0
                                ||
                            (chambers[chamberID].steps[i].targetHumidity - chambers[chamberID].steps[i-1].targetHumidity) >= HUMIDITY_RESETPID_THRESHOLD)
                        {
                            sendCommand(SERIAL_CMD_CHAMBER + string.Format("{0:d}", chamberID) + SERIAL_CMD_RESETPID + SERIAL_CMD_HUMIDITY + SERIAL_CMD_END);
                        }
                        */
                        return;
                    }
                    else
                    {
                        chambers[chamberID].steps[i].status = HumidityStep.stepStatus.STATUS_COMPLETED;
                    }
                }
            }

            // If no running steps were encountered, that means our list of steps has been exhausted and it's time to end the program
            // Note that the code below won't be reached if a WAITING step was found, because it would have ended the function using return.
            // Call stopControlStep() to wrap up.
            stopControlStep(chamberID, true);
        }

        // Send command to stop process controls in a chamber
        private void stopControlStep(int chamberID, bool noStepsRemaining)
        {
            if (chambers[chamberID].controlRunning)
            {// Control is running, so stop it
                // Stop step timer
                chambers[chamberID].stepTimer.Stop();

                // Disallow user from clicking the pause control button to avoid
                // more instructions being sent
                if (chamberID == activeChamberID_)
                {
                    updateButtonEnabled(HumidityControl_pause, false);
                }

                // Depending on whether this is truly the end of control program or just a pause,
                // set the appropriate flags
                if (noStepsRemaining)
                {// No more steps remaining; this command is to end the control program
                    chambers[chamberID].endControlWaiting = true;
                }
                else
                {// There are still steps remaining; this is just to pause the control program
                    chambers[chamberID].pauseControlWaiting = true;

                    // With the above flag set, refreshing the steps would disable the skip button
                    // on active step, thus preventing user from creating additional instructions
                    refreshSteps(chamberID);
                }

                // Send command
                commands_.queueCommandDAQ(  chamberID,
                                            chambers[chamberID].DAQInterval,
                                            chambers[chamberID].scaleOffset,
                                            chambers[chamberID].scaleFactor,
                                            chambers[chamberID].RHRaw1,
                                            chambers[chamberID].RHStd1,
                                            chambers[chamberID].RHRaw2,
                                            chambers[chamberID].RHStd2,
                                            false);
            }
            else
            {// Control wasn't running, but this function was called; happens when no steps are
             // available to run but user clicked button to start control
             // runNextControlStep would have flagged any waiting steps as completed, so all we do
             // here is update the controls to non-control state
                if (chamberID == activeChamberID_)
                {
                    updateControlsStateDAQ(chamberID);
                }

                // Repaint the steps to account for any steps that have been changed to COMPLETED status.
                refreshSteps(chamberID);
            }
        }

        // Calculate the setpoints for RH and velocity for a given control step and store them in the appropriate chamber instance.
        // Setpoints are calculated differently depending on whether it's a ramp or hold control step.
        private void calculateAndStoreSetpoint(int chamberID, int stepNumber, bool initialize)
        {
            if (chambers[chamberID].steps[stepNumber].type == HumidityStep.stepType.TYPE_RAMP)
            {// Ramp step
                if (initialize)
                {
                    // Total time for this ramp step, in seconds
                    chambers[chamberID].rampTotalSeconds = chambers[chamberID].steps[stepNumber].timeRemaining.TotalSeconds;

                    // If the control program is in the second step or later, use the setpoint (not reading) of the prev step as the initial setpoint
                    // If control program was just started, use the current reading.
                    if (stepNumber > 0)
                    {
                        chambers[chamberID].RHInitial = chambers[chamberID].steps[stepNumber - 1].targetHumidity;
                        chambers[chamberID].fanSpeedInitial = chambers[chamberID].steps[stepNumber - 1].targetFanSpeed;
                    }
                    else
                    {
                        chambers[chamberID].RHInitial = chambers[chamberID].humidity;
                        chambers[chamberID].fanSpeedInitial = chambers[chamberID].velocity;
                    }
                }

                // Calculate the new temporary setpoint
                double timeRatio = (chambers[chamberID].rampTotalSeconds - chambers[chamberID].steps[stepNumber].timeRemaining.TotalSeconds) / chambers[chamberID].rampTotalSeconds;
                chambers[chamberID].RHCurSetpoint          = chambers[chamberID].RHInitial + timeRatio * (chambers[chamberID].steps[stepNumber].targetHumidity - chambers[chamberID].RHInitial);
                chambers[chamberID].fanSpeedCurSetpoint    = chambers[chamberID].fanSpeedInitial + timeRatio * (chambers[chamberID].steps[stepNumber].targetFanSpeed - chambers[chamberID].fanSpeedInitial);

                // Calculate when to ramp to next setpoint.
                // If there is more than enough time remaining in the current control step, then schedule the next ramp step...
                if (chambers[chamberID].steps[stepNumber].timeRemaining.CompareTo(new TimeSpan(0, 0, RAMP_INTERVAL)) > 0)
                {
                    chambers[chamberID].nextRampTime = chambers[chamberID].steps[stepNumber].timeRemaining.Subtract(new TimeSpan(0, 0, RAMP_INTERVAL));
                }
                else
                {// ...otherwise, force the next ramp time to be unreachable (an additional hour). Note that this case also includes the situation when
                 // the next ramp time is equal to end of the the timer (0 second) because of the way conditional statement is written
                    chambers[chamberID].nextRampTime = chambers[chamberID].steps[stepNumber].timeRemaining.Subtract(new TimeSpan(1, 0, RAMP_INTERVAL));
                }

                // There is big chance that the resulting setpoint has multiple decimal places. Since RH and velocity in this program is
                // restricted to a single decimal place, apply modifications to the new temporary setpoint to obey this restriction.
                if (chambers[chamberID].steps[stepNumber].targetHumidity - chambers[chamberID].RHInitial > 0)
                {// Increasing humidity
                    // This forces the computed number to the nearest highest 1st decimal place
                    chambers[chamberID].RHCurSetpoint = Math.Ceiling(chambers[chamberID].RHCurSetpoint * 10) / 10;

                    // If the resultant temp setpoint overshoots the target RH, set it to the target RH
                    if (chambers[chamberID].RHCurSetpoint > chambers[chamberID].steps[stepNumber].targetHumidity)
                    {
                        chambers[chamberID].RHCurSetpoint = chambers[chamberID].steps[stepNumber].targetHumidity;
                    }
                }
                else
                {// Decreasing humidity
                    // This forces the computed number to the nearest lowest 1st decimal place
                    chambers[chamberID].RHCurSetpoint = Math.Floor(chambers[chamberID].RHCurSetpoint * 10) / 10;

                    // If the resultant temp setpoint undershoots the target RH, set it to the target RH
                    if (chambers[chamberID].RHCurSetpoint < chambers[chamberID].steps[stepNumber].targetHumidity)
                    {
                        chambers[chamberID].RHCurSetpoint = chambers[chamberID].steps[stepNumber].targetHumidity;
                    }
                }

                // Do the same for velocity
                if (chambers[chamberID].steps[stepNumber].targetFanSpeed - chambers[chamberID].fanSpeedInitial > 0)
                {// Increasing velocity
                    // This forces the computed number to the nearest highest 1st decimal place
                    chambers[chamberID].fanSpeedCurSetpoint = Math.Ceiling(chambers[chamberID].fanSpeedCurSetpoint * 10) / 10;

                    // If the resultant temp setpoint overshoots the target velocity, set it to the target velocity
                    if (chambers[chamberID].fanSpeedCurSetpoint > chambers[chamberID].steps[stepNumber].targetFanSpeed)
                    {
                        chambers[chamberID].fanSpeedCurSetpoint = chambers[chamberID].steps[stepNumber].targetFanSpeed;
                    }
                }
                else
                {// Decreasing velocity
                    // This forces the computed number to the nearest lowest 1st decimal place
                    chambers[chamberID].fanSpeedCurSetpoint = Math.Floor(chambers[chamberID].fanSpeedCurSetpoint * 10) / 10;

                    // If the resultant temp setpoint undershoots the target velocity, set it to the target velocity
                    if (chambers[chamberID].fanSpeedCurSetpoint < chambers[chamberID].steps[stepNumber].targetFanSpeed)
                    {
                        chambers[chamberID].fanSpeedCurSetpoint = chambers[chamberID].steps[stepNumber].targetFanSpeed;
                    }
                }
            }
            else
            {// Hold step; the variables specific to ramping are not crucial.
                // The target setpoints should have already been set upon starting this active step.
                chambers[chamberID].RHCurSetpoint          = chambers[chamberID].steps[stepNumber].targetHumidity;
                chambers[chamberID].fanSpeedCurSetpoint    = chambers[chamberID].steps[stepNumber].targetFanSpeed;
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Event handlers
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void menu_config_openConfig_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete all the current settings and steps. Are you sure you want to proceed?", "WARNING", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (loadConfigFile(false))
                {
                    // Update the form to the current chamber
                    updateForm(activeChamberID_);

                    // Reset Arduino
                    resetArduino();
                }
            }
        }

        private void menu_config_saveConfig_Click(object sender, EventArgs e)
        {
            if (saveConfigFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveConfigFile(saveConfigFileDialog.FileName, true);
            }
        }

        // Enable/disable autosave
        private void menu_autosave_enableDisable_Click(object sender, EventArgs e)
        {
            if (autosaving_)
            {
                // Already autosaving; Disable it
                autosaving_ = false;
                autosaverTimer_.Stop();
                autosaverTimer_.Elapsed -= autosaverTimerElapsed;

                menu_autosave_status.BackColor = Color.DarkGray;
                menu_autosave_status.Text = "Autosave disabled";
                menu_autosave_enableDisable.Text = "Enable autosave";
            }
            else
            {
                // Not autosaving; Enable it
                // Save current state first
                saveProgramState();

                autosaving_ = true;
                autosaverTimer_.Interval = autosaveInterval_ * 60 * 1000; // Convert min to ms
                autosaverTimer_.AutoReset = true;
                autosaverTimer_.Elapsed += autosaverTimerElapsed;
                autosaverTimer_.Start();

                menu_autosave_status.BackColor = Color.LightGreen;
                menu_autosave_status.Text = "Autosave enabled";
                menu_autosave_enableDisable.Text = "Disable autosave";
            }
        }

        // Changing the value of autosave interval
        private void menu_autosave_interval_value_TextChanged(object sender, EventArgs e)
        {
            if (!uint.TryParse(menu_autosave_interval_value.Text, out autosaveInterval_))
            {
                // User input is not an unsigned integer. Force it to default value of 5 min.
                autosaveInterval_ = 5;
                menu_autosave_interval_value.Text = "5";
            }

            // If autosaving is already running, reset the timer to the new interval
            if (autosaving_)
            {
                // Save current state first
                saveProgramState();

                // Upon changing the interval, the timer behaves as if it "reset" so that the next
                // elapsed event is raised after this new interval is elapsed. See:
                // https://stackoverflow.com/a/7045721
                autosaverTimer_.Interval = autosaveInterval_ * 60 * 1000; // Convert min to ms
            }
        }

        // Interval for autosaving has elapsed
        private void autosaverTimerElapsed(Object source, System.Timers.ElapsedEventArgs e)
        {
            saveProgramState();
        }

        private void saveProgramState()
        {
            // Create the autosave folder if it doesn't exist; otherwise this does nothing
            Directory.CreateDirectory(autosaveFolder_);

            string fileName = string.Format("{0}{1:yyyy-MM-dd_HHmm_ss}.csv", autosaveFolder_, DateTime.Now);
            saveConfigFile(fileName, false);
        }

        private void menu_autosave_load_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete all the current settings and steps. Are you sure you want to proceed?", "WARNING", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (loadConfigFile(true))
                {
                    // Update the form to the current chamber
                    updateForm(activeChamberID_);

                    // Reset Arduino.
                    resetArduino();

                    // resetArduino sends request for chamber connection. Upon receiving request
                    // that a chamber is connected/disconnected, handleChamberConnection will be called
                    // and appropriate actions will be taken to resume DAQ/recording/control.

                    // Recording will be resumed once confirmation for successful DAQ command is successful.
                    // See updateControlsDAQSuccess()
                }
            }
        }

        // Used to prevent mousewheel scrolling from changing values of numericUpDown controls
        private void chill_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private void Chamber_prev_Click(object sender, EventArgs e)
        {
            // Do not allow user to change chambers if the form is updating
            if ((sender as Button).Enabled && !controlsUpdating_)
            {
                // Redundant check in case button was not disabled for first chamber
                if (activeChamberID_ > 0)
                {
                    controlsUpdating_ = true;
                    activeChamberID_--;
                    updateForm(activeChamberID_);
                    controlsUpdating_ = false;
                }
            }
        }

        private void Chamber_next_Click(object sender, EventArgs e)
        {
            // Do not allow user to change chambers if the form is updating
            if ((sender as Button).Enabled && !controlsUpdating_)
            {
                // Redundant check in case button was not disabled for last chamber
                if (activeChamberID_ < chamberCount_ - 1)
                {
                    controlsUpdating_ = true;
                    activeChamberID_++;
                    updateForm(activeChamberID_);
                    controlsUpdating_ = false;
                }
            }
        }

        private void DAQ_interval_value_ValueChanged(object sender, EventArgs e)
        {
            chambers[activeChamberID_].DAQInterval = Convert.ToUInt32(Data_DAQ_interval_value.Value);
        }

        private void DAQ_start_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                startDAQ(activeChamberID_, true, false);
            }
        }

        private void DAQ_stop_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                stopDAQ(activeChamberID_, true);
            }
        }

        private void OtherFunc_save_flow_browse_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                if (recordFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Data_save_flow_path.Text = recordFileDialog.FileName;
                    recordFileDialog.FileName = "";
                    chambers[activeChamberID_].recordFilePath = Data_save_flow_path.Text;
                }
            }
        }


        private void OtherFunc_save_flow_start_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                startRecording(activeChamberID_, true);
            }
        }

        private void OtherFunc_save_flow_stop_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                stopRecording(activeChamberID_, true);
            }
        }

        private void OtherFunc_scale_flow_tare_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                startScaleTare(activeChamberID_, true);
            }
        }

        private void OtherFunc_scale_flow_calValue_ValueChanged(object sender, EventArgs e)
        {
            if ((sender as NumericUpDown).Enabled)
            {
                chambers[activeChamberID_].calibrationWeight = WeighScale_flow_cal_value.Value;
            }
        }

        private void OtherFunc_scale_flow_calButton_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                startScaleCalibrate(activeChamberID_, Convert.ToInt16(Math.Round(WeighScale_flow_cal_value.Value)), true);
            }
        }

        private void Advanced_button_show_Click(object sender, EventArgs e)
        {
            updateFlowVisibility(Advanced_options, true);
            updateButtonEnabled(Advanced_button_show, false);
            updateButtonVisibility(Advanced_button_show, false);
            updateButtonEnabled(Advanced_button_hide, true);
            updateButtonVisibility(Advanced_button_hide, true);
        }

        private void Advanced_button_hide_Click(object sender, EventArgs e)
        {
            updateFlowVisibility(Advanced_options, false);
            updateButtonEnabled(Advanced_button_show, true);
            updateButtonVisibility(Advanced_button_show, true);
            updateButtonEnabled(Advanced_button_hide, false);
            updateButtonVisibility(Advanced_button_hide, false);
        }

        private void Advanced_humidity_RH1_apply_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                startRHCalibrate(activeChamberID_, true, true);
            }
        }

        private void Advanced_humidity_RH2_apply_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                startRHCalibrate(activeChamberID_, false, true);
            }
        }

        private void HumidityControl_addStep_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                suspendLayoutControl(HumidityControl);
                updateButtonEnabled(HumidityControl_addStep, false);

                // Default settings for a new step
                // Note: step IDs are zero-indexed while count starts from 1, so can use count directly instead of adding 1
                chambers[activeChamberID_].steps.Add(new HumidityStep(Convert.ToUInt16(chambers[activeChamberID_].steps.Count), HumidityStep.stepType.TYPE_HOLD, 40, 0, 0, 0, 0, 0, HumidityStep.stepStatus.STATUS_WAITING));
                refreshSteps(activeChamberID_);
                updateButtonEnabled(HumidityControl_addStep, true);
                resumeLayoutControl(HumidityControl);
            }
        }

        private void HumidityControl_start_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                startControl(activeChamberID_, true, false);
            }
        }

        private void HumidityControl_pause_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                pauseControl(activeChamberID_, true);
            }
        }

        private void changeStepOrder_Click(object sender, EventArgs e, UInt16 stepNumber, bool moveUp)
        {
            if ((sender as Button).Enabled)
            {
                chambers[activeChamberID_].updateOrder(stepNumber, moveUp);
                refreshSteps(activeChamberID_);
            }
        }


        private void stepType_SelectedIndexChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            switch (Convert.ToString((sender as ComboBox).SelectedItem))
            {
                case HumidityStep_Control.CONTROL_STEP_TYPE_RAMP:
                    chambers[activeChamberID_].steps[stepNumber].type = HumidityStep.stepType.TYPE_RAMP;
                    break;
                case HumidityStep_Control.CONTROL_STEP_TYPE_HOLD:
                default:
                    chambers[activeChamberID_].steps[stepNumber].type = HumidityStep.stepType.TYPE_HOLD;
                    break;
            }
        }

        private void targetHumidity_ValueChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            chambers[activeChamberID_].steps[stepNumber].targetHumidity = Convert.ToDouble((sender as NumericUpDown).Value);
        }

        private void duration_day_value_ValueChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            chambers[activeChamberID_].steps[stepNumber].updateTimeRemaining(1, Convert.ToInt16((sender as NumericUpDown).Value));
        }

        private void duration_hour_value_ValueChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            chambers[activeChamberID_].steps[stepNumber].updateTimeRemaining(2, Convert.ToInt16((sender as NumericUpDown).Value));
        }

        private void duration_minute_value_ValueChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            chambers[activeChamberID_].steps[stepNumber].updateTimeRemaining(3, Convert.ToInt16((sender as NumericUpDown).Value));
        }

        private void duration_second_value_ValueChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            chambers[activeChamberID_].steps[stepNumber].updateTimeRemaining(4, Convert.ToInt16((sender as NumericUpDown).Value));
        }

        private void stepTimer_Elapsed(object sender, EventArgs e, int chamberID, UInt16 stepNumber)
        {
            chambers[chamberID].steps[stepNumber].timeRemaining = chambers[chamberID].steps[stepNumber].timeRemaining.Subtract(OneSecInterval);
            //System.Diagnostics.Debug.WriteLine("Elapsed for chamber " + Convert.ToString(chamberID));
            // If the step is of type ramp...
            if (chambers[chamberID].steps[stepNumber].type == HumidityStep.stepType.TYPE_RAMP)
            {   //... then check if it's time to switch to the next setpoint
                if (chambers[chamberID].steps[stepNumber].timeRemaining.CompareTo(chambers[chamberID].nextRampTime) <= 0)
                {   // Time to switch to the next setpoint
                    // Only send instruction to the Arduino if there is connection
                    if (!chambers[chamberID].controlPaused)
                    {
                        // Flag this as waiting for a ramp setpoint to be executed
                        chambers[chamberID].rampControlStepWaiting = true;

                        // Calculate the new setpoints
                        calculateAndStoreSetpoint(chamberID, stepNumber, false);

                        // Send command
                        commands_.queueCommandSetpoint( chamberID,
                                                        chambers[chamberID].RHCurSetpoint,
                                                        chambers[chamberID].fanSpeedCurSetpoint);

                        // Don't have to pause the timer since we are still running control while ramping to next setpoint
                    }

                    // If there is more than enough time remaining in the current control step, then schedule the next ramp step...
                    if (chambers[chamberID].steps[stepNumber].timeRemaining.CompareTo(new TimeSpan(0, 0, RAMP_INTERVAL)) > 0)
                    {
                        chambers[chamberID].nextRampTime = chambers[chamberID].steps[stepNumber].timeRemaining.Subtract(new TimeSpan(0, 0, RAMP_INTERVAL));
                    }
                    else
                    {// ...otherwise, force the next ramp time to be unreachable (an additional hour). Note that this case also includes the situation when
                     // the next ramp time is equal to end of the the timer (0 second) because of the way conditional statement is written
                        chambers[chamberID].nextRampTime = chambers[chamberID].steps[stepNumber].timeRemaining.Subtract(new TimeSpan(1, 0, RAMP_INTERVAL));
                    }
                }
            }

            try
            {
                if (chamberID == activeChamberID_ && !controlsUpdating_)
                {
                    updateNumericValue(chambers[chamberID].steps[stepNumber].dayRemainingControl, chambers[chamberID].steps[stepNumber].timeRemaining.Days);
                    updateNumericValue(chambers[chamberID].steps[stepNumber].hourRemainingControl, chambers[chamberID].steps[stepNumber].timeRemaining.Hours);
                    updateNumericValue(chambers[chamberID].steps[stepNumber].minuteRemainingControl, chambers[chamberID].steps[stepNumber].timeRemaining.Minutes);
                    updateNumericValue(chambers[chamberID].steps[stepNumber].secondRemainingControl, chambers[chamberID].steps[stepNumber].timeRemaining.Seconds);
                }
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);
            }

            // Check if this step is done
            if (chambers[chamberID].steps[stepNumber].timeRemaining.Days <= 0 &&
                chambers[chamberID].steps[stepNumber].timeRemaining.Hours <= 0 &&
                chambers[chamberID].steps[stepNumber].timeRemaining.Minutes <= 0 &&
                chambers[chamberID].steps[stepNumber].timeRemaining.Seconds <= 0)
            {
                // Step complete; prepare for next step
                // Note minimal change of appearance of the controls; this will be done
                // once the command to move to next step was successfully executed
                chambers[chamberID].stepTimer.Stop();

                // Flag this as moving to the next step
                chambers[chamberID].nextControlStepWaiting = true;

                // Since refreshSteps disables skip button for the active step if either nextControlStepWaiting
                // or skipControlStepWaiting are raised, refresh the steps after setting the above flag.
                // Disabling the skip button prevents user from creating additional instructions
                // to move to next step
                refreshSteps(chamberID);

                runNextControlStep(chamberID, false, false, false);
            }
        }

        private void targetVelocity_ValueChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            chambers[activeChamberID_].steps[stepNumber].targetFanSpeed = Convert.ToDouble((sender as NumericUpDown).Value);
        }

        private void stepStatus_SelectedIndexChanged(object sender, EventArgs e, UInt16 stepNumber)
        {
            switch (Convert.ToString((sender as ComboBox).SelectedItem))
            {
                case HumidityStep_Control.CONTROL_STEP_STATUS_COMPLETED:
                    chambers[activeChamberID_].steps[stepNumber].status = HumidityStep.stepStatus.STATUS_COMPLETED;
                    break;
                case HumidityStep_Control.CONTROL_STEP_STATUS_RUNNING:
                    chambers[activeChamberID_].steps[stepNumber].status = HumidityStep.stepStatus.STATUS_RUNNING;
                    break;
                case HumidityStep_Control.CONTROL_STEP_STATUS_WAITING:
                default:
                    chambers[activeChamberID_].steps[stepNumber].status = HumidityStep.stepStatus.STATUS_WAITING;
                    break;
            }
        }


        private void deleteButton_Click(object sender, EventArgs e, UInt16 stepNumber)
        {
            if ((sender as Button).Enabled)
            {
                updateButtonEnabled((sender as Button), false);

                controlsUpdating_ = true;

                for (int i = (stepNumber + 1); i < chambers[activeChamberID_].steps.Count; i++)
                {
                    chambers[activeChamberID_].steps[i].order = Convert.ToUInt16(i - 1);
                }

                chambers[activeChamberID_].steps.RemoveAt(stepNumber);

                chambers[activeChamberID_].sortSteps();
                refreshSteps(activeChamberID_);

                controlsUpdating_ = false;
            }
        }

        private void skipButton_Click(object sender, EventArgs e, UInt16 stepNumber)
        {
            // Sanity check if this button is currently displayed, and is for the active step
            if ((sender as Button).Enabled && stepNumber == chambers[activeChamberID_].activeStep)
            {
                // Disable the skip button
                updateButtonEnabled((sender as Button), false);

                // Stop timer
                chambers[activeChamberID_].stepTimer.Stop();

                // Flag as waiting for the step to be skipped
                chambers[activeChamberID_].skipControlStepWaiting = true;

                // Find for the next control step and run it
                runNextControlStep(activeChamberID_, false, true, false);
            }
        }

        private void Other_graph_interval_value_ValueChanged(object sender, EventArgs e)
        {
            chartSpacingMinute_ = Convert.ToInt16(Graph_interval_value.Value);
            redrawDisplaychart(Displaychart);
        }

        // Interval for checking connection to Arduino has elapsed
        private void connectionCheckTimerElapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            // Check if the previous check has been responded to. Make appropriate changes to controls if
            // haven't received response
            if (!receivedConnectionResponse_)
            {
                handleArduinoConnection(false);
            }

            // Send the next connection check
            receivedConnectionResponse_ = false;
            commands_.sendCommandConnectionArduino(true);
        }

        // Confirm with user before closing the program
        // Also places Arduino and chambers to idle state
        private void AgenatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string message = "Are you sure that you want to close the program? ALL UNSAVED DATA WILL BE LOST.\n\nPress Yes to close; press No to resume the program.";
            const string caption = "Close program";
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                // User cancelled closing
                e.Cancel = true;
            }
            else
            {
                // Stop all timers so they don't raise further events
                connectionCheckTimer_.Stop();
                foreach (Chamber chamber in chambers)
                {
                    chamber.stepTimer.Stop();
                }

                // User closes program; stop all chamber operation
                if (arduinoConnected_ && threadsStarted)
                {
                    // Inform the Arduino the C# program is closing, so put to idle and stop sending serial updates
                    commands_.queueCommandDisconnect(false);

                    // Give ample time for the command to be sent out
                    System.Threading.Thread.Sleep(1000);

                    // Stop all infinite loops
                    commands_.stopCommandOut();
                    commands_.stopCommandIn();
                    errorLogger_.stopLoggingErrors();
                    commandLogger_.stopLoggingCommands();

                    // Give time for the infinite loops to finish current iteration and hit the flag check
                    System.Threading.Thread.Sleep(500);

                    // End the threads
                    commandLogWorker_.Join();
                    errorLogWorker_.Join();
                    commandInWorker_.Join();
                    commandOutWorker_.Join();
                }
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // State-modifying functions
        //
        // These functions modify the state of the program and corresponding controls.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void updateArduinoConnection()
        {
            receivedConnectionResponse_ = true;
        }

        private void handleArduinoConnection(bool connected)
        {
            if (connected)
            {// Arduino was previously disconnected, but has now reconnected
                /* The necessary tasks for resuming chamber operations will be handled
                 * by other functions. resetArduino asks for chamber connection status,
                 * whose response (success/fail) would trigger handleChamberConnection,
                 * which would then:
                 * a) If chamber is connected, send commands to resume DAQ/control. The
                 *    form controls would be updated accordingly based on the response to
                 *    the commands.
                 * b) If chamber is disconnected, the appropriate flags would be set and
                 *    updateControlsChamberDisconnect would be called to modify form controls
                 */ 
                arduinoConnected_ = true;
            }
            else
            {// Lost connection to Arduino.
                arduinoConnected_ = false;

                // Consider all chambers as disconnected
                // This also pauses all DAQ/controls and step timers
                foreach (Chamber chamber in chambers)
                {
                    handleChamberConnection(chamber.id, false);
                }

                // Now, reset the Arduino and the port connection
                try
                {
                    resetArduino();
                }
                catch (Exception err)
                {
                    // Lazy exception catching for now
                    errorLogger_.logUnknownError(err);
                }
            }
        }

        private void resumeChamberState(int chamberID, bool showError)
        {
            if (chambers[chamberID].chamberConnected)
            {
                if (chambers[chamberID].DAQRunning)
                {// DAQ was previously running. Resume DAQ.
                    // First, clear all DAQ state flags, as if we are attempting to start DAQ from scratch
                    chambers[chamberID].DAQPaused = false;
                    chambers[chamberID].DAQRunning = false;

                    if (chambers[chamberID].controlRunning)
                    {// Control was running. Resume control after DAQ is confirmed to be running.
                     // First, clear all control state flags, as if we are attempting to start control from scratch
                        chambers[chamberID].controlPaused = false;
                        chambers[chamberID].controlRunning = false;

                        // Ask to begin DAQ, and once it is running, commands_ will
                        // internally ask to begin control mode
                        startDAQ(chamberID, showError, true);

                        // Upon receiving status of this command, changes to controls will be performed
                        // by updateControlsDAQSuccess/Fail and updateControlsControlSuccess/Fail
                    }
                    else
                    {// DAQ mode was running
                     // Ask to only begin DAQ
                        startDAQ(chamberID, showError, false);

                        // Upon receiving status of this command, changes to controls will be performed
                        // by updateControlsDAQSuccess/Fail
                    }
                }
                else
                {// Idle mode
                    if (chamberID == activeChamberID_)
                    {
                        updateControlsStateIdle(chamberID);
                    }
                }
            }
        }

        private void handleChamberConnection(int chamberID, bool connected)
        {
            if (connected)
            {// Chamber is connected
                chambers[chamberID].chamberConnected = true;

                if (chamberID == activeChamberID_)
                {
                    updateConnectionStatusTextbox(chamberID);
                }

                // Resume the previous state of the chamber
                resumeChamberState(chamberID, false);
            }
            else
            {// Chamber is disconnected
                chambers[chamberID].chamberConnected = false;

                // If control is running, the stuff to be disabled is slightly different than when only DAQ is running
                if (chambers[chamberID].controlRunning)
                {
                    // Setting DAQPaused to true still allows the user to start/stop recording while waiting for chamber reconnection.
                    // There is a check in the start recording event handler that prevents sending commands to the Arduino
                    // if the DAQ is paused.
                    chambers[chamberID].stepTimer.Stop();
                    chambers[chamberID].DAQPaused = true;
                    chambers[chamberID].controlPaused = true;
                }
                else if (chambers[chamberID].DAQRunning && !chambers[chamberID].controlRunning)
                {
                    // Setting DAQPaused to true still allows the user to start/stop recording while waiting for chamber reconnection.
                    // There is a check in the start recording event handler that prevents sending commands to the Arduino
                    // if the DAQ is paused.
                    chambers[chamberID].DAQPaused = true;
                }

                // Consider recording as paused
                if (chambers[chamberID].recording)
                {
                    chambers[chamberID].recordPaused = true;
                }

                // Update the controls
                updateControlsChamberDisconnect(chamberID);
            }
        }
        
        private void updateControlsStateIdle(int chamberID)
        {
            // Prepare to update controls
            controlsUpdating_ = true;

            // Modify appearance of DAQ section
            suspendLayoutControl(Data_DAQ_flow);
            updateButtonVisibility(Data_DAQ_start, true);
            updateButtonEnabled(Data_DAQ_start, true);
            updateButtonVisibility(Data_DAQ_stop, false);
            updateButtonEnabled(Data_DAQ_stop, false);
            changeControlBackColor(Data_DAQ_flow, SystemColors.Control);
            updateNumericEnabled(Data_DAQ_interval_value, true);
            Data_DAQ_flow.Controls.SetChildIndex(Data_DAQ_stop, Data_DAQ_flow.Controls.GetChildIndex(Data_DAQ_stop) + 1);
            resumeLayoutControl(Data_DAQ_flow);
            Data_DAQ_flow.PerformLayout();


            // Recording section
            // This should be handled by the calling function by calling stopRecording()

            // Weighing scale section
            updateButtonEnabled(Data_save_flow_start, false);
            updateButtonEnabled(WeighScale_flow_tare, false);
            updateButtonEnabled(WeighScale_flow_cal_button, false);

            // Humidity sensor calibration
            updateButtonEnabled(Advanced_humidity_RH1_std_apply, false);
            updateButtonEnabled(Advanced_humidity_RH2_std_apply, false);

            // Chamber control section
            updateButtonEnabled(HumidityControl_pause, false);
            updateButtonVisibility(HumidityControl_pause, false);
            updateButtonVisibility(HumidityControl_start, true);
            updateButtonEnabled(HumidityControl_start, false);
            changeControlBackColor(HumidityControl, SystemColors.Control);

            // Finish up updating controls
            controlsUpdating_ = false;

            // Refresh the control steps
            refreshSteps(chamberID);
        }

        private void updateControlsStateDAQ(int chamberID)
        {
            // Prepare to update controls
            controlsUpdating_ = true;

            // Modify appearance of DAQ section
            suspendLayoutControl(Data_DAQ_flow);
            updateButtonVisibility(Data_DAQ_start, false);
            updateButtonEnabled(Data_DAQ_start, false);
            updateButtonVisibility(Data_DAQ_stop, true);
            updateButtonEnabled(Data_DAQ_stop, true);
            changeControlBackColor(Data_DAQ_flow, Color.LightGreen);
            updateNumericEnabled(Data_DAQ_interval_value, false);
            Data_DAQ_flow.Controls.SetChildIndex(Data_DAQ_start, Data_DAQ_flow.Controls.GetChildIndex(Data_DAQ_start) + 1);
            resumeLayoutControl(Data_DAQ_flow);
            Data_DAQ_flow.PerformLayout();

            // Enable the other sections that require DAQ
            updateButtonEnabled(Data_save_flow_start, true);
            updateButtonEnabled(WeighScale_flow_tare, true);
            updateButtonEnabled(WeighScale_flow_cal_button, true);

            // Humidity sensor calibration
            updateButtonEnabled(Advanced_humidity_RH1_std_apply, true);
            updateButtonEnabled(Advanced_humidity_RH2_std_apply, true);

            // Chamber control section
            updateButtonEnabled(HumidityControl_pause, false);
            updateButtonVisibility(HumidityControl_pause, false);
            updateButtonVisibility(HumidityControl_start, true);
            updateButtonEnabled(HumidityControl_start, true);
            changeControlBackColor(HumidityControl, SystemColors.Control);

            // Finish up updating controls
            controlsUpdating_ = false;

            // Refresh the control steps
            refreshSteps(chamberID);
        }

        private void updateControlsStateControl(int chamberID)
        {
            // Prepare to update controls
            controlsUpdating_ = true;

            // Update the area for starting/pausing control
            updateButtonVisibility(HumidityControl_start, false);
            updateButtonEnabled(HumidityControl_pause, true);
            updateButtonVisibility(HumidityControl_pause, true);
            changeControlBackColor(HumidityControl, Color.LightGreen);

            // Update the steps controls
            refreshSteps(chamberID);

            controlsUpdating_ = false;
        }

        private void updateControlsIdleSuccess(int chamberID)
        {
            // No longer waiting for confirmation on stop DAQ command
            chambers[chamberID].stopDAQWaiting = false;

            if (chamberID == activeChamberID_)
            {
                updateControlsStateIdle(chamberID);

                // Flag DAQ as not running
                chambers[chamberID].DAQRunning = false;

                // If recording is ongoing, kill it
                if (chambers[chamberID].recording)
                {
                    // Stop recording of actively-displayed chamber
                    stopRecording(chamberID, true);
                }
            }
            else
            {
                // Flag DAQ as not running
                chambers[chamberID].DAQRunning = false;

                // If recording is ongoing, kill it
                if (chambers[chamberID].recording)
                {
                    // Not the active chamber
                    stopRecording(chamberID, false);
                }
            }
        }

        private void updateControlsIdleFail(int chamberID)
        {// Command was not executed.

            // No longer waiting for confirmation on stop DAQ command
            chambers[chamberID].stopDAQWaiting = false;

            // Revert the initial changes made to the controls if this is the actively displayed chamber
            if (chamberID == activeChamberID_)
            {
                updateControlsStateDAQ(chamberID);
            }

            // If the command wasn't successful, inform the user of the failure
            if (chambers[chamberID].showCommandError)
            {
                MessageBox.Show("Failed to stop data acquisition for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");
            }
        }

        private void updateControlsDAQSuccess(int chamberID)
        {
            if (chambers[chamberID].startDAQWaiting)
            {// Start DAQ was successful
             // Command was successfully executed; update controls accordingly

                // Clear waiting flag
                chambers[chamberID].startDAQWaiting = false;

                if (chamberID == activeChamberID_)
                {
                    // This is the actively-displayed chamber, so update the looks of the form
                    updateControlsStateDAQ(chamberID);

                    // Flag DAQ as running
                    chambers[chamberID].DAQRunning = true;

                    // Update chart
                    controlsUpdating_ = true;

                    try
                    {
                        // Reset the chart data
                        chambers[chamberID].DAQStart = DateTime.Now;
                        chambers[chamberID].chartTime.Clear();
                        chambers[chamberID].chartRH.Clear();
                        chambers[chamberID].chartWeight.Clear();
                        chambers[chamberID].plottedPoints = 0;

                        if (Displaychart.Series["Relative humidity"].Points.Count > 0)
                        {
                            Displaychart.Series["Relative humidity"].Points.Clear();
                        }

                        if (Displaychart.Series["Weight"].Points.Count > 0)
                        {
                            Displaychart.Series["Weight"].Points.Clear();
                        }
                    }
                    catch (Exception err)
                    {
                        errorLogger_.logUnknownError(err);
                    }

                    // Finish up updating controls
                    controlsUpdating_ = false;
                }
                else
                {
                    // Not actively-displayed chamber
                    // Flag DAQ as running
                    chambers[chamberID].DAQRunning = true;

                    try
                    {
                        // Reset the chart data
                        chambers[chamberID].DAQStart = DateTime.Now;
                        chambers[chamberID].chartTime.Clear();
                        chambers[chamberID].chartRH.Clear();
                        chambers[chamberID].chartWeight.Clear();
                        chambers[chamberID].plottedPoints = 0;
                    }
                    catch (Exception err)
                    {
                        errorLogger_.logUnknownError(err);
                    }
                }

                // Start recording if we are supposed to. This only happens when loading autosaves.
                // Under normal program runs, this would not occur.
                // And only do this if record wasn't already running and is currently paused (due to chamber disconnection).
                // Otherwise, startRecording will attempt to re-open streamwriter to the file,
                // which is already open.
                if (!chambers[chamberID].recordPaused)
                {
                    if (chambers[chamberID].recording)
                    {
                        startRecording(chamberID, chamberID == activeChamberID_);
                    }
                }
                else
                {
                    chambers[chamberID].recordPaused = false;
                }
            }
            else if (chambers[chamberID].endControlWaiting || chambers[chamberID].pauseControlWaiting)
            {// Stop control was successful
                if (chambers[chamberID].endControlWaiting)
                {// End of program, so active step is done
                    chambers[chamberID].steps[chambers[chamberID].activeStep].status = HumidityStep.stepStatus.STATUS_COMPLETED;
                }
                else if (chambers[chamberID].pauseControlWaiting)
                {// Pausing the controls, so set the active step to waiting
                    chambers[chamberID].steps[chambers[chamberID].activeStep].status = HumidityStep.stepStatus.STATUS_WAITING;
                }

                // Clear all involved waiting flags, including the skip and next control steps
                // since they could be set before calling runnextcontrolstep() which ends up
                // stopping the control due to no remaining steps to run
                chambers[chamberID].endControlWaiting = false;
                chambers[chamberID].pauseControlWaiting = false;
                chambers[chamberID].skipControlStepWaiting = false;
                chambers[chamberID].nextControlStepWaiting = false;

                // Remove timer conditions
                chambers[chamberID].stepTimer.Elapsed -= chambers[chamberID].stepTimerElapsedHandler;
                chambers[chamberID].stepTimerElapsedHandler = null;

                // Flag control as stopped running
                chambers[chamberID].controlRunning = false;

                // Indicate no steps are actively running
                chambers[chamberID].activeStep = -1;

                // Update controls to reflect non-control state
                if (chamberID == activeChamberID_)
                {
                    updateControlsStateDAQ(chamberID);
                }
            }
        }

        private void updateControlsDAQFail(int chamberID)
        {
            if (chambers[chamberID].startDAQWaiting)
            {// User wanted to start DAQ
             // Revert the initial changes made to the controls if this is the actively displayed chamber

                // Clear waiting flag
                chambers[chamberID].startDAQWaiting = false;

                // Update controls, revert back to idle state
                if (chamberID == activeChamberID_)
                {
                    updateControlsStateIdle(chamberID);
                }

                // Inform the user of the failure
                if (chambers[chamberID].showCommandError)
                {
                    MessageBox.Show("Failed to start data acquisition for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");
                }
            }
            else if (chambers[chamberID].endControlWaiting || chambers[chamberID].pauseControlWaiting)
            {// Program was automatically ending control program or user was trying to pause control

                if (chambers[chamberID].pauseControlWaiting)
                {// This case is not too serious, since user click the button, he/she is there
                 // to handle the chamber not stopping

                    // Clear waiting flag
                    chambers[chamberID].pauseControlWaiting = false;

                    // Update controls, revert back to control state
                    if (chamberID == activeChamberID_)
                    {
                        updateControlsStateControl(chamberID);
                    }

                    // Let user know of this error
                    MessageBox.Show("Failed to pause control for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");

                    // Clear flag for waiting for pause control command
                    chambers[chamberID].pauseControlWaiting = false;

                    // Continue counting down the timer
                    chambers[chamberID].stepTimer.Start();
                }
                else if (chambers[chamberID].endControlWaiting)
                {// But if the program was trying to end control by itself due to finishing
                 // all the steps, keep trying until succeed in ending control because this
                 // situation could arise in automatic fashion by the program exhausting all
                 // control steps. Note this could result in infinite loop (but we are using
                 // events, so program won't hang).

                    // Clear waiting flag
                    chambers[chamberID].endControlWaiting = false;

                    // Resend command
                    stopControlStep(chamberID, true);

                    // We don't restart the timer here since we are at the end of the
                    // control program already
                }
            }
        }

        private void updateControlsControlSuccess(int chamberID)
        {
            // No longer waiting for confirmation on begin control mode command
            chambers[chamberID].startControlWaiting = false;

            // Register this step as the active one
            int activeStep = chambers[chamberID].tempActiveStep;
            chambers[chamberID].activeStep = activeStep;
            chambers[chamberID].steps[activeStep].status = HumidityStep.stepStatus.STATUS_RUNNING;

            // Only add an event handler if it doesn't already exist. The event handler could already exist
            // if we are resuming control after a chamber reconnects.
            if (chambers[chamberID].stepTimerElapsedHandler == null)
            {
                chambers[chamberID].stepTimerElapsedHandler = (sender, e) => { stepTimer_Elapsed(sender, e, chamberID, Convert.ToUInt16(activeStep)); };
                chambers[chamberID].stepTimer.Elapsed += chambers[chamberID].stepTimerElapsedHandler;
            }

            // Flag control as running
            chambers[chamberID].controlRunning = true;
            chambers[chamberID].controlPaused = false;
            
            // Modify controls as necessary
            if (chamberID == activeChamberID_)
            {
                updateControlsStateControl(chamberID);
            }

            // Start step timer once everything is done
            chambers[chamberID].stepTimer.Start();
        }

        private void updateControlsControlFail(int chamberID)
        {// Command failed to execute

            // No longer waiting for confirmation on begin control mode command
            chambers[chamberID].startControlWaiting = false;

            // Revert controls
            if (chamberID == activeChamberID_)
            {
                updateControlsStateDAQ(chamberID);
            }

            // Inform the user of the failure
            if (chambers[chamberID].showCommandError)
            {
                MessageBox.Show("Failed to start control for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");
            }
        }

        private void updateControlsSetpointSuccess(int chamberID)
        {
            if (chambers[chamberID].nextControlStepWaiting || chambers[chamberID].skipControlStepWaiting)
            {// This was a change in step, either by end of step timer or user skipping (as opposed to ramping setpoint)
             // Notice the changes made here are same whether the step change is caused by
             // end of timer or user skipping
                chambers[chamberID].steps[chambers[chamberID].activeStep].status = HumidityStep.stepStatus.STATUS_COMPLETED;
                chambers[chamberID].stepTimer.Elapsed -= chambers[chamberID].stepTimerElapsedHandler;
                chambers[chamberID].stepTimerElapsedHandler = null;

                // Make changes to recognize this new step as the active step
                int activeStep = chambers[chamberID].tempActiveStep;
                chambers[chamberID].activeStep = activeStep;
                chambers[chamberID].steps[activeStep].status = HumidityStep.stepStatus.STATUS_RUNNING;

                // Workaround to replace the previous event handler with a new one (Direct assignment doesn't work).
                chambers[chamberID].stepTimerElapsedHandler = (sender, e) => { stepTimer_Elapsed(sender, e, chamberID, Convert.ToUInt16(activeStep)); };
                chambers[chamberID].stepTimer.Elapsed += chambers[chamberID].stepTimerElapsedHandler;

                // Adjust the flags to indicate no longer waiting
                if (chambers[chamberID].nextControlStepWaiting)
                {
                    chambers[chamberID].nextControlStepWaiting = false;
                }
                // Do the same for the skip flag, if it was set
                if (chambers[chamberID].skipControlStepWaiting)
                {
                    chambers[chamberID].skipControlStepWaiting = false;
                }

                // Update the steps controls
                refreshSteps(chamberID);

                // Start step timer once everything is done
                chambers[chamberID].stepTimer.Start();
            }
            else if (chambers[chamberID].rampControlStepWaiting)
            {// Successfully changed ramp setpoint
             // There's nothing much that needs to be done here; calculateAndStoreRHSetpoint()
             // already stored the new temp setpoint in the appropriate chamber instance.
                chambers[chamberID].rampControlStepWaiting = false;

                // No need to restart timer because ramp setpoint changes do not stop the timer
            }
        }

        private void updateControlsSetpointFail(int chamberID)
        {
            // Note that the step timer is running if the change in setpoint is called by stepTimer_Elapsed for a ramp setpoint change,
            // but is stopped if its due to the end of a control step (also by stepTimer_Elapsed), or a skip step button is clicked.
            if (chambers[chamberID].skipControlStepWaiting)
            {// Failed to skip the current step, so just continue on with the current step
                // Flag as no longer waiting
                chambers[chamberID].skipControlStepWaiting = false;

                // Remove the nextControlStepWaiting flag just as an extra measure
                chambers[chamberID].nextControlStepWaiting = false;

                // Refresh the steps controls to re-enable the skip button in the event that the user tried to skip the previous step
                refreshSteps(chamberID);

                // Inform the user of the failure
                if (chambers[chamberID].showCommandError)
                {
                    MessageBox.Show("Failed to skip control step for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");
                }

                // Restart the step timer that was paused when clicking the skip button
                chambers[chamberID].stepTimer.Start();
            }
            else if (chambers[chamberID].nextControlStepWaiting)
            {// Failed to move to the next step after timer is over
             // Now, this is a big problem, since the program automatically does this
             // and user may not be available. So keep retrying until we (hopefully) get it done.
             // Note this could result in infinite loop (but we are using
             // queues and events, so program won't hang).
                runNextControlStep(chamberID, false, false, false);
            }
            else if (chambers[chamberID].rampControlStepWaiting)
            {// Failed to move to change the setpoint during a ramp step
             // Since the ramp setpoints are continually changed during the entire step,
             // don't need to do anything drastic here; the next ramp setpoint
             // may work. If all of the ramp setpoints don't work, then there's no use repeating
             // this attempt anyway.
                chambers[chamberID].rampControlStepWaiting = false;
                // One thing to note is that the temp setpoint was updated by calculateAndStoreRHSetpoint();
                // but this shouldn't make a big difference in the timespan between one failed setpoint
                // change and a subsequent successful setpoint change.

                // No need to restart timer because ramp setpoint changes do not stop the timer
            }
        }

        private void updateControlsRHCalibrateSuccess(int chamberID)
        {
            if (chamberID == activeChamberID_)
            {
                // This is the actively-displayed chamber, so disable the buttons
                controlsUpdating_ = true;
                updateButtonEnabled(Advanced_humidity_RH1_std_apply, true);
                updateNumericEnabled(Advanced_humidity_RH1_std_value, true);
                updateButtonEnabled(Advanced_humidity_RH2_std_apply, true);
                updateNumericEnabled(Advanced_humidity_RH2_std_value, true);
                if (chambers[chamberID].RH1CalibrateWaiting)
                {
                    updateNumericValue(Advanced_humidity_RH1_raw_value, Convert.ToDecimal(chambers[chamberID].RHRaw1));
                }
                else if (chambers[chamberID].RH2CalibrateWaiting)
                {
                    updateNumericValue(Advanced_humidity_RH2_raw_value, Convert.ToDecimal(chambers[chamberID].RHRaw2));
                }
                controlsUpdating_ = false;
            }
            
            // No longer waiting for confirmation on calibrate command
            chambers[chamberID].RH1CalibrateWaiting = false;
            chambers[chamberID].RH2CalibrateWaiting = false;
        }

        private void updateControlsRHCalibrateFail(int chamberID)
        {
            if (chamberID == activeChamberID_)
            {
                // This is the actively-displayed chamber, so disable the buttons
                controlsUpdating_ = true;
                updateButtonEnabled(Advanced_humidity_RH1_std_apply, true);
                updateNumericEnabled(Advanced_humidity_RH1_std_value, true);
                updateButtonEnabled(Advanced_humidity_RH2_std_apply, true);
                updateNumericEnabled(Advanced_humidity_RH2_std_value, true);
                controlsUpdating_ = false;
            }

            // No longer waiting for confirmation on calibrate command
            chambers[chamberID].RH1CalibrateWaiting = false;
            chambers[chamberID].RH2CalibrateWaiting = false;

            // If the command wasn't successful, inform the user of the failure
            if (chambers[chamberID].showCommandError)
            {
                MessageBox.Show("Failed to calibrate the humidity sensor for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");
            }
        }

        private void updateControlsScaleTareSuccess(int chamberID)
        {
            if (chamberID == activeChamberID_)
            {
                // This is the actively-displayed chamber, so enable the buttons
                controlsUpdating_ = true;
                updateButtonEnabled(WeighScale_flow_tare, true);
                controlsUpdating_ = false;
            }

            // No longer waiting for confirmation on tare command
            chambers[chamberID].scaleTareWaiting = false;
        }

        private void updateControlsScaleTareFail(int chamberID)
        {
            if (chamberID == activeChamberID_)
            {
                // This is the actively-displayed chamber, so enable the buttons
                controlsUpdating_ = true;
                updateButtonEnabled(WeighScale_flow_tare, true);
                controlsUpdating_ = false;
            }

            // No longer waiting for confirmation on tare command
            chambers[chamberID].scaleTareWaiting = false;

            // If the command wasn't successful, inform the user of the failure
            if (chambers[chamberID].showCommandError)
            {
                MessageBox.Show("Failed to tare the scale for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");
            }
        }

        private void updateControlsScaleCalibrateSuccess(int chamberID)
        {
            if (chamberID == activeChamberID_)
            {
                // This is the actively-displayed chamber, so disable the buttons
                controlsUpdating_ = true;
                updateButtonEnabled(WeighScale_flow_cal_button, true);
                updateNumericEnabled(WeighScale_flow_cal_value, true);
                controlsUpdating_ = false;
            }

            // No longer waiting for confirmation on calibrate command
            chambers[chamberID].scaleCalibrateWaiting = false;
        }

        private void updateControlsScaleCalibrateFail(int chamberID)
        {
            if (chamberID == activeChamberID_)
            {
                // This is the actively-displayed chamber, so disable the buttons
                controlsUpdating_ = true;
                updateButtonEnabled(WeighScale_flow_cal_button, true);
                updateNumericEnabled(WeighScale_flow_cal_value, true);
                controlsUpdating_ = false;
            }

            // No longer waiting for confirmation on calibrate command
            chambers[chamberID].scaleCalibrateWaiting = false;

            // If the command wasn't successful, inform the user of the failure
            if (chambers[chamberID].showCommandError)
            {
                MessageBox.Show("Failed to calibrate the scale for chamber " + chamberID.ToString() + ". Try again or restart the program", "Error");
            }
        }

        // Updates the scale factor for the given chamber and replaces the value in the displayed textbox
        private void updateScaleFactor(int chamberID, double scaleFactor)
        {
            // Save the scale factor
            chambers[chamberID].scaleFactor = Math.Round(scaleFactor, scaleFactorDecimalPlaces);

            // Update the scale factor textbox
            if (chamberID == activeChamberID_)
            {
                updateNumericValue(Advanced_weigh_factor_value, Convert.ToDecimal(Math.Truncate(chambers[chamberID].scaleFactor * Math.Pow(10, scaleFactorDecimalPlaces)) / Math.Pow(10, scaleFactorDecimalPlaces)));
            }
        }

        // Updates the scale offset for the given chamber and replaces the value in the displayed textbox
        private void updateScaleOffset(int chamberID, int scaleOffset)
        {
            // Save the scale offset
            chambers[chamberID].scaleOffset = scaleOffset;
            
            // Update the scale factor textbox
            if (chamberID == activeChamberID_)
            {
                updateNumericValue(Advanced_weigh_offset_value, Convert.ToDecimal(Math.Truncate(chambers[chamberID].scaleOffset * Math.Pow(10, scaleOffsetDecimalPlaces)) / Math.Pow(10, scaleOffsetDecimalPlaces)));
            }
        }

        private void updateReadings(string fullBuffer, string chamberIDStr, string humidityStr, string temperatureStr, string weightStr, string velocityStr, string fan1SpeedStr, string fan2SpeedStr)
        {
            int chamberID;

            if (int.TryParse(chamberIDStr, out chamberID))
            {
                processAndStoreData(fullBuffer, humidityStr, chamberID, 1); // Humidity
                processAndStoreData(fullBuffer, temperatureStr, chamberID, 2); // Temperature
                processAndStoreData(fullBuffer, weightStr, chamberID, 3); // Weight
                processAndStoreData(fullBuffer, velocityStr, chamberID, 4); // Velocity
                processAndStoreData(fullBuffer, fan1SpeedStr, chamberID, 5); // Fan 1 speed
                processAndStoreData(fullBuffer, fan2SpeedStr, chamberID, 6); // Fan 2 speed

                // The following code is for plotting points on chart and logging data
                // Update the data for plotting the chart
                // Time elapsed since beginning of DAQ (seconds)
                double elapsedTime = Math.Round(DateTime.Now.Subtract(chambers[chamberID].DAQStart).TotalSeconds);

                if (chambers[chamberID].plottedPoints < chambers[chamberID].chartPointsMax)
                {
                    chambers[chamberID].plottedPoints++;
                }
                else
                {
                    chambers[chamberID].chartTime.RemoveAt(0);
                    chambers[chamberID].chartRH.RemoveAt(0);
                    chambers[chamberID].chartWeight.RemoveAt(0);

                    if (chamberID == activeChamberID_)
                    {
                        removeChartPoint(Displaychart, "Relative humidity", 0);
                        removeChartPoint(Displaychart, "Weight", 0);
                    }
                }

                chambers[chamberID].chartTime.Add(elapsedTime);

                if (chambers[chamberID].humidityOK)
                {
                    chambers[chamberID].chartRH.Add(chambers[chamberID].humidity);

                    if (chamberID == activeChamberID_)
                    {
                        addChartXY(Displaychart, "Relative humidity", elapsedTime, chambers[chamberID].humidity);
                    }
                }
                else
                {
                    chambers[chamberID].chartRH.Add(-100000); // Indicates an error!

                    if (chamberID == activeChamberID_)
                    {
                        addChartXY(Displaychart, "Relative humidity", elapsedTime, 0);
                        updateChartPointEmpty(Displaychart, "Relative humidity", Displaychart.Series["Relative humidity"].Points.Count - 1, true);
                    }
                }

                if (chambers[chamberID].weightOK)
                {
                    chambers[chamberID].chartWeight.Add(chambers[chamberID].weight);

                    if (chamberID == activeChamberID_)
                    {
                        addChartXY(Displaychart, "Weight", elapsedTime, chambers[chamberID].weight);
                    }
                }
                else
                {
                    chambers[chamberID].chartWeight.Add(-100000);

                    if (chamberID == activeChamberID_)
                    {
                        addChartXY(Displaychart, "Weight", elapsedTime, 0);
                        updateChartPointEmpty(Displaychart, "Weight", Displaychart.Series["Weight"].Points.Count - 1, true);
                    }
                }

                // Redraw the chart
                if (chamberID == activeChamberID_)
                {
                    if (!controlsUpdating_)
                    {
                        try
                        {
                            redrawDisplaychart(Displaychart);
                        }
                        catch (Exception err)
                        {
                            errorLogger_.logUnknownError(err);
                        }
                    }
                }

                // Log data
                if (chambers[chamberID].recording)
                {
                    string stepNumber;
                    string stepType;
                    string RHCurSetpoint;
                    string velocityCurSetpoint;
                    string humidityValue;
                    string temperatureValue;
                    string weightValue;
                    string velocityValue;
                    string fan1SpeedValue;
                    string fan2SpeedValue;

                    if (!chambers[chamberID].controlPaused && chambers[chamberID].controlRunning)
                    {
                        stepNumber = string.Format("{0:d}", (chambers[chamberID].activeStep + 1));    // Start counting from 1
                        RHCurSetpoint = string.Format("{0:f1}", chambers[chamberID].RHCurSetpoint);
                        velocityCurSetpoint = string.Format("{0:f1}", chambers[chamberID].fanSpeedCurSetpoint);

                        if (chambers[chamberID].steps[chambers[chamberID].activeStep].type == HumidityStep.stepType.TYPE_HOLD)
                        {
                            stepType = "Hold";
                        }
                        else
                        {
                            stepType = "Ramp";
                        }
                    }
                    else
                    {
                        stepNumber = "";
                        stepType = "";
                        RHCurSetpoint = "";
                        velocityCurSetpoint = "";
                    }

                    if (chambers[chamberID].humidityOK)
                    {
                        humidityValue = string.Format("{0:f1}", chambers[chamberID].humidity);
                    }
                    else
                    {
                        humidityValue = "";
                    }

                    if (chambers[chamberID].temperatureOK)
                    {
                        temperatureValue = string.Format("{0:f1}", chambers[chamberID].temperature);
                    }
                    else
                    {
                        temperatureValue = "";
                    }

                    if (chambers[chamberID].weightOK)
                    {
                        weightValue = string.Format("{0:f0}", chambers[chamberID].weight);
                    }
                    else
                    {
                        weightValue = "";
                    }

                    if (chambers[chamberID].velocityOK)
                    {
                        velocityValue = string.Format("{0:f1}", chambers[chamberID].velocity);
                    }
                    else
                    {
                        velocityValue = "";
                    }

                    if (chambers[chamberID].fan1SpeedOK)
                    {
                        fan1SpeedValue = string.Format("{0:f0}", chambers[chamberID].fan1Speed);
                    }
                    else
                    {
                        fan1SpeedValue = "";
                    }

                    if (chambers[chamberID].fan2SpeedOK)
                    {
                        fan2SpeedValue = string.Format("{0:f0}", chambers[chamberID].fan2Speed);
                    }
                    else
                    {
                        fan2SpeedValue = "";
                    }


                    try
                    {
                        chambers[chamberID].CSVWriter.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}",
                                                            new Object[] {  DateTime.Now.ToString(),
                                                                                ",",
                                                                                stepNumber,
                                                                                ",",
                                                                                stepType,
                                                                                ",",
                                                                                RHCurSetpoint,
                                                                                ",",
                                                                                velocityCurSetpoint,
                                                                                ",",
                                                                                humidityValue,
                                                                                ",",
                                                                                temperatureValue,
                                                                                ",",
                                                                                weightValue,
                                                                                ",",
                                                                                velocityValue,
                                                                                ",",
                                                                                fan1SpeedValue,
                                                                                ",",
                                                                                fan2SpeedValue,
                                                                                Environment.NewLine });
                    }
                    catch (ArgumentNullException err)
                    {
                        errorLogger_.logCProgError(041, err);
                    }
                    catch (ObjectDisposedException err)
                    {
                        errorLogger_.logCProgError(042, err);
                    }
                    catch (FormatException err)
                    {
                        errorLogger_.logCProgError(043, err);
                    }
                    catch (IOException err)
                    {
                        errorLogger_.logCProgError(044, err);
                    }
                    catch (EncoderFallbackException err)
                    {
                        errorLogger_.logCProgError(045, err);
                    }
                    catch (Exception err)
                    {
                        errorLogger_.logUnknownError(err);
                    }
                }
            }
            else
            {
                errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_DAQ, "updateReadings, chamberID", fullBuffer);
            }
        }

        // Used for extracting readings/errors from Serial data from Arduino
        private void processAndStoreData(string buffer, string reading, int chamberID, int dataCategory)
        {
            try
            {
                bool readingOK = false;
                bool logError = false;
                string textOutput = "";
                double parsedReading = 0;

                if (reading.StartsWith(Commands.SERIAL_REPLY_READING_ERROR))
                {
                    // The reading was unsuccessfully obtained due to an error
                    // The Arduino directly sends a separate error string upon encountering a measurement error
                    // that will be handled by the serialDataReceivedHandler(). This section is just to
                    // let user know there is a reading with error
                    readingOK = false;
                    textOutput = "Error";
                    logError = false;
                }
                else
                {
                    // The substring extracted from the appropriate category of buffer string is an error
                    // but still need to check if it is empty or not.
                    // Additionally, check if there are any characters other than decimal digits or dots in the string.
                    // The @ symbol is to specify a string without having to escape any characters (Otherwise need to use double backstrokes).
                    if (!String.IsNullOrEmpty(reading) || Regex.Matches(reading, @"[^\d\.]").Count > 0)
                    {
                        if (double.TryParse(reading, out parsedReading))
                        {
                            readingOK = true;
                            textOutput = reading;
                            logError = false;
                        }
                        else
                        {
                            // Failed to parse into double
                            readingOK = false;
                            textOutput = "Error";
                            logError = true;
                        }
                    }
                    else
                    {
                        // Something is wrong with the reading
                        readingOK = false;
                        textOutput = "Error";
                        logError = true;
                    }
                }

                switch (dataCategory)
                {
                    case 1: // humidity
                        chambers[chamberID].humidityOK = readingOK;
                        if (readingOK)
                        {
                            chambers[chamberID].humidity = parsedReading;
                        }
                        if (chamberID == activeChamberID_ && !controlsUpdating_)
                        {
                            updateTextBox(Readings_humidity_flow_value, textOutput);
                        }
                        if (logError)
                        {
                            errorLogger_.logCProgError(ErrorLogger.PROG_READING_HUMIDITY, "processAndStoreData", buffer);
                        }
                        break;
                    case 2: // temperature
                        chambers[chamberID].temperatureOK = readingOK;
                        if (readingOK)
                        {
                            chambers[chamberID].temperature = parsedReading;
                        }
                        if (chamberID == activeChamberID_ && !controlsUpdating_)
                        {
                            updateTextBox(Readings_temperature_flow_value, textOutput);
                        }
                        if (logError)
                        {
                            errorLogger_.logCProgError(ErrorLogger.PROG_READING_TEMPERATURE, "processAndStoreData", buffer);
                        }
                        break;
                    case 3: // weight
                        chambers[chamberID].weightOK = readingOK;
                        if (readingOK)
                        {
                            // Round to nearest integer
                            chambers[chamberID].weight = parsedReading;
                        }
                        if (chamberID == activeChamberID_ && !controlsUpdating_)
                        {
                            // Force readings with -0 to be 0
                            if (textOutput.Equals("-0"))
                            {
                                textOutput = "0";
                            }
                            updateTextBox(Readings_weight_flow_value, textOutput);
                        }
                        if (logError)
                        {
                            errorLogger_.logCProgError(ErrorLogger.PROG_READING_WEIGHT, "processAndStoreData", buffer);
                        }
                        break;
                    case 4: // velocity
                        chambers[chamberID].velocityOK = readingOK;
                        if (readingOK)
                        {
                            chambers[chamberID].velocity = parsedReading;
                        }
                        if (chamberID == activeChamberID_ && !controlsUpdating_)
                        {
                            updateTextBox(Readings_velocity_flow_value, textOutput);
                        }
                        if (logError)
                        {
                            errorLogger_.logCProgError(ErrorLogger.PROG_READING_VELOCITY, "processAndStoreData", buffer);
                        }
                        break;
                    case 5: // fan 1 speed
                        chambers[chamberID].fan1SpeedOK = readingOK;
                        if (readingOK)
                        {
                            chambers[chamberID].fan1Speed = parsedReading;
                        }
                        if (chamberID == activeChamberID_ && !controlsUpdating_)
                        {
                            updateTextBox(Readings_fan1Speed_flow_value, textOutput);
                        }
                        if (logError)
                        {
                            errorLogger_.logCProgError(ErrorLogger.PROG_READING_FAN1, "processAndStoreData", buffer);
                        }
                        break;
                    case 6: // fan 2 speed
                        chambers[chamberID].fan2SpeedOK = readingOK;
                        if (readingOK)
                        {
                            chambers[chamberID].fan2Speed = parsedReading;
                        }
                        if (chamberID == activeChamberID_ && !controlsUpdating_)
                        {
                            updateTextBox(Readings_fan2Speed_flow_value, textOutput);
                        }
                        if (logError)
                        {
                            errorLogger_.logCProgError(ErrorLogger.PROG_READING_FAN2, "processAndStoreData", buffer);
                        }
                        break;
                    default:
                        errorLogger_.logCProgError(ErrorLogger.PROG_READING_CATEGORY, "processAndStoreData", buffer);
                        break;
                }
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);
            }
        }
        
        


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Thread-safe control-modifying functions
        //
        // Attempts to modify a control in a thread separate from the thread running the form will result in an error
        // because it is an unsafe practice. These functions ensure that we can modify controls while sticking to
        // safe practices.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private void suspendLayoutControl(Control refControl)
        {
            if (refControl.InvokeRequired)
            {
                suspendLayoutControlCallback d = new suspendLayoutControlCallback(suspendLayoutControl);
                this.Invoke(d, new object[] { refControl });
            }
            else
            {
                refControl.SuspendLayout();
            }
        }

        private void resumeLayoutControl(Control refControl)
        {
            if (refControl.InvokeRequired)
            {
                resumeLayoutControlCallback d = new resumeLayoutControlCallback(resumeLayoutControl);
                this.Invoke(d, new object[] { refControl });
            }
            else
            {
                refControl.ResumeLayout(false);
            }
        }

        private void updateFlowVisibility(FlowLayoutPanel flowControl, bool visible)
        {
            if (flowControl.InvokeRequired)
            {
                updateFlowVisibilityCallback d = new updateFlowVisibilityCallback(updateFlowVisibility);
                this.Invoke(d, new object[] { flowControl, visible });
            }
            else
            {
                flowControl.Visible = visible;
            }
        }

        private void updateTextBox(TextBox boxControl, string data)
        {
            if (boxControl.InvokeRequired)
            {
                updateTextboxCallback d = new updateTextboxCallback(updateTextBox);
                this.Invoke(d, new object[] { boxControl, data });
            }
            else
            {
                boxControl.Text = data;
            }
        }

        private void updateTextBoxEnabled(TextBox textBoxControl, bool enabled)
        {
            if (textBoxControl.InvokeRequired)
            {
                updateTextBoxEnabledCallback d = new updateTextBoxEnabledCallback(updateTextBoxEnabled);
                this.Invoke(d, new object[] { textBoxControl, enabled });
            }
            else
            {
                textBoxControl.Enabled = enabled;
            }
        }

        private void updateNumericValue(NumericUpDown refControl, decimal data)
        {
            if (refControl.InvokeRequired)
            {
                updateNumericValueCallback d = new updateNumericValueCallback(updateNumericValue);
                this.Invoke(d, new object[] { refControl, data });
            }
            else
            {
                refControl.Value = data;
            }
        }

        private void updateNumericEnabled(NumericUpDown numericControl, bool enabled)
        {
            if (numericControl.InvokeRequired)
            {
                updateNumericEnabledCallback d = new updateNumericEnabledCallback(updateNumericEnabled);
                this.Invoke(d, new object[] { numericControl, enabled });
            }
            else
            {
                numericControl.Enabled = enabled;
            }
        }

        private void updateButtonEnabled(Button buttonControl, bool enabled)
        {
            if (buttonControl.InvokeRequired)
            {
                updateButtonEnabledCallback d = new updateButtonEnabledCallback(updateButtonEnabled);
                this.Invoke(d, new object[] { buttonControl, enabled });
            }
            else
            {
                buttonControl.Enabled = enabled;
            }
        }

        private void updateButtonVisibility(Button buttonControl, bool visible)
        {
            if (buttonControl.InvokeRequired)
            {
                updateButtonVisibilityCallback d = new updateButtonVisibilityCallback(updateButtonVisibility);
                this.Invoke(d, new object[] { buttonControl, visible });
            }
            else
            {
                buttonControl.Visible = visible;
            }
        }

        private void updateLabelVisibility(Label labelControl, bool visible)
        {
            if (labelControl.InvokeRequired)
            {
                updateLabelVisibilityCallback d = new updateLabelVisibilityCallback(updateLabelVisibility);
                this.Invoke(d, new object[] { labelControl, visible });
            }
            else
            {
                labelControl.Visible = visible;
            }
        }

        private void changeControlBackColor(Control refControl, Color color)
        {
            if (refControl.InvokeRequired)
            {
                changeControlBackColorCallback d = new changeControlBackColorCallback(changeControlBackColor);
                this.Invoke(d, new object[] { refControl, color });
            }
            else
            {
                refControl.BackColor = color;
            }
        }

        private void changeControlForeColor(Control refControl, Color color)
        {
            if (refControl.InvokeRequired)
            {
                changeControlForeColorCallback d = new changeControlForeColorCallback(changeControlForeColor);
                this.Invoke(d, new object[] { refControl, color });
            }
            else
            {
                refControl.ForeColor = color;
            }
        }

        private void removeChartPoint(Chart chartControl, string seriesName, int pointIndex)
        {
            if (chartControl.InvokeRequired)
            {
                removeChartPointCallback d = new removeChartPointCallback(removeChartPoint);
                this.Invoke(d, new object[] { chartControl, seriesName, pointIndex });
            }
            else
            {
                chartControl.Series[seriesName].Points.RemoveAt(pointIndex);
            }
        }

        private void addChartXY(Chart chartControl, string seriesName, double xVal, double yVal)
        {
            if (chartControl.InvokeRequired)
            {
                addChartXYCallback d = new addChartXYCallback(addChartXY);
                this.Invoke(d, new object[] { chartControl, seriesName, xVal, yVal });
            }
            else
            {
                chartControl.Series[seriesName].Points.AddXY(xVal, yVal);
            }
        }


        private void updateChartPointEmpty(Chart chartControl, string seriesName, int pointIndex, bool empty)
        {
            if (chartControl.InvokeRequired)
            {
                updateChartPointEmptyCallback d = new updateChartPointEmptyCallback(updateChartPointEmpty);
                this.Invoke(d, new object[] { chartControl, seriesName, pointIndex, empty });
            }
            else
            {
                chartControl.Series[seriesName].Points[pointIndex].IsEmpty = empty;
            }
        }

        // Update controls depending on whether the Arduino is connected or not
        private void updateControlsControlBoardConnection()
        {
            if (arduinoConnected_)
            {   // Enable the appropriate controls such as DAQ and environmental control
                //TODO
            }
            else
            {   // Disable most controls such as DAQ and environmental control

            }
        }

        // Update controls for a disconnected chamber
        private void updateControlsChamberDisconnect(int chamberID)
        {
            // Only update the controls if this is the actively displayed chamber
            if (chamberID == activeChamberID_)
            {
                if (chambers[chamberID].DAQPaused && chambers[chamberID].controlPaused)
                {   // Control (and DAQ) is paused
                    updateButtonEnabled(HumidityControl_pause, false);
                    refreshSteps(chamberID);
                }
                else if (chambers[chamberID].DAQPaused)
                {   // Only DAQ is paused
                    updateButtonEnabled(Data_DAQ_stop, false);
                    updateButtonEnabled(WeighScale_flow_tare, false);
                    updateButtonEnabled(WeighScale_flow_cal_button, false);
                    updateButtonEnabled(HumidityControl_start, false);

                    // Humidity sensor calibration
                    updateButtonEnabled(Advanced_humidity_RH1_std_apply, false);
                    updateButtonEnabled(Advanced_humidity_RH2_std_apply, false);
                }
                else
                {   // Chamber was in idle state. Disable the DAQ start button (DAQ is necessary for
                    // most of the other sections to come online)
                    updateButtonEnabled(Data_DAQ_start, false);
                }

                updateConnectionStatusTextbox(chamberID);
            }
        }

        // Update the status textbox on the form
        private void updateConnectionStatusTextbox(int chamberID)
        {
            // Check for connection to Arduino first (since that overrules everything else)
            if (!arduinoConnected_)
            {
                updateTextBox(Status_value, STATUS_CONTROL_DC);
                changeControlBackColor(Status_value, Color.Red);
                changeControlForeColor(Status_value, Color.White);
            }
            else if (!chambers[chamberID].chamberConnected)
            {   // If Arduino is connected, then only check for chamber connection
                updateTextBox(Status_value, STATUS_CHAMBER_DC);
                changeControlBackColor(Status_value, Color.Red);
                changeControlForeColor(Status_value, Color.White);
            }
            else
            {   // Everything is good
                updateTextBox(Status_value, STATUS_OK);
                changeControlBackColor(Status_value, SystemColors.Control);
                changeControlForeColor(Status_value, SystemColors.WindowText);
            }
        }

        private void redrawDisplaychart(Chart chartControl)
        {
            if (chartControl.InvokeRequired)
            {
                redrawDisplaychartCallback d = new redrawDisplaychartCallback(redrawDisplaychart);
                this.Invoke(d, new object[] { chartControl });
            }
            else
            {
                try
                {
                    double elapsedTime = chartControl.Series["Relative humidity"].Points[chartControl.Series["Relative humidity"].Points.Count - 1].XValue;
                    double minTime = chartControl.Series["Relative humidity"].Points[0].XValue;

                    for (int i = 0; i < chartControl.ChartAreas[0].AxisX.CustomLabels.Count; i++)
                    {
                        // Must convert the spacings to seconds because the data acquisition rate is in seconds
                        chartControl.ChartAreas[0].AxisX.CustomLabels[i].Text = Convert.ToString(i * chartSpacingMinute_);

                        // The position is defined in terms of the actual data points. So using a fixed number to define the position is not good because
                        // the spacing becomes smaller as more data points are squeezed into the x-axis. It's better to scale the spacing with chartSpacingMinute_
                        chartControl.ChartAreas[0].AxisX.CustomLabels[i].FromPosition = elapsedTime - (i * chartSpacingMinute_ * 60) + (10 * chartSpacingMinute_);
                        chartControl.ChartAreas[0].AxisX.CustomLabels[i].ToPosition = elapsedTime - (i * chartSpacingMinute_ * 60) - (10 * chartSpacingMinute_);
                    }

                    chartControl.ChartAreas[0].AxisX.Minimum = elapsedTime - (chartControl.ChartAreas[0].AxisX.CustomLabels.Count - 1) * chartSpacingMinute_ * 60;
                    chartControl.ChartAreas[0].AxisX.Maximum = elapsedTime;

                    chartControl.Refresh();
                }
                catch (Exception err)
                {
                    errorLogger_.logUnknownError(err);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Interfacing functions
        //
        // Allow other classes to access certain private vars in this class
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int getActiveChamberID()
        {
            return activeChamberID_;
        }

        public bool getArduinoConnectionStatus()
        {
            return arduinoConnected_;
        }

        public void setArduinoConnectionStatus(bool connected)
        {
            arduinoConnected_ = connected;
        }
    }// END AgenatorForm class def.


}// END Agenator namespace
