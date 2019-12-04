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
        Image _backgroundDesktopPlaying = null;
        ImageBrush _backgroundApp = null;
        public static MainWindow Context { get; private set; }
        public MainWindow()
        {
            try
            {
                InitializeComponent();

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
                Player.OnTrackDurationChanged += Player_OnTrackDurationChanged;
                Player.OnDeviceChanged += Player_OnDeviceChanged;
                using (var devices = new MMDeviceEnumerator())
                    ActiveDevice = devices.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).OrderByDescending(x => x.AudioMeterInformation.MasterPeakValue).FirstOrDefault();
                InitializeDiscord();
                //KeyDown += MainWindowGrid_PreviewKeyDown;
                Loaded += MainWindow_Loaded;
                MouseDown += Window_MouseDown;
                btn_Minimize.Click += (s, e) => this.WindowState = WindowState.Minimized;
                btn_Close.Click += (s, e) => this.Close();
                this.AlbumImage.MouseDown += AlbumImage_MouseDown;
                this.cb_SearchBox.TextChanged += async (s, e) =>
                {
                    var q = cb_SearchBox.Text;
                    string query = default;
                    SearchType searchType = SearchType.All;
                    if (q.Contains(":"))
                    {
                        var data = q.Split(':');
                        var qtype = data[0].ToLower();
                        if (qtype == "t" || qtype == "track")
                        {
                            searchType = SearchType.Track;
                        }
                        else if (qtype == "ab" || qtype == "album")
                        {
                            searchType = SearchType.Album;
                        }
                        else if (qtype == "a" || qtype == "artist")
                        {
                            searchType = SearchType.Artist;
                        }
                        else if (qtype == "p" || qtype == "playlist")
                        {
                            searchType = SearchType.Playlist;
                        }
                        query = data[1];
                    }
                    else
                    {
                        query = q;
                    }
                    if (string.IsNullOrWhiteSpace(q)) return;
                    var result = await Player.SearchAsync(query, searchType, 10);
                    if (result == null) return;
                    cb_SearchBox.SetInternalValue(result);



                };
                this.cb_SearchBox.SelectionChanged += async (s, e) =>
                {
                    if (cb_SearchBox.SelectedItem == null) return;
                    var text = cb_SearchBox.SelectedItem.ToString();
                    var result = cb_SearchBox.GetTrackUrl(text);
                    if (!string.IsNullOrWhiteSpace(result.uri))
                    {
                        switch (result.searchType)
                        {
                            case SearchType.Artist:
                                Process.Start(result.uri);
                                break;
                            case SearchType.Track:
                            case SearchType.All:
                                await Player.PlayTrackAsync(result.uri);
                                break;
                            default:
                                await Player.PlayAsync(result.uri);
                                break;
                        }

                    }
                };
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
        }

        private void Player_OnDeviceChanged(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(UpdateUI);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            animation = new AnimationController(this);

            MouseEnter += OnMouseEnterEvent;
            MouseLeave += OnMouseLeaveEvent;
            #region get current background image

            if (Wallpaper.TryGetWallpaper(out var filePath))
            {
                wallpaper = new Wallpaper(filePath);
            }
            else
            {
                wallpaper = new Wallpaper();
                System.Windows.Forms.Application.Exit();
            }
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
                    Task.Run(() => Process.Start(Player.URL));
                    break;
            }
        }

        private void Player_OnTrackDurationChanged(IMusic playbackContext)
        {
            try
            {
                using (var devices = new MMDeviceEnumerator())
                {
                    ActiveDevice = devices.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).OrderByDescending(x => x.AudioMeterInformation.MasterPeakValue).FirstOrDefault();
                    lbl_CurrentTime.Content = playbackContext.Position_ms.ToMinutes();
                    lbl_TimeLeft.Content = $"-{Extension.ToMinutes(playbackContext.Duration_ms - playbackContext.Position_ms)}";
                    PlayProgress.Value = playbackContext.Position_ms;
                    VolumeProgress.Value = playbackContext.Volume;
                    PlayProgress.Foreground = playbackContext.IsPlaying ? playColor : pauseColor;
                    VolumePath.Fill = playbackContext.IsMute ? pauseColor : playColor;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void OnTrackChanged(IMusic playbackContext)
        {
            _backgroundDesktopPlaying = null;
            _backgroundApp = null;
            try
            {
                if (Properties.Settings.Default.DiscordRichPresenceEnable)
                    UpdatePresence(playbackContext);
            }
            catch
            {

            }
            Dispatcher.InvokeAsync(UpdateUI);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }
        private void OnMouseLeaveEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            cb_SearchBox.Text = "";
            animation.TransitionDisable();
            Console.WriteLine("Mouse Leave");
        }
        private void OnMouseEnterEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            animation.TransitionEnable();
            Console.WriteLine("Mouse Enter");
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
            catch (Exception ex)
            {
                Chroma.SDKDisable();
            }
        }
        public static void UpdatePresence(IMusic music)
        {
            var presence = new DiscordRPC.RichPresence
            {
                largeImageKey = "spotify",
                //smallImageKey = "small"
            };//"itunes_logo_big" };
            presence.UpdateRPC(music);
        }
        private void UpdateUI()
        {
            try
            {
                if (_backgroundApp == null && _backgroundDesktopPlaying == null && Player.AlbumArtwork != null)
                {
                    Image applyOpacity(Image i0) => ImageProcessing.SetOpacity(i0, 0.6f, System.Drawing.Color.Black);
                    using (var clonnedAWImage = (Image)Player.AlbumArtwork.Clone())
                    {
                        var backgroundImage = applyOpacity(clonnedAWImage).Blur(Properties.Settings.Default.BlurRadial, this.Height / this.Width);
                        _backgroundApp = new ImageBrush(backgroundImage);
                        using (var secondImage = backgroundImage.ToImage())
                        {
                            //_backgroundApp.Opacity = 0.5;
                            var highlightSize = (int)Math.Round(SystemParameters.PrimaryScreenHeight * 0.555);
                            wallpaper.CalculateBackgroundImage(
                                Player.AlbumArtwork.Resize(highlightSize, highlightSize),
                                secondImage,
                                this.FontFamily.ToString(),
                                20.0f,
                                Player.Track,
                                Player.Album,
                                Player.Artist);
                            Task.Run(wallpaper.Enable);
                            _backgroundDesktopPlaying = wallpaper.TrackImage;
                            _backgroundApp.Freeze();
                        }
                    }
                    var albumImage = (Player.AlbumArtwork as Bitmap).ToBitmapImage();
                    AlbumImage.Source = albumImage;
                    //widget.WidgetImage.Source = albumImage;
                    //widget.WidgetBackgroundColor.Color = new System.Windows.Media.Color()
                    //{
                    //    R = Player.Album_StandardColor.Standard.R,
                    //    G = Player.Album_StandardColor.Standard.G,
                    //    B = Player.Album_StandardColor.Standard.B,
                    //    A = Player.Album_StandardColor.Standard.A
                    //};
                    this.Icon = AlbumImage.Source;
                    this.border_Form.Background = _backgroundApp;
                }
            }
            catch
            {

            }
            this.Title = $"Listening to {Player.Track} by {Player.Artist} on {Player.ActiveDevice.Name}";

            lbl_Track.Content = Player.Track;
            lbl_Album.Content = Player.Album;
            lbl_Artist.Content = Player.Artist;
            PlayProgress.Maximum = Player.Duration_ms;
        }
        private static void InitializeDiscord()
        {
#if WIN64
            try
            {
                var handlers = DiscordRPC.InitializeEventHandler();
                //383816327850360843 , iTunes RPC
                //418435305574760458 , my custom RPC
                DiscordRPC.Initialize("418435305574760458", ref handlers, true, null);
            }
            catch
            {
                Properties.Settings.Default.DiscordRichPresenceEnable = false;
                Properties.Settings.Default.Save();
            }
#else
            Properties.Settings.Default.DiscordRichPresenceEnable = false;
            Properties.Settings.Default.Save();
#endif
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
                    case Key.R:
                        Process.Start("https://github.com/Desz01ate/iTunesListenerX");
                        break;
                    case Key.O:
                        Process.Start(Player.URL);
                        break;
                    case Key.P:
                        GenerateFormImage();
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

        private void ChangeDevice_Click(object sender, MouseButtonEventArgs e)
        {
            var ds = new DeviceSelection(Player, this.Left, this.Top);
            ds.ShowDialog();
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
            ChromaTimer?.Dispose();
            _backgroundDesktopPlaying?.Dispose();
            wallpaper?.Dispose();
            Player?.Dispose();
            //widget?.Close();
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

        private void Btn_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            GenerateFormImage();
        }
    }
}
