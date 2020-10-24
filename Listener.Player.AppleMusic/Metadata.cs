using Listener.Core.Framework.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Player.AppleMusic
{
    public sealed class Metadata : IPlayerMetadata
    {
        public string ModuleName => "Apple Music";

        public string VersionName => "v1.0.0";
    }
}
