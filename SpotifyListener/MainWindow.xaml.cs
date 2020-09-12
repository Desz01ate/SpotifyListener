using SpotifyListener.ChromaExtension;
using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.ComponentModel;
using SpotifyListener.Interfaces;
using SpotifyListener.Classes;
//using SpotifyAPI.Web.Models;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web;
using SpotifyListener.Configurations;
using System.Text;
using System.Net.Http;
using System.Net;

namespace SpotifyListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Timer ChromaTimer = new Timer();
        public SpotifyPlayer Player { get; }
        private AnimationController animation;
        private static MMDevice ActiveDevice;
        private static readonly ChromaWrapper Chroma = ChromaWrapper.GetInstance;

        private readonly SolidColorBrush playColor = (SolidColorBrush)(new BrushConverter().ConvertFromString("#5aFF5a"));
        private readonly SolidColorBrush pauseColor = (SolidColorBrush)(new BrushConverter().ConvertFromString("#FF5a5a"));

        Wallpaper wallpaper = null;
        SearchPanel searchPanel;

        public static MainWindow Context { get; private set; }
        public readonly double InitWidth, InitHeight;
        public MainWindow()
        {
            try
            {
                Process.Start("spotify");
                InitializeComponent();
                InitWidth = this.Width;
                InitHeight = this.Height;
                ResizeMode = ResizeMode.CanMinimize;
                Visibility = Visibility.Hidden;

                SpotifyWebAPI client = default;
                SpotifyConfiguration.Context.OnClientReady += (s, e) =>
                {
                    client = e;
                };
                while (client == null)
                {
                    System.Threading.Thread.Sleep(100);
                }
                Player = new SpotifyPlayer(client);

                VolumePath.Fill = playColor;
                VolumeProgress.Foreground = lbl_Album.Foreground;

                Player.OnTrackChanged += OnTrackChanged;
                Player.OnDeviceChanged += Player_OnDeviceChanged;
                Player.OnTrackDurationChanged += (p) =>
                {
                    this.VolumePath.Fill = p.IsMute ? pauseColor : playColor;
                };
                Player.OnPlayStateChanged += (state) =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        this.PlayProgress.Foreground = state == Enums.PlayState.Play ? playColor : pauseColor;
                    });
                };
                using (var devices = new MMDeviceEnumerator())
                    ActiveDevice = devices.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).OrderByDescending(x => x.AudioMeterInformation.MasterPeakValue).FirstOrDefault();
                KeyDown += MainWindowGrid_PreviewKeyDown;
                Loaded += MainWindow_Loaded;
                MouseDown += Window_MouseDown;
                btn_Minimize.Click += (s, e) => this.WindowState = WindowState.Minimized;
                btn_Close.Click += (s, e) => this.Close();
                this.AlbumImage.MouseDown += AlbumImage_MouseDown;

                if (!Chroma.IsError)
                {
                    ChromaTimer.Interval = (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);//TimeSpan.FromMilliseconds((int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0));
                    ChromaTimer.Tick += ChromaTimer_Tick;
                    ChromaTimer.Start();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            this.Visibility = Visibility.Visible;
            Context = this;
            this.DataContext = Player;
        }

        private void Player_OnDeviceChanged(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                this.Title = $"Listening to {Player.Track} by {Player.Artist} on {Player.ActiveDevice.Name}";
            });
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            animation = new AnimationController(this);

            MouseEnter += OnMouseEnterEvent;
            MouseLeave += OnMouseLeaveEvent;
            #region get current background image

            if (Wallpaper.TryGetWallpaper(out var filePath))
            {
                wallpaper = new Wallpaper(filePath, 20f, this.FontFamily.ToString());
            }
            else
            {
                wallpaper = new Wallpaper(20f, this.FontFamily.ToString());
                System.Windows.Forms.Application.Exit();
            }
            wallpaper.SetPlayerBase(Player);
            #endregion

        }


        private void AlbumImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    Player.PlayPause();
                    //Player.PlayAsync("");
                    break;
                case MouseButton.Right:
                    Task.Run(() => Process.Start(Player.Url));
                    break;
            }
        }

        private void OnTrackChanged(IMusic playbackContext)
        {
            Dispatcher.InvokeAsync(() =>
            {
                this.Title = $"Listening to {Player.Track} by {Player.Artist} on {Player.ActiveDevice.Name}";
                this.Icon = Player.AlbumSource;
                wallpaper.Enable();
            });
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch
            {
                //do not remove try/catch block as the form will error when focus is lost.
            }
        }
        private void OnMouseLeaveEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animation.TransitionDisable();
        }
        private void OnMouseEnterEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animation.TransitionEnable();
        }
        private void ChromaTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!Properties.Settings.Default.ChromaSDKEnable)
                {
                    Chroma.SDKDisable();
                    return;
                }
                var volume = ActiveDevice.AudioMeterInformation.MasterPeakValue * (Properties.Settings.Default.VolumeScale / 10.0f);
                if (volume > 1) volume = 1;
                var density = Properties.Settings.Default.AdaptiveDensity && Player.IsPlaying ? volume * 0.7f : (Properties.Settings.Default.Density / 10.0);
                Chroma.LoadColor(Player, Player.IsPlaying, density);
                Chroma.SetDevicesBackground();
                if (Properties.Settings.Default.RenderPeakVolumeEnable)
                {
                    if (Properties.Settings.Default.SymmetricRenderEnable)
                    {
                        Chroma.MouseGrid.SetPeakVolumeSymmetric(Chroma.VolumeColor, volume);
                        Chroma.KeyboardGrid.SetPeakVolumeSymmetric(Chroma.VolumeColor, volume);
                    }
                    else if (Properties.Settings.Default.PeakChroma)
                    {
                        Chroma.MouseGrid.SetChromaPeakVolume(volume);
                        Chroma.KeyboardGrid.SetChromaPeakVolume(volume);
                    }
                    else
                    {
                        Chroma.MouseGrid.SetPeakVolume(Chroma.VolumeColor, volume);
                        Chroma.KeyboardGrid.SetPeakVolume(Chroma.VolumeColor, volume);
                        //Chroma.SetPeakVolume_Mouse(ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        //Chroma.SetPeakVolume_Keyboard(ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        //Chroma.SetPeakVolume_Headset_Mousepad();
                    }
                    Chroma.HeadsetGrid.SetPeakVolume(Chroma.VolumeColor);
                    Chroma.MousepadGrid.SetPeakVolume(Chroma.VolumeColor);
                }
                else
                {
                    Chroma.MouseGrid.SetPlayingPosition(Chroma.PositionColor_Foreground, Chroma.PositionColor_Background, Player.CalculatedPosition, Properties.Settings.Default.ReverseLEDRender);
                    Chroma.KeyboardGrid.SetPlayingPosition(Chroma.PositionColor_Foreground, Chroma.PositionColor_Background, Player.CalculatedPosition);
                    Chroma.MouseGrid.SetVolumeScale(Chroma.VolumeColor, Player.Volume, Properties.Settings.Default.ReverseLEDRender);
                }
                Chroma.KeyboardGrid.SetVolumeScale(Properties.Settings.Default.Volume.ToColoreColor(), Player.Volume);
                Chroma.KeyboardGrid.SetPlayingTime(TimeSpan.FromMilliseconds(Player.Position_ms));
                Chroma.MousepadGrid.SetPeakVolume(Chroma.VolumeColor);
                Chroma.HeadsetGrid.SetPeakVolume(Chroma.VolumeColor);
                Chroma.Apply();
            }
            catch
            {
                Chroma.SDKDisable();
            }
        }

        private void PlayProgress_Click(object sender, EventArgs e)
        {
            Player.SetPositionAsync((int)PlayProgress.CalculateRelativeValue()).ConfigureAwait(false);
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void FacebookShare()
        {
            var app_id = "139971873511766"; //StatusReporter
            var href = Player.Url;
            var redirect_uri = string.Empty;
            var hashtag = $"%23{Player.Artist}_{Player.Track}";
            hashtag = Regex.Replace(hashtag, @"[^0-9a-zA-Z:_%]+", "");
            var requestText = $"https://www.facebook.com/dialog/share?app_id={app_id}&text=test&display=page&href={href}&redirect_uri={redirect_uri}&hashtag={hashtag}";
            Process.Start(requestText);
        }
        private void MainWindowGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.A:
                        BackPath_Click(null, null);
                        break;
                    case Key.D:
                        NextPath_Click(null, null);
                        break;
                    case Key.W:
                        Player.SetVolume(Player.Volume + 10);
                        break;
                    case Key.S:
                        Player.SetVolume(Player.Volume - 10);
                        break;
                    case Key.Q:
                        Player.SetPosition(Player.Position_ms - 15);
                        break;
                    case Key.E:
                        Player.SetPosition(Player.Position_ms + 15);
                        break;
                    case Key.Space:
                        PlayPath_Click(null, null);
                        break;
                    case Key.F:
                        FacebookShare();
                        break;
                    case Key.O:
                        Process.Start(Player.Url);
                        break;
                    case Key.P:
                        GenerateFormImage();
                        System.Threading.Thread.Sleep(100);
                        break;
                }
            }
            catch (Exception ex)
            {
                //pass
            }
        }

        private void BackPath_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Position_ms > 3000)
                Player.SetPositionAsync(0).ConfigureAwait(false);
            else
                Player.Previous();
        }

        private void PlayPath_Click(object sender, RoutedEventArgs e)
        {
            Player.PlayPause();
        }

        private void NextPath_Click(object sender, RoutedEventArgs e)
        {
            Player.Next();
        }

        private void VolumePath_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Volume > 0)
            {
                Player.Mute();
            }
            else
            {
                Player.Unmute();
            }
        }

        private void VolumeProgress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var value = (int)VolumeProgress.CalculateRelativeValue();
            Player.SetVolume(value);
        }

        private void Settings_Click(object sender, MouseButtonEventArgs e)
        {
            using (var setting = new Settings())
            {
                setting.ShowDialog();
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //keep this running on main thread, otherwise it will terminated before the task is done.
            this.Hide();
            searchPanel?.Close();
            ChromaTimer?.Dispose();
            wallpaper?.Dispose();
            Player?.Dispose();
            Chroma?.Dispose();
            base.OnClosing(e);
        }

        private void GenerateFormImage()
        {
            var file = Path.GetTempFileName().Replace("tmp", "jpg");
            wallpaper.SaveWallpaperToFile(file);
            Process.Start(file);
        }

        private void Btn_Repeat_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Btn_Shuffle_Click(object sender, RoutedEventArgs e)
        {
            Player.ToggleShuffle();
        }

        private void AdjustSettings_Click(object sender, RoutedEventArgs e)
        {
            using (var setting = new Settings())
            {
                setting.ShowDialog();
            }
        }

        private void btn_device_Click(object sender, RoutedEventArgs e)
        {
            var ds = new DeviceSelection(Player, this.Left + this.InitWidth, this.Top + this.InitHeight);
            ds.ShowDialog();
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            if (searchPanel != null)
            {
                searchPanel.BringToFront();
            }
            else
            {
                searchPanel = new SearchPanel(Player, () => searchPanel = null);
                searchPanel.Show();
            }
        }

        private void Btn_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            GenerateFormImage();
        }
    }
}
