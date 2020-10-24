using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Core.Framework.Metadata
{
    /// <summary>
    /// This interface is required for metadata of each player host module.
    /// </summary>
    public interface IPlayerMetadata
    {
        public string ModuleName { get; }

        public string VersionName { get; }
    }
}
