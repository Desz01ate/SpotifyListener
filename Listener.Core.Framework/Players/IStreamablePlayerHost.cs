using Listener.Core.Framework.Events;
using Listener.Core.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Core.Framework.Players
{
    public interface IStreamablePlayerHost : IPlayerHost
    {
        Task PlayAsync(string url);

        Task PlayTrackAsync(string url);

        Task<IEnumerable<SearchResult>> SearchAsync(string search, SearchType searchType, int limit = 5);

        void Play(string url);

        IReadOnlyList<Device> AvailableDevices { get; }

        Device ActiveDevice { get; }

        void SetActiveDevice(object deiviceId);

        Task SetActiveDeviceAsync(object deviceId);

        event ActiveDeviceChangedEventHandler OnDeviceChanged;
    }
}
