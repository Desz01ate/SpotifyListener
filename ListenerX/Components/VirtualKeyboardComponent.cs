using ListenerX.ChromaExtension;
using ListenerX.Classes;
using ListenerX.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VirtualGrid.Interfaces;

namespace ListenerX.Components
{
    public class VirtualKeyboardComponent : PictureBox
    {
        private readonly System.Timers.Timer timer;

        private readonly VirtualLedGridExtensions.VirtualGridRendererImpl gridRenderer;

        public event EventHandler OnImageChanged;

        private readonly object lockObj = new object();

        public readonly IVirtualLedGrid VirtualGrid;
        public VirtualKeyboardComponent(IVirtualLedGrid virtualGrid, bool autoStart = true)
        {
            this.VirtualGrid = virtualGrid;
            this.timer = new System.Timers.Timer();
            this.timer.Interval = 33;
            this.timer.Elapsed += Timer_Tick;
            this.gridRenderer = VirtualLedGridExtensions.CreateGridRendererInstance(virtualGrid, 50, 50);
            if (autoStart)
            {
                this.timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                var nextFrame = this.gridRenderer.VisualizeRenderingGrid2(50, 50);
                if (nextFrame != null)
                {
                    this.Image = nextFrame;
                }
                this.OnImageChanged?.Invoke(this, null);
            }
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
            this.gridRenderer.Dispose();
            base.Dispose(disposing);
        }
    }
}
