using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.Interfaces
{
    public interface IChangableDevice
    {
        IList<Device> AvailableDevices { get; }
        Device ActiveDevice { get; }
        void SetActiveDevice(object deiviceId);
        Task SetActiveDeviceAsync(object deviceId);
    }
}
