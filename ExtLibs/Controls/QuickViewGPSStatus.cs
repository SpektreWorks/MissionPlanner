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
    public partial class QuickViewGPSStatus : SkiaSharp.Views.Desktop.SKControl
    {
        [System.ComponentModel.Browsable(true)]
        bool _gps_status = false;

        [System.ComponentModel.Browsable(true)]
        public bool gps_status { get { return _gps_status; } set { if (_gps_status == value) return; _gps_status = value; Invalidate(); } }

        public QuickViewGPSStatus()
        {
            //InitializeComponent();

            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);
            string gpstxt = "GPS";

            SKColor canvascolor;
            if ( gps_status == true )
            {
                canvascolor = SKColors.Empty;
            }
            else
            {
                canvascolor = SKColors.Red;
            }
            
            e2.Surface.Canvas.Clear(canvascolor);

            {
                Size extent = e.MeasureString("0".PadLeft(gpstxt.Length + 1, '0'), new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                float hRatio = this.Height / (float)(extent.Height);
                float wRatio = this.Width / (float)extent.Width;
                float ratio = (hRatio < wRatio) ? hRatio : wRatio;

                newSize = (newSize * ratio);// * 0.75f; // pixel to points

                newSize -= newSize % 5;

                if (newSize < 8 || newSize > 999999)
                    newSize = 8;

                extent = e.MeasureString(gpstxt, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                e.DrawString(gpstxt, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style), new SolidBrush(System.Drawing.SystemColors.Window), this.Width / 2 - extent.Width / 2, (this.Height / 2 - extent.Height / 2));
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
