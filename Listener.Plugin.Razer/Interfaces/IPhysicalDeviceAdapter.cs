using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.ChromaEffect.Interfaces
{
    public interface IPhysicalDeviceAdapter : IDisposable
    {
        Task ApplyAsync(IVirtualLedGrid virtualGrid);
    }
}
