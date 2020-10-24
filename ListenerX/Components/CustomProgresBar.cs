using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ListenerX.Components
{
    public class CustomProgresBar : ProgressBar
    {
        public object Content
        {
            get => null;
            private set
            {

            }
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }
    }
}
