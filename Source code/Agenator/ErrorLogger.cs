using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenator
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Error logger
    // Logs errors in an error log file.
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class ErrorLogger
    {
        // Error codes specific to Arduino
        // List of devices that serves two-fold purpose:
        // 1) 0 to 15 are the actual mapping to PWM pins of PCA9685.
        // 2) 20 and above are devices not directly connected to outputs of PCA9685
        // 3) 255 is a special case which is the entire chamber itself (for checking connection)
        // During error handling, this list is used to refer to the device that created the error.
        public const ushort DEVICE_CONNCHECK = 0;  // Not connected to anything; only used for checking chamber connection
        public const ushort DEVICE_HUMID = 1;  // MOSFET for humidifier
        public const ushort DEVICE_SOL_WET = 2;  // MOSFET for solenoid valve controlling air flow to humidifier
        public const ushort DEVICE_FAN2_PWM = 3;  // PWM input to fan 2
        public const ushort DEVICE_FAN_REG = 4;  // Pin on SN74HC590A to display current pulse count on output pins
        public const ushort DEVICE_FAN_CLR = 5;  // Pin on SN74HC590A to clear pulse counts
        public const ushort DEVICE_FAN_SHIFT = 6;  // Pin on SN74HC165 to shift the state of input pins into internal register
        public const ushort DEVICE_FAN1_PWM = 7;  // PWM input to fan 1
        public const ushort DEVICE_SOL_DRY = 8;  // MOSFET for solenoid valve controlling air flow to dry columns
        public const ushort DEVICE_PUMP = 9;  // Relay for air pump
        public const ushort DEVICE_FAN2_SELX = 10; // Active LOW for selecting fan 2 on SN74LV125A controlling CLK/DAT lines and SN74LV244A controlling the fan reg, shift, and clr pins
        public const ushort DEVICE_FAN1_SELX = 11; // Active LOW for selecting fan 1 on SN74LV125A controlling CLK/DAT lines and SN74LV244A controlling the fan reg, shift, and clr pins
        public const ushort DEVICE_FS5_SELX = 12; // Active LOW for selecting FS5 on SN74LV125A controlling CLK/DAT lines
        public const ushort DEVICE_HX711_SELX = 13; // Active LOW for selecting HX711 on SN74LV125A controlling CLK/DAT lines
        public const ushort DEVICE_CLKDAT_ENX = 14; // Active LOW for enabling the CLK and DAT lines to this chamber
        public const ushort DEVICE_CLKDAT_EN = 15; // Active HIGH for enabling the CLK and DAT lines to this chamber
        public const ushort DEVICE_HX711 = 18; // HX711
        public const ushort DEVICE_FS5 = 19; // FS5
        public const ushort DEVICE_HIH8000 = 20; // Humidity and temperature sensor
        public const ushort DEVICE_FAN1 = 21; // Fan 1
        public const ushort DEVICE_FAN2 = 22; // Fan 2
        public const ushort DEVICE_CHAMBER = 255; // The chamber itself

        // Error code offsets for Arduino
        public const ushort OFFSET_ARDUINO = 200; // Add 200 because all Arduino error codes start from 200
        public const ushort OFFSET_PCA9685 = 30;  // Add 50 because the first 50 errors are reserved for non-PCA9685 devices

        // Error codes concerning chamber connection
        public const ushort ERR_DISCONNECT = 0;
        public const ushort ERR_CONNECT = 255;

        // Error codes for C# program (full list TBD because lazy)
        // Readings
        public const ushort PROG_READING_CORRUPT = 50;
        public const ushort PROG_READING_HUMIDITY = 51;
        public const ushort PROG_READING_TEMPERATURE = 52;
        public const ushort PROG_READING_WEIGHT = 53;
        public const ushort PROG_READING_VELOCITY = 54;
        public const ushort PROG_READING_FAN1 = 55;
        public const ushort PROG_READING_FAN2 = 56;
        public const ushort PROG_READING_CATEGORY = 57;
        // Command attempts
        public const ushort PROG_COMMAND_MAXATTEMPT = 60;
        public const ushort PROG_COMMAND_FAIL = 61;
        // public const ushort PROG_COMMAND_UNLISTED       = 62;
        // public const ushort PROG_COMMAND_DISCONNECTED   = 63;
        // public const ushort PROG_COMMAND_DISCONNECTED_RETRY = 64;
        public const ushort PROG_COMMAND_ERROR = 65;
        // Corrupted response
        public const ushort PROG_REPLY_CORRUPT_STARTEND = 100;
        public const ushort PROG_REPLY_CORRUPT_CMDRESPONSE = 101;
        public const ushort PROG_REPLY_CORRUPT_CHAMBERCONN = 102;
        public const ushort PROG_REPLY_CORRUPT_SCALEOFFSET = 103;
        public const ushort PROG_REPLY_CORRUPT_SCALEFACTOR = 104;
        public const ushort PROG_REPLY_CORRUPT_DAQ = 105;
        public const ushort PROG_REPLY_CORRUPT_ERROR = 106;
        public const ushort PROG_REPLY_CORRUPT_COMMAND = 107;
        // Corrupted command received by Arduino
        public const ushort PROG_REPLY_CORRUPTCMD_START = 110;
        public const ushort PROG_REPLY_CORRUPTCMD_END = 111;
        public const ushort PROG_REPLY_CORRUPTCMD_PARAM_LESS = 112;
        public const ushort PROG_REPLY_CORRUPTCMD_PARAM_MORE = 113;
        public const ushort PROG_REPLY_CORRUPTCMD_PARAM_NONE = 114;
        public const ushort PROG_REPLY_CORRUPTCMD_UNKNOWN = 115;
        
        // Arduino errors
        public const ushort ARD_ABNORMAL_WEIGHT_TARE = 27;
        public const ushort ARD_ABNORMAL_WEIGHT_CAL = 28;


        // Other vars
        private bool errorLoggingActive_ = false;
        private readonly string errorLogLocation_;
        private ConcurrentQueue<string[]> errorLog_;

        public ErrorLogger(string errorLogLocation)
        {
            errorLogLocation_ = errorLogLocation;
            errorLog_ = new ConcurrentQueue<string[]>();
        }


        // Errors generated by the C# program (starts at 0, ends at 199)
        public void logCProgError(ushort error, Exception err, string message = "")
        {
            try
            {
                string errorPrefix = "  | C# Program | ";
                errorPrefix += "L" + getLineNumber(err) + " | ";
                errorPrefix += "E" + Convert.ToString(error) + " | ";

                queueError(error, errorPrefix, message);
            }
            catch (Exception)
            {
                // If the error file is opened by some other program, the Agenator program would not be able to write to it. This
                // exception catching is just to prevent unhandled exceptions. No handling will be done for this situations, so
                // the exception would not be logged.
            }
        }

        public void logCProgError(ushort error, string functionName, string message = "")
        {
            try
            {
                string errorPrefix = "  | C# Program | ";
                errorPrefix += "Function " + functionName + " | ";
                errorPrefix += "E" + Convert.ToString(error) + " | ";

                queueError(error, errorPrefix, message);
            }
            catch (Exception)
            {
                // If the error file is opened by some other program, the Agenator program would not be able to write to it. This
                // exception catching is just to prevent unhandled exceptions. No handling will be done for this situations, so
                // the exception would not be logged.
            }
        }

        // Errors sent by Arduino (starts at 200)
        public void logArduinoError(ushort error, int chamberNumber, int device, string functionName)
        {
            try
            {
                string errorPrefix = "  | Chamber " + Convert.ToString(chamberNumber).PadRight(2) + " | ";  // Pad the chamber number to match column width of C# program errors
                errorPrefix += "Function " + functionName + " | ";

                // Check if this is one of the PCA9685 PWM outputs (ranges from 0 to 15)
                if (device < 16)
                {
                    // Add 200 because all Arduino error codes start from 200
                    // Add 50 because the first 50 errors are reserved for non-PCA9685 devices
                    error += OFFSET_ARDUINO + OFFSET_PCA9685;

                    switch (device)
                    {
                        case DEVICE_CONNCHECK:
                            errorPrefix += "PCA9685 output 0, Connection check: ";
                            break;
                        case DEVICE_HUMID:
                            errorPrefix += "PCA9685 output 1, Humidifier: ";
                            break;
                        case DEVICE_SOL_WET:
                            errorPrefix += "PCA9685 output 2, Wet solenoid valve: ";
                            break;
                        case DEVICE_FAN2_PWM:
                            errorPrefix += "PCA9685 output 3, Fan 2 PWM: ";
                            break;
                        case DEVICE_FAN_REG:
                            errorPrefix += "PCA9685 output 4, Fan register: ";
                            break;
                        case DEVICE_FAN_CLR:
                            errorPrefix += "PCA9685 output 5, Fan clear: ";
                            break;
                        case DEVICE_FAN_SHIFT:
                            errorPrefix += "PCA9685 output 6, Fan shift: ";
                            break;
                        case DEVICE_FAN1_PWM:
                            errorPrefix += "PCA9685 output 7, Fan 1 PWM: ";
                            break;
                        case DEVICE_SOL_DRY:
                            errorPrefix += "PCA9685 output 8, Dry solenoid valve: ";
                            break;
                        case DEVICE_PUMP:
                            errorPrefix += "PCA9685 output 9, Air pump: ";
                            break;
                        case DEVICE_FAN2_SELX:
                            errorPrefix += "PCA9685 output 10, Fan 2 select: ";
                            break;
                        case DEVICE_FAN1_SELX:
                            errorPrefix += "PCA9685 output 11, Fan 1 select: ";
                            break;
                        case DEVICE_FS5_SELX:
                            errorPrefix += "PCA9685 output 12, FS5 select: ";
                            break;
                        case DEVICE_HX711_SELX:
                            errorPrefix += "PCA9685 output 13, HX711 select: ";
                            break;
                        case DEVICE_CLKDAT_ENX:
                            errorPrefix += "PCA9685 output 14, CLK/DAT enable LOW: ";
                            break;
                        case DEVICE_CLKDAT_EN:
                            errorPrefix += "PCA9685 output 15, CLK/DAT enable HIGH: ";
                            break;
                        default:
                            errorPrefix += "Unknown PCA9685 output: ";
                            break;
                    }
                }
                else
                {   // Not PCA9685 outputs
                    // Add 200 because all Arduino error codes start from 200
                    error += OFFSET_ARDUINO;
                }

                // Put error on queue
                queueError(error, errorPrefix);
            }
            catch (Exception)
            {
                // If the error file is opened by some other program, the Agenator program would not be able to write to it. This
                // exception catching is just to prevent unhandled exceptions. No handling will be done for this situations, so
                // the exception would not be logged.
            }
        }

        private void queueError(ushort error, string errorPrefix, string message = "")
        {
            try
            {
                string errorMsg;

                switch (error)
                {
                    /************************************************
                    * Errors generated within C# program            *
                    ************************************************/
                    case 001:
                        // Resetting Arduino (toggling DTR on serial port) and the port itself
                        errorMsg = "Resetting Arduino thru DTR signal.";
                        break;
                    case 011:
                        // SerialPort: InvalidOperationException : The specified port is not open.
                        errorMsg = "Serial port is not open. " + message;
                        break;
                    case 012:
                        // SerialPort: ArgumentNullException : Argument given to SerialPort function was null.
                        errorMsg = "Argument given to a SerialPort function was null. " + message;
                        break;
                    case 013:
                        // SerialPort: TimeoutException : The operation did not complete before the time-out period ended..
                        errorMsg = "SerialPort operation did not complete before the time-out period ended. " + message;
                        break;
                    case 031:
                        // A group of exceptions thrown when trying to open the file path specified by the user
                        errorMsg = "Error with file path: " + message;
                        break;
                    case 041:
                        // ArgumentNullException	
                        // format or arg is null.
                        errorMsg = "CSVWriter format or arg is null.";
                        break;
                    case 042:
                        // ObjectDisposedException
                        // The TextWriter is closed.
                        errorMsg = "CSVWriter TextWriter stream is closed.";
                        break;
                    case 043:
                        // FormatException	
                        // format is not a valid composite format string.
                        // - or -
                        // The index of a format item is less than 0(zero), or greater than or equal to the length of the arg array. 
                        errorMsg = "CSVWriter has an invalid format string for writing.";
                        break;
                    case 044:
                        // IOException
                        // CSVWriter encountered an I/O error
                        errorMsg = "CSVWriter encountered an I/O error";
                        break;
                    case 045:
                        // EncoderFallbackException	
                        // The current encoding does not support displaying half of a Unicode surrogate pair.
                        errorMsg = "CSVWriter encoding has an issue.";
                        break;
                    /************************************************
                    * DAQ readings                                  *
                    ************************************************/
                    case PROG_READING_CORRUPT:
                        // DAQ string sent by Arduino is corrupt/missing some parts.
                        errorMsg = "Received a corrupt DAQ string from Arduino.";
                        break;
                    case PROG_READING_HUMIDITY:
                        // Humidity reading received from Arduino is an empty string.
                        errorMsg = "Humidity reading from Arduino is not in correct format. Original full string: " + message;
                        break;
                    case PROG_READING_TEMPERATURE:
                        // Temperature reading received from Arduino is an empty string.
                        errorMsg = "Temperature reading from Arduino is not in correct format. Original full string: " + message;
                        break;
                    case PROG_READING_WEIGHT:
                        // Weight reading received from Arduino is an empty string.
                        errorMsg = "Weight reading from Arduino is not in correct format. Original full string: " + message;
                        break;
                    case PROG_READING_VELOCITY:
                        // Velocity reading received from Arduino is an empty string.
                        errorMsg = "Velocity reading from Arduino is not in correct format. Original full string: " + message;
                        break;
                    case PROG_READING_FAN1:
                        // Fan 1 speed reading received from Arduino is an empty string.
                        errorMsg = "Fan speed reading from Arduino is not in correct format. Original full string: " + message;
                        break;
                    case PROG_READING_FAN2:
                        // Fan 2 speed reading received from Arduino is an empty string.
                        errorMsg = "Fan speed reading from Arduino is not in correct format. Original full string: " + message;
                        break;
                    case PROG_READING_CATEGORY:
                        // Unknown reading category provided to processAndStoreData()
                        errorMsg = "Unknown reading category provided to processAndStoreData(). Seral buffer: " + message;
                        break;
                    /************************************************
                    * Sending out commands                          *
                    ************************************************/
                    case PROG_COMMAND_MAXATTEMPT:
                        // Exceeded max retries for sending command to Arduino because all of them timed out.
                        errorMsg = "Exceeded max attempts (all timed out) to send the command: " + message;
                        break;
                    case PROG_COMMAND_FAIL:
                        // Command was received by Arduino, but failed to execute.
                        errorMsg = "Arduino failed to execute the command: " + message;
                        break;
                    /*
                    case PROG_COMMAND_UNLISTED:
                        // Received a confirmation from Arduino, but the command wasn't put up in the commandResponses_ list.
                        errorMsg = "Received confirmation for a command from Arduino, but command wasn't sent or listed in commandResponses_: " + message;
                        break;
                    case PROG_COMMAND_DISCONNECTED:
                        // Attempted to send a command while Arduino was disconnected. No command will be sent
                        // in this situation.
                        errorMsg = "Attempted to send a command while Arduino was disconnected. Command: " + message;
                        break;
                    case PROG_COMMAND_DISCONNECTED_RETRY:
                        // Attempted to resend a command while Arduino was disconnected. No command will be sent
                        // in this situation.
                        errorMsg = "Attempted to *re-send* a command while Arduino was disconnected. Command: " + message;
                        break;
                    */
                    case PROG_COMMAND_ERROR:
                        // Encountered an error while trying to deal with a command response.
                        errorMsg = "Encountered an error while processing a command response. Command: " + message;
                        break;
                    /************************************************
                    * Corrupted responses from Arduino              *
                    ************************************************/
                    case PROG_REPLY_CORRUPT_STARTEND:
                        // The string sent from Arduino doesn't have both the start and end flags.
                        // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                        errorMsg = "Received a message from Arduino without proper start and end flags: " + message;
                        break;
                    case PROG_REPLY_CORRUPT_COMMAND:
                        // The string sent from Arduino doesn't have a predefined reply type.
                        // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                        errorMsg = "Received a message from Arduino that has an unlisted reply type: " + message;
                        break;
                    case PROG_REPLY_CORRUPT_CHAMBERCONN:
                        // The string sent from Arduino for chamber connection has incorrect parameters.
                        // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                        errorMsg = "Received a chamber connection confirmation from Arduino with incorrect format: " + message;
                        break;
                    case PROG_REPLY_CORRUPT_SCALEOFFSET:
                        // The string sent from Arduino for scale offset has incorrect parameters.
                        // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                        errorMsg = "Received a scale offset confirmation from Arduino with incorrect format: " + message;
                        break;
                    case PROG_REPLY_CORRUPT_SCALEFACTOR:
                        // The string sent from Arduino for scale factor has incorrect parameters.
                        // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                        errorMsg = "Received a scale factor confirmation from Arduino with incorrect format: " + message;
                        break;
                    case PROG_REPLY_CORRUPT_DAQ:
                        // The string sent from Arduino for DAQ has incorrect parameters.
                        // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                        errorMsg = "Received a DAQ string from Arduino with incorrect format: " + message;
                        break;
                    case PROG_REPLY_CORRUPT_ERROR:
                        // The string sent from Arduino for general error has incorrect parameters.
                        // Chamber number is set to -1 to indicate that this is not an error from a specific chamber.
                        errorMsg = "Received an error string from Arduino with incorrect format: " + message;
                        break;
                    /************************************************
                    * Arduino received a corrupted command          *
                    ************************************************/
                    case PROG_REPLY_CORRUPTCMD_START:
                        // Arduino received a command without a start flag
                        errorMsg = "Arduino received a command from Arduino without the start flag";
                        break;
                    case PROG_REPLY_CORRUPTCMD_END:
                        // Arduino received a command without a start flag
                        errorMsg = "Arduino received a command from Arduino without the end flag";
                        break;
                    case PROG_REPLY_CORRUPTCMD_PARAM_LESS:
                        // Arduino received a command without a start flag
                        errorMsg = "Arduino received a command from Arduino with less parameters than expected";
                        break;
                    case PROG_REPLY_CORRUPTCMD_PARAM_MORE:
                        // Arduino received a command without a start flag
                        errorMsg = "Arduino received a command from Arduino with more parameters than expected";
                        break;
                    case PROG_REPLY_CORRUPTCMD_PARAM_NONE:
                        // Arduino received a command without a start flag
                        errorMsg = "Arduino received a command from Arduino without parameters, even though they are expected.";
                        break;
                    case PROG_REPLY_CORRUPTCMD_UNKNOWN:
                        // Arduino received a command without a start flag
                        errorMsg = "Arduino received a command from Arduino with an unknown corruption type: " + message;
                        break;
                    /************************************************
                    * Errors sent from Arduino                      *
                    * 200 - 230 are reserved for non-PCA9685 errors *
                    ************************************************/
                    // Note 200 is taken by ERR_DISCONNECT (near end of this list)
                    case 1 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: I2C timeout while waiting for successful completion of a Start bit";
                        break;
                    case 2 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: I2C timeout while waiting for ACK/NACK while addressing slave in transmit mode (MT)";
                        break;
                    case 3 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: I2C timeout while waiting for ACK/NACK while sending data to the slave";
                        break;
                    case 4 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: I2C timeout while waiting for successful completion of a Repeated Start";
                        break;
                    case 5 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: I2C timeout while waiting for ACK/NACK while addressing slave in receiver mode (MR)";
                        break;
                    case 6 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: I2C timeout while waiting for ACK/NACK while receiving data from the slave";
                        break;
                    case 7 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: I2C timeout while waiting for successful completion of the Stop bit";
                        break;
                    case 8 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: \"See datasheet of microcontroller chip for exact meaning\"";
                        break;
                    case 9 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: The address for the device associated with this HIH8000_customI2C class has not been given yet";
                        break;
                    case 10 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000_customI2C: Unknown error";
                        break;
                    case 11 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: I2C timeout while waiting for successful completion of a Start bit";
                        break;
                    case 12 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: I2C timeout while waiting for ACK/NACK while addressing slave in transmit mode (MT)";
                        break;
                    case 13 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: I2C timeout while waiting for ACK/NACK while sending data to the slave";
                        break;
                    case 14 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: I2C timeout while waiting for successful completion of a Repeated Start";
                        break;
                    case 15 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: I2C timeout while waiting for ACK/NACK while addressing slave in receiver mode (MR)";
                        break;
                    case 16 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: I2C timeout while waiting for ACK/NACK while receiving data from the slave";
                        break;
                    case 17 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: I2C timeout while waiting for successful completion of the Stop bit";
                        break;
                    case 18 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: \"See datasheet of microcontroller chip for exact meaning\"";
                        break;
                    case 19 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: SPI timeout while waiting for a data byte";
                        break;
                    case 20 + OFFSET_ARDUINO:
                        errorMsg = "TachoSensor: Unknown error";
                        break;
                    case 21 + OFFSET_ARDUINO:
                        errorMsg = "FS5 breakout: SPI timeout while waiting for a data byte";
                        break;
                    case 22 + OFFSET_ARDUINO:
                        errorMsg = "HX711 breakout: SPI timeout while waiting for a data byte";
                        break;
                    case 23 + OFFSET_ARDUINO:
                        errorMsg = "PCA9685: SPI timeout while waiting for the SPI lines to be activated";
                        break;
                    case 24 + OFFSET_ARDUINO:
                        errorMsg = "HIH8000: Obtained an RH reading that is abnormally different from the previous RH reading";
                        break;
                    case 25 + OFFSET_ARDUINO:
                        errorMsg = "FS5: Obtained a velocity reading that is abnormally different from the previous velocity reading";
                        break;
                    case 26 + OFFSET_ARDUINO:
                        errorMsg = "HX711: No weight readings were obtained during a measurement cycle";
                        break;
                    case ARD_ABNORMAL_WEIGHT_TARE + OFFSET_ARDUINO:
                        errorMsg = "HX711: No weight readings were obtained for taring the weighing scale during a measurement cycle";
                        break;
                    case ARD_ABNORMAL_WEIGHT_CAL + OFFSET_ARDUINO:
                        errorMsg = "HX711: No weight readings were obtained for calibrating the weighing scale during a measurement cycle";
                        break;
                    /************************************************
                    * Errors sent from Arduino, specifically PCA9685*
                    * 230 - <255 are reserved for PCA9685 errors    *
                    ************************************************/
                    case 1 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "I2C timeout while waiting for successful completion of a Start bit";
                        break;
                    case 2 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "I2C timeout while waiting for ACK/NACK while addressing slave in transmit mode (MT)";
                        break;
                    case 3 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "I2C timeout while waiting for ACK/NACK while sending data to the slave";
                        break;
                    case 4 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "I2C timeout while waiting for successful completion of a Repeated Start";
                        break;
                    case 5 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "I2C timeout while waiting for ACK/NACK while addressing slave in receiver mode (MR)";
                        break;
                    case 6 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "I2C timeout while waiting for ACK/NACK while receiving data from the slave";
                        break;
                    case 7 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "I2C timeout while waiting for successful completion of the Stop bit";
                        break;
                    case 8 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "\"See datasheet of microcontroller chip for exact meaning\"";
                        break;
                    case 9 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "This chip is set as a proxy device";
                        break;
                    case 10 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "The given PWM channel is out of range";
                        break;
                    case 11 + OFFSET_ARDUINO + OFFSET_PCA9685:
                        errorMsg = "Any other PCA9685 errors";
                        break;
                    /************************************************
                    * Special "errors" sent from Arduino            *
                    * Indicate chamber connection/disconnection     *
                    ************************************************/
                    case ERR_DISCONNECT:
                        errorMsg = "Chamber is disconnected.";
                        break;
                    case ERR_CONNECT:
                        // This is not an actual error; just a notification sent by Arduino saying the chamber is connected.
                        // Fill it up anyway just in case somewhere in code failed to check for this.
                        errorMsg = "Chamber is connected.";
                        break;
                    /************************************************
                    * Unknown errors                                *
                    ************************************************/
                    case 999:
                        // This is a safety net for any exceptions that I hadn't thought to catch. Helps prevent program from crashing due to uncaught exceptions.
                        errorMsg = "Exception not programmed to be caught (yet): " + message;
                        break;
                    default:
                        errorMsg = "Unknown error code";
                        break;
                }

                // Put error on queue
                string[] errorFragments = { errorPrefix, errorMsg };
                errorLog_.Enqueue(errorFragments);
            }
            catch (Exception)
            {
                // If we had problem logging the error, we really can't do anything anymore. This
                // exception catching is just to prevent unhandled exceptions. No handling will be
                // done for this situation, so the exception would not be logged.
            }
        }

        // Obtain line number for easier debugging
        // NOTE: This requires the .PDB file to be in the same location as the .EXE file.
        public string getLineNumber(Exception err)
        {
            StackTrace st = new StackTrace(err, true);
            int line = st.GetFrame(st.FrameCount - 1).GetFileLineNumber();
            return Convert.ToString(line);
            //handleCProgError(999, String.Concat("Line ", Convert.ToString(line), ": ", err.Message));
        }

        // Handle unknown errors
        public void logUnknownError(Exception err)
        {
            logCProgError(999, err, err.Message);
        }

        // This should be run on its own thread.
        public void logErrors()
        {
            // Goes through the queue of errors to-be-logged and logs them.
            errorLoggingActive_ = true;

            while (errorLoggingActive_)
            {
                string[] errorFragments;

                // Check if there are any commands to be logged
                while (errorLog_.TryDequeue(out errorFragments))
                {
                    try
                    {
                        // Log the error
                        StreamWriter errorLogger = new StreamWriter(new FileStream(errorLogLocation_, FileMode.Append, FileAccess.Write, FileShare.Read)); // default encoding is UTF-8
                        errorLogger.Write("{0}{1}{2}{3}", new Object[] {  DateTime.Now.ToString(),
                                                                        errorFragments[0],
                                                                        errorFragments[1],
                                                                        Environment.NewLine });
                        errorLogger.Close();
                    }
                    catch (Exception)
                    {
                        // If we had problem logging the error, we really can't do anything anymore. This
                        // exception catching is just to prevent unhandled exceptions. No handling will be
                        // done for this situation, so the exception would not be logged.
                    }
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        public void stopLoggingErrors()
        {
            errorLoggingActive_ = false;
        }
    }
}