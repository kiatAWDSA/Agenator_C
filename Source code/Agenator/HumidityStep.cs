using System;
using System.Windows.Forms;

namespace Agenator
{
    public class HumidityStep
    {
        public enum stepType
        {
            TYPE_HOLD = 1,
            TYPE_RAMP = 2
        }

        public enum stepStatus
        {
            STATUS_COMPLETED = 1,
            STATUS_RUNNING = 2,
            STATUS_WAITING = 3
        }

        public UInt16 order;
        public stepType type;
        public double targetHumidity;
        public double targetFanSpeed;
        public stepStatus status;
        public TimeSpan timeRemaining;
        public NumericUpDown dayRemainingControl;
        public NumericUpDown hourRemainingControl;
        public NumericUpDown minuteRemainingControl;
        public NumericUpDown secondRemainingControl;

        public HumidityStep(UInt16 order, stepType type, double targetHumidity, int dayRemaining, int hourRemaining, int minuteRemaining, int secondRemaining, double targetFanSpeed, stepStatus status)
        {
            this.order = order;
            this.type = type;
            this.targetHumidity = targetHumidity;
            timeRemaining = new TimeSpan(dayRemaining, hourRemaining, minuteRemaining, secondRemaining);
            this.targetFanSpeed = targetFanSpeed;
            this.status = status;
        }

        public void updateTimeRemaining(uint timeType, int newValue)
        {
            int days    = timeRemaining.Days;
            int hours   = timeRemaining.Hours;
            int minutes = timeRemaining.Minutes;
            int seconds = timeRemaining.Seconds;

            switch(timeType)
            {
                case 1: // days
                    timeRemaining = new TimeSpan(newValue, hours, minutes, seconds);
                    break;
                case 2: // hours
                    timeRemaining = new TimeSpan(days, newValue, minutes, seconds);
                    break;
                case 3: // minutes
                    timeRemaining = new TimeSpan(days, hours, newValue, seconds);
                    break;
                case 4: // seconds
                    timeRemaining = new TimeSpan(days, hours, minutes, newValue);
                    break;
            }
        }
    }
}
