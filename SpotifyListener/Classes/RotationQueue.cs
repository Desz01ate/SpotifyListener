using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyListener.Classes
{
    /// <summary>
    /// Fixed-size queue which automatically re-arrange element index when accessed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RotationQueue<T> : IEnumerable<T>
    {
        private readonly ConcurrentQueue<T> q;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="limitSize"></param>
        public RotationQueue(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            //limit = source.Count();
            q = new ConcurrentQueue<T>(source);
        }
        public T GetFirstItem()
        {
            if (q.TryDequeue(out var result))
            {
                q.Enqueue(result);
                return result;
            }
            return default;
        }
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
