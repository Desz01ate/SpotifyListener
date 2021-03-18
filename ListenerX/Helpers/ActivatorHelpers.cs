using Listener.Core.Framework.Metadata;
using Listener.Core.Framework.Players;
using Listener.Core.Framework.Plugins;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace ListenerX.Helpers
{
    public static class ActivatorHelpers
    {
        public static IPlayerMetadata Metadata { get; private set; }

        private static IReadOnlyList<IChromaEffect> _effects;
        public static IReadOnlyList<IChromaEffect> Effects = _effects ??= LoadChromaPlugins();

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

        private static List<IChromaEffect> LoadChromaPlugins()
        {
            var effects = new List<IChromaEffect>();
            var type = typeof(IChromaEffect);
            var assembly = type.Assembly;
            effects.AddRange(AssemblyHelpers.LoadInstances<IChromaEffect>(assembly));

            var pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            var files = Directory.EnumerateFiles(pluginDir, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var plugin = AssemblyHelpers.LoadInstance<IChromaEffect>(file);
                if (plugin == null)
                    continue;

                Console.WriteLine($" [Plugin][{DateTime.Now}] {Path.GetFileName(file)} loaded.");
                effects.Add(plugin);
            }

            return effects;
        }
    }
}
