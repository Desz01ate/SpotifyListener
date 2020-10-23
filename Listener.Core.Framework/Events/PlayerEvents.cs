using Listener.Core.Framework.Models;
using Listener.Core.Framework.Players;

namespace Listener.Core.Framework.Events
{
    public delegate void TrackChangedEventHandler(IPlayerHost playbackContext);
    public delegate void TrackProgressionChangedEventHandler(IPlayerHost playbackContext);
    public delegate void TrackPlayStateChangedEventHandler(PlayState state);
    public delegate void ActiveDeviceChangedEventHandler(Device device);
}
