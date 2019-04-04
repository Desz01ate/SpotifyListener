using SpotifyListener.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace SpotifyListener
{
    public enum State
    {
        Playing,
        Pause,
        Stop
    }

    public class Music : EventArgs, IMusic, IChromaRender
    {
        private SpotifyAPI.Web.SpotifyWebAPI client = null;

        public string Track { get; private set; }
        public string Album { get; private set; }
        public string Artist { get; private set; }
        public string URL { get; private set; }
        public string ArtworkURL { get; private set; }
        public int Position_ms { get; set; }
        public int Duration_ms { get; private set; }
        private int _volume = 0;
        public int Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                var test = client.SetVolume(_volume, ActiveDevice.Id);
            }
        }
        public Image AlbumArtwork { get; private set; }
        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            private set
            {
                if (value != IsPlaying)
                {
                    if (value)
                    {
                        OnResume(null, null);
                    }
                    else
                    {
                        OnPaused(null, null);
                    }
                }
                _isPlaying = value;
            }
        }
        public SpotifyAPI.Web.Models.Device ActiveDevice
        {
            get; private set;
        } = new SpotifyAPI.Web.Models.Device();//=> AvailableDevices.Where(x => x.IsActive).FirstOrDefault();
        public List<SpotifyAPI.Web.Models.Device> AvailableDevices { get; private set; } = new List<SpotifyAPI.Web.Models.Device>();
        private System.Windows.Forms.Timer _refreshTokenTimer = new System.Windows.Forms.Timer();
        private bool Expired = false;
        private StandardColor _standardColor = new StandardColor();
        [JsonIgnore]
        public StandardColor Album_StandardColor => _standardColor;
        private DevicesColor _razerColor = new DevicesColor();
        [JsonIgnore]
        public DevicesColor Album_RazerColor => _razerColor;

        public event EventHandler OnTrackChanged;
        public event EventHandler OnDeviceChanged;
        public event EventHandler OnResume;
        public event EventHandler OnPaused;

        public Music(string accessToken, string refreshToken)
        {
            client = new SpotifyAPI.Web.SpotifyWebAPI()
            {
                AccessToken = accessToken,
                TokenType = "Bearer"
            };
            _refreshTokenTimer.Interval = 1000;
            _refreshTokenTimer.Tick += async (s, e) =>
           {
               if (Expired)
               {
                   var auth = new SpotifyAPI.Web.Auth.AuthorizationCodeAuth("7b2f38e47869431caeda389929a1908e", "90dc137926e34bd78a1737266b3df20b", "http://localhost", "http://localhost", SpotifyAPI.Web.Enums.Scope.AppRemoteControl, "");
                   var token = await auth.RefreshToken(refreshToken);

                   client.AccessToken = token.AccessToken;
                   Properties.Settings.Default.AccessToken = token.AccessToken;
                   Properties.Settings.Default.Save();
                   Expired = false;
               }

           };
            _refreshTokenTimer.Start();
        }

        public override string ToString()
        {
            var result = $"{Track} - {Album} by {Artist}";
            if (result.Length >= 64)
                result = result.Substring(0, 63);
            return result;
        }

        public void Get(int albumColorMode = 0)
        {
            GetAsync(albumColorMode).RunSynchronously();
        }
        public async Task GetAsync(int albumColoreMode = 0)
        {
            var currentTrack = await client.GetPlayingTrackAsync();
            if (currentTrack.IsPlaying)
            {
                var devices = (await client.GetDevicesAsync()).Devices;
                if (devices.Except(AvailableDevices).Count() > 0)
                {
                    AvailableDevices = devices;
                }
                var currentActiveDevice = devices.Find(x => x.IsActive);
                if (ActiveDevice.Id != currentActiveDevice.Id)
                {
                    ActiveDevice = currentActiveDevice;
                    OnDeviceChanged(ActiveDevice, null);
                }
                if (currentActiveDevice.VolumePercent != Volume)
                {
                    Volume = ActiveDevice.VolumePercent;
                }


                Duration_ms = currentTrack.Item.DurationMs;
                Position_ms = currentTrack.ProgressMs;

                var testURL = currentTrack.Item.ExternUrls.FirstOrDefault();
                var url = testURL.Equals(default(KeyValuePair<string, string>)) ? string.Empty : testURL.Value;
                if (URL != url)
                {
                    URL = url;
                    AlbumArtwork = null;
                }
                if (AlbumArtwork == null)
                {
                    Track = currentTrack.Item.Name;
                    Album = currentTrack.Item.Album.Name;
                    Artist = string.Join(",", currentTrack.Item.Artists.Select(x => x.Name));

                    if (currentTrack.Item.Album.Images.Count() > 0)
                    {
                        ArtworkURL = currentTrack.Item.Album.Images[0].Url;
                        var client = new HttpClient();
                        var byteArray = await client.GetByteArrayAsync(ArtworkURL);
                        Image image = (Image)((new ImageConverter()).ConvertFrom(byteArray));
                        client.Dispose();
                        AlbumArtwork = image;
                    }
                    else
                    {
                        AlbumArtwork = await HTMLHelper.GetImage(Track, Album, Artist);
                    }

                    _standardColor.Standard = albumColoreMode == 0 ? AlbumArtwork.DominantColor() : AlbumArtwork.AverageColor();
                    _standardColor.Complemented = _standardColor.Standard.InverseColor();
                    _razerColor.Standard = _standardColor.Standard.ToColoreColor();//.SoftColor().ToColoreColor();
                    _razerColor.Complemented = _standardColor.Complemented.ToColoreColor();
                    OnTrackChanged.Invoke(currentTrack, null);
                };
            }
            else if (currentTrack.HasError())
            {
                Expired = true;
            }
            IsPlaying = currentTrack.IsPlaying;

        }
        public void PlayPause()
        {

            if (IsPlaying)
            {
                var response = client.PausePlayback(ActiveDevice.Id);
                //OnPaused(null, null);
            }
            else
            {
                client.ResumePlayback(ActiveDevice.Id, "", null, "", Position_ms);
                //OnResume(null, null);
            }
            IsPlaying = !IsPlaying;
        }
        public async Task PlayPauseAsync()
        {
            if (IsPlaying)
            {
                await client.PausePlaybackAsync();
                //OnPaused(null, null);
            }
            else
            {
                await client.ResumePlaybackAsync("", "", null, "", Position_ms);
                //OnResume(null, null);
            }
            IsPlaying = !IsPlaying;
        }


        public void Next()
        {
            client.SkipPlaybackToNext();
        }
        public async Task NextAsync()
        {
            await client.SkipPlaybackToPreviousAsync();
        }
        public void Previous()
        {
            client.SkipPlaybackToPrevious();
        }
        public async Task PreviousAsync()
        {
            await client.SkipPlaybackToPreviousAsync();
        }
        public void Stop()
        {
            throw new NotImplementedException();
        }
        public async Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public void SetPosition(int asMillisecond)
        {
            SetPositionAsync(asMillisecond);
        }
        public async Task SetPositionAsync(int asMillisecond)
        {
            await client.SeekPlaybackAsync(asMillisecond, ActiveDevice.Id);
        }
        public double CalculatedPosition
        {
            get
            {
                try
                {
                    return Math.Round(((double)Position_ms / Duration_ms) * 10, 2);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public async Task SetActiveDeviceAsync(string id)
        {
            await client.ResumePlaybackAsync(id, "", null, "", Position_ms);
        }
    }
}
