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
using FuorieTest;

namespace SoundLibrary.SoundAnalysis
{
    public struct BlockInfo
    {
        private double _mainFrequency;
        private FrequencyChart _chart;
        public double MainFrequency { get { return _mainFrequency; } }
        public FrequencyChart Chart { get{return _chart;} }

        public BlockInfo(double mainFrequency)
        {
            _mainFrequency = mainFrequency;
            _chart = null;
        }

        public BlockInfo(double mainFrequency,FrequencyChart chart):this(mainFrequency)
        {
            _chart = chart;
        }
    }
}