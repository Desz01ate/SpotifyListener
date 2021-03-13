using Listener.Plugin.ChromaEffect.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Plugin.ChromaEffect.Interfaces
{
    /// <summary>
    /// A virtual LED grid that represent the physical setup.
    /// </summary>
    public interface IVirtualLedGrid : IEnumerable<IVirtualKey>
    {
        /// <summary>
        /// Indexer for accesing color of the virtual grid using specific index.
        /// </summary>
        /// <param name="column">Specific column.</param>
        /// <param name="row">Specific row.</param>
        /// <returns><seealso cref="Color"/></returns>
        Color this[int column, int row] { get; set; }

        /// <summary>
        /// Row count for virtual grid.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Column count for virtual grid.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Set a single color to all keys.
        /// </summary>
        /// <param name="color"></param>
        void Set(Color color);

        /// <summary>
        /// Set per-key color.
        /// </summary>
        /// <param name="colors"></param>
        void Set(Color[][] colors);
    }
}
