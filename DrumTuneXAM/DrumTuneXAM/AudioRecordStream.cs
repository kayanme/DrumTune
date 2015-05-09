using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SoundAnalysis;
using Android.Media;

namespace DrumTuneXAM
{
    internal class AudioRecordStream:IQueueStream<short>
    {
        private AudioRecord _r;
        public AudioRecordStream(AudioRecord r )
        {
            _r = r;
        }

        public void PutBytes(short[] buffer, int length)
        {
           
        }

        public short[] GetBlock(int count)
        {
           
            var e = new short[count];
            if (_r.RecordingState == RecordState.Stopped)
                return e;
            _r.Read(e, 0,count);
            return e;
        }

        public void Dispose()
        {
            _r.Dispose();
        }
    }
}