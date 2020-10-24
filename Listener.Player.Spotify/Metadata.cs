using Listener.Core.Framework.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Player.Spotify
{
    public sealed class Metadata : IPlayerMetadata
    {
        public string ModuleName => "Spotify";

        public string VersionName => "v1.0.0";
    }
}
