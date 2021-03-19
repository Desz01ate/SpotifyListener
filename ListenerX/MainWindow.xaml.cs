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
using ListenerX.Cscore;
using ListenerX.Extensions;
using Listener.Core.Framework.Plugins;
using System.Drawing;
using System.Web;

namespace ListenerX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Timer chromaTimer = new Timer();
        private AnimationController animation;

        private readonly ChromaWorker chroma;

        private readonly SolidColorBrush playColor =
            (SolidColorBrush)(new BrushConverter().ConvertFromString("#5aFF5a"));

        private readonly SolidColorBrush pauseColor =
            (SolidColorBrush)(new BrushConverter().ConvertFromString("#FF5a5a"));

        private Wallpaper wallpaper;
        private SearchPanel searchPanel;

        private readonly IStreamablePlayerHost player;

        private readonly double InitWidth, InitHeight;

        private readonly IListenerPlugin[] plugins;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                this.Background = System.Windows.Media.Brushes.Gray;

                InitWidth = this.Width;
                InitHeight = this.Height;
                ResizeMode = ResizeMode.CanMinimize;
                Visibility = Visibility.Hidden;

                //player = ActivatorHelpers.LoadPlayerHost<Listener.Player.Spotify.SpotifyPlayerHost>();
                player = new Listener.Player.Spotify.SpotifyPlayerHost();
                plugins = ActivatorHelpers.LoadPlugins().ToArray();

                var maxEffectCount = ActivatorHelpers.Effects.Count - 1;
                if (Properties.Settings.Default.RenderStyle > maxEffectCount)
                {
                    Properties.Settings.Default.RenderStyle = maxEffectCount;
                    Properties.Settings.Default.Save();
                }

                VolumePath.Fill = playColor;
                VolumeProgress.Foreground = lbl_Album.Foreground;

                player.TrackChanged += OnTrackChanged;
                player.DeviceChanged += Player_OnDeviceChanged;
                player.TrackDurationChanged += (p) =>
                {
                    this.VolumePath.Fill = p.IsMute ? pauseColor : playColor;
                };

                player.TrackPlayStateChanged += (state) =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        StreamGeometry buttonShape;
                        SolidColorBrush color;
                        if (state == PlayState.Play)
                        {
                            buttonShape = (StreamGeometry)this.FindResource("pausePath");
                            color = playColor;
                        }
                        else
                        {
                            buttonShape = (StreamGeometry)this.FindResource("playPath");
                            color = pauseColor;
                        }
                        this.PlayPath.Data = buttonShape;
                        this.PlayProgress.Foreground = color;
                    });
                };

                KeyDown += MainWindowGrid_PreviewKeyDown;
                Loaded += MainWindow_Loaded;
                MouseDown += Window_MouseDown;
                btn_Minimize.Click += (s, e) => this.WindowState = WindowState.Minimized;
                btn_Close.Click += (s, e) => this.Close();
                this.AlbumImage.MouseDown += AlbumImage_MouseDown;

                if (Properties.Settings.Default.ChromaSDKEnable)
                {
                    var chroma = ChromaWorker.Instance;
                    this.chroma = chroma;
                    chromaTimer.Interval =
                        (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);
                    chromaTimer.Tick += ChromaTimer_Tick;
                    chromaTimer.Start();
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

            wallpaper = new Wallpaper(this.player, this.FontFamily.ToString());
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
                    Task.Run(() => OpenerHelpers.Open(player.Url));
                    break;
            }
        }

        private void OnTrackChanged(IPlayerHost playbackContext)
        {
            Dispatcher.InvokeAsync(() =>
            {
                foreach (var plugin in this.plugins)
                {
                    plugin.OnTrackChanged(playbackContext);
                }

                this.Title = $"Listening to {player.Track} by {player.Artist} on {player.ActiveDevice.Name}";

                var albumImageSource = playbackContext.AlbumArtwork.ToBitmapImage(ImageFormat.Jpeg);
                using var background = ImageProcessing.CalculateBackgroundSource(playbackContext.AlbumArtwork, (int)this.InitWidth, (int)this.InitHeight, 10);
                var albumBackgroundSource = ImageProcessing.ToSafeMemoryBrush(background as Bitmap);

                this.Icon = albumImageSource;
                this.AlbumImage.Source = albumImageSource;
                this.Background = albumBackgroundSource;

                this.chroma?.LoadColor(this.player.AlbumArtwork);


                if (Properties.Settings.Default.ArtworkWallpaperEnable)
                {
                    wallpaper.Enable();
                }
                else
                {
                    wallpaper.Disable();
                }
                GC.Collect();
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
                var effect = ActivatorHelpers.Effects[Properties.Settings.Default.RenderStyle];
                double[] spectrumData = OutputDevice.ActiveDevice.GetSpectrums(effect.RequiredSpectrumRange).Select(x => Math.Min(x * Properties.Settings.Default.Amplitude, 100)).ToArray();
                chroma.SetEffect(effect, spectrumData, this.player.CalculatedPosition);
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
                        OpenerHelpers.Open(player.Url);
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
            OutputDevice.ActiveDevice?.Dispose();
            base.OnClosing(e);
        }

        private void GenerateFormImage()
        {
            //var fileName = "10." + RegularExpressionHelpers.AlphabetCleaner($"{player.Track}-{player.Album}-{player.Artist}") + ".jpg";
            //string path;
            //if (CacheFileManager.IsFileExists(fileName))
            //{
            //    path = CacheFileManager.GetFullCachePath(fileName);
            //}
            //else
            //{
            //    using var image = wallpaper.GetWallpaperImage(3840, 2160);
            //    path = CacheFileManager.SaveCache(fileName, image.ToByteArray(ImageFormat.Jpeg));
            //    //path = wallpaper.GetWallpaperImage();
            //}
            var path = wallpaper.GetWallpaperImage();
            OpenerHelpers.Open(path);
        }

        private void Btn_Repeat_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Btn_Shuffle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AdjustSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var setting = new Settings();
                setting.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
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
            var query = $"{this.player.Artist} {this.player.Album} {this.player.Track} lyrics";
            var encodedQuery = HttpUtility.UrlEncode(query);
            var url = $"https://www.google.com/search?q={encodedQuery}";
            OpenerHelpers.Open(url);
        }

        private void Btn_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            GenerateFormImage();
        }
    }
}