using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenator
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Commands
    // Handles all communication with the Arduino
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class Commands
    {
        // Default settings for sending commands
        private const int DEFAULT_COMMAND_MAXATTEMPTS       = 3;    // The default max number of times to attempt a command.
        private const int DEFAULT_COMMAND_RESPONSETIMEOUT   = 1500;  // The default time to be considered as timeout (ms).

        // Codes used during communication with Arduino
        public const string SERIAL_CMD_START                   = "^";
        public const string SERIAL_CMD_CONNECTION_ARDUINO      = "w";
        public const string SERIAL_CMD_CONNECTION_CHAMBER      = "g";
        public const string SERIAL_CMD_READ                    = "r";
        public const string SERIAL_CMD_RH_CALIBRATE            = "h";
        public const string SERIAL_CMD_SCALE_TARE              = "t";
        public const string SERIAL_CMD_SCALE_CALIBRATE         = "k";
        public const string SERIAL_CMD_STATE_IDLE              = "i";
        public const string SERIAL_CMD_STATE_ACQUISITION       = "a";
        public const string SERIAL_CMD_STATE_CONTROL           = "c";
        public const string SERIAL_CMD_STATE_CONTROL_SETPOINT  = "s";
        public const string SERIAL_CMD_DISCONNECT              = "b";
        public const string SERIAL_CMD_SEPARATOR               = "|";
        public const string SERIAL_CMD_END                     = "@";
        public const string SERIAL_CMD_EOL                     = "\n";
        // public const string SERIAL_CMD_RESETPID     = "d";
        // public const string SERIAL_CMD_HUMIDITY     = "h";
        // public const string SERIAL_CMD_VELOCITY     = "v";

        // Two ways error messages can be received: through one dedicated message for a single device,
        // or through a data acquisition update where multiple sensors could report errors.
        public const string SERIAL_REPLY_START                 = "^";
        public const string SERIAL_REPLY_CONNECTION            = "y";
        public const string SERIAL_REPLY_CORRUPTCMD            = "x";
            public const string SERIAL_REPLY_CORRUPTCMD_START      = "s";  // The command string does not have a start flag
            public const string SERIAL_REPLY_CORRUPTCMD_END        = "e";  // The command string does not have an end flag
            public const string SERIAL_REPLY_CORRUPTCMD_PARAM_LESS = "l";  // There are fewer params in a command line than expected
            public const string SERIAL_REPLY_CORRUPTCMD_PARAM_MORE = "m";  // There are more params in a command line than expected
            public const string SERIAL_REPLY_CORRUPTCMD_PARAM_NONE = "n";  // There are no params in a command line, even though some are expected
        public const string SERIAL_REPLY_CHAMBERCONN           = "c";
            public const string SERIAL_REPLY_CHAMBERCONN_CONN      = "y";
            public const string SERIAL_REPLY_CHAMBERCONN_DISC      = "n";
        public const string SERIAL_REPLY_CMDRESPONSE           = "r";
            public const string SERIAL_REPLY_CMDRESPONSE_SUCC      = "y";
            public const string SERIAL_REPLY_CMDRESPONSE_FAIL      = "n";
        public const string SERIAL_REPLY_SCALEOFFSET           = "o";
        public const string SERIAL_REPLY_SCALEFACTOR           = "k";
        public const string SERIAL_REPLY_ACQUISITION           = "a";
        public const string SERIAL_REPLY_READING_ERROR         = "e";
        public const string SERIAL_REPLY_ERROR                 = "e";
        public const char SERIAL_REPLY_SEPARATOR               = '|';
        public const string SERIAL_REPLY_END                   = "@";
        public const string SERIAL_REPLY_EOL                   = "\n";

        // State of the command response
        public const int COMMAND_STATE_SUCCESS          = 0;
        public const int COMMAND_STATE_FAIL             = 1;
        public const int COMMAND_STATE_TIMEOUT          = 2;
        public const int COMMAND_STATE_DISCONNECTED     = 3;
        public const int COMMAND_STATE_ERROR            = 4;

        // References to external classes
        private AgenatorForm parentForm_;
        private CommandLogger commandLogger_;
        private ErrorLogger errorLogger_;
        private SerialPort port_;

        // General vars
        private readonly int maxCommandID_;   // The upper limit of command ids before rollover occurs
        private int curCommandID_ = 0;  // Tracks the most recently assigned command ID
        private bool commandOutActive_ = false;
        private bool commandOutSending_ = false;
        private bool commandInActive_ = false;
        private List<OutgoingCommand> commandsWaiting_;        // Stores the commands which are waiting for responses
        private ConcurrentQueue<OutgoingCommand> commandsOut_; // Queue for outgoing commands
        private ConcurrentQueue<IncomingCommand> commandsIn_;  // Queue for status of commands which we are awaiting responses for


        /*********************************
         *      CALLBACK FUNCTIONS
         * ******************************/
        // Callback function for updating the controls when Arduino is connected
        public delegate void handleArduinoConnectionCallback(bool connected);
        private handleArduinoConnectionCallback handleArduinoConnection;

        // Callback function for handling chamber reconnection/disconnection
        public delegate void handleChamberConnectionCallback(int chamberID, bool connected);
        private handleChamberConnectionCallback handleChamberConnection;

        // Callback function for handling scale factor update from Arduino
        public delegate void updateScaleFactorCallback(int chamberID, double scaleFactor);
        private updateScaleFactorCallback updateScaleFactor;

        // Callback function for handling scale offset update from Arduino
        public delegate void updateScaleOffsetCallback(int chamberID, int scaleOffset);
        private updateScaleOffsetCallback updateScaleOffset;

        // Callback function for handling readings update from Arduino
        public delegate void updateReadingsCallback(string fullBuffer, string chamberIDStr, string humidityStr, string temperatureStr, string weightStr, string velocityStr, string fan1SpeedStr, string fan2SpeedStr);
        private updateReadingsCallback updateReadings;
        
        // Callback function for starting control mode
        public delegate void startControlCallback(int chamberID, bool showError, bool resumeAutosave);
        private startControlCallback startControl;

        // Callback function for updating controls in parent form when start idle mode command is successful
        public delegate void updateControlsIdleSuccessCallback(int chamberID);
        private updateControlsIdleSuccessCallback updateControlsIdleSuccess;

        // Callback function for updating controls in parent form when start idle mode command failed
        public delegate void updateControlsIdleFailCallback(int chamberID);
        private updateControlsIdleFailCallback updateControlsIdleFail;

        // Callback function for updating controls in parent form when start DAQ command is successful
        public delegate void updateControlsDAQSuccessCallback(int chamberID);
        private updateControlsDAQSuccessCallback updateControlsDAQSuccess;

        // Callback function for updating controls in parent form when start DAQ command failed
        public delegate void updateControlsDAQFailCallback(int chamberID);
        private updateControlsDAQFailCallback updateControlsDAQFail;

        // Callback function for updating controls in parent form when start control command is successful
        public delegate void updateControlsControlSuccessCallback(int chamberID);
        private updateControlsControlSuccessCallback updateControlsControlSuccess;

        // Callback function for updating controls in parent form when start control command failed
        public delegate void updateControlsControlFailCallback(int chamberID);
        private updateControlsControlFailCallback updateControlsControlFail;

        // Callback function for updating controls in parent form when change setpoint command is successful
        public delegate void updateControlsSetpointSuccessCallback(int chamberID);
        private updateControlsSetpointSuccessCallback updateControlsSetpointSuccess;

        // Callback function for updating controls in parent form when change setpoint command failed
        public delegate void updateControlsSetpointFailCallback(int chamberID);
        private updateControlsSetpointFailCallback updateControlsSetpointFail;

        // Callback function for updating controls in parent form when calibrate humidity sensor command is successful
        public delegate void updateControlsRHCalibrateSuccessCallback(int chamberID);
        private updateControlsRHCalibrateSuccessCallback updateControlsRHCalibrateSuccess;

        // Callback function for updating controls in parent form when calibrate humidity sensor command failed
        public delegate void updateControlsRHCalibrateFailCallback(int chamberID);
        private updateControlsRHCalibrateFailCallback updateControlsRHCalibrateFail;

        // Callback function for updating controls in parent form when tare weighing scale command is successful
        public delegate void updateControlsScaleTareSuccessCallback(int chamberID);
        private updateControlsScaleTareSuccessCallback updateControlsScaleTareSuccess;

        // Callback function for updating controls in parent form when tare weighing scale command failed
        public delegate void updateControlsScaleTareFailCallback(int chamberID);
        private updateControlsScaleTareFailCallback updateControlsScaleTareFail;

        // Callback function for updating controls in parent form when calibrate weighing scale command is successful
        public delegate void updateControlsScaleCalibrateSuccessCallback(int chamberID);
        private updateControlsScaleCalibrateSuccessCallback updateControlsScaleCalibrateSuccess;

        // Callback function for updating controls in parent form when calibrate weighing scale command failed
        public delegate void updateControlsScaleCalibrateFailCallback(int chamberID);
        private updateControlsScaleCalibrateFailCallback updateControlsScaleCalibrateFail;


        public Commands(int maxCommandID, string commandLogSavePath, AgenatorForm parentForm, ErrorLogger errorHandler, CommandLogger commandLogger, SerialPort port,
                        handleArduinoConnectionCallback         handleArduinoConnectionFunc,
                        handleChamberConnectionCallback         handleChamberConnectionFunc,
                        updateScaleFactorCallback               updateScaleFactorFunc,
                        updateScaleOffsetCallback               updateScaleOffsetFunc,
                        updateReadingsCallback                  updateReadingsFunc,
                        startControlCallback                    startControlFunc,
                        updateControlsIdleSuccessCallback       updateControlsIdleSuccessFunc,
                        updateControlsIdleFailCallback          updateControlsIdleFailFunc,
                        updateControlsDAQSuccessCallback        updateControlsDAQSuccessFunc,
                        updateControlsDAQFailCallback           updateControlsDAQFailFunc,
                        updateControlsControlSuccessCallback    updateControlsControlSuccessFunc,
                        updateControlsControlFailCallback       updateControlsControlFailFunc,
                        updateControlsSetpointSuccessCallback   updateControlsSetpointSuccessFunc,
                        updateControlsSetpointFailCallback      updateControlsSetpointFailFunc,
                        updateControlsRHCalibrateSuccessCallback updateControlsRHCalibrateSuccessFunc,
                        updateControlsRHCalibrateFailCallback   updateControlsRHCalibrateFailFunc,
                        updateControlsScaleTareSuccessCallback  updateControlsScaleTareSuccessFunc,
                        updateControlsScaleTareFailCallback     updateControlsScaleTareFailFunc,
                        updateControlsScaleCalibrateSuccessCallback  updateControlsScaleCalibrateSuccessFunc,
                        updateControlsScaleCalibrateFailCallback     updateControlsScaleCalibrateFailFunc)
        {
            // Assign vars
            maxCommandID_       = maxCommandID;
            parentForm_         = parentForm;
            errorLogger_        = errorHandler;
            commandLogger_      = commandLogger;
            port_               = port;

            // Initialize stuff
            commandsWaiting_ = new List<OutgoingCommand>();
            commandsOut_ = new ConcurrentQueue<OutgoingCommand>();
            commandsIn_ = new ConcurrentQueue<IncomingCommand>();

            // Assign the callback functions
            handleArduinoConnection         = new handleArduinoConnectionCallback(handleArduinoConnectionFunc);
            handleChamberConnection         = new handleChamberConnectionCallback(handleChamberConnectionFunc);
            updateScaleFactor               = new updateScaleFactorCallback(updateScaleFactorFunc);
            updateScaleOffset               = new updateScaleOffsetCallback(updateScaleOffsetFunc);
            updateReadings                  = new updateReadingsCallback(updateReadingsFunc);
            startControl                    = new startControlCallback(startControlFunc);
            updateControlsIdleSuccess       = new updateControlsIdleSuccessCallback(updateControlsIdleSuccessFunc);
            updateControlsIdleFail          = new updateControlsIdleFailCallback(updateControlsIdleFailFunc);
            updateControlsDAQSuccess        = new updateControlsDAQSuccessCallback(updateControlsDAQSuccessFunc);
            updateControlsDAQFail           = new updateControlsDAQFailCallback(updateControlsDAQFailFunc);
            updateControlsControlSuccess    = new updateControlsControlSuccessCallback(updateControlsControlSuccessFunc);
            updateControlsControlFail       = new updateControlsControlFailCallback(updateControlsControlFailFunc);
            updateControlsSetpointSuccess   = new updateControlsSetpointSuccessCallback(updateControlsSetpointSuccessFunc);
            updateControlsSetpointFail      = new updateControlsSetpointFailCallback(updateControlsSetpointFailFunc);
            updateControlsRHCalibrateSuccess    = new updateControlsRHCalibrateSuccessCallback(updateControlsRHCalibrateSuccessFunc);
            updateControlsRHCalibrateFail       = new updateControlsRHCalibrateFailCallback(updateControlsRHCalibrateFailFunc);
            updateControlsScaleTareSuccess       = new updateControlsScaleTareSuccessCallback(updateControlsScaleTareSuccessFunc);
            updateControlsScaleTareFail          = new updateControlsScaleTareFailCallback(updateControlsScaleTareFailFunc);
            updateControlsScaleCalibrateSuccess  = new updateControlsScaleCalibrateSuccessCallback(updateControlsScaleCalibrateSuccessFunc);
            updateControlsScaleCalibrateFail     = new updateControlsScaleCalibrateFailCallback(updateControlsScaleCalibrateFailFunc);
        }

        // Handles the event when the timer has timed out.
        // Renew the timed out command and place it on the outgoing queue, keeping
        // the total number of attempts intact.
        // Note that the renewed command will have a different command ID.
        private void handleTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OutgoingCommand command = (sender as CommandTimer).command;

            try
            {
                command.attempts++;

                // System.Diagnostics.Debug.WriteLine("Timed out during attempt: " + Convert.ToString(command.attempts));

                // Check if we are still waiting for a response for the command, and haven't exceeded max attempts
                // The first check is necessary because we might receive a response before a timeout condition,
                // therefore we should not send again
                if (command.attempts < command.maxAttempts && commandsWaiting_.Find(x => (x.commandID == command.commandID)) != null)
                {// Resend the command since timeout has occured but we are within max attempts
                    // Place on command response queue indicating this command has timed-out, and should be resent
                    commandsIn_.Enqueue(new IncomingCommand(command.commandID, IncomingCommand.COMMAND_STATUS_TIMEOUT));
                }
                else
                {
                    // Place on command response queue indicating this command has reached maximum retry attempts
                    commandsIn_.Enqueue(new IncomingCommand(command.commandID, IncomingCommand.COMMAND_STATUS_MAXATTEMPT));
                }
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);

                // The time out condition is the safety net for commands that are not received. Therefore, it
                // is essential that it works. If it doesn't, then put on queue that an error was encountered.
                // If that still doesn't work, at least we have something from the errorLog to work with.
                try
                { commandsIn_.Enqueue(new IncomingCommand(command.commandID, IncomingCommand.COMMAND_STATUS_ERROR)); }
                catch (Exception err2)
                { errorLogger_.logUnknownError(err2); }
            }
        }

        /*************************************
         * Handles the events raised by SerialPort
         * upon receiving data
         ************************************/
        public void handleSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string readBuffer = port_.ReadTo(SERIAL_REPLY_EOL);

                // Check if the start and end of the response have the correct flags
                if (readBuffer.Substring(0, 1) == SERIAL_REPLY_START && readBuffer.Substring(readBuffer.Length - 1, 1) == SERIAL_REPLY_END)
                {
                    int commandPos = 1; // Account for offset from SERIAL_REPLY_START
                    string commandType = readBuffer.Substring(commandPos, 1);
                    int endPos = readBuffer.LastIndexOf(SERIAL_REPLY_END);



                    // System.Diagnostics.Debug.WriteLine("Received reply: " + readBuffer);


                    switch (commandType)
                    {
                        case SERIAL_REPLY_CONNECTION:
                            /*********************************
                            *   ARDUINO CONNECTION CHECK     *
                            * *******************************/
                            /* Response to the connection check sent by the C# program
                            * Format:
                            * ^c@
                            * where    ^    is SERIAL_REPLY_START
                            *          c    is SERIAL_REPLY_CONNECTION
                            *          @    is SERIAL_REPLY_END
                            */
                            parentForm_.updateArduinoConnection();
                            if (!parentForm_.getArduinoConnectionStatus())
                            {   // Arduino was previously disconnected, so we now restore the state of the program (call the control-modifying callback)
                                // and ask Arduino to verify chamber connections
                                // Responses for chamber connection will be handled by this same event handler (handleSerialDataReceived)
                                parentForm_.Invoke(handleArduinoConnection, new object[] { true });
                            }
                            break;
                        case SERIAL_REPLY_CMDRESPONSE:
                            {
                                /**********************************
                                *     RESPONSE TO COMMAND        *
                                * *******************************/
                                /* Arduino's response to a received command
                                * Format:
                                * ^c[commandID]|[chamberID]|[executedCommandType]|[succ/fail]@
                                * where    ^               is SERIAL_REPLY_START
                                *          c               is SERIAL_REPLY_CMDRESPONSE
                                *          [commandID]     is the ID for the command
                                *          [chamberID]     is the ID for the chamber
                                *          |               is SERIAL_REPLY_SEPARATOR
                                *          [executedCommandType]   is the type of command sent by C# program (e.g. SERIAL_CMD_STATE_ACQUISITION)
                                *          [succ/fail]     is SERIAL_REPLY_CMDRESPONSE_SUCCESS or SERIAL_REPLY_CMDRESPONSE_FAIL
                                *          @               is SERIAL_REPLY_END
                                */

                                // System.Diagnostics.Debug.WriteLine("Received command confirmation: " + readBuffer);

                                // Get the parameters
                                string[] parameters = extractReplyParams(readBuffer, commandPos, endPos);

                                // Sanity check for number of parameters
                                if (parameters.Length == 4)
                                {
                                    int commandID = Convert.ToInt32(parameters[0]);
                                    int chamberID = Convert.ToInt32(parameters[1]);
                                    string executedCommandType = parameters[2];
                                    bool executionSuccess = false;

                                    // Get the status of command execution, and also check if this is corrupted.
                                    if (parameters[3].Equals(SERIAL_REPLY_CMDRESPONSE_SUCC))
                                    {
                                        executionSuccess = true;
                                    }
                                    else if (parameters[3].Equals(SERIAL_REPLY_CMDRESPONSE_FAIL))
                                    {
                                        executionSuccess = false;
                                    }

                                    // Check if the executed command type is one of the known ones, or corrupted.
                                    switch (executedCommandType)
                                    {
                                        case SERIAL_CMD_STATE_IDLE:
                                        case SERIAL_CMD_STATE_ACQUISITION:
                                        case SERIAL_CMD_STATE_CONTROL:
                                        case SERIAL_CMD_STATE_CONTROL_SETPOINT:
                                        case SERIAL_CMD_RH_CALIBRATE:
                                        case SERIAL_CMD_SCALE_TARE:
                                        case SERIAL_CMD_SCALE_CALIBRATE:
                                            // One of the valid command types; Add to queue for processing this completed command
                                            if (executionSuccess)
                                            {
                                                commandsIn_.Enqueue(new IncomingCommand(commandID, IncomingCommand.COMMAND_STATUS_SUCC));
                                            }
                                            else
                                            {
                                                commandsIn_.Enqueue(new IncomingCommand(commandID, IncomingCommand.COMMAND_STATUS_FAIL));
                                            }
                                            break;
                                        default:
                                            // Unrecognized command type, consider as corrupted.
                                            // Log error and return function
                                            errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_CMDRESPONSE, "handleSerialDataReceived, executedCommandType", readBuffer);
                                            return;
                                    }
                                }
                                else
                                {// Somehow, number of parameters is wrong.
                                    errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_CMDRESPONSE, "handleSerialDataReceived, SERIAL_REPLY_CMDRESPONSE", readBuffer);
                                }
                            }
                            break;
                        case SERIAL_REPLY_CHAMBERCONN:
                            {
                                /*********************************
                                *       Chamber Connection       *
                                * *******************************/
                                /* Update from Arduino on connection to a certain chamber
                                * Format:
                                * ^c[chamberID]|[deviceID]|[errorCode]@
                                * where    ^               is SERIAL_REPLY_START
                                *          c               is SERIAL_REPLY_CHAMBERCONN
                                *          [chamberID]     is the ID for the chamber
                                *          |               is SERIAL_REPLY_SEPARATOR
                                *          [status]        is either SERIAL_REPLY_CHAMBERCONN_CONN or SERIAL_REPLY_CHAMBERCONN_DISC
                                *          @               is SERIAL_REPLY_END
                                */

                                // Get the parameters
                                string[] parameters = extractReplyParams(readBuffer, commandPos, endPos); ;

                                // Sanity check for number of parameters
                                if (parameters.Length == 2)
                                {
                                    int chamberID = Convert.ToInt16(parameters[0]);

                                    if (parameters[1] == SERIAL_REPLY_CHAMBERCONN_CONN)
                                    {   // Update from chamber saying it regained connection or is connected
                                        parentForm_.Invoke(handleChamberConnection, new object[] { chamberID, true });
                                    }
                                    else if (parameters[1] == SERIAL_REPLY_CHAMBERCONN_DISC)
                                    {   // Update from chamber saying it just or is already disconnected
                                        parentForm_.Invoke(handleChamberConnection, new object[] { chamberID, false });
                                    }
                                    else
                                    {
                                        // Unrecognized status, consider as corrupted.
                                        // Log error and return function
                                        errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_CHAMBERCONN, "handleSerialDataReceived, parameters", readBuffer);
                                        return;
                                    }
                                }
                                else
                                {
                                    errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_CHAMBERCONN, "handleSerialDataReceived, SERIAL_REPLY_CHAMBERCONN", readBuffer);
                                }
                            }
                            break;
                        case SERIAL_REPLY_SCALEFACTOR:
                            {
                                /*********************************
                                *         Scale factor           *
                                * *******************************/
                                /* Calculated scale factor based on the calibration weight
                                * Format:
                                * ^c[chamberID]|[scaleFactor]@
                                * where    ^               is SERIAL_REPLY_START
                                *          c               is SERIAL_REPLY_SCALEFACTOR
                                *          [chamberID]     is the ID for the chamber
                                *          |               is SERIAL_REPLY_SEPARATOR
                                *          [scaleFactor]   is the calculated scale factor
                                *          @               is SERIAL_REPLY_END
                                */
                                // Get the parameters
                                string[] parameters = extractReplyParams(readBuffer, commandPos, endPos);

                                // Sanity check for number of parameters
                                if (parameters.Length == 2)
                                {
                                    int chamberID = Convert.ToInt16(parameters[0]);
                                    double scaleFactor = Convert.ToDouble(parameters[1]);

                                    // Let the parent form handle the rest
                                    parentForm_.Invoke(updateControlsScaleCalibrateSuccess, new object[] { chamberID });
                                    parentForm_.Invoke(updateScaleFactor, new object[] { chamberID, scaleFactor });
                                }
                                else
                                {
                                    errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_SCALEFACTOR, "handleSerialDataReceived, SERIAL_REPLY_SCALEFACTOR", readBuffer);
                                }
                            }
                            break;
                        case SERIAL_REPLY_SCALEOFFSET:
                            {
                                /**********************************
                                *          SCALE OFFSET          *
                                * *******************************/
                                /* Calculated scale offset based on taring
                                * Format:
                                * ^c[chamberID]|[scaleOffset]@
                                * where    ^               is SERIAL_REPLY_START
                                *          c               is SERIAL_REPLY_SCALEOFFSET
                                *          [chamberID]     is the ID for the chamber
                                *          |               is SERIAL_REPLY_SEPARATOR
                                *          [scaleOffset]   is the calculated scale offset
                                *          @               is SERIAL_REPLY_END
                                */
                                // Get the parameters
                                string[] parameters = extractReplyParams(readBuffer, commandPos, endPos);

                                // Sanity check for number of parameters
                                if (parameters.Length == 2)
                                {
                                    int chamberID = Convert.ToInt16(parameters[0]);
                                    int scaleOffset = Convert.ToInt32(parameters[1]);

                                    // Let the parent form handle the rest
                                    parentForm_.Invoke(updateControlsScaleTareSuccess, new object[] { chamberID });
                                    parentForm_.Invoke(updateScaleOffset, new object[] { chamberID, scaleOffset });
                                }
                                else
                                {
                                    errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_SCALEOFFSET, "handleSerialDataReceived, SERIAL_REPLY_SCALEOFFSET", readBuffer);
                                }
                            }
                            break;
                        case SERIAL_REPLY_ACQUISITION:
                            {
                                /*********************************
                                *               DAQ              *
                                * *******************************/
                                /* Receiving a DAQ string
                                * Format:
                                * ^c[chamberID]|[humidity]|[temperature]|[weight]|[velocity]|[fan1spd]|[fan2spd]@
                                * where    ^               is SERIAL_REPLY_START
                                *          c               is SERIAL_REPLY_ACQUISITION
                                *          [chamberID]     is the ID for the chamber
                                *          |               is SERIAL_REPLY_SEPARATOR
                                *          [humidity]      is the relative humidity (%)
                                *          [temperature]   is the temperature (°C)
                                *          [weight]        is the weight (g)
                                *          [velocity]      is the air velocity (m/s)
                                *          [fan1spd]       is the speed of fan 1 (rpm)
                                *          [fan2spd]       is the speed of fan 2 (rpm)
                                *          @               is SERIAL_REPLY_END
                                */

                                // Get the parameters
                                string[] parameters = extractReplyParams(readBuffer, commandPos, endPos);

                                // Sanity check for number of parameters
                                if (parameters.Length == 7)
                                {
                                    parentForm_.Invoke(updateReadings, new object[] {   readBuffer,
                                                                                        parameters[0],
                                                                                        parameters[1],
                                                                                        parameters[2],
                                                                                        parameters[3],
                                                                                        parameters[4],
                                                                                        parameters[5],
                                                                                        parameters[6] });
                                }
                                else
                                {
                                    errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_DAQ, "handleSerialDataReceived, SERIAL_REPLY_ACQUISITION", readBuffer);
                                }
                            }
                            break;
                        case SERIAL_REPLY_ERROR:
                            {
                                /*********************************
                                *               ERROR            *
                                * *******************************/
                                /* An error occured while Arduino was performing an operation
                                * Format:
                                * ^c[chamberID]|[deviceID]|[errorCode]@
                                * where    ^               is SERIAL_REPLY_START
                                *          c               is SERIAL_REPLY_ERROR
                                *          [chamberID]     is the ID for the chamber
                                *          |               is SERIAL_REPLY_SEPARATOR
                                *          [deviceID]      is the ID of the implicated device (see HumidityChamber.cpp)
                                *          [errorCode]     is the error code (see HumidityChamber.cpp)
                                *          @               is SERIAL_REPLY_END
                                */

                                // Get the parameters
                                string[] parameters = extractReplyParams(readBuffer, commandPos, endPos);

                                // Sanity check for number of parameters
                                if (parameters.Length == 3)
                                {
                                    int chamberID = Convert.ToInt16(parameters[0]);
                                    int deviceID = Convert.ToInt16(parameters[1]);
                                    ushort errorCode = Convert.ToUInt16(parameters[2]);

                                    // Specially for weighing scale taring and calibrating, the errors invoke additional control-modifying functions
                                    if (errorCode == ErrorLogger.ARD_ABNORMAL_WEIGHT_TARE)
                                    {
                                        parentForm_.Invoke(updateControlsScaleTareFail, new object[] { chamberID });
                                    }
                                    else if (errorCode == ErrorLogger.ARD_ABNORMAL_WEIGHT_CAL)
                                    {
                                        parentForm_.Invoke(updateControlsScaleCalibrateFail, new object[] { chamberID });
                                    }

                                    // All Arduino errors are handled by the ErrorLogger class
                                    errorLogger_.logArduinoError(errorCode, chamberID, deviceID, "handleSerialDataReceived, SERIAL_REPLY_ERROR");
                                }
                                else
                                {
                                    errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_ERROR, "handleSerialDataReceived, SERIAL_REPLY_ERROR", readBuffer);
                                }
                            }
                            break;
                        case SERIAL_REPLY_CORRUPTCMD:
                            {
                                /*********************************
                                *        INVALID COMMAND         *
                                * *******************************/
                                /* Arduino received a command which doesn't follow the standards
                                * Format:
                                * ^c[commandType]|[deviceID]|[errorCode]@
                                * where    ^               is SERIAL_REPLY_START
                                *          c               is SERIAL_REPLY_CORRUPTCMD
                                *          [corruptionType]is the type of corruption detected in the command received by Arduino
                                *          @               is SERIAL_REPLY_END
                                */

                                // Get the parameters
                                string corruptionType = trimSerialBuffer(readBuffer, commandPos, endPos);

                                // Sanity check for number of parameters
                                switch (corruptionType)
                                {
                                    case SERIAL_REPLY_CORRUPTCMD_START:
                                        // The command string does not have a start flag
                                        errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPTCMD_START, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                                        break;
                                    case SERIAL_REPLY_CORRUPTCMD_END:
                                        // The command string does not have an end flag
                                        errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPTCMD_END, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                                        break;
                                    case SERIAL_REPLY_CORRUPTCMD_PARAM_LESS:
                                        // There are fewer params in a command line than expected
                                        errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPTCMD_PARAM_LESS, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                                        break;
                                    case SERIAL_REPLY_CORRUPTCMD_PARAM_MORE:
                                        // There are more params in a command line than expected
                                        errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPTCMD_PARAM_MORE, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                                        break;
                                    case SERIAL_REPLY_CORRUPTCMD_PARAM_NONE:
                                        // There are no params in a command line, even though some are expected
                                        errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPTCMD_PARAM_NONE, "handleSerialDataReceived, SERIAL_REPLY_CORRUPTCMD", readBuffer);
                                        break;
                                    default:
                                        // All Arduino errors are handled by the ErrorLogger class
                                        errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPTCMD_UNKNOWN, "handleSerialDataReceived, SERIAL_REPLY_ERROR", readBuffer);
                                        break;
                                }
                            }
                            break;
                        default:
                            // The reply sent from Arduino doesn't comply with any of the defined formats, so assume it is corrupted.
                            // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                            errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_COMMAND, "handleSerialDataReceived, Unknown command", readBuffer);
                            break;
                    }
                }
                else
                {
                    // The string sent from Arduino doesn't have the correct start and end flags, so assume it is corrupted.
                    // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                    errorLogger_.logCProgError(ErrorLogger.PROG_REPLY_CORRUPT_STARTEND, "handleSerialDataReceived", readBuffer);
                }
            }
            catch (TimeoutException err)
            {
                errorLogger_.logCProgError(013, err, err.Message);
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);
            }
        }

        // Remove the start flag, command type, and end flag from the serial buffer
        // to give only the reply parameters.
        private string trimSerialBuffer(string rawBuffer, int commandPos, int endPos)
        {
            // Trim the buffer to only the params and separators
            return rawBuffer.Substring(commandPos + 1, endPos - commandPos - 1);
        }

        // Split the trimmed buffer based on a delimiter and return the splitted parts as an array.
        private string[] extractReplyParams(string rawBuffer, int commandPos, int endPos)
        {
            string trimmedBuffer = trimSerialBuffer(rawBuffer, commandPos, endPos);

            // Get the parameters
            return trimmedBuffer.Split(SERIAL_REPLY_SEPARATOR);
        }
        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Queue handlers
        //
        // Tackle the tasks in queue for outgoing commands and command statuses.
        // These functions should be run in unique threads, and loop endlessly until commandMonitoringActive_ becomes false.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Sends the commands in the outgoing queue (commandsOut_).
        // Note that this only sends messages if the Arduino was connected.
        // awaitReponse is especially important here; it would decide if
        // we should wait for a confirmation from Arduino that it has received the command.
        public void sendCommands()
        {
            commandOutActive_ = true;
            commandOutSending_ = true;
            
            while (commandOutActive_)
            {
                if (commandOutSending_ && parentForm_.getArduinoConnectionStatus())
                {
                    try
                    {
                        OutgoingCommand outgoingCommand;

                        while (commandsOut_.TryDequeue(out outgoingCommand))
                        {
                            // Update and assign command id
                            // The modulus (%) operator takes the remainder of divison of 
                            // (curCommandID_ + 1) by (maxCommandID_ + 1)
                            curCommandID_ = (curCommandID_ + 1) % (maxCommandID_ + 1);
                            outgoingCommand.commandID = curCommandID_;

                            // Attach a separator after the command ID if there are parameters available
                            string separator = "";
                            if (!string.IsNullOrEmpty(outgoingCommand.parameterString))
                            {
                                separator = SERIAL_CMD_SEPARATOR;
                            }

                            // Timer for tracking command responses
                            CommandTimer responseTimer = new CommandTimer();

                            if (outgoingCommand.awaitResponse)
                            {// Begin forming the timer for tracking the response here, so less delay later when starting the timer
                                // Add this command to the waiting list. This will be referred to upon receiving
                                // a response for this command, or the command timed out.
                                commandsWaiting_.Add(outgoingCommand);

                                // Prepare timer
                                responseTimer.Interval = outgoingCommand.timeout;
                                responseTimer.AutoReset = false;
                                responseTimer.command = outgoingCommand;
                                // For the "=> {}" notation (statement lambdas): https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions#statement-lambdas
                                responseTimer.Elapsed += handleTimerElapsed;
                            }
                            else
                            {
                                responseTimer.Dispose();
                            }

                            // Send the command
                            try
                            {
                                outgoingCommand.fullCommand = string.Format("{0}{1}{2}{3}{4}{5}",
                                                                            SERIAL_CMD_START,
                                                                            outgoingCommand.commandType,
                                                                            outgoingCommand.commandID,
                                                                            separator,
                                                                            outgoingCommand.parameterString,
                                                                            SERIAL_CMD_END);

                                // Send the command
                                // System.Diagnostics.Debug.WriteLine("Sending command: " + outgoingCommand.fullCommand);
                                port_.Write(outgoingCommand.fullCommand + SERIAL_CMD_EOL);

                                // Won't reach here if the sending fails
                                if (outgoingCommand.awaitResponse)
                                {// Begin waiting for response from Arduino
                                    responseTimer.Start();
                                }

                                // Log command
                                commandLogger_.logCommand(outgoingCommand);
                            }
                            catch (InvalidOperationException err)
                            {
                                errorLogger_.logCProgError(11, err, err.Message);
                            }
                            catch (ArgumentNullException err)
                            {
                                errorLogger_.logCProgError(12, err, err.Message);
                            }
                            catch (TimeoutException err)
                            {
                                errorLogger_.logCProgError(13, err, err.Message);
                            }
                            catch (Exception err)
                            {
                                errorLogger_.logUnknownError(err);
                            }
                            
                            System.Threading.Thread.Sleep(10);
                        }
                    }
                    catch (Exception err)
                    {
                        errorLogger_.logUnknownError(err);
                    }
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        // Handles the the command statuses in the incoming queue (commandsIn_).
        /* Four possible situations for incomingCommand.status:
         * COMMAND_STATUS_SUCC: Command received by Arduino and carried out successfully
         * COMMAND_STATUS_FAIL: Command received by Arduino, but failed to execute
         * COMMAND_STATUS_TIME: Timed out while waiting for Arduino to confirm receipt of command
         * COMMAND_STATUS_ERROR: Encountered an error while processing the response from Arduino
         */
        public void handleCommandStatuses()
        {
            commandInActive_ = true;

            while (commandInActive_)
            {
                try
                {
                    IncomingCommand incomingCommand;

                    while (commandsIn_.TryDequeue(out incomingCommand))
                    {
                        // Find the outgoing command associated with this command status
                        OutgoingCommand outgoingCommand = commandsWaiting_.Find(x => (x.commandID == incomingCommand.commandID));

                        // If a match wasn't found, Find() returns the default value of CommandResponseTimer which should be null.
                        if (outgoingCommand != null)
                        {// Found a match
                            switch (incomingCommand.status)
                            {
                                case IncomingCommand.COMMAND_STATUS_SUCC:
                                    {
                                        // Command received by Arduino and carried out successfully.
                                        // Execute callback functions to update controls and vars in parent form.
                                        int chamberID = outgoingCommand.chamberID;
                                        string commandType = outgoingCommand.commandType;

                                        switch (commandType)
                                        {
                                            case SERIAL_CMD_STATE_IDLE:
                                            /************************************************************
                                            *  IDLE (when stopping acquisition)
                                            * **********************************************************/
                                                parentForm_.Invoke(updateControlsIdleSuccess, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_STATE_ACQUISITION:
                                            /************************************************************
                                            *  ACQUISITION (Start DAQ or stop control)
                                            * **********************************************************/
                                                parentForm_.Invoke(updateControlsDAQSuccess, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_STATE_CONTROL:
                                            /************************************************************
                                            *  CONTROL (Start control)
                                            * **********************************************************/
                                                parentForm_.Invoke(updateControlsControlSuccess, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_STATE_CONTROL_SETPOINT:
                                            /************************************************************
                                            *  CHANGE SETPOINT
                                            * **********************************************************/
                                                parentForm_.Invoke(updateControlsSetpointSuccess, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_RH_CALIBRATE:
                                            /************************************************************
                                            *  CALIBRATE RH
                                            * **********************************************************/
                                                parentForm_.Invoke(updateControlsRHCalibrateSuccess, new object[] { chamberID });
                                                break;
                                                // Don't need anything specific for command responses for taring and calibrating weighing scale because
                                                // the success relies upon receiving the offset/scaling values at the end of the measurement cycle.
                                        }

                                        // When loading an autosave file or resuming from disconnection, it may be necessary to resume to control mode.
                                        // This is done by starting DAQ first, waiting for confirmation, then starting control mode.
                                        // This section handles this unique sequence
                                        if (outgoingCommand.controlAfterDAQ)
                                        {// DAQ has started, now resume control mode
                                            parentForm_.Invoke(startControl, new object[] { outgoingCommand.chamberID, false, true });
                                        }
                                    }
                                    break;
                                case IncomingCommand.COMMAND_STATUS_TIMEOUT:
                                    // Command timed-out; resend it.
                                    // Prepare new command to be re-sent (keeping in mind of no. of attempts)
                                    OutgoingCommand resendCommand = new OutgoingCommand(outgoingCommand.commandType, outgoingCommand.awaitResponse, outgoingCommand.timeout, outgoingCommand.maxAttempts, outgoingCommand.parameterString);
                                    resendCommand.attempts = outgoingCommand.attempts;
                                    resendCommand.fullCommand = outgoingCommand.fullCommand;
                                    resendCommand.chamberID = outgoingCommand.chamberID;
                                    resendCommand.controlAfterDAQ = outgoingCommand.controlAfterDAQ;

                                    // Place new command on outgoing queue
                                    commandsOut_.Enqueue(resendCommand);
                                    break;
                                case IncomingCommand.COMMAND_STATUS_FAIL:
                                    // Command received by Arduino, but failed to execute
                                case IncomingCommand.COMMAND_STATUS_MAXATTEMPT:
                                    // Reached maximum attempts for resending commands (due to time-out) to Arduino
                                case IncomingCommand.COMMAND_STATUS_ERROR:
                                    // Encountered an error while processing the response from Arduino
                                    {
                                        // Execute callback functions to revert controls and update vars in parent form to reflect failed command.
                                        int chamberID = outgoingCommand.chamberID;
                                        string commandType = outgoingCommand.commandType;

                                        switch (commandType)
                                        {
                                            case SERIAL_CMD_STATE_IDLE:
                                                /************************************************************
                                                *  IDLE (when stopping acquisition)
                                                * **********************************************************/
                                                parentForm_.Invoke(updateControlsIdleFail, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_STATE_ACQUISITION:
                                                /************************************************************
                                                *  ACQUISITION (Start DAQ or stop control)
                                                * **********************************************************/
                                                parentForm_.Invoke(updateControlsDAQFail, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_STATE_CONTROL:
                                                /************************************************************
                                                *  CONTROL (Start control)
                                                * **********************************************************/
                                                parentForm_.Invoke(updateControlsControlFail, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_STATE_CONTROL_SETPOINT:
                                                /************************************************************
                                                *  CHANGE SETPOINT
                                                * **********************************************************/
                                                parentForm_.Invoke(updateControlsSetpointFail, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_RH_CALIBRATE:
                                                /************************************************************
                                                *  TARE WEIGHING SCALE
                                                * **********************************************************/
                                                parentForm_.Invoke(updateControlsRHCalibrateFail, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_SCALE_TARE:
                                                /************************************************************
                                                *  TARE WEIGHING SCALE
                                                * **********************************************************/
                                                parentForm_.Invoke(updateControlsScaleTareFail, new object[] { chamberID });
                                                break;
                                            case SERIAL_CMD_SCALE_CALIBRATE:
                                                /************************************************************
                                                *  CALIBRATE WEIGHING SCALE
                                                * **********************************************************/
                                                parentForm_.Invoke(updateControlsScaleCalibrateFail, new object[] { chamberID });
                                                break;
                                        }

                                        // Log errors depending on the type of command failure
                                        switch (incomingCommand.status)
                                        {
                                            case IncomingCommand.COMMAND_STATUS_FAIL:
                                                errorLogger_.logCProgError(ErrorLogger.PROG_COMMAND_FAIL, "handleCommandStatuses", outgoingCommand.fullCommand);
                                                break;
                                            case IncomingCommand.COMMAND_STATUS_MAXATTEMPT:
                                                errorLogger_.logCProgError(ErrorLogger.PROG_COMMAND_MAXATTEMPT, "handleCommandStatuses", outgoingCommand.fullCommand);
                                                break;
                                            case IncomingCommand.COMMAND_STATUS_ERROR:
                                                errorLogger_.logCProgError(ErrorLogger.PROG_COMMAND_ERROR, "handleCommandStatuses", outgoingCommand.fullCommand);
                                                break;
                                        }
                                    }
                                    break;
                            }

                            // Done with this command; remove it from the waiting list.
                            commandsWaiting_.Remove(outgoingCommand);
                        }
                        else
                        {// Did not find a match in terms of commandID.
                            /* This situation happens when:
                             * A) a response is received while waiting to handle a timed-out command in queue.
                             * B) time out occurs while waiting to handle a response in queue.
                             * C) commandID is changed due to noise, thus there is naturally no match.
                             *    The original command will time out because it doesn't receive a response.
                             * Case A and B are complements of each other, therefore no resolution is required
                             * for either case. As for case C, the original command will time out and be handled
                             * accordingly. Therefore, we don't need to do anything special here.
                             * */
                        }
                    }
                }
                catch (Exception err)
                {
                    errorLogger_.logUnknownError(err);
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        // Stop sending out commands, but keep the thread running it alive
        public void pauseCommandOut()
        {
            commandOutSending_ = false;
        }

        // Resume sending out commands
        public void resumeCommandOut()
        {
            commandOutSending_ = true;
        }

        // Clear the outgoing command queue
        public void clearCommandOut()
        {
            OutgoingCommand dummy;

            // Dequeue all outgoing commands
            while (commandsOut_.TryDequeue(out dummy)) { }
        }

        // Ends the command sending loop at the end of its current iteration
        public void stopCommandOut()
        {
            commandOutActive_ = false;
        }

        // Ends the command status parser loop at the end of its current iteration
        public void stopCommandIn()
        {
            commandInActive_ = false;
        }

        // Get a count of how many command responses are waiting to be handled
        public int getCommandInTotal()
        {
            return commandsIn_.Count;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Commands
        //
        // Commands to Arduino
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Check if the Arduino is connected
        // Very similar to sendCommands(), but skips the command queue (and hence,
        // the check for connection to Arduino) since this function itself verifies the connection
        public bool sendCommandConnectionArduino(bool logCommand)
        {
            try
            {
                // The timeout and maxAttempts will not be used.
                OutgoingCommand outgoingCommand = new OutgoingCommand(SERIAL_CMD_CONNECTION_ARDUINO, false, DEFAULT_COMMAND_RESPONSETIMEOUT, DEFAULT_COMMAND_MAXATTEMPTS);

                outgoingCommand.fullCommand = string.Format("{0}{1}{2}",
                                                            SERIAL_CMD_START,
                                                            SERIAL_CMD_CONNECTION_ARDUINO,
                                                            SERIAL_CMD_END);

                // Send out the command (skips the command queue)
                port_.Write(outgoingCommand.fullCommand + SERIAL_CMD_EOL);

                // Log command
                if (logCommand)
                {
                    commandLogger_.logCommand(outgoingCommand);
                }
                return true;
            }
            catch (InvalidOperationException err)
            {
                errorLogger_.logCProgError(11, err, err.Message);
                return false;
            }
            catch (ArgumentNullException err)
            {
                errorLogger_.logCProgError(12, err, err.Message);
                return false;
            }
            catch (TimeoutException err)
            {
                errorLogger_.logCProgError(13, err, err.Message);
                return false;
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);
                return false;
            }
        }

        // Ask for update on the connection status of ALL the chambers
        public void queueCommandConnectionChamber(int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            // The Arduino's reponse to this is to check connection status of each chamber, and send
            // an "error" message that will be handled by handleSerialDataReceived.

            // Queue command
            commandsOut_.Enqueue(new OutgoingCommand(SERIAL_CMD_CONNECTION_CHAMBER, false, timeout, maxAttempts));
        }

        // Begin idle mode
        public void queueCommandIdle(int chamberID, bool awaitResponse, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            string paramString = string.Format("{0:D}", chamberID);

            // Queue command
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_STATE_IDLE, awaitResponse, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            commandsOut_.Enqueue(command);
        }

        // ControlAfterDAQ determines if we should immediately queue a command to start control mode once
        // DAQ has begun successfully. This is used when loading autosaves.
        public void queueCommandDAQ(int chamberID, uint DAQInterval, int scaleOffset, double scaleFactor, double humidityRaw1, double humidityStd1, double humidityRaw2, double humidityStd2, bool controlAfterDAQ, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            // Command string
            string scaleFactorDecimalPlaces = Convert.ToString(parentForm_.scaleFactorDecimalPlaces);
            string humidityDecimalPlaces = Convert.ToString(parentForm_.humidityDecimalPlaces);
            string paramString = string.Format("{0:D}{1}{2:D}{3}{4:D}{5}{6:F" + scaleFactorDecimalPlaces + "}{7}{8:F" + humidityDecimalPlaces + "}{9}{10:F" + humidityDecimalPlaces + "}{11}{12:F" + humidityDecimalPlaces + "}{13}{14:F" + humidityDecimalPlaces + "}",
                                                chamberID,
                                                SERIAL_CMD_SEPARATOR,
                                                DAQInterval,
                                                SERIAL_CMD_SEPARATOR,
                                                scaleOffset,
                                                SERIAL_CMD_SEPARATOR,
                                                scaleFactor,
                                                SERIAL_CMD_SEPARATOR,
                                                humidityRaw1,
                                                SERIAL_CMD_SEPARATOR,
                                                humidityStd1,
                                                SERIAL_CMD_SEPARATOR,
                                                humidityRaw2,
                                                SERIAL_CMD_SEPARATOR,
                                                humidityStd2);

            // Queue command, making sure to assign controlAfterDAQ flag
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_STATE_ACQUISITION, true, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            command.controlAfterDAQ = controlAfterDAQ;
            commandsOut_.Enqueue(command);
        }

        // Begin control mode
        public void queueCommandControl(int chamberID, double RHSetpoint, double fanSpeedSetpoint, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            string paramString = string.Format("{0:D}{1}{2:F1}{3}{4:F0}",
                                                chamberID,
                                                SERIAL_CMD_SEPARATOR,
                                                RHSetpoint,
                                                SERIAL_CMD_SEPARATOR,
                                                fanSpeedSetpoint);

            // Queue command
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_STATE_CONTROL, true, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            commandsOut_.Enqueue(command);
        }

        // Change the setpoints of the currently running control mode
        public void queueCommandSetpoint(int chamberID, double RHSetpoint, double fanSpeedSetpoint, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            string paramString = string.Format("{0:D}{1}{2:F1}{3}{4:F0}",
                                                chamberID,
                                                SERIAL_CMD_SEPARATOR,
                                                RHSetpoint,
                                                SERIAL_CMD_SEPARATOR,
                                                fanSpeedSetpoint);

            // Queue command
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_STATE_CONTROL_SETPOINT, true, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            commandsOut_.Enqueue(command);
        }

        // Ask for a reading at the end of the current data acquisition cycle
        public void queueCommandRead(int chamberID, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            string paramString = string.Format("{0:D}", chamberID);

            // Queue command, keeping in mind that we do not need to await response for read commands.
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_READ, false, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            commandsOut_.Enqueue(command);
        }

        // Update one of the calibration points for the relative humidity sensor
        public void queueCommandCalibrateRH(int chamberID, bool point1, double humidityRaw, double humidityStd, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            string pointIdentifier;
            if (point1)
            {
                pointIdentifier = "1";
            }
            else
            {
                pointIdentifier = "2";
            }

            string paramString = string.Format("{0:D}{1}{2:D}{3}{4:f1}{5}{6:f1}",
                                            chamberID,
                                            SERIAL_CMD_SEPARATOR,
                                            pointIdentifier,
                                            SERIAL_CMD_SEPARATOR,
                                            humidityRaw,
                                            SERIAL_CMD_SEPARATOR,
                                            humidityStd);

            // Queue command
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_RH_CALIBRATE, true, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            commandsOut_.Enqueue(command);
        }

        // Tare the weighing scale
        public void queueCommandTareScale(int chamberID, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            string paramString = string.Format("{0:D}", chamberID);

            // Queue command
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_SCALE_TARE, true, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            commandsOut_.Enqueue(command);
        }

        // Calibrate the weighing scale based on a given reference weight (g)
        public void queueCommandCalibrateScale(int chamberID, short refWeight, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            string paramString = string.Format("{0:D}{1}{2:D}",
                                            chamberID,
                                            SERIAL_CMD_SEPARATOR,
                                            refWeight);

            // Queue command
            OutgoingCommand command = new OutgoingCommand(SERIAL_CMD_SCALE_CALIBRATE, true, timeout, maxAttempts, paramString);
            command.chamberID = chamberID;
            commandsOut_.Enqueue(command);
        }

        // Tell Arduino to stop sending serial command (this does not actually disconnect the device)
        // This also forces all chambers into idle mode
        public void queueCommandDisconnect(bool awaitResponse, int timeout = DEFAULT_COMMAND_RESPONSETIMEOUT, int maxAttempts = DEFAULT_COMMAND_MAXATTEMPTS)
        {
            // Queue command
            commandsOut_.Enqueue(new OutgoingCommand(SERIAL_CMD_DISCONNECT, false, timeout, maxAttempts));
        }

        // Tell Arduino to stop sending serial command (this does not actually disconnect the device)
        // This also forces all chambers into idle mode
        // Just as sendCommandConnectionArduino(), skips the command queue (and hence,
        // the check for connection to Arduino) because the situation that calls for this function does not guarantee connection to Arduino
        public bool sendCommandDisconnect(bool logCommand)
        {
            try
            {
                // The timeout and maxAttempts will not be used.
                OutgoingCommand outgoingCommand = new OutgoingCommand(SERIAL_CMD_DISCONNECT, false, DEFAULT_COMMAND_RESPONSETIMEOUT, DEFAULT_COMMAND_MAXATTEMPTS);

                // Even though all commands usually have a command ID, the disconnect command is unique in that command ID is entirely optional
                outgoingCommand.fullCommand = string.Format("{0}{1}{2}",
                                                            SERIAL_CMD_START,
                                                            SERIAL_CMD_DISCONNECT,
                                                            SERIAL_CMD_END);

                // Send out the command (skips the command queue)
                port_.Write(outgoingCommand.fullCommand + SERIAL_CMD_EOL);

                // Log command
                if (logCommand)
                {
                    commandLogger_.logCommand(outgoingCommand);
                }
                return true;
            }
            catch (InvalidOperationException err)
            {
                errorLogger_.logCProgError(11, err, err.Message);
                return false;
            }
            catch (ArgumentNullException err)
            {
                errorLogger_.logCProgError(12, err, err.Message);
                return false;
            }
            catch (TimeoutException err)
            {
                errorLogger_.logCProgError(13, err, err.Message);
                return false;
            }
            catch (Exception err)
            {
                errorLogger_.logUnknownError(err);
                return false;
            }
        }
    }
}
