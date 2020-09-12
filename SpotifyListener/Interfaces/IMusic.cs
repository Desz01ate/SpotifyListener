using SpotifyListener.Delegations;
using SpotifyListener.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SpotifyListener.Interfaces
{
    public interface IMusic : INotifyPropertyChanged
    {
        string Track { get; }
        string Album { get; }
        string Artist { get; }
        string Genre { get; }
        string Type { get; }
        string Url { get; }
        int Position_ms { get; }
        int Duration_ms { get; }
        int Volume { get; }
        Image AlbumArtwork { get; }
        ImageSource AlbumSource { get; }
        System.Windows.Media.Brush AlbumBackgroundSource { get; }
        bool IsPlaying { get; }
        bool IsMute { get; }
        double CalculatedPosition { get; }
        event TrackChangedEventHandler OnTrackChanged;
        event TrackProgressionChangedEventHandler OnTrackDurationChanged;
        event EventHandler OnDeviceChanged;
        void Get(int albumColorMode);
        Task GetAsync(int albumColorMode);
        void PlayPause();
        Task PlayPauseAsync();
        void Stop();
        Task StopAsync();
        void Next();
        Task NextAsync();
        void Previous();
        Task PreviousAsync();
        void SetPosition(int asMillisecond);
        Task SetPositionAsync(int asMillisecond);
        void Mute();
        void Unmute();
        void SetVolume(int volume);
        void ToggleShuffle();
        void SetRepeat(RepeatState state);
    }
}
