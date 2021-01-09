using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.ComponentModel;
using System.Drawing.Imaging;
using Listener.Core.Framework.Players;
using Listener.Core.Framework.Events;
using Listener.Core.Framework.Models;
using Listener.Core.Framework.Helpers;
using Listener.ImageProcessing;
using ListenerX.ChromaExtension;
using ListenerX.Classes;
using ListenerX.Helpers;
using ListenerX.Foundation.Struct;

namespace ListenerX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer chromaTimer = new Timer();
        //private readonly Timer defaultAudioEndpointTimer = new Timer();
        private AnimationController animation;
        private readonly MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
        private MMDevice defaultAudioEndpoint;
        private readonly ChromaWorker chroma;

        private readonly SolidColorBrush playColor =
            (SolidColorBrush)(new BrushConverter().ConvertFromString("#5aFF5a"));

        private readonly SolidColorBrush pauseColor =
            (SolidColorBrush)(new BrushConverter().ConvertFromString("#FF5a5a"));

        private Wallpaper wallpaper;
        private SearchPanel searchPanel;

        private readonly IStreamablePlayerHost player;

        private readonly double InitWidth, InitHeight;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                InitWidth = this.Width;
                InitHeight = this.Height;
                ResizeMode = ResizeMode.CanMinimize;
                Visibility = Visibility.Hidden;

                player = this.LoadPlayerHost<Listener.Player.Spotify.SpotifyPlayerHost>();

                VolumePath.Fill = playColor;
                VolumeProgress.Foreground = lbl_Album.Foreground;

                player.OnTrackChanged += OnTrackChanged;
                player.OnDeviceChanged += Player_OnDeviceChanged;
                player.OnTrackDurationChanged += (p) =>
                {
                    this.VolumePath.Fill = p.IsMute ? pauseColor : playColor;
                };

                player.OnTrackPlayStateChanged += (state) =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        this.PlayProgress.Foreground = state == PlayState.Play ? playColor : pauseColor;
                    });
                };

                //defaultAudioEndpointTimer.Interval = 1000;
                //defaultAudioEndpointTimer.Tick += DefaultAudioEndpointTimer_Tick;
                //defaultAudioEndpointTimer.Start();
                defaultAudioEndpoint = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                KeyDown += MainWindowGrid_PreviewKeyDown;
                Loaded += MainWindow_Loaded;
                MouseDown += Window_MouseDown;
                btn_Minimize.Click += (s, e) => this.WindowState = WindowState.Minimized;
                btn_Close.Click += (s, e) => this.Close();
                this.AlbumImage.MouseDown += AlbumImage_MouseDown;

                if (Properties.Settings.Default.ChromaSDKEnable)
                {
                    var chroma = ChromaWorker.Instance;
                    if (!chroma.IsError)
                    {
                        this.chroma = chroma;
                        chromaTimer.Interval =
                            (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);
                        chromaTimer.Tick += ChromaTimer_Tick;
                        chromaTimer.Start();
                    }
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Environment.Exit(1);
            }

            this.Visibility = Visibility.Visible;
            this.DataContext = player;
        }

        //private void DefaultAudioEndpointTimer_Tick(object sender, EventArgs e)
        //{
        //    var currentDefaultAudioEndpoint = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        //    if (defaultAudioEndpoint == null || defaultAudioEndpoint.FriendlyName != currentDefaultAudioEndpoint.FriendlyName)
        //    {
        //        defaultAudioEndpoint?.Dispose();
        //        defaultAudioEndpoint = currentDefaultAudioEndpoint;
        //    }
        //}

        private void Player_OnDeviceChanged(Device device)
        {
            Dispatcher.InvokeAsync(() =>
            {
                this.Title = $"Listening to {player.Track} by {player.Artist} on {player.ActiveDevice.Name}";
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
                wallpaper = new Wallpaper(filePath, this.FontFamily.ToString());
            }
            else
            {
                wallpaper = new Wallpaper(this.FontFamily.ToString());
                System.Windows.Forms.Application.Exit();
            }

            wallpaper.SetPlayerBase(player);

            #endregion
        }


        private void AlbumImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    player.PlayPause();
                    //Player.PlayAsync("");
                    break;
                case MouseButton.Right:
                    Task.Run(() => Process.Start(player.Url));
                    break;
            }
        }

        private void OnTrackChanged(IPlayerHost playbackContext)
        {
            Dispatcher.InvokeAsync(() =>
            {
                this.Title = $"Listening to {player.Track} by {player.Artist} on {player.ActiveDevice.Name}";
                this.Icon = player.AlbumSource;
                //this.btn_lyrics.Visibility = string.IsNullOrWhiteSpace(playbackContext.Lyrics)
                //    ? Visibility.Hidden
                //    : Visibility.Visible;
                var colors = player.AlbumArtwork.GetDominantColors(2);
                var standardRenderColor = new StandardColor();
                standardRenderColor.Standard = colors[0];
                standardRenderColor.Complemented = colors[1];

                this.chroma?.LoadColor(standardRenderColor);


                if (Properties.Settings.Default.ArtworkWallpaperEnable)
                {
                    wallpaper.Enable();
                }
                else
                {
                    wallpaper.Disable();
                }

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
                if (defaultAudioEndpoint == null)
                {
                    chroma.SDKDisable();
                    return;
                }

                float volume = defaultAudioEndpoint.AudioMeterInformation.MasterPeakValue * (Properties.Settings.Default.VolumeScale / 10.0f);
                if (volume > 1 || volume < 0e-6 || !player.IsPlaying)
                    volume = 1;

                if (Properties.Settings.Default.RenderPeakVolumeEnable && Properties.Settings.Default.PeakChroma)
                {
                    chroma.PeakVolumeChromaEffects(volume, Properties.Settings.Default.SymmetricRenderEnable);
                }
                else if (Properties.Settings.Default.RenderPeakVolumeEnable)
                {
                    chroma.PeakVolumeEffects(volume, Properties.Settings.Default.SymmetricRenderEnable);
                }
                else
                {
                    chroma.PlayingPositionEffects(this.player, volume, Properties.Settings.Default.ReverseLEDRender);
                }

                //chroma.KeyboardGrid.SetVolumeScale(Properties.Settings.Default.Volume.ToColoreColor(), player.Volume);
                //chroma.KeyboardGrid.SetPlayingTime(TimeSpan.FromMilliseconds(player.Position_ms));
                //chroma.MousepadGrid.SetPeakVolume(chroma.PrimaryColor);
                //chroma.HeadsetGrid.SetPeakVolume(chroma.PrimaryColor);
                chroma.ApplyAsync().Wait();
            }
            catch (Exception ex)
            {
                chroma.SDKDisable();
            }
        }

        private void PlayProgress_Click(object sender, EventArgs e)
        {
            player.SetPositionAsync((int)PlayProgress.CalculateRelativeValue());
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void FacebookShare()
        {
            var app_id = "139971873511766"; //StatusReporter
            var href = player.Url;
            var redirect_uri = string.Empty;
            var hashtag = $"%23{player.Artist}_{player.Track}";
            hashtag = RegularExpressionHelpers.AlphabetCleaner(hashtag);
            var requestText =
                $"https://www.facebook.com/dialog/share?app_id={app_id}&text=test&display=page&href={href}&redirect_uri={redirect_uri}&hashtag={hashtag}";
            Process.Start(requestText);
        }

        private void MainWindowGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (!e.IsDown)
                {
                    return;
                }

                switch (e.Key)
                {
                    case Key.A:
                        BackPath_Click(null, null);
                        break;
                    case Key.D:
                        NextPath_Click(null, null);
                        break;
                    case Key.W:
                        player.SetVolume(player.Volume + 10);
                        break;
                    case Key.S:
                        player.SetVolume(player.Volume - 10);
                        break;
                    case Key.Q:
                        player.SetPosition(player.Position_ms - 15);
                        break;
                    case Key.E:
                        player.SetPosition(player.Position_ms + 15);
                        break;
                    case Key.Space:
                        PlayPath_Click(null, null);
                        break;
                    case Key.F:
                        FacebookShare();
                        break;
                    case Key.O:
                        Process.Start(player.Url);
                        break;
                    case Key.P:
                        GenerateFormImage();
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
            if (player.Position_ms > 3000)
                player.SetPositionAsync(0).ConfigureAwait(false);
            else
                player.Previous();
        }

        private void PlayPath_Click(object sender, RoutedEventArgs e)
        {
            player.PlayPause();
        }

        private void NextPath_Click(object sender, RoutedEventArgs e)
        {
            player.Next();
        }

        private void VolumePath_Click(object sender, RoutedEventArgs e)
        {
            if (player.Volume > 0)
            {
                player.Mute();
            }
            else
            {
                player.Unmute();
            }
        }

        private void VolumeProgress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var value = (int)VolumeProgress.CalculateRelativeValue();
            player.SetVolume(value);
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
            chromaTimer?.Dispose();
            //defaultAudioEndpointTimer?.Dispose();
            wallpaper?.Dispose();
            player?.Dispose();
            chroma?.Dispose();
            defaultAudioEndpoint?.Dispose();
            deviceEnumerator?.Dispose();
            base.OnClosing(e);
        }

        private void GenerateFormImage()
        {
            var fileName = "10." + RegularExpressionHelpers.AlphabetCleaner($"{player.Track}-{player.Album}-{player.Artist}") + ".jpg";
            string path;
            if (CacheFileManager.IsFileExists(fileName))
            {
                path = CacheFileManager.GetFullCachePath(fileName);
            }
            else
            {
                using var image = wallpaper.GetWallpaperImage(3840, 2160);
                path = CacheFileManager.SaveCache(fileName, image.ToByteArray(ImageFormat.Jpeg));
            }
            Process.Start(path);
        }

        private void Btn_Repeat_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Btn_Shuffle_Click(object sender, RoutedEventArgs e)
        {

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
            var ds = new DeviceSelection(player, this.Left + this.InitWidth, this.Top + this.InitHeight);
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
                searchPanel = new SearchPanel(player, () => searchPanel = null);
                searchPanel.Show();
            }
        }

        private void btn_lyrics_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($"https://www.google.com/search?q={this.player.Artist.Replace(" ", "+")}+{this.player.Album.Replace(" ", "+")}+{this.player.Track.Replace(" ", "+")}+lyrics");
            //if (lyricsDisplay != null)
            //{
            //    lyricsDisplay.BringToFront();
            //}
            //else
            //{        
            //if (!string.IsNullOrWhiteSpace(this.player.Lyrics))
            //{
            //    lyricsDisplay = new LyricsDisplay(player, this.Left + InitWidth, this.Top, () => lyricsDisplay = null);
            //    lyricsDisplay.Show();
            //}
            //else
            //{
            //Process.Start($"https://www.google.com/search?q={this.player.Artist.Replace(" ", "+")}+{this.player.Album.Replace(" ", "+")}+{this.player.Track.Replace(" ", "+")}+lyrics");
            //}
            //}
        }

        private void Btn_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            GenerateFormImage();
        }
    }
}