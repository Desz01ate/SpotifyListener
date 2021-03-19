using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ListenerX.Helpers
{
    public static class OpenerHelpers
    {
        public static void Open(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true }); // Works ok on windows
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", path);  // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", path); // Not tested
            }
            else
            {
                throw new NotSupportedException(RuntimeInformation.OSDescription);
            }
        }
    }
}
