using Listener.Core.Framework.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Core.Framework.Plugins
{
    public interface IListenerPlugin
    {
        void OnTrackChanged(IPlayerHost player);
    }
}
