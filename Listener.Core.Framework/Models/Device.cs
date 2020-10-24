using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Core.Framework.Models
{
    public class Device
    {
        public static readonly Device Default = new Device(null, false, false, null, null, -1);
        public Device(string id, bool isActive, bool isRestricted, string name, string type, int volumePercent)
        {
            Id = id;
            IsActive = isActive;
            IsRestricted = isRestricted;
            Name = name;
            Type = type;
            VolumePercent = volumePercent;
        }
        public string Id { get; }

        public bool IsActive { get; }

        public bool IsRestricted { get; }

        public string Name { get; }

        public string Type { get; }

        public int VolumePercent { get; }
    }
}
