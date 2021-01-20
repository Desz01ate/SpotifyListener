using Listener.Core.Framework.Players;
using Listener.Core.Framework.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Plugin.Log
{
    public class Logger : IListenerPlugin
    {
        const string DIR = "logs";
        const string PATH = "log.txt";
        public Logger()
        {
            Directory.CreateDirectory(DIR);
        }
        public void OnTrackChanged(IPlayerHost player)
        {
            using (var sw = File.AppendText(Path.Combine(DIR, PATH)))
            {
                sw.WriteLine($"[{DateTime.Now}] {player.Track}\n");
            }
        }
    }
}
