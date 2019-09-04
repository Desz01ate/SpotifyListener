using SpotifyListener.Interfaces;

namespace SpotifyListener.Delegations
{
    public delegate void TrackChangedEventArgs(IMusic playbackContext);
    public delegate void TrackProgressionChangeEventArgs(IMusic playbackContext);
}
