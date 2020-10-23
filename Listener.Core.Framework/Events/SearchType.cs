using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Core.Framework.Events
{
    [Flags]
    public enum SearchType
    {
        Artist = 1,
        Album = 2,
        Track = 4,
        Playlist = 8,
        All = 16
    }
}
