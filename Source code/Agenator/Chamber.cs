using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Timers;

namespace Agenator
{
    class Chamber
    {
        public int id;
        public int activeStep                           = -1;   // Will be set to -1 if no control steps are currently running
        public int tempActiveStep                       = -1;   // Used to denote the to-be-active step while waiting for Arduino to confirm receipt of command
        public uint chartPointsMax;                             // Maximum number of points to be plotted on the chart
        public UInt32 DAQInterval                       = 1;    // Minimum is every 1 second

        // These vars are related to the state of the chamber
        public bool DAQRunning                          = false;
        public bool controlRunning                      = false;
        public bool recording                           = false;
        public bool legitFilePath                       = false;

        // These vars deal with awaiting command confirmation from Arduino
        public bool startDAQWaiting                     = false;
        public bool stopDAQWaiting                      = false;
        public bool startControlWaiting                 = false;
        public bool nextControlStepWaiting              = false;
        public bool skipControlStepWaiting              = false;
        public bool rampControlStepWaiting              = false;
        public bool pauseControlWaiting                 = false;
        public bool endControlWaiting                   = false;
        public bool RH1CalibrateWaiting                 = false;
        public bool RH2CalibrateWaiting                 = false;
        public bool scaleTareWaiting                    = false;
        public bool scaleCalibrateWaiting               = false;
        public bool showCommandError                    = false;

        // These vars are for readings
        public bool humidityOK                          = true;
        public bool temperatureOK                       = true;
        public bool weightOK                            = true;
        public bool velocityOK                          = true;
        public bool fan1SpeedOK                         = true;
        public bool fan2SpeedOK                         = true;
        public double humidity                          = 0;
        public double temperature                       = 0;
        public double weight                            = 0;
        public double velocity                          = 0;
        public double fan1Speed                         = 0;
        public double fan2Speed                         = 0;

        // Humidity sensor settings
        public double RHStd1                            = 33.6;
        public double RHRaw1                            = 33.6;
        public double RHStd2                            = 75.6;
        public double RHRaw2                            = 75.6;

        // Weighing scale settings
        public double scaleFactor                       = 100.538;
        public int scaleOffset                          = 1000;

        // These vars deal with chamber connection and disconnection
        public bool chamberConnected                    = false;
        public bool DAQPaused                           = false;
        public bool controlPaused                       = false;
        public bool recordPaused                        = false;

        // Vars used in control of RH and fan speed
        public TimeSpan nextRampTime                    = new TimeSpan(0,0,0);
        public double rampTotalSeconds                  = 0;
        public double RHCurSetpoint                     = 0;
        public double RHInitial                         = 0;
        public double fanSpeedCurSetpoint               = 0;
        public double fanSpeedInitial                   = 0;

        // Charting variables
        public uint plottedPoints                       = 0;
        public List<double> chartTime                   = new List<double>();
        public List<double> chartRH                     = new List<double>();   //  If value is -100000, means there is an error!
        public List<double> chartWeight                 = new List<double>();   //  If value is -100000, means there is an error!
        public DateTime DAQStart                        = new DateTime();

        // Recording variables
        public FileStream recordStream                  = null;
        public StreamWriter CSVWriter                   = null;

        public decimal calibrationWeight                = 0;
        public string recordFilePath                    = "";
        public List<HumidityStep> steps                 = new List<HumidityStep>();
        public Timer stepTimer                          = new Timer();
        public ElapsedEventHandler stepTimerElapsedHandler  = null;

        // When initializing a chamber class, create one dummy humidity control step
        public Chamber(int id, uint chartIntervalMax)
        {
            this.id = id;
            stepTimer.Interval = 1000; // 1000 milliseconds
            stepTimer.AutoReset = true;
            stepTimer.Stop();

            // Chart intervals are in min while data acquisition is in seconds; convert to seconds
            // +1 because we are converting intervals to points
            chartPointsMax = chartIntervalMax * 60 + 1;
        }

        // Sort the control steps according to their step number
        public void sortSteps()
        {
            steps = steps.OrderBy(s => s.order).ToList();
        }

        // Reorganize the list of steps, which is necessary after removing a step from the list.
        // The reorganization basically changes the step numbers so that the void left from the
        // removed step is quenched.
        public void updateOrder(UInt16 affectedStepNumber, bool moveUp)
        {
            if (moveUp)
            {
                steps[affectedStepNumber].order--;
                steps[affectedStepNumber - 1].order++;
            }
            else
            {
                steps[affectedStepNumber].order++;
                steps[affectedStepNumber + 1].order--;
            }

            sortSteps();
        }
    }
}
