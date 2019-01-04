using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Agenator
{
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Command logger
    // Logs commands into a given log file.
    //
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class CommandLogger
    {
        private ConcurrentQueue<OutgoingCommand> commandLog_;
        private bool commandLoggingActive_ = false;
        private readonly string commandLogLocation_;
        private ErrorLogger errorLogger_;

        public CommandLogger (string fileSavePath, ErrorLogger errorLogger)
        {
            commandLogLocation_ = fileSavePath;
            errorLogger_ = errorLogger;
            commandLog_ = new ConcurrentQueue<OutgoingCommand>();
        }

        // This should be run on its own thread.
        public void logCommands()
        {
            // Goes through the queue of commands to-be-logged and logs them.
            // Note that SERIAL_CMD_EOL is only sent out during port_.Write(),
            // so the original command phrase doesn't have it. This is important so that we don't
            // create additional newlines while logging.

            commandLoggingActive_ = true;

            while (commandLoggingActive_)
            {
                OutgoingCommand outgoingCommand;

                // Check if there are any commands to be logged
                while (commandLog_.TryDequeue(out outgoingCommand))
                {
                    try
                    {
                        // Check if this is a resent command
                        string resendPostfix = "";

                        if (outgoingCommand.attempts > 0)
                        {
                            resendPostfix = " (Re-sent)";
                        }

                        // Log the command
                        StreamWriter commandLogger = new StreamWriter(new FileStream(commandLogLocation_, FileMode.Append, FileAccess.Write, FileShare.Read)); // default encoding is UTF-8
                        commandLogger.Write("{0}{1}{2}{3}{4}",
                                            new Object[] {  DateTime.Now.ToString(),
                                                            " | ",
                                                            outgoingCommand.fullCommand,
                                                            resendPostfix,
                                                            Environment.NewLine});
                        commandLogger.Close();
                    }
                    catch (Exception err)
                    {
                        errorLogger_.logUnknownError(err);
                    }
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        public void logCommand(OutgoingCommand command)
        {
            commandLog_.Enqueue(command);
        }

        public void stopLoggingCommands()
        {
            commandLoggingActive_ = false;
        }
    }
}
