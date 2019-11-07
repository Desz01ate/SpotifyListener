using SpotifyListener.Delegations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.Interfaces
{
    public interface IMusic : IChromaRender
    {
        string Track { get; }
        string Album { get; }
        string Artist { get; }
        string URL { get; }
        int Position_ms { get; }
        int Duration_ms { get; }
        int Volume { get; }
        Image AlbumArtwork { get; }
        bool IsPlaying { get; }
        bool IsMute { get; }
        double CalculatedPosition { get; }
        event TrackChangedEventArgs OnTrackChanged;
        event TrackProgressionChangeEventArgs OnTrackDurationChanged;
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
    }
}
