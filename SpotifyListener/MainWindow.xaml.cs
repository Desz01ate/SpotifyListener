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
using LineAPI;
using Component;
using System.Dynamic;

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
        private static string endpoint = "https://ituneslistenerapi20180912032938.azurewebsites.net/";
        private static HttpClient traditionClient = new HttpClient();
        private static Music player;
        //private static HttpClient client = new HttpClient();
        private static MMDevice ActiveDevice;
        private static ChromaWrapper Chroma = ChromaWrapper.GetInstance;
        private System.Drawing.Color borderColor = System.Drawing.Color.White;
        private MessagingAPI messagingAPI;
        private SolidColorBrush playColor = (SolidColorBrush)(new BrushConverter().ConvertFromString("#5aFF5a"));
        private SolidColorBrush pauseColor = (SolidColorBrush)(new BrushConverter().ConvertFromString("#FF5a5a"));
        private byte previousVolume = 0;
        private System.Windows.Media.Brush RedBrush = (new BrushConverter().ConvertFromString("#FF0000")) as System.Windows.Media.Brush;
        DoubleAnimation fadeAnimation = new DoubleAnimation() { From = 0, To = 1, Duration = TimeSpan.FromSeconds(1) };
        DoubleAnimation slideLeft = new DoubleAnimation() { To = -150, Duration = TimeSpan.FromMilliseconds(500) };

        public MainWindow()
        {
            try
            {
                ResizeMode = ResizeMode.CanMinimize;
                if (!Directory.Exists(Extension.ImageDirectory))
                    Directory.CreateDirectory(Extension.ImageDirectory);
                if (File.Exists("url.json"))
                    HTMLHelper.UrlMemoized = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("url.json"));
                InitializeComponent();

                player = new Music(Properties.Settings.Default.AccessToken, Properties.Settings.Default.RefreshToken);
                messagingAPI = new MessagingAPI("https://7d2d9000.ap.ngrok.io");
                VolumePath.Fill = playColor;
                VolumeProgress.Foreground = lbl_Album.Foreground;
                player.OnTrackChanged += delegate
                {
                    Dispatcher.BeginInvoke(new MethodInvoker(UpdateUI));
                    dynamic body = new ExpandoObject();
                    body.Title = player.Track;
                    body.Album = player.Album;
                    body.Artist = player.Artist;
                    body.URL = player.URL;
                    //body.LyricsURL = "https://www.google.com";
                    body.ImageURL = player.ArtworkURL;

                    messagingAPI.Execute("/api/Track", body);
                };
                ActiveDevice = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active).OrderByDescending(x => x.AudioMeterInformation.MasterPeakValue).FirstOrDefault();
                InitializeDiscord(); //discord RPC api only work with x64 system
                                     //
                TrackDetailTimer.Interval = 500;
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
                KeyDown += async (s, e) =>
                {
                    switch (e.Key)
                    {
                        case Key.A:
                            BackPath_Click(null, null);
                            //player.PlayerEngine.PreviousTrack();
                            break;
                        case Key.D:
                            NextPath_Click(null, null);
                            //player.PlayerEngine.NextTrack();
                            break;
                        case Key.W:
                            player.Volume += 10;
                            break;
                        case Key.S:
                            player.Volume -= 10;
                            break;
                        case Key.Q:
                            player.Position_ms -= 15;
                            break;
                        case Key.E:
                            player.Position_ms += 15;
                            break;
                        case Key.Space:
                            PlayPath_Click(null, null);
                            //player.PlayerEngine.PlayPause();
                            break;
                        /*
                    case ConsoleKey.H:
                        MainEvent.Reset();
                        ShowHelp();
                        break;
                        */
                        case Key.R:
                            Process.Start("https://github.com/Desz01ate/iTunesListenerX");
                            break;
                        case Key.O:
                            Process.Start(player.URL);
                            break;

                    }
                };
                Loaded += (s, e) =>
                {
                    AlbumImage.BringToFront();
                    AlbumImage.MouseDown += (_s, _e) =>
                    {

                        switch (_e.ChangedButton)
                        {
                            case MouseButton.Left:
                                player.PlayPause();
                                break;
                            case MouseButton.Right:
                                Task.Run(async () =>
                                {
                                    try
                                    {
                                        Process.Start(player.URL);
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
                    var slideAnimation_enter = new DoubleAnimation()
                    {
                        From = 0,
                        To = -200,
                        Duration = TimeSpan.FromMilliseconds(500),
                        //                  AutoReverse = true
                    };
                    var slideAnimation_leave = new DoubleAnimation()
                    {
                        From = -200,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(500),
                        //                AutoReverse = true
                    };
                    var fadeInAnimation = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromMilliseconds(500),
                    };
                    var fadeOutAnimation = new DoubleAnimation()
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(200)
                    };
                    var moveX_enter = new DoubleAnimation()
                    {
                        From = 0,
                        To = 150,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    var moveY_enter = new DoubleAnimation()
                    {
                        From = 0,
                        To = -220,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    var moveX_leave = new DoubleAnimation()
                    {
                        From = 150,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    var moveY_leave = new DoubleAnimation()
                    {
                        From = -220,
                        To = 0,
                        Duration = TimeSpan.FromMilliseconds(0)
                    };
                    MouseEnter += (_s, _e) =>
                    {
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
                    };
                    MouseLeave += (_s, _e) =>
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
                    };
                };
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
        private async void WebServiceTimer_TickAsync(object sender, EventArgs e)
        {
            try
            {
                var response = await traditionClient.GetAsync($@"{endpoint}/api/command");//client.Execute(getCommandRequest);
                var result = await response.Content.ReadAsStringAsync();
                if (result.Contains("NEXT"))
                {
                    NextPath_Click(null, null);
                    //player.PlayerEngine.NextTrack();
                    return;
                }
                else if (result.Contains("PREVIOUS"))
                {
                    BackPath_Click(null, null);
                    //player.PlayerEngine.PreviousTrack();
                    return;
                }
                else if (result.Contains("PLAY_PAUSE"))
                {
                    PlayPath_Click(null, null);
                    //player.PlayerEngine.PlayPause();
                    return;
                }
            }
            catch
            {

            }
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
                var density = Properties.Settings.Default.AdaptiveDensity && player.IsPlaying ? ActiveDevice.AudioMeterInformation.MasterPeakValue : (Properties.Settings.Default.Density / 10.0);
                Chroma.LoadColor(player, density);
                Chroma.SetDevicesBackground();
                if (Properties.Settings.Default.RenderPeakVolumeEnable)
                {
                    if (Properties.Settings.Default.SymmetricRenderEnable)
                    {
                        Chroma.MouseGrid.SetPeakVolumeSymmetric(Chroma.VolumeColor, ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        Chroma.KeyboardGrid.SetPeakVolumeSymmetric(Chroma.VolumeColor, ActiveDevice.AudioMeterInformation.MasterPeakValue);
                    }
                    else if (Properties.Settings.Default.PeakChroma)
                    {
                        Chroma.MouseGrid.SetChromaPeakVolume(ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        Chroma.KeyboardGrid.SetChromaPeakVolume(ActiveDevice.AudioMeterInformation.MasterPeakValue);
                    }
                    else
                    {
                        Chroma.MouseGrid.SetPeakVolume(Chroma.VolumeColor, ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        Chroma.KeyboardGrid.SetPeakVolume(Chroma.VolumeColor, ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        //Chroma.SetPeakVolume_Mouse(ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        //Chroma.SetPeakVolume_Keyboard(ActiveDevice.AudioMeterInformation.MasterPeakValue);
                        //Chroma.SetPeakVolume_Headset_Mousepad();
                    }
                    Chroma.HeadsetGrid.SetPeakVolume(Chroma.VolumeColor);
                    Chroma.MousepadGrid.SetPeakVolume(Chroma.VolumeColor);
                }
                else
                {
                    Chroma.MouseGrid.SetPlayingPosition(Chroma.PositionColor_Foreground, Chroma.PositionColor_Background, player.CalculatedPosition, Properties.Settings.Default.ReverseLEDRender);
                    Chroma.KeyboardGrid.SetPlayingPosition(Chroma.PositionColor_Foreground, Chroma.BackgroundColor, player.CalculatedPosition);
                    Chroma.MouseGrid.SetVolumeScale(Chroma.VolumeColor, player.Volume, Properties.Settings.Default.ReverseLEDRender);
                }
                Chroma.KeyboardGrid.SetVolumeScale(Properties.Settings.Default.Volume.ToColoreColor(), player.Volume);
                Chroma.KeyboardGrid.SetPlayingTime(TimeSpan.FromSeconds(player.Position_ms));
                Chroma.MousepadGrid.SetPeakVolume(Chroma.VolumeColor);
                Chroma.HeadsetGrid.SetPeakVolume(Chroma.VolumeColor);
                Chroma.Apply();
            }
            catch (Exception chromaEx)
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
                    await player.GetAsync();

                });

                if (Properties.Settings.Default.DiscordRichPresenceEnable)
                    UpdatePresenceAsync();
                lbl_CurrentTime.Content = player.Position_ms.ToMinutes();
                lbl_TimeLeft.Content = $"-{Extension.ToMinutes(player.Duration_ms - player.Position_ms)}";
                PlayProgress.Value = player.Position_ms;
                VolumeProgress.Value = player.Volume;
                PlayProgress.Foreground = player.IsPlaying ? playColor : pauseColor;
            }
            catch
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Settings().Show();
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
                if (!player.IsPlaying)
                {
                    presence.details = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPauseDetail, player));
                    presence.state = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPauseState, player));
                }
                else
                {
                    presence.details = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPlayDetail, player));
                    presence.state = Extension.TruncateString(Extension.RenderString(Properties.Settings.Default.DiscordPlayState, player));
                    presence.startTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() - player.Position_ms;
                    presence.endTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds() + ((player.Duration_ms - player.Position_ms) / 1000);
                }
                presence.largeImageText = player.URL;
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
                AlbumImage.Source = (player.AlbumArtwork as Bitmap).ToBitmapImage();
                this.Icon = AlbumImage.Source;
                this.Background = new ImageBrush(player.AlbumArtwork.Blur(10));//, this.Height / this.Width));
            }
            catch
            {

            }
            this.Title = $"Listening to {player.Track} by {player.Artist} on {player.ActiveDevice.Name}";
            var fontColor = (System.Windows.Media.Brush)(new BrushConverter().ConvertFromString(player.Album_StandardColor.Standard.ContrastColor().ToHex()));
            BackPath.Fill = fontColor;
            PlayPath.Fill = fontColor;
            NextPath.Fill = fontColor;
            VolumeProgress.Foreground = fontColor;
            lbl_Track.Content = player.Track;
            lbl_Album.Content = player.Album;
            lbl_Artist.Content = player.Artist;
            PlayProgress.Maximum = player.Duration_ms;
            lbl_Track.Foreground = fontColor;
            lbl_Album.Foreground = fontColor;
            lbl_Artist.Foreground = fontColor;
            lbl_CurrentTime.Foreground = fontColor;
            lbl_TimeLeft.Foreground = fontColor;
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
            player.SetPositionAsync((int)PlayProgress.CalculateRelativeValue());
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new Settings().Show();
        }

        private void MainWindowGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.A:
                        BackPath_Click(null, null);
                        //player.PlayerEngine.PreviousTrack();
                        break;
                    case Key.D:
                        NextPath_Click(null, null);
                        //player.PlayerEngine.NextTrack();
                        break;
                    case Key.W:
                        player.Volume += 10;
                        break;
                    case Key.S:
                        player.Volume -= 10;
                        break;
                    case Key.Q:
                        player.Position_ms -= 15;
                        break;
                    case Key.E:
                        player.Position_ms += 15;
                        break;
                    case Key.Space:
                        PlayPath_Click(null, null);
                        //player.PlayerEngine.PlayPause();
                        break;
                    /*
                case ConsoleKey.H:
                    MainEvent.Reset();
                    ShowHelp();
                    break;
                    */
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
                        Task.Run(async () =>
                        {
                            try
                            {
                                Process.Start(player.URL);
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
            if (player.Position_ms > 10)
                player.Position_ms = 0;
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
                previousVolume = (byte)player.Volume;
                player.Volume = 0;
            }
            else
            {
                player.Volume = previousVolume;
            }
        }

        private void VolumeProgress_MouseDown(object sender, MouseButtonEventArgs e)
        {
            player.Volume = (int)VolumeProgress.CalculateRelativeValue();
        }
    }
}
