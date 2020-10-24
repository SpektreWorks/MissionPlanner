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
    public partial class QuickViewFuel : SkiaSharp.Views.Desktop.SKControl
    {
        [System.ComponentModel.Browsable(true)]
        public string desc
        {
            get { return _desc; }
            set { if (_desc == value) return; _desc = value; Invalidate(); }
        }

        string _str = "";

        public double alert_high { get; set; } = 0;
        public double alert_low { get; set; } = 0;
        public double attention_offset { get; set; } = 0;
        private Color _happy_color = Color.Empty;
        private Color _attention_color = Color.Orange;
        private Color _alert_color = Color.Red;
        public Color happy_color { get { return _happy_color; } set { if (_happy_color == value) return; _happy_color = value; Invalidate(); } }
        public Color attention_color { get { return _attention_color; } set { if (_attention_color == value) return; _attention_color = value; Invalidate(); } }
        public Color alert_color { get { return _alert_color; } set { if (_alert_color == value) return; _alert_color = value; Invalidate(); } }

        public float numFontSize { get; set; } = -1.0f;

        [System.ComponentModel.Browsable(true)]
        public string str { get { return _str; } set { if (_str == value) return; _str = value; Invalidate(); } }

        private string _desc = "";
        private Color _strcolor;

        [System.ComponentModel.Browsable(true)]
        public Color strColor { get { return _strcolor; } set { if (_strcolor == value) return; _strcolor = value; Invalidate(); } }

        public QuickViewFuel()
        {
            //InitializeComponent();

            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);

            string[] s = str.Split(';');
            if ( s.Length == 1 )
            {
                return;
            }
            string fuel_str = s[0];
            string time_str = s[1];
            string remaining_str = fuel_str + "lb " + time_str + "hr";

            float fuel_f = float.Parse(fuel_str);
            float time_f = float.Parse(time_str);

            Color canvascolor = happy_color;

            if (alert_low != 0)
            {
                if (time_f <= alert_low)
                    canvascolor = alert_color;
                else if (time_f < (alert_low + attention_offset))
                    canvascolor = attention_color;
            }

            if (alert_high != 0)
            {
                if (time_f >= alert_high)
                    canvascolor = alert_color;
                else if ( time_f > (alert_high - attention_offset))
                        canvascolor = attention_color;
            }
            
            e2.Surface.Canvas.Clear(Extensions.ToSKColor(canvascolor));
            int y = 0;
            {
                Size extent = e.MeasureString(desc, this.Font).ToSize();

                var mid = extent.Width / 2;

                e.DrawString(desc, this.Font, new SolidBrush(this.ForeColor), this.Width / 2 - mid, 0);

                y = extent.Height;
            }
            //
            {
                this.strColor = Color.White;
                Size extent = e.MeasureString(remaining_str, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();
                e.DrawString(remaining_str, new Font(this.Font.FontFamily, (float)numFontSize, this.Font.Style), new SolidBrush(this.strColor), (this.Width / 2 - extent.Width / 2)-4, y + ((this.Height - y) / 2 - extent.Height / 2));
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
