using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenator
{
    class OutgoingCommand
    {
        public int commandID;   // Identifier of this command

        public readonly string commandType;     // The type of the command. See the constants at beginning of Commands class definition
        public readonly string parameterString; // Would contain parameters of the command such as chamber ID, etc..
        public readonly bool awaitResponse;     // Whether to wait for a response to this command or not
        public readonly int timeout;            // Max amount of time (ms) to wait before considering this command as timed out
        public readonly int maxAttempts;        // Max number of times to retry this command if it times out
        public int attempts;                    // Tracks the number of attempts to send this command
        public string fullCommand;              // The full command string
        public int chamberID;                   // Chamber ID. Only set and used by commands specific to chambers.

        // These are used only for loading autosaves with control mode active
        // DO NOT use for other commands.
        // TODO: Good practice would be to make another class that inherits this class but with these extra variables
        //       In fact, a class for chamber-specific commands should be made for the chamberID var too.
        public bool controlAfterDAQ = false;

        public OutgoingCommand(string givenCommandType, bool givenAwaitResponse, int givenTimeout, int givenMaxAttempts, string givenParameterString = "")
        {
            commandType     = givenCommandType;
            parameterString = givenParameterString;
            awaitResponse   = givenAwaitResponse;
            timeout         = givenTimeout;
            maxAttempts     = givenMaxAttempts;
            attempts        = 0;
        }
    }
}
