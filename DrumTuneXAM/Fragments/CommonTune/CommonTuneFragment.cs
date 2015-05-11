using Android.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.OS;
using Android.Views;
using Fragments.CommonTune;
using Fragments.StandartTune;
using DrumTuneXAM;

namespace Fragments
{
    public class CommonTuneFragment:Fragment
    {
        private CommonTuneController _controller;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var t = new CommonTuneView(inflater.Context) { Controller = _controller};
            return t;
        }
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _controller = new CommonTuneController();

        }

       

        public override void OnDestroy()
        {
            _controller.Dispose();
            base.OnDestroy();
        }
    }
}
