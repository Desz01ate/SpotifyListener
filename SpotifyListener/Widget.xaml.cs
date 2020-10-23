using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SpotifyListener
{
    /// <summary>
    /// Interaction logic for Widget.xaml
    /// </summary>
    public partial class Widget : Window
    {
        private static Widget Context;
        public static Widget GetContext(MainWindow mainWindow)
        {
            if (Context == null)
            {
                Context = new Widget(mainWindow);
            }
            return Context;
        }
        #region P/Invoke
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
               int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd,
           IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr BeginDeferWindowPos(int nNumWindows);
        [DllImport("user32.dll")]
        static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        const UInt32 SWP_NOZORDER = 0x0004;
        const int WM_ACTIVATEAPP = 0x001C;
        const int WM_ACTIVATE = 0x0006;
        const int WM_SETFOCUS = 0x0007;
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const int WM_WINDOWPOSCHANGING = 0x0046;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);

            IntPtr windowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource src = HwndSource.FromHwnd(windowHandle);
            src.AddHook(new HwndSourceHook(WndProc));
        }
        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SETFOCUS)
            {
                IntPtr _hWnd = new WindowInteropHelper(this).Handle;
                SetWindowPos(_hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                handled = true;
            }
            return IntPtr.Zero;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            if (windowHandle != IntPtr.Zero)
            {
                HwndSource src = HwndSource.FromHwnd(windowHandle);
                src.RemoveHook(new HwndSourceHook(this.WndProc));
            }
        }
        #endregion
        private MainWindow _parentWindow { get; set; }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        Widget(MainWindow mainWindow)
        {
            _parentWindow = mainWindow;
            this.ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
            var compensation = 5;
            var startLoc = System.Windows.SystemParameters.PrimaryScreenWidth - compensation;
            MouseEnterAnimation.Duration = TimeSpan.FromMilliseconds(400);
            MouseLeaveAnimation.Duration = TimeSpan.FromMilliseconds(400);
            MouseEnterAnimation.From = startLoc;
            MouseEnterAnimation.To = startLoc - WidgetWindow.Width + compensation;
            MouseLeaveAnimation.From = startLoc - WidgetWindow.Width + compensation;
            MouseLeaveAnimation.To = startLoc;
            WidgetWindow.Left = startLoc;
            WidgetWindow.Top = 150 + compensation + WidgetWindow.Height;
            WidgetImage.MouseDown += Widget_OnMouseDown;
            this.Loaded += Window_Loaded;
            this.Closing += Window_Closing;
            this.MouseDown += Window_MouseDown;
        }

        private void Widget_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _parentWindow.WindowState = WindowState.Normal;
        }
    }
}
