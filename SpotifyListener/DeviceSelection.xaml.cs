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
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private bool initializing;
        public DeviceSelection(Music player)
        {
            initializing = true;
            InitializeComponent();
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

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
    }
}
