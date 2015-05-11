using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Fragments.StandartTune
{
    internal sealed class LagView
    {
        private readonly LagInfo _info;
        private static readonly Color SelectedLagFrameColor = Color.DarkRed;
        private static readonly Color NotSelectedFrameLagColor = Color.DarkSlateGray;

        private static readonly Color SelectedLagColor = Color.SlateGray;
        private static readonly Color NotSelectedLagColor = Color.SlateGray;

        public LagView(LagInfo info)
        {
            _info = info;

        }

        public void Draw(Canvas canvas,int x,int y)
        {
            var t = new Paint {Color = _info.IsRecording ? SelectedLagFrameColor : NotSelectedFrameLagColor };
            var t2 = new Paint { Color = _info.IsRecording ? SelectedLagColor : NotSelectedLagColor };
            canvas.DrawRoundRect(new RectF { Top = y - 50, Bottom = y + 50, Left = x - 100, Right =x +100 },10,10, t);
            canvas.DrawRoundRect(new RectF { Top = y - 40, Bottom = y + 40, Left = x - 90, Right = x + 90 }, 10, 10, t2);
            if (_info.Frequency.HasValue)
            {
                canvas.DrawText(_info.Frequency.Value.ToString(), x, y+30,
                    new Android.Text.TextPaint { Color = Color.Black, TextSize = 100,TextAlign = Paint.Align.Center });
            }
            else
                canvas.DrawText("-", x, y+30,
                    new Android.Text.TextPaint { Color = Color.Black, TextSize = 100, TextAlign = Paint.Align.Center });
        }
    }
}