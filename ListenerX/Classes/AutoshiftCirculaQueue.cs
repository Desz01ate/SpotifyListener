using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ListenerX.Classes
{
    class AutoshiftCirculaQueue<T> : CircularQueue<T>
    {
        readonly Timer shiftTimer;
        public AutoshiftCirculaQueue(int limitSize, int shiftInMs) : base(limitSize)
        {
            this.shiftTimer = new Timer();
            this.shiftTimer.Interval = shiftInMs;
            this.shiftTimer.Elapsed += ShiftTimer_Elapsed;
            this.shiftTimer.Start();
        }

        public AutoshiftCirculaQueue(IEnumerable<T> source, int shiftInMs) : base(source)
        {
            this.shiftTimer = new Timer();
            this.shiftTimer.Interval = shiftInMs;
            this.shiftTimer.Elapsed += ShiftTimer_Elapsed;
            this.shiftTimer.Start();
        }

        private void ShiftTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.ShiftLeft();
        }
    }
}
