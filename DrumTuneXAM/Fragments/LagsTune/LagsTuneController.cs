using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DrumTuneXAM;
using Fragments.Annotations;
using SoundLibrary.SoundAnalysis;

namespace Fragments.StandartTune
{
    internal class LagsTuneController:INotifyPropertyChanged,IDisposable
    {
        private int _lagCount;
        private readonly Listener _listener;
        private readonly Processor _processor;
        private LagInfo[] _lags;
        private readonly LinkedList<int> _lagsForTuning = new LinkedList<int>();
     
        public LagsTuneController()
        {
            _listener = new Listener();
            _processor = new Processor(_listener.Rate, _listener.BlockStream);
            _processor.ResultReady += ProcessTuneResult;                     
        }

        public double? DesiredFrequency
        {
            get { return _lags[0].DesiredFrequency; }
            set
            {
                foreach (var lagInfo in _lags)
                {
                    lagInfo.DesiredFrequency = value;
                }
            }
        }
        public void StartFullTune()
        {
            _lagsForTuning.Clear();
            for (int i = 0; i < _lags.Length; i++)
            {
                var lagInfo = _lags[i];
                if (lagInfo.Frequency == null)
                {
                    _lagsForTuning.AddLast(i);
                }
            }
            TuneNextLag(null);
        }

        public void StopFullTune()
        {
            if (_processor.Working)
                _processor.Stop();
            if (_listener.Working)
                _listener.Stop();            
        }

        public void ResetLag(int position)
        {
            Lags[position].Frequency = null;
        }

        public void ResetAllLags()
        {
            foreach (var lagInfo in _lags)
            {
                lagInfo.Frequency = null;
            }
        }

        public void TuneLag(int position)
        {
            _lagsForTuning.Clear();
            _lagsForTuning.AddFirst(position);
            TuneNextLag(null);
          
        }

        private void TuneNextLag(LagInfo recentlyTuned)
        {
            if (recentlyTuned != null)
            {
                recentlyTuned.IsRecording = false;
                _lagsForTuning.RemoveFirst();
            }
            else
            {
                _listener.Start();
                _processor.Start();
            }
            var next = _lagsForTuning.First;
            if (next == null)
            {
                if (_processor.Working)
                  _processor.Stop();
                if (_listener.Working)
                  _listener.Stop();
            }
            else
            {
                _lags[next.Value].IsRecording = true;
            }
        }
       
        private void ProcessTuneResult(BlockInfo block)
        {
            var tunedLag = _lags.FirstOrDefault(k => k.IsRecording);
            if (tunedLag != null)
            {
                tunedLag.Frequency = block.MainFrequency;
                TuneNextLag(tunedLag);
            }
        }

        public LagInfo[] Lags
        {
            get { return _lags; }
            private set
            {
                if (Equals(value, _lags)) return;
                _lags = value;
                OnPropertyChanged();
            }
        }

        public int LagCount
        {
            get { return _lagCount; }
            set
            {
                _lagCount = value;
                Lags = Enumerable.Range(0, value).Select(k => new LagInfo()).ToArray();
                OnPropertyChanged();        
            }
        }



        public void Dispose()
        {
            _processor.ResultReady -= ProcessTuneResult;    
            _processor.Dispose();
            _listener.Dispose();
        
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}