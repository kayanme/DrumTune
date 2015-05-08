using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundAnalysis
{
    public class StraightQueueStream<T>:IQueueStream<T>,IDisposable where T:struct
    {
        public StraightQueueStream()
        {
            Working = true;
        }
     
        private ManualResetEventSlim _waitLock = new ManualResetEventSlim();

        public bool Working { get; private set; }


        private LinkedList<T[]> _buffers = new LinkedList<T[]>();
        public int Count { get; private set; }
        public void PutBytes(T[] buffer, int length)
        {
			
            lock (_buffers)
            {
				if (Working) {
					var b = new T[length];
					Array.Copy (buffer, b, length);
					_buffers.AddLast (b);
					Count += b.Length;
					lock (_waitLock)
						if (Working)
					      _waitLock.Set ();
				}
            }
        }
     
        public T[] GetBlock(int count)
        {
            while (Count < count)
            {
				lock (_waitLock) {
					if (!Working)
						return new T[0];
				}
				_waitLock.Reset ();
				_waitLock.Wait ();

            }
            lock (_buffers)
            {

                var t = new List<T[]>();
                int leftCount = count;
                while (leftCount != 0)
                {
                    var target = _buffers.First();
                    _buffers.RemoveFirst();
                   
					if (target.Length > leftCount) {
						var remain = new T[leftCount];
						Array.Copy (target, remain, leftCount);
						Array.Copy (target, leftCount, target, 0, target.Length - leftCount);
						Array.Resize (ref target, target.Length - leftCount);
						_buffers.AddFirst (target);
						leftCount -= remain.Length;
						t.Add (remain);
					} else {
						leftCount -= target.Length;                 
						t.Add(target);
					}
                   
                }
                Count -= count;
                return t.SelectMany(k => k).ToArray();
            }
        }

        public void Dispose()
        {
			_waitLock.Wait ();
			lock (_waitLock) {
				Working = false;
				_waitLock.Set ();
				_waitLock.Dispose ();
			}
        }
    }
}
