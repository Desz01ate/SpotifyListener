using Listener.Core.Framework.Metadata;
using Listener.Core.Framework.Players;
using Listener.Core.Framework.Plugins;
using Listener.Plugin.Razer.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Helpers
{
    public static class ActivatorHelpers
    {
        public static IPlayerMetadata Metadata { get; private set; }
        public static IReadOnlyList<IRazerEffect> Effects { get; private set; }

        public static IStreamablePlayerHost LoadPlayerHost<T>() where T : IStreamablePlayerHost
        {
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
                if (plugin == null) 
                    continue;

                Console.WriteLine($" [Plugin][{DateTime.Now}] {Path.GetFileName(file)} loaded.");
                yield return plugin;
            }
        }

        public static void LoadRazerChromaPlugins(this MainWindow mainWindow)
        {
            var effects = new List<IRazerEffect>();
            var type = typeof(IRazerEffect);
            var assembly = type.Assembly;
            effects.AddRange(AssemblyHelpers.LoadInstances<IRazerEffect>(assembly));

            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            var files = Directory.EnumerateFiles(pluginDir, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var plugin = AssemblyHelpers.LoadInstance<IRazerEffect>(file);
                if (plugin == null)
                    continue;

                Console.WriteLine($" [Plugin][{DateTime.Now}] {Path.GetFileName(file)} loaded.");
                effects.Add(plugin);
            }

            Effects = effects;
        }
    }
}
