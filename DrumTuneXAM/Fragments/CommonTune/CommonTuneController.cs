using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DrumTuneXAM;
using Fragments.Annotations;
using Fragments.StandartTune;
using SoundLibrary.SoundAnalysis;

namespace Fragments.CommonTune
{
    internal class CommonTuneController:INotifyPropertyChanged,IDisposable
    {
        private int _lagCount;
        private readonly Listener _listener;
        private readonly Processor _processor;
        private double? _desiredFrequency;
        private double? _frequency;

        public CommonTuneController()
        {
            _listener = new Listener();
            _processor = new Processor(_listener.Rate, _listener.BlockStream);
            _processor.ResultReady += ProcessTuneResult;                     
        }

        public double? DesiredFrequency
        {
            get { return _desiredFrequency; }
            set
            {
                if (value.Equals(_desiredFrequency)) return;
                _desiredFrequency = value;
                OnPropertyChanged();
                OnPropertyChanged("FrequencyDiff");
            }
        }

        public double? Frequency
        {
            get { return _frequency; }
            private set
            {
                if (value.Equals(_frequency)) return;
                _frequency = value;
                OnPropertyChanged();
                OnPropertyChanged("FrequencyDiff");
            }
        }

        public void ResetTune()
        {
            Frequency = null;
        }

        public double? FrequencyDiff { get { return Frequency - DesiredFrequency; }}

        private void ProcessTuneResult(BlockInfo block)
        {
            Frequency = block.MainFrequency;
        }


        public void StartTune()
        {
            _listener.Start();
            _processor.Start();
        }

        public void StopTune()
        {
            _processor.Stop();
            _listener.Stop();           
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