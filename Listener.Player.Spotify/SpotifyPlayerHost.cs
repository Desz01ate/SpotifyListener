using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using AutoMapper;
using Listener.Core.Framework.Events;
using Listener.Core.Framework.Helpers;
using Listener.Core.Framework.Models;
using Listener.Core.Framework.Players;
using Listener.ImageProcessing;
using SpotifyAPI.Web;
using Device = Listener.Core.Framework.Models.Device;
using SpotifyDevice = SpotifyAPI.Web.Models.Device;

namespace Listener.Player.Spotify
{
    public sealed class SpotifyPlayerHost : IStreamablePlayerHost
    {
        private readonly HttpClient httpClient;
        private SpotifyWebAPI client;
        private string _track, _album, _artist, _genre, _type, _url, _artworkUrl, _lyrics;
        private int _pos_ms, _dur_ms, _vol;
        private bool _isPlaying;
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
        public string Lyrics
        {
            get
            {
                return _lyrics;
            }
            private set
            {
                _lyrics = value;
                OnPropertyChanged(nameof(Lyrics));
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
                var currentTime = TimeSpan.FromMilliseconds(Position_ms);
                return string.Format("{0:0}:{1:00}", currentTime.Minutes, currentTime.Seconds);
            }
        }
        public string TimeLeft
        {
            get
            {
                var remaining = TimeSpan.FromMilliseconds(Duration_ms - Position_ms);
                return string.Format("{0:0}:{1:00}", remaining.Minutes, remaining.Seconds);
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
            }
        }

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
        public Listener.Core.Framework.Models.Device ActiveDevice
        {
            get; private set;
        } = Listener.Core.Framework.Models.Device.Default;
        public IReadOnlyList<Device> AvailableDevices => AvailableSpotifyDevices.Select(d => _deviceMapper.Map<SpotifyDevice, Device>(d)).ToList();
        private List<SpotifyDevice> AvailableSpotifyDevices = new List<SpotifyDevice>();
        private System.Windows.Forms.Timer _trackFetcherTimer = new System.Windows.Forms.Timer();

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Mapper _deviceMapper;

        public event TrackChangedEventHandler TrackChanged;

        public event TrackProgressionChangedEventHandler TrackDurationChanged;

        public event TrackPlayStateChangedEventHandler TrackPlayStateChanged;

        public event ActiveDeviceChangedEventHandler DeviceChanged;

