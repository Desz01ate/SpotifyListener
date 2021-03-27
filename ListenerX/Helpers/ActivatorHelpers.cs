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
using System.Reflection;
using System.Windows.Navigation;
using Unosquare.Swan;

namespace ListenerX.Helpers
{
    public class ActivatorHelpers
    {
        //public static IPlayerMetadata Metadata { get; private set; }

        private IReadOnlyDictionary<string, IStreamablePlayerHost> _players;
        public IReadOnlyDictionary<string, IStreamablePlayerHost> Players => _players ??= LoadPlayerHosts();
        private IStreamablePlayerHost _activePlayerModule { get; set; }

        private IReadOnlyList<IChromaEffect> _effects;
        public IReadOnlyList<IChromaEffect> Effects => _effects ??= LoadChromaPlugins();

        public static readonly ActivatorHelpers Instance = new ActivatorHelpers();

        public event EventHandler PlayerModuleChanged;
        private ActivatorHelpers()
        {

        }

        private Dictionary<string, IStreamablePlayerHost> LoadPlayerHosts()
        {
            var players = new Dictionary<string, IStreamablePlayerHost>
            {
                { "Spotify", new SpotifyPlayerHost() },
                { "Apple Music", new AppleMusicPlayerHost() }
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

        public void LoadPlayerModule(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(name);
            if (Players.TryGetValue(name, out var module))
            {
                _activePlayerModule?.StopAsync();
                _activePlayerModule = module;
                _activePlayerModule.StartAsync();
                PlayerModuleChanged?.Invoke(_activePlayerModule, null);
            }
        }

        public IStreamablePlayerHost GetDefaultPlayerHost()
        {
            var playerName = Properties.Settings.Default.ActiveModule;
            LoadPlayerModule(playerName);
            return _activePlayerModule;
        }
    }
}
