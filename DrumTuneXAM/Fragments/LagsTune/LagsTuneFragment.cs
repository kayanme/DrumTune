using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.OS;
using Android.Views;
using Fragments.StandartTune;
using DrumTuneXAM;
namespace Fragments
{
    public class LagsTuneFragment:Fragment
    {
        private LagsTuneController _controller;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var t = new LagsTuneView(inflater.Context) { Controller = _controller};
            return t;
        }
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _controller = new LagsTuneController() { LagCount = 6};

        }       

        public override void OnDestroy()
        {
            _controller.Dispose();
            base.OnDestroy();
        }
    }
}
