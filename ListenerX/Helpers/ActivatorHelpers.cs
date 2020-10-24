using Listener.Core.Framework.Metadata;
using Listener.Core.Framework.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Helpers
{
    public static class ActivatorHelpers
    {
        public static IPlayerMetadata Metadata { get; private set; }
        public static IStreamablePlayerHost LoadPlayerHost<T>(this MainWindow mainWindow) where T : IStreamablePlayerHost
        {
            if (mainWindow == null)
                throw new ArgumentNullException(nameof(mainWindow));
            var type = typeof(T);
            var moduleLocation = type.Assembly.Location;
            Metadata = AssemblyHelpers.LoadInstance<IPlayerMetadata>(moduleLocation);
            var instance = Activator.CreateInstance(type, new object[] { mainWindow.Width, mainWindow.Height }) as IStreamablePlayerHost;
            return instance;
        }
    }
}
