using Listener.Core.Framework.Players;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ListenerX
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
        private readonly double? location_X, location_Y;
        private readonly IStreamablePlayerHost player;
        public DeviceSelection(IStreamablePlayerHost player, double? location_X = null, double? location_Y = null)
        {
            InitializeComponent();
            this.location_X = location_X;
            this.location_Y = location_Y;
            this.player = player;
            this.SourceInitialized += DeviceSelectionForm_SourceInitialized;
            this.lbl_close.MouseDown += (s, e) => this.Close();
            this.player.DeviceChanged += delegate
            {
                Refresh();
            };
            Refresh();
        }
        const int spaceFactor = 50;
        private void Refresh()
        {
            initializing = true;
            grd_devices.Children.Clear();

            if (location_X.HasValue && location_Y.HasValue)
            {
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                Left = location_X.Value - this.Width - 7;
                Top = location_Y.Value - 7;
            }


            this.ResizeMode = ResizeMode.NoResize;
            var selectIndex = 0;
            for (var idx = 0; idx < player.AvailableDevices.Count(); idx++)
            {
                var device = player.AvailableDevices[idx];
                var text = device.Name;
                if (device.IsActive)
                {
                    selectIndex = idx;
                    var activeText = new TextBlock();
                    activeText.Text = "Active";
                    activeText.Foreground = Brushes.Green;
                    activeText.Margin = new Thickness(0, (idx + 1) * spaceFactor, 30, 0);
                    activeText.VerticalAlignment = VerticalAlignment.Top;
                    activeText.HorizontalAlignment = HorizontalAlignment.Right;
                    grd_devices.Children.Add(activeText);
                }
                else
                {
                    var button = new Label();
                    button.Content = "Switch";
                    button.MouseDown += async (s, e) =>
                    {
                        if (initializing) return;
                        try
                        {
                            await player.SetActiveDeviceAsync(device.Id);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    };
                    button.Margin = new Thickness(0, (idx + 1) * spaceFactor, 20, 0);
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.HorizontalAlignment = HorizontalAlignment.Right;
                    grd_devices.Children.Add(button);
                }
                var icon = new Image();
                var logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri($"pack://application:,,,/{Assembly.GetExecutingAssembly().GetName().Name};component/Resources/{device.Type.ToLower()}.png");
                logo.EndInit();
                logo.Freeze();
                icon.Margin = new Thickness(20, ((idx + 1) * spaceFactor) - (spaceFactor / 2.5), 0, 0);
                icon.HorizontalAlignment = HorizontalAlignment.Left;
                icon.VerticalAlignment = VerticalAlignment.Top;
                icon.Width = 50;
                icon.Height = 50;
                icon.Source = logo;
                grd_devices.Children.Add(icon);
                var textBlock = new TextBlock();
                textBlock.Text = text;
                textBlock.Margin = new Thickness(90, (idx + 1) * spaceFactor, 0, 0);
                textBlock.Width = 200;
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                grd_devices.Children.Add(textBlock);
            }
            this.Height = (spaceFactor * 1.3 * player.AvailableDevices.Count()) + 50;
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
