using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpotifyListener
{
    /// <summary>
    /// Interaction logic for DeviceSelection.xaml
    /// </summary>
    public partial class DeviceSelection : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private List<SpotifyAPI.Web.Models.Device> _device;
        private Music _player;
        public DeviceSelection(List<SpotifyAPI.Web.Models.Device> devices, Action<string> action)
        {
            InitializeComponent();
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            this.ResizeMode = ResizeMode.NoResize;
            _device = devices;
            var selectIndex = 0;
            for (var idx = 0; idx < _device.Count(); idx++)
            {
                var text = _device[idx].Name;
                if (_device[idx].IsActive)
                {
                    text += " (CURRENT ACTIVE)";
                    selectIndex = idx;
                }
                cb_options.Items.Add(text);
                cb_options.SelectedIndex = selectIndex;
                cb_options.SelectionChanged += delegate
                {
                    action(_device[cb_options.SelectedIndex].Id);
                    Close();
                };
            }
        }
    }
}
