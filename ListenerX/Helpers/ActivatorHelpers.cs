using Listener.Core.Framework.Metadata;
using Listener.Core.Framework.Players;
using Listener.Core.Framework.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
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
            var assembly = type.Assembly;
            Metadata = AssemblyHelpers.LoadInstance<IPlayerMetadata>(assembly);
            var instance = Activator.CreateInstance(type) as IStreamablePlayerHost;
            return instance;
        }

        public static IEnumerable<IListenerPlugin> LoadPlugins()
        {
            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            Directory.CreateDirectory(pluginDir);
            var files = Directory.EnumerateFiles(pluginDir, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var plugin = AssemblyHelpers.LoadInstance<IListenerPlugin>(file);
                Console.WriteLine($" [Plugin][{DateTime.Now}] {Path.GetFileName(file)} loaded.");
                yield return plugin;
            }
        }
    }
}