        public SpotifyPlayerHost()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<SpotifyDevice, Device>());
            this._deviceMapper = new Mapper(config);
            this.httpClient = new HttpClient();
        }


        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Process.Start("spotify");
            }
            catch
            {
                //Spotify is not installed, no problem just skip it.
            }
            SpotifyWebAPI webApiClient = null;
            var spotifyAuthentication = new SpotifyAuthentication();
            var retry = 0;
            spotifyAuthentication.ClientReady += (auth, spotifyClient) => webApiClient = spotifyClient;
            while (webApiClient == null)
            {
                if (retry++ >= 10)
                {
                    throw new TimeoutException();
                }
                System.Threading.Thread.Sleep(1000);
            }

            this.client = webApiClient;
            _trackFetcherTimer.Interval = 1000;
            _trackFetcherTimer.Tick += async (s, e) => await GetAsync();
            _trackFetcherTimer.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _trackFetcherTimer?.Stop();
            _trackFetcherTimer?.Dispose();
            return Task.CompletedTask;
        }

        public override string ToString()
        {
            var result = $"{Track} - {Album} by {Artist}";
            if (result.Length >= 64)
                result = result.Substring(0, 63);
            return result;
        }

        public void Get()
        {
            GetAsync().RunSynchronously();
        }

        public async Task GetAsync()
        {
            try
            {
                if (client == null) return;
                var currentTrack = await client.GetPlayingTrackAsync();
                if (currentTrack.IsPlaying)
                {
                    var devices = (await client.GetDevicesAsync()).Devices;
                    if (devices.Except(AvailableSpotifyDevices).Any())
                    {
                        AvailableSpotifyDevices = devices;
                    }
                    var currentActiveDevice = devices.Find(x => x.IsActive);
                    if (ActiveDevice.Id != currentActiveDevice.Id)
                    {
                        ActiveDevice = _deviceMapper.Map<SpotifyDevice, Device>(currentActiveDevice);
                        DeviceChanged?.Invoke(ActiveDevice);
                    }

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
                            var mainArtist = currentTrack.Item.Artists.First().Name;
                            if (currentTrack.Item.Artists.Count() > 1)
                            {
                                var artistFeat = $@"{mainArtist} feat. ";
                                artistFeat += string.Join(", ", currentTrack.Item.Artists.Skip(1).Select(x => x.Name));
                                Artist = artistFeat;
                            }
                            else
                            {
                                Artist = mainArtist;
                            }

                            //Lyrics = await LyricsHelpers.GetLyricsAsync(mainArtist, this.Track);

                            if (currentTrack.Item.Album.Images.Any())
                            {
                                ArtworkURL = currentTrack.Item.Album.Images[0].Url;
                                AlbumArtwork = await GetImageAsync(ArtworkURL);
                            }
                            else
                            {
                                AlbumArtwork = new Bitmap(1, 1);
                            }

                            TrackChanged?.Invoke(this);
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
                        TrackChanged?.Invoke(this);
                    }
                }

                if (IsPlaying != currentTrack.IsPlaying)
                    TrackPlayStateChanged?.Invoke(currentTrack.IsPlaying ? PlayState.Play : PlayState.Pause);
                IsPlaying = currentTrack.IsPlaying;
                TrackDurationChanged?.Invoke(this);
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

        public void PlayPause()
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
        public async Task PlayPauseAsync()
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


        public void Next()
        {
            client.SkipPlaybackToNext();
        }
        public async Task NextAsync()
        {
            await client.SkipPlaybackToPreviousAsync().ConfigureAwait(false);
        }
        public void Previous()
        {
            client.SkipPlaybackToPrevious();
        }
        public async Task PreviousAsync()
        {
            await client.SkipPlaybackToPreviousAsync().ConfigureAwait(false);
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
            await client.SeekPlaybackAsync(asMillisecond, ActiveDevice.Id).ConfigureAwait(false);
        }
        public void SetVolume(int volume)
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
        public void SetActiveDevice(object id)
        {
            client.ResumePlayback(id as string, "", null, "", Position_ms);
        }
        public async Task SetActiveDeviceAsync(object id)
        {
            var res = await client.ResumePlaybackAsync(id as string, "", new List<string>(new string[] { Url }), "", Position_ms).ConfigureAwait(false);
        }
        public void ClearImage()
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
        public void Mute()
        {
            _PreviousVolume = this.Volume;
            this.SetVolume(0);
            IsMute = true;
        }
        public void Unmute()
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
        public async Task<IEnumerable<SearchResult>> SearchAsync(string search, SearchType searchType, int limit = 5)
        {
            var result = await client.SearchItemsAsync(search, (SpotifyAPI.Web.Enums.SearchType)searchType, limit).ConfigureAwait(false);
            switch (searchType)
            {
                case Listener.Core.Framework.Events.SearchType.Album:
                    return result?.Albums?.Items.Select(x => new SearchResult($@"{x.Name} by {x.Artists.FirstOrDefault()?.Name}", searchType, x.Uri));
                case Listener.Core.Framework.Events.SearchType.Playlist:
                    return result?.Playlists?.Items.Select(x => new SearchResult($@"{x.Name} by {x.Owner.DisplayName}", searchType, x.Uri));
                case Listener.Core.Framework.Events.SearchType.Artist:
                    return result?.Artists?.Items.Select(x => new SearchResult($@"{x.Name}", searchType, x.ExternalUrls.FirstOrDefault().Value));
                default:
                    return result?.Tracks?.Items.Select(x => new SearchResult($@"{x.Name} - {x.Album.Name} by {x.Artists.FirstOrDefault()?.Name}", searchType, x.Uri));
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}
