using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.Interfaces
{
    interface IMusic 
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
        event EventHandler OnTrackChanging;
        event EventHandler OnTrackChanged;
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
    }
}
