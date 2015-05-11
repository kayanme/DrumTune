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
using SoundAnalysis.Streams;
using SoundAnalysis;
using Android.Media;
using System.Threading;
using System.Threading.Tasks;

namespace DrumTuneXAM
{
    public sealed class Listener : IDisposable
    {
        public BlockPickStream BlockStream { get; private set; }
       
        private AudioRecord _soundStream;
        public bool Working { get; private set; }
        public int Rate { get; private set; }
        public Listener()
        {
            Rate = GetRate();
          
            _soundStream = new AudioRecord(AudioSource.Mic, Rate, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit,
                AudioRecord.GetMinBufferSize(Rate, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit) * 10);
         
            BlockStream = new BlockPickStream(new AudioRecordStream(_soundStream), Rate / 5, 4, 400, Rate * 3);

        }

        private int GetRate()
        {
            var rate = new int[] { 4000, 8000, 11025, 16000, 22050, 44100 }
                .Where(k => AudioRecord.GetMinBufferSize(k, ChannelIn.Mono, Android.Media.Encoding.Pcm16bit) != -2)
                .Last();
            return rate;
        }

        
      
        public void Start()
        {
            Working = true;
            _soundStream.StartRecording();
          
        }

        public void Stop()
        {
            Working = false; 
            _soundStream.Stop();               
        }

        public void Dispose()
        {
            Stop();
            _soundStream.Dispose();
        }
    }
}