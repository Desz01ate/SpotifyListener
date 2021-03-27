using Listener.Core.Framework.Events;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;


namespace Listener.Core.Framework.Players
{
    public interface IPlayerHost : INotifyPropertyChanged, IPlayerModule, IDisposable
    {
        string Track { get; }
        string Album { get; }
        string Artist { get; }
        string Genre { get; }
        string Type { get; }
        string Url { get; }
        string Lyrics { get; }
        int Position_ms { get; }
        int Duration_ms { get; }
        string CurrentTime { get; }
        string TimeLeft { get; }
        int Volume { get; }
        System.Drawing.Image AlbumArtwork { get; }

        bool IsPlaying { get; }

        bool IsMute { get; }

        double CalculatedPosition { get; }

        event TrackChangedEventHandler TrackChanged;
        event TrackProgressionChangedEventHandler TrackDurationChanged;
        event TrackPlayStateChangedEventHandler TrackPlayStateChanged;

        void Get();
        Task GetAsync();
        void PlayPause();
        Task PlayPauseAsync();
        void Stop();
        Task StopAsync();
        void Next();
        Task NextAsync();
        void Previous();
        Task PreviousAsync();
        void SetPosition(int position);
        Task SetPositionAsync(int position);
        void Mute();
        void Unmute();
        void SetVolume(int volume);
    }
}
