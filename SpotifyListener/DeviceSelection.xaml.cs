using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MOVE = 0xF010;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private bool initializing;
        public DeviceSelection(Music player, double? location_X = null, double? location_Y = null)
        {
            initializing = true;
            InitializeComponent();
            this.SourceInitialized += DeviceSelectionForm_SourceInitialized;

            if (location_X.HasValue && location_Y.HasValue)
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                Left = location_X.Value;
                Top = location_Y.Value;
            }


            this.ResizeMode = ResizeMode.NoResize;
            var selectIndex = 0;
            for (var idx = 0; idx < player.AvailableDevices.Count(); idx++)
            {
                var device = player.AvailableDevices[idx];
                var text = device.Name;
                if (device.IsActive)
                {
                    text += " (CURRENT ACTIVE)";
                    selectIndex = idx;
                }
                cb_options.Items.Add(text);
            }
            cb_options.SelectedIndex = selectIndex;
            cb_options.SelectionChanged += async delegate
            {
                if (initializing) return;
                try
                {
                    await player.SetActiveDeviceAsync(player.AvailableDevices[cb_options.SelectedIndex].Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    this.Close();
                }
            };
            initializing = false;
        }



        private void DeviceSelectionForm_SourceInitialized(object sender, EventArgs e)
        {
            //thanks to https://stackoverflow.com/questions/2400819/wpf-disable-window-moving

            WindowInteropHelper helper = new WindowInteropHelper(this);
            //SetWindowLong(helper.Handle, GWL_STYLE, GetWindowLong(helper.Handle, GWL_STYLE) & ~WS_SYSMENU);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                case WM_SYSCOMMAND:
                    int command = wParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                    {
                        handled = true;
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
