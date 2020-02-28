using NListener.Core.Enum;
using NListener.Core.Foundation.Struct;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace NListener.Core.Interface
{
    public interface IMusicTracker
    {
        public delegate void TrackChangedEventArgs(IMusicTracker playbackContext);
        public delegate void TrackProgressionChangeEventArgs(IMusicTracker playbackContext);
        string Track { get; }
        string Album { get; }
        string Artist { get; }
        string Genre { get; }
        string Type { get; }
        string URL { get; }
        int Position_ms { get; }
        int Duration_ms { get; }
        int Volume { get; }
        Image AlbumArtwork { get; }
        bool IsPlaying { get; }
        bool IsShuffle { get; }
        bool IsRepeat { get; }
        bool IsMute { get; }
        double CalculatedPosition { get; }
        DrawingColor Color { get; }

        event TrackChangedEventArgs OnTrackChanged;
        event TrackProgressionChangeEventArgs OnTrackDurationChanged;
        event EventHandler OnDeviceChanged;
        void Get(ColorRenderingMode renderingMode);
        Task GetAsync(ColorRenderingMode renderingMode);
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
