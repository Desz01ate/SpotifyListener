using SpotifyListener.ChromaExtension;
using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Dynamic;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Interop;

namespace SpotifyListener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer TrackDetailTimer = new Timer();
        Timer ChromaTimer = new Timer();
        Timer WebServiceCommandTimer = new Timer();
        public Music Player { get; private set; }
        //private static HttpClient client = new HttpClient();
        private static MMDevice ActiveDevice;
        private static ChromaWrapper Chroma = ChromaWrapper.GetInstance;
        private System.Drawing.Color borderColor = System.Drawing.Color.White;
        //private MessagingAPI messagingAPI;
        private SolidColorBrush playColor = (SolidColorBrush)(new BrushConverter().ConvertFromString("#5aFF5a"));
        private SolidColorBrush pauseColor = (SolidColorBrush)(new BrushConverter().ConvertFromString("#FF5a5a"));
        private byte previousVolume = 0;
        private System.Windows.Media.Brush RedBrush = (new BrushConverter().ConvertFromString("#FF0000")) as System.Windows.Media.Brush;
        DoubleAnimation fadeAnimation = new DoubleAnimation() { From = 0, To = 1, Duration = TimeSpan.FromSeconds(1) };
        DoubleAnimation slideLeft = new DoubleAnimation() { To = -150, Duration = TimeSpan.FromMilliseconds(500) };
        DoubleAnimation slideAnimation_enter;
        DoubleAnimation slideAnimation_leave;
        DoubleAnimation fadeInAnimation;
        DoubleAnimation fadeOutAnimation;
        DoubleAnimation moveX_enter;
        DoubleAnimation moveY_enter;
        DoubleAnimation moveX_leave;
        DoubleAnimation moveY_leave;
        Wallpaper wallpaper = null;
        Image _backgroundImage = null;
        Image _backgroundDesktopPlaying = null;
        ImageBrush _backgroundApp = null;
        Widget widget = null;
        string _backgroundImagePath = string.Empty;


        private List<System.Windows.Controls.Button> setDeviceButtons = new List<System.Windows.Controls.Button>();

        public static MainWindow Context { get; private set; }
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                widget = new Widget(this);
                Player = new Music(Properties.Settings.Default.AccessToken, Properties.Settings.Default.RefreshToken);

                ResizeMode = ResizeMode.CanMinimize;
                Visibility = Visibility.Hidden;
                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.RefreshToken))
                {
                    System.Windows.MessageBox.Show("You must set refresh token, otherwise this application can't fetch data from Spotify server.", "SpotifyListener", MessageBoxButton.OK, MessageBoxImage.Warning);
                    new Settings().ShowDialog();
                }
                //messagingAPI = new MessagingAPI("https://7d2d9000.ap.ngrok.io");
                VolumePath.Fill = playColor;
                VolumeProgress.Foreground = lbl_Album.Foreground;
                Player.OnTrackChanged += delegate
                {
                    _backgroundDesktopPlaying = null;
                    _backgroundApp = null;
                    Dispatcher.InvokeAsync(UpdateUI);

                };
                Player.OnDeviceChanged += delegate
                {
                    Dispatcher.InvokeAsync(UpdateUI);
                };
                //player.OnResume += (s, e) =>
                //{
                //    Dispatcher.InvokeAsync(UpdateUI);
                //};
                //player.OnPaused += (s, e) =>
                //{
                //    Dispatcher.InvokeAsync(UpdateUI);
                //};

                ActiveDevice = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).OrderByDescending(x => x.AudioMeterInformation.MasterPeakValue).FirstOrDefault();
                InitializeDiscord(); //discord RPC api only work with x64 system
                                     //
                TrackDetailTimer.Interval = 1000;
                TrackDetailTimer.Tick += TrackDetailTimer_Tick;
                TrackDetailTimer.Start();

                if (!Chroma.IsError)
                {
                    ChromaTimer.Interval = (int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0);//TimeSpan.FromMilliseconds((int)Math.Round((1000.0 / Properties.Settings.Default.RenderFPS), 0));
                    ChromaTimer.Tick += ChromaTimer_Tick;
                    ChromaTimer.Start();
                }


                //WebServiceCommandTimer.Start();
                //
                //

                Closing += (s, e) =>
                {
                    var memoizedResult = JsonConvert.SerializeObject(HTMLHelper.UrlMemoized.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value), Formatting.Indented);
                    if (!File.Exists("url.json"))
                    {
                        var file = File.Create("url.json");
                        file.Close();
                    }
                    File.WriteAllText("url.json", memoizedResult);
                };
                KeyDown += MainWindowGrid_PreviewKeyDown;
                Loaded += (s, e) =>
                {
                    AlbumImage.BringToFront();
                    AlbumImage.MouseDown += (_s, _e) =>
                    {

                        switch (_e.ChangedButton)
                        {
                            case MouseButton.Left:
                                Player.PlayPause();
                                break;
                            case MouseButton.Right:
                                Task.Run(async () =>
                                {
                                    try
                                    {
                                        Process.Start(Player.URL);
                                    }
                                    catch
                                    {
                                        System.Windows.MessageBox.Show("Unable to fetch URL, please make sure the internet is connected.", "iTunesListenerX", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }
                                });
                                break;

                        }
                    };
                    var baseWidth = AlbumImage.Width;
                    var baseHeight = AlbumImage.Height;
                    AlbumImage.MouseEnter += (_s, _e) =>
                    {
                        AlbumImage.Width = baseWidth * 1.2;
                        AlbumImage.Height = baseHeight * 1.2;
                        //AlbumImage.RenderSize = hoverSize;
                    };
                    AlbumImage.MouseLeave += (_s, _e) =>
                    {
                        AlbumImage.Width = baseWidth;
                        AlbumImage.Height = baseHeight;
                    };
                    slideAnimation_enter = new DoubleAnimation()
                    {
                        From = 0,
                        To = -200,
                        Duration = TimeSpan.FromMilliseconds(500),
                        //                  AutoReverse = true
                    };
                    slideAnimation_leave = new DoubleAnimation()
                    {
                        From = -200,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(500),
                        //                AutoReverse = true
                    };
                    fadeInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromMilliseconds(500),
                    };
                    fadeOutAnimation = new DoubleAnimation()
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(200)
                    };
                    moveX_enter = new DoubleAnimation()
                    {
                        From = 0,
                        To = 150,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    moveY_enter = new DoubleAnimation()
                    {
                        From = 0,
                        To = -220,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    moveX_leave = new DoubleAnimation()
                    {
                        From = 150,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    moveY_leave = new DoubleAnimation()
                    {
                        From = -220,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    MouseEnter += OnMouseEnterEvent;
                    MouseLeave += OnMouseLeaveEvent;
                };
                StateChanged += OnFormSateChanged;
                #region get current background image
                byte[] SliceMe(byte[] source, int pos)
                {
                    byte[] dest = new byte[source.Length - pos];
                    Array.Copy(source, pos, dest, 0, dest.Length);
                    return dest;
                };
                byte[] path = (byte[])Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop").GetValue("TranscodedImageCache");
                var wallpaper_file_path = System.Text.Encoding.Unicode.GetString(SliceMe(path, 24)).TrimEnd("\0".ToCharArray());
                if (File.Exists(wallpaper_file_path))
                {
                    _backgroundImagePath = wallpaper_file_path;
                    _backgroundImage = Image.FromFile(wallpaper_file_path);
                    wallpaper = new Wallpaper(_backgroundImagePath);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show($@"{wallpaper_file_path} does not exists.");
                    System.Windows.Forms.Application.Exit();
                    wallpaper = new Wallpaper();
                }
                #endregion
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
            this.Visibility = Visibility.Visible;
            Context = this;
        }

        private void OnFormSateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (Properties.Settings.Default.DisableTrackBackgroundOnMinimized)
                {
                    Task.Run(() => wallpaper.Disable());
                    widget.Show();
                }
            }
            else if (WindowState == WindowState.Normal)
            {
                Task.Run(() => wallpaper.Enable());
                widget.Hide();
            }
        }
        private void OnMouseLeaveEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {

            //Width = 800;
            TranslateTransform t1 = new TranslateTransform(), t2 = new TranslateTransform();
            AlbumImage.RenderTransform = t1;
            lbl_Track.RenderTransform = t2;
            //AlbumImage.BeginAnimation(OpacityProperty, fadeInAnimation);
            t1.BeginAnimation(TranslateTransform.XProperty, slideAnimation_leave);
            t2.BeginAnimation(TranslateTransform.XProperty, moveX_leave);
            t2.BeginAnimation(TranslateTransform.YProperty, moveY_leave);
            lbl_Track.BeginAnimation(OpacityProperty, fadeInAnimation);
            BackPath.BeginAnimation(OpacityProperty, fadeOutAnimation);
            PlayPath.BeginAnimation(OpacityProperty, fadeOutAnimation);
            NextPath.BeginAnimation(OpacityProperty, fadeOutAnimation);
            VolumePath.BeginAnimation(OpacityProperty, fadeOutAnimation);
            VolumeProgress.BeginAnimation(OpacityProperty, fadeOutAnimation);

            lbl_settings.BeginAnimation(OpacityProperty, fadeOutAnimation);
            lbl_change_device.BeginAnimation(OpacityProperty, fadeOutAnimation);
            lbl_CurrentTime.BeginAnimation(OpacityProperty, fadeOutAnimation);
            lbl_TimeLeft.BeginAnimation(OpacityProperty, fadeOutAnimation);
            PlayProgress.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void OnMouseEnterEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            #region exceptional visibility case              
            lbl_settings.Visibility = Visibility.Visible;
            lbl_change_device.Visibility = Visibility.Visible;
            lbl_CurrentTime.Visibility = Visibility.Visible;
            lbl_TimeLeft.Visibility = Visibility.Visible;
            PlayProgress.Visibility = Visibility.Visible;
            #endregion
            //Width = 950;
            TranslateTransform t1 = new TranslateTransform(), t2 = new TranslateTransform();
            AlbumImage.RenderTransform = t1;
            lbl_Track.RenderTransform = t2;
            //AlbumImage.BeginAnimation(OpacityProperty, fadeInAnimation);
            t1.BeginAnimation(TranslateTransform.XProperty, slideAnimation_enter);
            t2.BeginAnimation(TranslateTransform.XProperty, moveX_enter);
            t2.BeginAnimation(TranslateTransform.YProperty, moveY_enter);
            lbl_Track.BeginAnimation(OpacityProperty, fadeInAnimation);
            BackPath.BeginAnimation(OpacityProperty, fadeInAnimation);
            PlayPath.BeginAnimation(OpacityProperty, fadeInAnimation);
            NextPath.BeginAnimation(OpacityProperty, fadeInAnimation);
            VolumePath.BeginAnimation(OpacityProperty, fadeInAnimation);
            VolumeProgress.BeginAnimation(OpacityProperty, fadeInAnimation);

            lbl_settings.BeginAnimation(OpacityProperty, fadeInAnimation);
            lbl_change_device.BeginAnimation(OpacityProperty, fadeInAnimation);
            lbl_CurrentTime.BeginAnimation(OpacityProperty, fadeInAnimation);
            lbl_TimeLeft.BeginAnimation(OpacityProperty, fadeInAnimation);
            PlayProgress.BeginAnimation(OpacityProperty, fadeInAnimation);
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


        private void TrackDetailTimer_Tick(object sender, EventArgs e)
        {

            try
            {
                ActiveDevice = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).OrderByDescending(x => x.AudioMeterInformation.MasterPeakValue).FirstOrDefault();
                Task.Run(async delegate
                {
                    await Player.GetAsync();
                    //try
                    //{

                    //}
                    //catch (HttpRequestException http_ex)
                    //{
                    //    TrackDetailTimer.Stop();
                    //    //System.Windows.Forms.MessageBox.Show("Please connect to the internet, otherwise this application will not work.", "Internet connection required", MessageBoxButtons.OK);
                    //    System.Windows.Application.Current.Shutdown();

                    //}

                });

                if (Properties.Settings.Default.DiscordRichPresenceEnable)
                    UpdatePresenceAsync();
                lbl_CurrentTime.Content = Player.Position_ms.ToMinutes();
                lbl_TimeLeft.Content = $"-{Extension.ToMinutes(Player.Duration_ms - Player.Position_ms)}";
                PlayProgress.Value = Player.Position_ms;
                VolumeProgress.Value = Player.Volume;
                PlayProgress.Foreground = Player.IsPlaying ? playColor : pauseColor;
            }
            catch
            {

            }
        }

        private async Task UpdatePresenceAsync()
        {
            try
            {
                var presence = new DiscordRPC.RichPresence
                {
                    largeImageKey = "spotify",
                    //smallImageKey = "small"
                };//"itunes_logo_big" };
                if (!Player.IsPlaying)
                {
                    presence.details = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPauseDetail, Player));
                    presence.state = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPauseState, Player));
                }
                else
                {
                    presence.details = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPlayDetail, Player));
                    presence.state = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPlayState, Player));
                    presence.startTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() - Player.Position_ms;
                    presence.endTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + ((Player.Duration_ms - Player.Position_ms) / 1000);
                }
                presence.largeImageText = Player.URL;
                DiscordRPC.UpdatePresence(presence);
            }
            catch
            {

            }
        }

        private void UpdateUI()
        {
            try
            {
                if (_backgroundApp == null && _backgroundDesktopPlaying == null)
                {
                    var backgroundImage = Player.AlbumArtwork.Blur(Properties.Settings.Default.BlurRadial, this.Height / this.Width);
                    _backgroundApp = new ImageBrush(backgroundImage);
                    _backgroundApp.Opacity = 0.6;
                    var highlightSize = (int)Math.Round(SystemParameters.PrimaryScreenHeight * 0.555);
                    Image applyOpacity(Image i0) => ImageProcessing.SetOpacity(i0, 0.6f, System.Drawing.Color.Black);
                    wallpaper.CalculateBackgroundImage(
                        Player.AlbumArtwork.Resize(highlightSize, highlightSize),
                        backgroundImage.ToImage(),
                        SystemParameters.PrimaryScreenWidth,
                        SystemParameters.PrimaryScreenHeight,
                        this.FontFamily.ToString(),
                        20.0f,
                        Player.Track,
                        Player.Album,
                        Player.Artist,
                        applyOpacity);
                    _backgroundDesktopPlaying = wallpaper.TrackImage;
                    //_backgroundDesktopPause = Wallpaper.GetBacgroundImageForTrack(
                    //    player.AlbumArtwork,
                    //    backgroundImage.ToImage(),
                    //    System.Windows.SystemParameters.PrimaryScreenWidth,
                    //    System.Windows.SystemParameters.PrimaryScreenHeight,
                    //    this.FontFamily.ToString(),
                    //    20.0f,
                    //    player.Track,
                    //    player.Album,
                    //    player.Artist,
                    //    applyOpc,
                    //    0.9f);
                }
                var albumImage = (Player.AlbumArtwork as Bitmap).ToBitmapImage();
                AlbumImage.Source = albumImage;
                widget.WidgetImage.Source = albumImage;
                widget.WidgetBackgroundColor.Color = new System.Windows.Media.Color()
                {
                    R = Player.Album_StandardColor.Standard.R,
                    G = Player.Album_StandardColor.Standard.G,
                    B = Player.Album_StandardColor.Standard.B,
                    A = Player.Album_StandardColor.Standard.A
                };
                this.Icon = AlbumImage.Source;
                this.Background = _backgroundApp;//player.IsPlaying ? _backgroundAppPlaying : _backgroundAppPause;
                                                 //Background.Opacity = 0.6;
                #region set desktop background
                //don't need to run on UI thread, it has nothing to do with the UI!
                OnFormSateChanged(null, null);
                //Task.Run(() =>
                //{
                //    //Wallpaper.Set(player.IsPlaying ? _backgroundDesktopPlaying : _backgroundDesktopPause, Wallpaper.Style.Centered);
                //    //wallpaper.Set(_backgroundDesktopPlaying, Wallpaper.Style.Centered);
                //    wallpaper.Enable();
                //});
                #endregion
            }
            catch
            {

            }
            this.Title = $"Listening to {Player.Track} by {Player.Artist} on {Player.ActiveDevice.Name}";
            //var fontColor = System.Windows.Media.Brushes.WhiteSmoke;//(System.Windows.Media.Brush)(new BrushConverter().ConvertFromString(player.Album_StandardColor.Standard.ContrastColor().ToHex()));

            //BackPath.Fill = fontColor;
            //PlayPath.Fill = fontColor;
            //NextPath.Fill = fontColor;
            //VolumeProgress.Foreground = fontColor;
            widget.WidgetTrack.Content = Player.Track;
            widget.WidgetAlbum.Content = Player.Album;
            widget.WidgetArtist.Content = Player.Artist;
            lbl_Track.Content = Player.Track;
            lbl_Album.Content = Player.Album;
            lbl_Artist.Content = Player.Artist;
            PlayProgress.Maximum = Player.Duration_ms;
            //lbl_Track.Foreground = fontColor;
            //lbl_Album.Foreground = fontColor;
            //lbl_Artist.Foreground = fontColor;
            //lbl_CurrentTime.Foreground = fontColor;
            //lbl_TimeLeft.Foreground = fontColor;
            //lbl_change_device.Foreground = fontColor;
            //lbl_settings.Foreground = fontColor;
            //lbl_Track.Background = fontBackColor;
            //lbl_Album.Background = fontBackColor;
            //lbl_Artist.Background = fontBackColor;
            //lbl_CurrentTime.Background = fontBackColor;
            //lbl_TimeLeft.Background = fontBackColor;
            //lbl_change_device.Background = fontBackColor;
            GC.Collect();
        }

        private static void HandleReadyCallback() { }
        private static void HandleErrorCallback(int errorCode, string message) { }
        private static void HandleDisconnectedCallback(int errorCode, string message) { }
        private static void InitializeDiscord()
        {
#if WIN64
            DiscordRPC.EventHandlers handlers = new DiscordRPC.EventHandlers
            {
                readyCallback = HandleReadyCallback,
                errorCallback = HandleErrorCallback,
                disconnectedCallback = HandleDisconnectedCallback
            };
            //383816327850360843 , iTunes RPC
            //418435305574760458 , my custom RPC
            DiscordRPC.Initialize("418435305574760458", ref handlers, true, null);
#else
            Properties.Settings.Default.DiscordRichPresenceEnable = false;
            Properties.Settings.Default.Save();
#endif
        }
        private void PlayProgress_Click(object sender, EventArgs e)
        {
            Player.SetPositionAsync((int)PlayProgress.CalculateRelativeValue());
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void FacebookShare()
        {
            var app_id = "139971873511766"; //StatusReporter
            var href = Player.URL;
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
                        Player.Volume += 10;
                        break;
                    case Key.S:
                        Player.Volume -= 10;
                        break;
                    case Key.Q:
                        Player.Position_ms -= 15;
                        break;
                    case Key.E:
                        Player.Position_ms += 15;
                        break;
                    case Key.Space:
                        PlayPath_Click(null, null);
                        break;
                    case Key.F:
                        FacebookShare();
                        break;
                    case Key.R:
                        Task.Run(() =>
                        {
                            try
                            {
                                Process.Start("https://github.com/Desz01ate/iTunesListenerX");
                            }
                            catch
                            {
                                System.Windows.MessageBox.Show("Unable to fetch URL, please make sure the internet is connected.", "iTunesListenerX", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                        break;
                    case Key.O:
                        Task.Run(() =>
                        {
                            try
                            {
                                Process.Start(Player.URL);
                            }
                            catch
                            {
                                System.Windows.MessageBox.Show("Unable to fetch URL, please make sure the internet is connected.", "iTunesListenerX", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                        break;
                }
            }
            catch
            {
                //pass
            }
        }

        private void BackPath_Click(object sender, RoutedEventArgs e)
        {
            if (Player.Position_ms > 10)
                Player.Position_ms = 0;
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
                previousVolume = (byte)Player.Volume;
                Player.Volume = 0;
            }
            else
            {
                Player.Volume = previousVolume;
            }
        }

        private void VolumeProgress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var value = (int)VolumeProgress.CalculateRelativeValue();
            Player.Volume = value;
        }

        private void ChangeDevice_Click(object sender, MouseButtonEventArgs e)
        {
            var ds = new DeviceSelection(Player, this.Left, this.Top);
            ds.ShowDialog();
        }

        private void Settings_Click(object sender, MouseButtonEventArgs e)
        {
            new Settings().ShowDialog();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            //keep this running on main thread, otherwise it will terminated before the task is done.
            //Wallpaper.Set(_backgroundImage, Wallpaper.Style.Stretched, _backgroundImagePath);
            wallpaper.Disable();
            widget?.Close();
            base.OnClosing(e);
        }
    }
}
