using SpotifyListener.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpotifyListener.Delegations;
using SpotifyListener.DatabaseManager;
using SpotifyListener.DatabaseManager.Models;
using SpotifyAPI.Web.Models;
using Image = System.Drawing.Image;
using SpotifyAPI.Web.Enums;
using SpotifyListener.Enums;
using SpotifyAPI.Web;
using System.ComponentModel;
using System.Windows.Media;
using SpotifyListener.Classes;
using System.Drawing.Imaging;

namespace SpotifyListener
{
    public class SpotifyPlayer : IStreamableMusic, IChromaRender
    {
        private readonly HttpClient httpClient;
        private readonly SpotifyWebAPI client;
        private string _track, _album, _artist, _genre, _type, _url, _artworkUrl;
        private int _pos_ms, _dur_ms, _vol;
        private bool _isPlaying, _isShuffle, _isRepeat;
        public string Track
        {
            get
            {
                return _track;
            }
            private set
            {
                _track = value;
                OnPropertyChanged(nameof(Track));
            }
        }
        public string Album
        {
            get
            {
                return _album;
            }
            private set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }
        public string Artist
        {
            get
            {
                return _artist;
            }
            private set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }
        public string Genre
        {
            get
            {
                return _genre;
            }
            private set
            {
                _genre = value;
                OnPropertyChanged(nameof(Genre));
            }
        }
        public string Type
        {
            get
            {
                return _type;
            }
            private set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }
        public string Url
        {
            get
            {
                return _url;
            }
            private set
            {
                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }
        public string ArtworkURL
        {
            get
            {
                return _artworkUrl;
            }
            private set
            {
                _artworkUrl = value;
                OnPropertyChanged(nameof(ArtworkURL));
            }
        }
        public string CurrentTime
        {
            get
            {
                return Position_ms.ToMinutes();
            }
        }
        public string TimeLeft
        {
            get
            {
                return $"-{Extension.ToMinutes(Duration_ms - Position_ms)}";
            }
        }
        public int Position_ms
        {
            get
            {
                return _pos_ms;
            }
            private set
            {
                _pos_ms = value;
                OnPropertyChanged(nameof(CurrentTime));
                OnPropertyChanged(nameof(TimeLeft));
                OnPropertyChanged(nameof(Position_ms));
            }
        }
        public int Duration_ms
        {
            get
            {
                return _dur_ms;
            }
            private set
            {
                _dur_ms = value;
                OnPropertyChanged(nameof(Duration_ms));
            }
        }
        public int Volume
        {
            get
            {
                return _vol;
            }
            set
            {
                _vol = value;
                OnPropertyChanged(nameof(Volume));
            }
        }
        private Image _albumImage;
        public Image AlbumArtwork
        {
            get
            {
                return _albumImage;
            }
            private set
            {
                _albumImage = value;
                if (value != null)
                {
                    _albumSource = value?.ToBitmapImage(ImageFormat.Jpeg);
                    OnPropertyChanged(nameof(AlbumSource));
                    using var background = Effects.BitmapHelper.CalculateBackgroundSource(value);
                    AlbumBackgroundSource = ImageProcessing.ToSafeMemoryBrush(background as Bitmap);
                    AlbumBackgroundSource.Freeze();
                    OnPropertyChanged(nameof(AlbumBackgroundSource));
                }
            }
        }


        private ImageSource _albumSource;
        public ImageSource AlbumSource => _albumSource;
        public System.Windows.Media.Brush AlbumBackgroundSource { get; private set; } = System.Windows.Media.Brushes.Gray;
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            private set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        public bool IsShuffle { get; private set; } = false;
        public bool IsRepeat { get; private set; } = false;
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
        public Device ActiveDevice
        {
            get; private set;
        } = new Device();//=> AvailableDevices.Where(x => x.IsActive).FirstOrDefault();
        public IList<Device> AvailableDevices { get; private set; } = new List<Device>();
        private System.Windows.Forms.Timer _trackFetcherTimer = new System.Windows.Forms.Timer();
        private StandardColor _standardColor = new StandardColor();
        [JsonIgnore]
        public StandardColor Album_StandardColor => _standardColor;
        private DevicesColor _razerColor = new DevicesColor();
        [JsonIgnore]
        public DevicesColor Album_RazerColor => _razerColor;

        public event TrackChangedEventHandler OnTrackChanged;
        public event EventHandler OnDeviceChanged;
        public event TrackProgressionChangedEventHandler OnTrackDurationChanged;
        public event TrackPlayStateChangedEventHandler OnPlayStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public SpotifyPlayer(SpotifyWebAPI client)
        {
            this.client = client;
            this.httpClient = new HttpClient();
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
            try
            {
                if (client == null) return;
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
                        OnDeviceChanged?.Invoke(ActiveDevice, null);
                    }
                    this.IsShuffle = currentTrack.ShuffleState;
                    this.IsRepeat = currentTrack.RepeatState == SpotifyAPI.Web.Enums.RepeatState.Context;

                    if (currentTrack.Item != null)
                    {
                        Duration_ms = currentTrack.Item.DurationMs;
                        Position_ms = currentTrack.ProgressMs;
                        Volume = currentActiveDevice.VolumePercent;
                        var testURL = currentTrack.Item.ExternUrls.FirstOrDefault();
                        var url = testURL.Equals(default(KeyValuePair<string, string>)) ? string.Empty : testURL.Value;
                        if (Url != url)
                        {
                            Url = url;
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
                                AlbumArtwork = await GetImageAsync(ArtworkURL);
                            }
                            else
                            {
                                AlbumArtwork = new Bitmap(1, 1);
                            }
                            _standardColor.Standard = albumColoreMode == 0 ? AlbumArtwork.DominantColor() : AlbumArtwork.AverageColor();
                            _standardColor.Complemented = _standardColor.Standard.InverseColor();
                            _razerColor.Standard = _standardColor.Standard.ToColoreColor();
                            _razerColor.Complemented = _standardColor.Complemented.ToColoreColor();
                            await Service.Context.ListenHistory.InsertAsync(new ListenHistory(this)).ConfigureAwait(false);
                            OnTrackChanged?.Invoke(this);
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
                        OnTrackChanged?.Invoke(this);
                    }
                }

                if (IsPlaying != currentTrack.IsPlaying)
                    OnPlayStateChanged?.Invoke(currentTrack.IsPlaying ? PlayState.Play : PlayState.Pause);
                IsPlaying = currentTrack.IsPlaying;
                OnTrackDurationChanged?.Invoke(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async ValueTask<Image> GetImageAsync(string artworkUrl)
        {
            Image image;
            var lookupKey = artworkUrl.Substring(artworkUrl.LastIndexOf("/") + 1);
            if (CacheFileManager.TryGetFileCache(lookupKey, out var fs))
            {
                using (fs)
                {
                    image = Image.FromStream(fs);
                }
            }
            else
            {
                var imageBytes = await httpClient.GetByteArrayAsync(new Uri(artworkUrl));
                image = (Image)(new ImageConverter()).ConvertFrom(imageBytes);
                CacheFileManager.SaveCache(lookupKey, imageBytes);
            }
            return image;
        }

        public virtual void PlayPause()
        {

            if (IsPlaying)
            {
                client.PausePlayback(ActiveDevice.Id);
            }
            else
            {
                client.ResumePlayback(ActiveDevice.Id, "", null, "", Position_ms);
            }
        }
        public virtual async Task PlayPauseAsync()
        {
            if (IsPlaying)
            {
                await client.PausePlaybackAsync().ConfigureAwait(false);
            }
            else
            {
                await client.ResumePlaybackAsync("", "", null, "", Position_ms).ConfigureAwait(false);
            }
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
            var res = await client.ResumePlaybackAsync(id as string, "", new List<string>(new string[] { Url }), "", Position_ms).ConfigureAwait(false);
        }
        public virtual void ClearImage()
        {
            this.AlbumArtwork?.Dispose();
        }
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _trackFetcherTimer.Stop();
                _trackFetcherTimer?.Dispose();
                AlbumArtwork?.Dispose();
                client?.Dispose();
                httpClient?.Dispose();
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
            await client.ResumePlaybackAsync(this.ActiveDevice.Id, url, null, 0, 0);
        }
        public async Task PlayTrackAsync(string url)
        {
            await client.ResumePlaybackAsync(this.ActiveDevice.Id, "", new List<string>() { url }, 0, 0);

        }
        public async Task<IEnumerable<(string track, SearchType searchType, string uri)>> SearchAsync(string search, SearchType searchType, int limit = 5)
        {
            var result = await client.SearchItemsAsync(search, searchType, limit).ConfigureAwait(false);
            switch (searchType)
            {
                case SearchType.Album:
                    return result?.Albums?.Items.Select(x => ($@"{x.Name} by {x.Artists.FirstOrDefault()?.Name}", searchType, x.Uri));
                case SearchType.Playlist:
                    return result?.Playlists?.Items.Select(x => ($@"{x.Name} by {x.Owner.DisplayName}", searchType, x.Uri));
                case SearchType.Artist:
                    return result?.Artists?.Items.Select(x => ($@"{x.Name}", searchType, x.ExternalUrls.FirstOrDefault().Value));
                default:
                    return result?.Tracks?.Items.Select(x => ($@"{x.Name} - {x.Album.Name} by {x.Artists.FirstOrDefault()?.Name}", searchType, x.Uri));
            }
        }

        public void ToggleShuffle()
        {
            var res = client.SetShuffle(!this.IsShuffle, ActiveDevice.Id);
            Console.WriteLine(res);
        }

        public void SetRepeat(Enums.RepeatState state)
        {
            client.SetRepeatMode((SpotifyAPI.Web.Enums.RepeatState)(int)state, ActiveDevice.Id);
        }
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
