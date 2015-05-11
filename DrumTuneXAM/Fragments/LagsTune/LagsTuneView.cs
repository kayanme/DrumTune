using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Fragments.StandartTune
{
    public sealed class LagsTuneView:View
    {
      

        public LagsTuneView(Context context) : base(context)
        {           
        }

        public LagsTuneView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
           
        }


        internal LagsTuneController Controller
        {
            get { return _controller; }
            set
            {
                if (value != null) value.PropertyChanged -= _controller_PropertyChanged;
                   _controller = value;
                if (_controller != null) 
                   _controller.PropertyChanged += _controller_PropertyChanged;
            }
        }

        void _controller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Lags")
            {
                LagViews = Controller.Lags.Select(k => new LagView(k)).ToArray();
            }
        }


        private LagView[] LagViews;
        private LagsTuneController _controller;

        private Tuple<int, int> FindCoordinate(int position,int width,int height)
        {
            var circleRad = Math.Min(width, height)/3;
            var centerX = width/2;
            var centerY = height/2;
            var angle = Math.PI * 2 * position / Controller.LagCount;

            var coordX = Math.Sin(angle)*circleRad + centerX;
            var coordY = Math.Cos(angle) * circleRad + centerY;
            return Tuple.Create((int)coordX, (int)coordY);
        }
             
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            var w = MeasureSpec.GetSize(widthMeasureSpec);
            var h = MeasureSpec.GetSize(heightMeasureSpec);
            var wm = MeasureSpec.GetMode(widthMeasureSpec);
            var hm = MeasureSpec.GetMode(heightMeasureSpec);
            
            SetMeasuredDimension(Math.Min(w, h), Math.Min(w, h));
        }

        private void DrawLags(Canvas canvas)
        {
            for (int i = 0; i < Controller.Lags.Length; i++)
            {
                var c = FindCoordinate(i, canvas.Width, canvas.Height);
                LagViews[i].Draw(canvas, c.Item1, c.Item2);
            }
        }
       

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);         
            DrawLags(canvas);
         
        }

   
    }
}