using Listener.Core.Framework.Metadata;
using Listener.Core.Framework.Players;
using Listener.Core.Framework.Plugins;
using Listener.Player.AppleMusic;
using Listener.Player.Spotify;
using Listener.Plugin.ChromaEffect.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Windows.Navigation;
using Unosquare.Swan;

namespace ListenerX.Helpers
{
    public class ModuleActivator
    {
        private IReadOnlyDictionary<string, Type> _players;
        public IReadOnlyDictionary<string, Type> Players => _players ??= LoadPlayerHosts();

        private IReadOnlyList<IChromaEffect> _effects;
        public IReadOnlyList<IChromaEffect> Effects => _effects ??= LoadChromaPlugins();

        private Dictionary<string, Type> LoadPlayerHosts()
        {
            var players = new Dictionary<string, Type>
            {

                { "Spotify", typeof(SpotifyPlayerHost) },
                { "Apple Music", typeof(AppleMusicPlayerHost) }
            };
            return players;
        }

        public IEnumerable<IListenerPlugin> LoadPlugins()
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

        private List<IChromaEffect> LoadChromaPlugins()
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

        private IStreamablePlayerHost LoadPlayerModule(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(name);

            if (Players.TryGetValue(name, out var module))
            {
                var activePlayerModule = (IStreamablePlayerHost)Activator.CreateInstance(module);
                activePlayerModule.StartAsync();
                return activePlayerModule;
            }

            return null;
        }

        public IStreamablePlayerHost GetDefaultPlayerHost(string hostName)
        {
            return LoadPlayerModule(hostName);
        }
    }
}
