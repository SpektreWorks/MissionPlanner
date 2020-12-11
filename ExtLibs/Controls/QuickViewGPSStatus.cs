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
        //bool _gps_status = false;

        //[System.ComponentModel.Browsable(true)]
        //public bool gps_status { get { return _gps_status; } set { if (_gps_status == value) return; _gps_status = value; Invalidate(); } }
        string _str = "";
        public string str { get { return _str; } set { if (_str == value) return; _str = value; Invalidate(); } }

        public QuickViewGPSStatus()
        {
            //InitializeComponent();

            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);

            string[] s = str.Split(';');
            if (s.Length != 3)
            {
                return;
            }
            string numsats = s[0];
            string hdop = s[1];
            bool gps_status = bool.Parse(s[2]);
            
            SKColor canvascolor;
            if (gps_status == true)
            {
                canvascolor = SKColors.Empty;
            }
            else
            {
                canvascolor = SKColors.Red;
            }

            e2.Surface.Canvas.Clear(canvascolor);

            {
                //string desc = "GPS";
                //Size extent1 = e.MeasureString(desc, new Font(this.Font.FontFamily, 12f, this.Font.Style)).ToSize();
                //float xpos1 = (this.Size.Width / 2) - (extent1.Width / 2);
                //e.DrawString(desc, new Font(this.Font.FontFamily, 12f, this.Font.Style), new SolidBrush(System.Drawing.SystemColors.Window), xpos1, 1);

                float fontsize = 12.5f;
                Size extent2 = e.MeasureString(numsats, new Font(this.Font.FontFamily, (float)fontsize, this.Font.Style)).ToSize();
                float xpos2 = (this.Size.Width / 2) - (extent2.Width / 2);
                Size extent3 = e.MeasureString(hdop, new Font(this.Font.FontFamily, (float)fontsize, this.Font.Style)).ToSize();
                float xpos3 = (this.Size.Width / 2) - (extent3.Width / 2);

                e.DrawString(numsats, new Font(this.Font.FontFamily, fontsize, this.Font.Style), new SolidBrush(SystemColors.Window), xpos2, this.Size.Height*0.1f);
                e.DrawString(hdop, new Font(this.Font.FontFamily, fontsize, this.Font.Style), new SolidBrush(SystemColors.Window), xpos3+2, this.Size.Height*0.45f);
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}
