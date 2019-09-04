using SpotifyListener.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpotifyListener.Delegations;

namespace SpotifyListener
{
    public class Music : EventArgs, IMusic, IChromaRender, IDisposable
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
                _isPlaying = value;
            }
        }
        public SpotifyAPI.Web.Models.Device ActiveDevice
        {
            get; private set;
        } = new SpotifyAPI.Web.Models.Device();//=> AvailableDevices.Where(x => x.IsActive).FirstOrDefault();
        public List<SpotifyAPI.Web.Models.Device> AvailableDevices { get; private set; } = new List<SpotifyAPI.Web.Models.Device>();
        private System.Windows.Forms.Timer _refreshTokenTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer _trackFetcherTimer = new System.Windows.Forms.Timer();
        private bool Expired = false;
        private StandardColor _standardColor = new StandardColor();
        [JsonIgnore]
        public StandardColor Album_StandardColor => _standardColor;
        private DevicesColor _razerColor = new DevicesColor();
        [JsonIgnore]
        public DevicesColor Album_RazerColor => _razerColor;

        public event TrackChangedEventArgs OnTrackChanged;
        public event EventHandler OnDeviceChanged;
        public event TrackProgressionChangeEventArgs OnTrackDurationChanged;

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
            _trackFetcherTimer.Interval = 500;
            _trackFetcherTimer.Tick += async (s, e) => await this.GetAsync();
            _trackFetcherTimer.Start();
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
                //if (currentActiveDevice.VolumePercent != Volume)
                //{
                //    Volume = ActiveDevice.VolumePercent;
                //}
                if (currentTrack.Item != null)
                {
                    Duration_ms = currentTrack.Item.DurationMs;
                    Position_ms = currentTrack.ProgressMs;
                    Volume = currentActiveDevice.VolumePercent;
                    var testURL = currentTrack.Item.ExternUrls.FirstOrDefault();
                    var url = testURL.Equals(default(KeyValuePair<string, string>)) ? string.Empty : testURL.Value;
                    if (URL != url)
                    {
                        URL = url;
                        AlbumArtwork?.Dispose();
                        AlbumArtwork = null;
                    }
                    if (AlbumArtwork == null)
                    {
                        Track = currentTrack.Item.Name;
                        Album = currentTrack.Item.Album.Name;
                        if (currentTrack.Item.Artists.Count() > 1)
                        {
                            var artistFeat = $@"{currentTrack.Item.Artists.First().Name} feat. ";
                            artistFeat += string.Join(", ", currentTrack.Item.Artists.Skip(1).Select(x => x.Name));
                            Artist = artistFeat;
                        }
                        else
                        {
                            Artist = currentTrack.Item.Artists.First().Name;
                        }

                        if (currentTrack.Item.Album.Images.Count() > 0)
                        {
                            ArtworkURL = currentTrack.Item.Album.Images[0].Url;
                            using (var client = new HttpClient())
                            {
                                var byteArray = await client.GetByteArrayAsync(ArtworkURL);
                                Image image = (Image)(new ImageConverter()).ConvertFrom(byteArray);
                                AlbumArtwork = image;
                            }
                        }
                        else
                        {
                            AlbumArtwork = await HTMLHelper.GetImage(Track, Album, Artist);
                        }
                        _standardColor.Standard = albumColoreMode == 0 ? AlbumArtwork.DominantColor() : AlbumArtwork.AverageColor();
                        _standardColor.Complemented = _standardColor.Standard.InverseColor();
                        _razerColor.Standard = _standardColor.Standard.ToColoreColor();//.SoftColor().ToColoreColor();
                        _razerColor.Complemented = _standardColor.Complemented.ToColoreColor();
                        OnTrackChanged(this);
                    };
                }
                else
                {
                    Track = "Loading...";
                    Artist = "Loading...";
                    Album = "Loading...";
                    Duration_ms = 0;
                    Position_ms = 0;
                    Volume = 0;
                    OnTrackChanged(this);
                }
            }
            else if (currentTrack.HasError())
            {
                Expired = true;
            }



            IsPlaying = currentTrack.IsPlaying;
            OnTrackDurationChanged(this);
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
            SetPositionAsync(asMillisecond).RunSynchronously();
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
        public void SetVolume(int volume)
        {
            client.SetVolume(volume);
        }
        public async Task SetActiveDeviceAsync(string id)
        {
            await client.ResumePlaybackAsync(id, "", null, "", Position_ms);
        }
        public void ClearImage()
        {
            this.AlbumArtwork?.Dispose();
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTokenTimer.Stop();
                _refreshTokenTimer.Dispose();
                _trackFetcherTimer.Stop();
                _trackFetcherTimer.Dispose();
                AlbumArtwork.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
