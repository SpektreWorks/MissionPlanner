using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;
using SkiaSharp;

namespace MissionPlanner.Controls
{
    public partial class QuickViewAirspeed : SkiaSharp.Views.Desktop.SKControl
    {
        [System.ComponentModel.Browsable(true)]
        float _arspd = 0;

        public double alert_high { get; set; } = 0;
        public double alert_low { get; set; } = 0;
        public double attention_offset { get; set; } = 0;

        [System.ComponentModel.Browsable(true)]
        public float arspd { get { return _arspd; } set { if (_arspd == (value*1.94384F)) return; _arspd = (value*1.94383F); Invalidate(); } }

        public QuickViewAirspeed()
        {
            //InitializeComponent();

            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);
            string arspdtxt = "Airspeed";

            SKColor canvascolor = SKColors.Empty;
            if (alert_low != 0)
            {
                if (arspd <= alert_low)
                    canvascolor = SKColors.Red;
                else if (arspd < (alert_low + attention_offset))
                    canvascolor = SKColors.Orange;
            }

            if (alert_high != 0)
            {
                if (arspd >= alert_high)
                    canvascolor = SKColors.Red;
                else if (arspd > (alert_high - attention_offset))
                    canvascolor = SKColors.Orange;
            }

            e2.Surface.Canvas.Clear(canvascolor);

            {
                Size extent = e.MeasureString("0".PadLeft(arspdtxt.Length + 1, '0'), new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                float hRatio = this.Height / (float)(extent.Height);
                float wRatio = this.Width / (float)extent.Width;
                float ratio = (hRatio < wRatio) ? hRatio : wRatio;

                newSize = (newSize * ratio);// * 0.75f; // pixel to points

                newSize -= newSize % 5;

                if (newSize < 8 || newSize > 999999)
                    newSize = 8;

                extent = e.MeasureString(arspdtxt, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                e.DrawString(arspdtxt, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style), new SolidBrush(System.Drawing.SystemColors.Window), this.Width / 2 - extent.Width / 2, (this.Height / 2 - extent.Height / 2));
            }
        }

        public override void Refresh()
        {
            if (this.Visible)
                base.Refresh();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.Visible && this.ThisReallyVisible())
                base.OnInvalidated(e);
        }

        float newSize = 8;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}
