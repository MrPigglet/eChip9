using System;
using System.Timers;

namespace eChip9
{
    public class DecreaseTimer
    {
        private Timer timer;
        public byte value;
        public DecreaseTimer() // Create a timer that decreases at a rate of 60 hz
        {
            timer = new Timer();
            timer.Interval = (1000 / 60);
            timer.Elapsed += DecreaseValue;
            timer.Enabled = true;
        }
        private void DecreaseValue(Object source, ElapsedEventArgs e)
        {
            // Remove 1 from value 60 times per second as long as value is larger than 0
            if (value > 0) 
            {
                value--;
            }
        }
    }
}
