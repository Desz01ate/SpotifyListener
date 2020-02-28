using SpotifyListener.Enums;
using SpotifyListener.Interfaces;

namespace SpotifyListener.Delegations
{
    public delegate void TrackChangedEventHandler(IMusic playbackContext);
    public delegate void TrackProgressionChangedEventHandler(IMusic playbackContext);
    public delegate void TrackPlayStateChangedEventHandler(PlayState state);
}
