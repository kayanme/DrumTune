using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundAnalysis
{
    public interface IQueueStream<T> :IDisposable where T : struct
    {
        void PutBytes(T[] buffer, int length);
        T[] GetBlock(int count);
    }
}
