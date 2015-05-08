using FuorieTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundAnalysis.Streams
{
    public sealed class BlockPickStream
    {
        private IQueueStream<short> _inStream;
        int _bandWidth;
        public double _decay;
        int _maxSamples;
		public double _attack;

        public BlockPickStream(IQueueStream<short> inStream,int bandWidth,double attack,double decay,int maxSamples)
        {
            _inStream = inStream;
            _bandWidth = bandWidth;
            _decay = decay;
            _maxSamples = maxSamples;
            _attack = attack;
            _block = new short[0];
        }

        private double _lastLevel;
		public double LastLevel{get{return _lastLevel;}}
		public double LastAttack{ get;private set;}
        private bool _isBlockRecording;

        private short[] _block;

        private LinkedList<short[]> _preparedBlocks = new LinkedList<short[]>();

        public bool ProcessAndShowBlockReadyness()
        {
            var block = _inStream.GetBlock(_bandWidth);
            var level = SpectralAnalyzer.AverageLevelAnalysis(block);

			if (_lastLevel != 0)
				LastAttack = level / _lastLevel;
			
			if (level>_decay && LastAttack > _attack)
            {
				_lastLevel = level;
                if (_isBlockRecording)
                {
                    lock (_preparedBlocks)
                    _preparedBlocks.AddLast(_block);
                    _block = block;
                    return true;
                }
                else
                {
                    _block = block;
                    _isBlockRecording = true;
                    return false;
                }
            }

            _lastLevel = level;
            if (_isBlockRecording)
            {
                Array.Resize(ref _block, _block.Length + block.Length);
                Array.Copy(block, 0, _block, _block.Length - block.Length, block.Length);
            }

            if (_block.Length>_maxSamples)
            {               
                _isBlockRecording = false;
                lock (_preparedBlocks)
                    _preparedBlocks.AddLast(_block);
				_block = new short[0];
                return true;                
            }

			if (_isBlockRecording && level < _decay)
            {
                _isBlockRecording = false;

                lock (_preparedBlocks)
                    _preparedBlocks.AddLast(_block);
				_block = new short[0];
                return true;                
            }
            return false;
        }


        public short[] GetBlock()
        {
             lock (_preparedBlocks)
                 if (_preparedBlocks.Any())
                 {
                     var t = _preparedBlocks.First();
                     _preparedBlocks.RemoveFirst();
                     return t;
                 }
                 else
                 {
                     return null;
                 }
        }
    }
}
