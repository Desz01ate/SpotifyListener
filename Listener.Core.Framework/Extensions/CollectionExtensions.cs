using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listener.Core.Framework.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Cast ICollection to IReadonlyCollection only convert if necessary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> src)
        {
            if (src == null)
                throw new ArgumentException(nameof(src));
            return src as IReadOnlyCollection<T> ?? new ReadonlyCollectionAdapter<T>(src);
        }

        sealed class ReadonlyCollectionAdapter<T> : IReadOnlyCollection<T>
        {
            private readonly ICollection<T> _internalSource;
            public int Count => _internalSource.Count;

            public IEnumerator<T> GetEnumerator() => _internalSource.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public ReadonlyCollectionAdapter(ICollection<T> src)
            {
                this._internalSource = src;
            }
        }
    }
}
