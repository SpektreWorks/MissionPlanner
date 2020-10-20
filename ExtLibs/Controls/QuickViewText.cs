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
    public partial class QuickViewText : SkiaSharp.Views.Desktop.SKControl
    {
        [System.ComponentModel.Browsable(true)]
        float _val = 0;

        public string valtxt { get; set; } = "";
        public float valfontsize { get; set; } = 13;
        public string headertxt { get; set; } = "";
        public float headerfontsize { get; set; } = 8.25F;

        public double alert_high { get; set; } = 0;
        public double alert_low { get; set; } = 0;
        public double attention_offset { get; set; } = 0;

        [System.ComponentModel.Browsable(true)]
        public float val { get { return _val; } set { if (_val == value) return; _val = value; Invalidate(); } }

        public QuickViewText()
        {
            //InitializeComponent();

            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);

            SKColor canvascolor = SKColors.Empty;
            if (alert_low != 0)
            {
                if (val <= alert_low)
                    canvascolor = SKColors.Red;
                else if (val < (alert_low + attention_offset))
                    canvascolor = SKColors.Orange;
            }

            if (alert_high != 0)
            {
                if (val >= alert_high)
                    canvascolor = SKColors.Red;
                else if (val > (alert_high - attention_offset))
                    canvascolor = SKColors.Orange;
            }

            e2.Surface.Canvas.Clear(canvascolor);

            {
                Size h_extent = e.MeasureString(headertxt, new Font(this.Font.FontFamily, (float)headerfontsize, this.Font.Style)).ToSize();
                float header_xpos = (this.Size.Width / 2) - (h_extent.Width / 2);
                float header_ypos = (this.Size.Height * 0.15F) - (h_extent.Height / 2);
                e.DrawString(headertxt, new Font(this.Font.FontFamily, headerfontsize, this.Font.Style), new SolidBrush(System.Drawing.SystemColors.Window), header_xpos, header_ypos);

                Size extent = e.MeasureString(valtxt, new Font(this.Font.FontFamily, (float)valfontsize, this.Font.Style)).ToSize();
                float xpos = (this.Size.Width / 2) - (extent.Width / 2);
                float ypos = (this.Size.Height / 2) - (extent.Height / 2) + (h_extent.Height * 0.5F);
                e.DrawString(valtxt, new Font(this.Font.FontFamily, valfontsize, this.Font.Style), new SolidBrush(System.Drawing.SystemColors.Window), xpos, ypos);

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
