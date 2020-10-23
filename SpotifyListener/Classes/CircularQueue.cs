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
    public class CircularQueue<T>
    {
        private readonly ConcurrentQueue<T> q;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="limitSize"></param>
        public CircularQueue(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            q = new ConcurrentQueue<T>(source);
        }
        public T Dequeue()
        {
            if (q.TryDequeue(out var result))
            {
                q.Enqueue(result);
                return result;
            }
            return default;
        }

        public T Peek()
        {
            if(q.TryPeek(out var result))
            {
                return result;
            }
            return default;
        }
    }
}
