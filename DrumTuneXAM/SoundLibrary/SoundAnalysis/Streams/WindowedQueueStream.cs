using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundAnalysis
{
    public class WindowedQueueStream<T>:IQueueStream<T>,IDisposable where T:struct
    {
        private int _windowShift;
        public WindowedQueueStream(int windowShift)
        {
            Working = true;
            _windowShift = windowShift;
        }

        private ManualResetEventSlim _waitLock = new ManualResetEventSlim();

        public bool Working { get; private set; }


        private LinkedList<T[]> _buffers = new LinkedList<T[]>();
        public int Count { get; private set; }
        public void PutBytes(T[] buffer, int length)
        {
            lock (_buffers)
            {
                var b = new T[length];
                Array.Copy(buffer, b, length);
                _buffers.AddLast(b);
                Count += b.Length;
                _waitLock.Set();
            }
        }
     
        public T[] GetBlock(int count)
        {
          
            while (Count < count)
            {
                _waitLock.Reset();
                _waitLock.Wait();
            }
            lock (_buffers)
            {
                var leftCount = count;
                var t = new List<T[]>();
                while (leftCount != 0)
                {
                    var target = _buffers.First();
                    _buffers.RemoveFirst();

                    if (target.Length > leftCount)
                    {
                        var remain = new T[leftCount];
                        Array.Copy(target, remain, leftCount);
                        Array.Copy(target, leftCount, target, 0, target.Length - leftCount);
                        Array.Resize(ref target, target.Length - leftCount);
                        _buffers.AddFirst(target);
                        leftCount -= remain.Length;
                        t.Add(remain);
                    }
                    else
                    {
                        leftCount -= target.Length;
                        t.Add(target);
                    }

                }
              
             
                var fullTarget = t.SelectMany(k => k).ToArray();
                var leftOver = new T[fullTarget.Length - _windowShift];
                Array.Copy(fullTarget, _windowShift, leftOver, 0, fullTarget.Length - _windowShift);
				_buffers.AddFirst (leftOver);
				Count -=  _windowShift;
                return fullTarget;
            }
        }

        public void Dispose()
        {
            Working = false;
        }
    }
}
