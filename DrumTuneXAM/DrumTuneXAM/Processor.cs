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
using SoundLibrary.SoundAnalysis;
using SoundAnalysis.Streams;
using System.Threading.Tasks;

namespace DrumTuneXAM
{
    internal class Processor:IDisposable
    {
        private int _rate;
        private BlockPickStream _block;
        private bool _work;
        private Task _process;
        public Processor(int rate, BlockPickStream blocks)
        {
            _rate = rate;
            _block = blocks;
        }

        private void Process()
        {           
            var size = 8192 * 4;         
            var analyzer = new BlockAnalyzer(size, _rate);
            do
            {
                if (_block.ProcessAndShowBlockReadyness())
                {
                    var amp = _block.GetBlock();
                    var analysis = analyzer.AnalyzeBlock(amp);                                    
                    ResultReady(analysis);
                }              
            } while (_work);
        }
        public void Start()
        {
            _work = true;
            _process = new Task(Process);
            _process.Start();
        }
        public void Stop()
        {
            _work = false;
            _process.Wait();
            _process.Dispose();
        }

        public event Action<BlockInfo> ResultReady = (a) => { };

        public void Dispose()
        {
            Stop();           
        }
    }
}