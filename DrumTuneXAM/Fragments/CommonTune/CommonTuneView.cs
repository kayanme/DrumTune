using System;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Fragments.StandartTune;

namespace Fragments.CommonTune
{
    public sealed class CommonTuneView:View
    {
      

        public CommonTuneView(Context context) : base(context)
        {           
        }

        public CommonTuneView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
           
        }


        internal CommonTuneController Controller
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
          
        }


        private LagView[] LagViews;
        private CommonTuneController _controller;

      
             
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
            
        }
       

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);         
            DrawLags(canvas);
         
        }

   
    }
}