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
using System.ComponentModel;

namespace Fragments.StandartTune
{
    public class LagInfo:INotifyPropertyChanged
    {
        private double? _frequency;
        private bool _isRecording;
        private double? _desiredFrequency;

        public double? Frequency
        {
            get { return _frequency; }
            set {
                if (_frequency == value) return;
                _frequency = value;
                RaiseChange("Frequency");
                RaiseChange("FrequencyDiff");
            }
        }

        public double? DesiredFrequency
        {
            get { return _desiredFrequency; }
            set {
                if (_desiredFrequency == value) return;
                _desiredFrequency = value;
                RaiseChange("DesiredFrequency");
                RaiseChange("FrequencyDiff");
            }
        }

        public double? FrequencyDiff
        {
            get { return Frequency - DesiredFrequency; }
        }

        public bool IsRecording
        {
            get { return _isRecording; }
            set { if (_isRecording == value) return; 
                _isRecording = value; 
                RaiseChange("IsRecording"); }
        }

        public void RaiseChange(string name)
        {
            PropertyChanged(this,new PropertyChangedEventArgs(name) );
        }
        public event PropertyChangedEventHandler PropertyChanged = (o, e) => { };
    }
}