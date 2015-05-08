using System;
using Android.Views;
using Android.Content;
using Android.Graphics;
using FuorieTest;
using System.Linq;

namespace DrumTuneXAM
{
	public class NoteView:View
	{

		private Paint _paint = new Paint ();
		private Bitmap _bmp;
		public NoteView (Context context):base(context)
		{
			_paint.StrokeWidth = 1;
		}

		public NoteView (Context context,Android.Util.IAttributeSet pairs):base(context)
		{
			_paint.StrokeWidth = 1;

		}
		private FrequencyChart _chart;
		public void LoadChart(FrequencyChart chart)
		{
			_chart = chart;		
			PostInvalidate ();
		}

		private void LoadChartIn(FrequencyChart chart,int width,int height)
		{
			var pix = new int[width * height];

			if (chart.Count != 0) {
				int bandWidth = (width / chart.Count) + 1;
			
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						var tx = x;
						var ty = height - y - 1;
						var inBand = (1 + chart [x / bandWidth].DBLevel/120) > ((double)y / height);
						var res = (width * ty) + tx;
						pix [res] = inBand ? Color.Green : Color.Black;
					}
				}
			}
			_bmp = Bitmap.CreateBitmap (pix, width, height, Bitmap.Config.Argb8888);

		}



		protected override void OnDraw (Canvas canvas)
		{
			
			base.OnDraw (canvas);

			if (_chart != null && canvas.Height>0) {
				LoadChartIn (_chart, canvas.Width, canvas.Height);
				canvas.DrawBitmap (_bmp, 0, 0, _paint);
			}
		}


	}
}

