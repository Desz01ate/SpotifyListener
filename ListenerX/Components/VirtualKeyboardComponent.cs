using ListenerX.Classes;
using ListenerX.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListenerX.Components
{
    public class VirtualKeyboardComponent : PictureBox
    {
        private readonly System.Timers.Timer timer;

        private readonly int boxSize;

        public event EventHandler OnImageChanged;
        
        public VirtualKeyboardComponent(int boxSize, bool autoStart = true)
        {
            this.boxSize = boxSize;
            this.timer = new System.Timers.Timer();
            this.timer.Interval = 40;
            this.timer.Elapsed += Timer_Tick;
            if (autoStart)
            {
                this.timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                var nextFrame = AbstractKeyGrid.ActiveGrid.VisualizeRenderingGrid(this.boxSize, this.boxSize);
                if (nextFrame != null)
                {
                    this.Image?.Dispose();
                    this.Image = nextFrame;
                }
                this.OnImageChanged?.Invoke(this, null);
            });
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            this.timer.Dispose();
            base.Dispose(disposing);
        }
    }
}
