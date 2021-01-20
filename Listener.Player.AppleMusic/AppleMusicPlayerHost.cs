using Listener.Core.Framework.Events;
using Listener.Core.Framework.Models;
using Listener.Core.Framework.Players;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using iTunesLib;
using System.Linq;
using System.Drawing;
using Listener.Core.Framework.Helpers;
using System.Text;
using System.Net;
using System.IO;
using IF.Lastfm.Core.Api;
using Listener.ImageProcessing;
using System.Drawing.Imaging;
using System.Net.Http;

namespace Listener.Player.AppleMusic
{
    public class AppleMusicPlayerHost : IStreamablePlayerHost
    {
        private string _track, _album, _artist, _genre, _type, _url, _artworkUrl, _lyrics;
        private int _pos_ms, _dur_ms, _vol;
        private bool _isPlaying;

        private List<Device> devices = new List<Device>() {
            new Device("0",true,false,"This PC","computer",100),
        };

        public IReadOnlyList<Device> AvailableDevices => devices;

        public Device ActiveDevice => AvailableDevices[0];

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

        public string CurrentTime
        {
            get
            {
                var currentTime = TimeSpan.FromSeconds(Position_ms);
                return string.Format("{0:0}:{1:00}", currentTime.Minutes, currentTime.Seconds);
            }
        }
        public string TimeLeft
        {
            get
            {
                var remaining = TimeSpan.FromSeconds(Duration_ms - Position_ms);
                return string.Format("{0:0}:{1:00}", remaining.Minutes, remaining.Seconds);
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

        public System.Drawing.Image AlbumArtwork
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

        public bool IsPlaying { get; private set; }

        public bool IsMute { get; private set; }

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

        public event ActiveDeviceChangedEventHandler OnDeviceChanged;
        public event TrackChangedEventHandler OnTrackChanged;
        public event TrackProgressionChangedEventHandler OnTrackDurationChanged;
        public event TrackPlayStateChangedEventHandler OnTrackPlayStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private ITPlayerState currentPlayState;

        private readonly iTunesApp app;

        private readonly HttpClient httpClient;

        private System.Windows.Forms.Timer _trackFetcherTimer = new System.Windows.Forms.Timer();

        delegate void Router(object arg);
        public AppleMusicPlayerHost()
        {
            this.app = new iTunesApp();

            var initImage = new Bitmap(100, 100);
            using (Graphics gfx = Graphics.FromImage(initImage))
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                gfx.FillRectangle(brush, 0, 0, 100, 100);
            }
            this.AlbumArtwork = initImage;

            _trackFetcherTimer.Interval = 1000;
            _trackFetcherTimer.Tick += (s, e) => Get();
            _trackFetcherTimer.Start();

            httpClient = new HttpClient();
        }

        public void Dispose()
        {
            return;
        }

        public void Get()
        {
            this.Volume = this.app.SoundVolume;
            IITTrack currentTrack = this.app.CurrentTrack;
            if (currentTrack == null)
            {
                return;
            }

            ITPlayerState t = ITPlayerState.ITPlayerStateFastForward;
            if (currentPlayState != this.app.PlayerState)
            {
                currentPlayState = this.app.PlayerState;
                OnTrackPlayStateChanged?.Invoke(currentPlayState switch
                {
                    ITPlayerState.ITPlayerStatePlaying => PlayState.Play,
                    _ => PlayState.Pause
                });
            }

            if (this.Track != currentTrack.Name || this.Album != currentTrack.Album)
            {
                this.Track = currentTrack.Name;
                this.Artist = currentTrack.Artist;
                this.Album = currentTrack.Album;
                this.Genre = currentTrack.Genre;
                this.Duration_ms = currentTrack.Duration;
                GetImageUrlByInfoAsync(Album, Artist, Track).ContinueWith(url =>
                {
                    url.Wait();
                    string lookupKey;
                    if (string.IsNullOrWhiteSpace(url.Result))
                    {
                        lookupKey = "not_found.jpg";
                    }
                    else
                    {
                        lookupKey = $"{currentTrack.trackID}{currentTrack.TrackDatabaseID}{currentTrack.sourceID}";
                    }
                    GetImageAsync(lookupKey, url.Result).ContinueWith(image =>
                    {
                        image.Wait();
                        this.AlbumArtwork = image.Result;
                        this.OnTrackChanged?.Invoke(this);
                    });
                    return;
                });
            }
            this.Position_ms = this.app.PlayerPosition;
            this.IsPlaying = currentPlayState == ITPlayerState.ITPlayerStatePlaying;
        }

        public Task GetAsync()
        {
            Get();
            return Task.CompletedTask;
        }

        public void Mute()
        {
            this.app.Mute = true;
        }

        public void Next()
        {
            this.app.NextTrack();
        }

        public Task NextAsync()
        {
            this.app.NextTrack();
            return Task.CompletedTask;
        }

        public void Play(string url)
        {
            this.app.Play();
        }

        public Task PlayAsync(string url)
        {
            this.app.Play();
            return Task.CompletedTask;
        }

        public void PlayPause()
        {
            this.app.PlayPause();
        }

        public Task PlayPauseAsync()
        {
            this.app.PlayPause();
            return Task.CompletedTask;
        }

        public Task PlayTrackAsync(string url)
        {
            throw new NotImplementedException();
        }

        public void Previous()
        {
            this.app.PreviousTrack();
        }

        public Task PreviousAsync()
        {
            this.app.PreviousTrack();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<SearchResult>> SearchAsync(string search, SearchType searchType, int limit = 5)
        {
            return Task.FromResult(Enumerable.Empty<SearchResult>());
        }

        public void SetActiveDevice(object deiviceId)
        {
            throw new NotImplementedException();
        }

        public Task SetActiveDeviceAsync(object deviceId)
        {
            throw new NotImplementedException();
        }

        public void SetPosition(int position)
        {
            this.app.PlayerPosition = position;
        }

        public Task SetPositionAsync(int position)
        {
            this.app.PlayerPosition = position;
            return Task.CompletedTask;
        }

        public void SetVolume(int volume)
        {
            this.app.SoundVolume = volume;
        }

        public void Stop()
        {
            this.app.Stop();
        }

        public Task StopAsync()
        {
            this.app.Stop();
            return Task.CompletedTask;
        }

        public void Unmute()
        {
            this.app.Mute = false;
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task<Image> GetImageAsync(string lookupKey, string artworkUrl)
        {
            Image image;
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

        internal static async Task<string> GetImageUrlByInfoAsync(string album, string artist, string track)
        {
            try
            {
                using var client = new LastfmClient("b879a7b918126d4043340cdc125c1729", "ebae21e810735a742a11dd17f68d8c24");
                var response = await client.Album.GetInfoAsync(artist, album, true);
                return response.Content.Images.ExtraLarge?.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
