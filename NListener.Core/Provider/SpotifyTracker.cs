using NListener.Core.Enum;
using NListener.Core.Foundation.Struct;
using NListener.Core.Interface;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NListener.Core.Provider
{
    public class SpotifyTracker : IMusicTracker, IChromaSupport
    {
        private readonly SpotifyWebAPI spotifyWebClient;
        public SpotifyTracker(SpotifyWebAPI spotifyClient)
        {
            this.spotifyWebClient = spotifyClient;
            Task.Run(async () =>
            {
                while (true)
                {
                    await GetAsync(ColorRenderingMode.Average);
                    await Task.Delay(500);
                }
            });
        }
        private RazerChromaColor _chromaColor = new RazerChromaColor();
        public RazerChromaColor ChromaColor => _chromaColor;

        public string Track { get; private set; }

        public string Album { get; private set; }

        public string Artist { get; private set; }

        public string Genre { get; private set; }

        public string Type { get; private set; }

        public string URL { get; private set; }

        public int Position_ms { get; private set; }

        public int Duration_ms { get; private set; }
        private int _PreviousVolume { get; set; }
        public int Volume { get; private set; }
        public string AlbumURL { get; private set; }
        public System.Drawing.Image AlbumArtwork { get; private set; }

        public bool IsPlaying { get; private set; }

        public bool IsMute { get; private set; }
        public bool IsShuffle { get; private set; }
        public bool IsRepeat { get; private set; }

        public double CalculatedPosition { get; private set; }
        public DrawingColor _color = new DrawingColor();
        public DrawingColor Color => _color;
        public Device? ActiveDevice { get; private set; } = new Device();//=> AvailableDevices?.Where(x => x.IsActive).FirstOrDefault();
        public List<Device> AvailableDevices { get; private set; } = new List<Device>();

        public event IMusicTracker.TrackChangedEventArgs OnTrackChanged;
        public event IMusicTracker.TrackProgressionChangeEventArgs OnTrackDurationChanged;
        public event EventHandler OnDeviceChanged;

        public void Get(ColorRenderingMode renderingMode)
        {
            GetAsync(renderingMode).RunSynchronously();
        }

        public virtual async Task GetAsync(ColorRenderingMode renderingMode)
        {
            if (spotifyWebClient == null) return;
            var currentTrack = await spotifyWebClient.GetPlayingTrackAsync();
            if (currentTrack.IsPlaying)
            {
                var devices = (await spotifyWebClient.GetDevicesAsync()).Devices;
                if (devices.Except(AvailableDevices).Any())
                {
                    AvailableDevices = devices;
                }
                var currentActiveDevice = devices.Find(x => x.IsActive);
                if (ActiveDevice?.Id != currentActiveDevice.Id)
                {
                    ActiveDevice = currentActiveDevice;
                    OnDeviceChanged(ActiveDevice, null);
                }
                this.IsShuffle = currentTrack.ShuffleState;
                this.IsRepeat = currentTrack.RepeatState == SpotifyAPI.Web.Enums.RepeatState.Context;

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
                        var artist = await spotifyWebClient.GetArtistAsync(currentTrack.Item.Artists.FirstOrDefault()?.Id);
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
                            this.AlbumURL = currentTrack.Item.Album.Images[0].Url;
                            using var client = new HttpClient();
                            var byteArray = await client.GetByteArrayAsync(AlbumURL);
                            System.Drawing.Image image = (System.Drawing.Image)(new ImageConverter()).ConvertFrom(byteArray);
                            AlbumArtwork = image;
                        }
                        else
                        {
                            AlbumArtwork = new Bitmap(1, 1);
                        }
                        var color = renderingMode == ColorRenderingMode.Average ? Foundation.Helpers.BitmapHelper.AverageColor((Bitmap)AlbumArtwork) : Foundation.Helpers.BitmapHelper.AverageColor((Bitmap)AlbumArtwork);
                        this._color = new DrawingColor(color.red, color.green, color.blue);
                        this._chromaColor = new RazerChromaColor(color.red, color.green, color.blue);
                        //await SQLiteService.Context.ListenHistories.InsertAsync(new ListenHistory(this)).ConfigureAwait(false);
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



            IsPlaying = currentTrack.IsPlaying;
            OnTrackDurationChanged(this);
        }


        public void Mute()
        {
            _PreviousVolume = this.Volume;
            this.SetVolume(0);
            IsMute = true;
        }

        public void Next()
        {
            spotifyWebClient.SkipPlaybackToNext();
        }

        public async Task NextAsync()
        {
            await spotifyWebClient.SkipPlaybackToPreviousAsync().ConfigureAwait(false);
        }

        public void PlayPause()
        {
            if (IsPlaying)
            {
                spotifyWebClient.PausePlayback(ActiveDevice.Id);
                //OnPaused(null, null);
            }
            else
            {
                spotifyWebClient.ResumePlayback(ActiveDevice.Id, "", null, "", Position_ms);
                //OnResume(null, null);
            }
            IsPlaying = !IsPlaying;
        }

        public async Task PlayPauseAsync()
        {
            if (IsPlaying)
            {
                await spotifyWebClient.PausePlaybackAsync().ConfigureAwait(false);
                //OnPaused(null, null);
            }
            else
            {
                await spotifyWebClient.ResumePlaybackAsync("", "", null, "", Position_ms).ConfigureAwait(false);
                //OnResume(null, null);
            }
            IsPlaying = !IsPlaying;
        }

        public void Previous()
        {
            spotifyWebClient.SkipPlaybackToPrevious();
        }

        public async Task PreviousAsync()
        {
            await spotifyWebClient.SkipPlaybackToPreviousAsync().ConfigureAwait(false);
        }

        public void SetPosition(int asMillisecond)
        {
            SetPositionAsync(asMillisecond).RunSynchronously();
        }

        public async Task SetPositionAsync(int asMillisecond)
        {
            await spotifyWebClient.SeekPlaybackAsync(asMillisecond, ActiveDevice.Id).ConfigureAwait(false);
        }

        public void SetRepeat(RepeatState state)
        {
            throw new NotImplementedException();
        }

        public void SetVolume(int volume)
        {
            if (volume < 0)
            {
                spotifyWebClient.SetVolume(0);
            }
            else if (volume > 100)
            {
                spotifyWebClient.SetVolume(100);
            }
            else
            {
                spotifyWebClient.SetVolume(volume);
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public void ToggleShuffle()
        {
            throw new NotImplementedException();
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
            await spotifyWebClient.ResumePlaybackAsync(this.ActiveDevice.Id, url, null, 0, 0);
        }
    }
}
