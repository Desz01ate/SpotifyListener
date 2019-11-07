using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.Interfaces
{
    public interface IStreamableMusic : IMusic, IChangableDevice, IDisposable
    {
    }
}
