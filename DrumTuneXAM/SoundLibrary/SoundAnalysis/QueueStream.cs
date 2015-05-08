using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FuorieTest
{
    public class QueueStream<T>:IDisposable where T:struct
    {
        public QueueStream()
        {
            Working = true;
        }

        private Queue<T[]> _buffers = new Queue<T[]>();
        public int Count { get; private set; }
        public void PutBytes(T[] buffer, int length)
        {
            lock (_buffers)
            {
                var b = new T[length];
                Array.Copy(buffer, b, length);
                _buffers.Enqueue(buffer);
                Count += buffer.Length;
                _waitLock.Set();
            }
        }
        private ManualResetEventSlim _waitLock = new ManualResetEventSlim();

        public bool Working { get; private set; }

        public T[] GetBlock(int count)
        {

            while (Count < count)
            {
                _waitLock.Reset();
                _waitLock.Wait();
            }
            lock (_buffers)
            {

                var t = new List<T[]>();
                while (count != 0)
                {
                    var b = _buffers.Peek();
                    T[] target;
                    if (b.Length < count)
                    {
                        target = _buffers.Dequeue();
                    }
                    else
                    {
                        target = new T[count];
                        Array.Copy(b, target, count);
                        Array.Copy(b, count , b, 0, b.Length - count);
                        Array.Resize(ref b, b.Length - count);
                    }
                    count -= target.Length;
                    Count -= target.Length;
                    t.Add(target);
                }
                Count -= count;
                return t.SelectMany(k => k).ToArray();
            }
        }

        public void Dispose()
        {
            Working = false;
        }
    }
}
