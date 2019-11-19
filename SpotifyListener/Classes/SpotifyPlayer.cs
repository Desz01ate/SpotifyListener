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
using SpotifyListener.DatabaseManager;
using SpotifyListener.DatabaseManager.Models;

namespace SpotifyListener
{
    public class SpotifyPlayer : IStreamableMusic, IChromaRender
    {
        private SpotifyAPI.Web.SpotifyWebAPI client = null;

        public string Track { get; private set; }
        public string Album { get; private set; }
        public string Artist { get; private set; }
        public string Genre { get; private set; }
        public string Type { get; private set; }
        public string URL { get; private set; }
        public string ArtworkURL { get; private set; }
        public int Position_ms { get; private set; }
        public int Duration_ms { get; private set; }
        public int Volume { get; private set; } = 0;
        public Image AlbumArtwork { get; private set; }
        public bool IsPlaying { get; private set; } = false;
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
        public SpotifyAPI.Web.Models.Device ActiveDevice
        {
            get; private set;
        } = new SpotifyAPI.Web.Models.Device();//=> AvailableDevices.Where(x => x.IsActive).FirstOrDefault();
        public IList<SpotifyAPI.Web.Models.Device> AvailableDevices { get; private set; } = new List<SpotifyAPI.Web.Models.Device>();
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

        public SpotifyPlayer(string accessToken, string refreshToken)
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
                    var token = await auth.RefreshToken(refreshToken).ConfigureAwait(false);

                    client.AccessToken = token.AccessToken;
                    Properties.Settings.Default.AccessToken = token.AccessToken;
                    Properties.Settings.Default.Save();
                    Expired = false;
                }

            };
            _refreshTokenTimer.Start();
            _trackFetcherTimer.Interval = 1000;
            _trackFetcherTimer.Tick += async (s, e) => await GetAsync().ConfigureAwait(false);
            _trackFetcherTimer.Start();
        }

        public override string ToString()
        {
            var result = $"{Track} - {Album} by {Artist}";
            if (result.Length >= 64)
                result = result.Substring(0, 63);
            return result;
        }

        public virtual void Get(int albumColorMode = 0)
        {
            GetAsync(albumColorMode).RunSynchronously();
        }
        public virtual async Task GetAsync(int albumColoreMode = 0)
        {
            var currentTrack = await client.GetPlayingTrackAsync();
            if (currentTrack.IsPlaying)
            {
                var devices = (await client.GetDevicesAsync()).Devices;
                if (devices.Except(AvailableDevices).Any())
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
                        var artist = await client.GetArtistAsync(currentTrack.Item.Artists.FirstOrDefault()?.Id);
                        if (artist != null)
                        {
                            Genre = artist.Genres.FirstOrDefault();
                            Type = artist.Type;
                        }
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

                        if (currentTrack.Item.Album.Images.Any())
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
                            AlbumArtwork = new Bitmap(1, 1);
                        }
                        _standardColor.Standard = albumColoreMode == 0 ? AlbumArtwork.DominantColor() : AlbumArtwork.AverageColor();
                        _standardColor.Complemented = _standardColor.Standard.InverseColor();
                        _razerColor.Standard = _standardColor.Standard.ToColoreColor();//.SoftColor().ToColoreColor();
                        _razerColor.Complemented = _standardColor.Complemented.ToColoreColor();
                        await SQLiteService.Context.ListenHistories.InsertAsync(new ListenHistory(this)).ConfigureAwait(false);
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
        public virtual void PlayPause()
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
        public virtual async Task PlayPauseAsync()
        {
            if (IsPlaying)
            {
                await client.PausePlaybackAsync().ConfigureAwait(false);
                //OnPaused(null, null);
            }
            else
            {
                await client.ResumePlaybackAsync("", "", null, "", Position_ms).ConfigureAwait(false);
                //OnResume(null, null);
            }
            IsPlaying = !IsPlaying;
        }


        public virtual void Next()
        {
            client.SkipPlaybackToNext();
        }
        public virtual async Task NextAsync()
        {
            await client.SkipPlaybackToPreviousAsync().ConfigureAwait(false);
        }
        public virtual void Previous()
        {
            client.SkipPlaybackToPrevious();
        }
        public virtual async Task PreviousAsync()
        {
            await client.SkipPlaybackToPreviousAsync().ConfigureAwait(false);
        }
        public virtual void Stop()
        {
            throw new NotImplementedException();
        }
        public virtual async Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public virtual void SetPosition(int asMillisecond)
        {
            SetPositionAsync(asMillisecond).RunSynchronously();
        }
        public virtual async Task SetPositionAsync(int asMillisecond)
        {
            await client.SeekPlaybackAsync(asMillisecond, ActiveDevice.Id).ConfigureAwait(false);
        }
        public virtual void SetVolume(int volume)
        {

            if (volume < 0)
            {
                client.SetVolume(0);
            }
            else if (volume > 100)
            {
                client.SetVolume(100);
            }
            else
            {
                client.SetVolume(volume);
            }
        }
        public virtual void SetActiveDevice(object id)
        {
            client.ResumePlayback(id as string, "", null, "", Position_ms);
        }
        public virtual async Task SetActiveDeviceAsync(object id)
        {
            await client.ResumePlaybackAsync(id as string, "", null, "", Position_ms).ConfigureAwait(false);
        }
        public virtual void ClearImage()
        {
            this.AlbumArtwork?.Dispose();
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshTokenTimer.Stop();
                _refreshTokenTimer?.Dispose();
                _trackFetcherTimer.Stop();
                _trackFetcherTimer?.Dispose();
                AlbumArtwork?.Dispose();
                client?.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private int _PreviousVolume = 0;
        public bool IsMute { get; private set; }
        public virtual void Mute()
        {
            _PreviousVolume = this.Volume;
            this.SetVolume(0);
            IsMute = true;
        }
        public virtual void Unmute()
        {
            this.SetVolume(_PreviousVolume);
            IsMute = false;
        }
        public void Play(string url)
        {

        }
        public async Task PlayAsync(string url)
        {
            await client.ResumePlaybackAsync(this.ActiveDevice.Id, "", new List<string>() { "https://open.spotify.com/track/5C0ivQMxes2lWuOANhvVAm" }, 0, 0);
        }
    }
}
