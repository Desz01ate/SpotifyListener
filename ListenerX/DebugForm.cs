using ListenerX.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListenerX
{
    public partial class DebugForm : Form
    {
        Timer timer;
        public DebugForm()
        {
            InitializeComponent();
            this.Width = 1520;
            this.Height = 400;
            pictureBox1.Width = this.Width;
            pictureBox1.Height = this.Height;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 40;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Image image = pictureBox1.Image;
            var newImage = AbstractKeyGrid.GetDefaultGrid().VisualizeRenderingGrid();
            if (newImage != null)
            {
                pictureBox1.Image = newImage;
                if (image != null)
                    image.Dispose();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timer?.Dispose();
            base.OnClosing(e);
        }
    }
}
