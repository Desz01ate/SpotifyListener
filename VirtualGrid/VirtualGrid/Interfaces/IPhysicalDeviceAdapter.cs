using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualGrid.Interfaces
{
    /// <summary>
    /// An interface to represent physical device tied to the <see cref="IVirtualLedGrid"/>
    /// </summary>
    public interface IPhysicalDeviceAdapter : IDisposable
    {
        /// <summary>
        /// Apply the effects from <see cref="IVirtualLedGrid"/> to physical devices.
        /// </summary>
        /// <param name="virtualGrid">Instance of <see cref="IVirtualLedGrid"/></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task ApplyAsync(IVirtualLedGrid virtualGrid, CancellationToken cancellationToken = default);
    }
}
