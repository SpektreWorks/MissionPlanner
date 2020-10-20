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
    public partial class QuickViewLockStatus : SkiaSharp.Views.Desktop.SKControl
    {
        [System.ComponentModel.Browsable(true)]
        bool _lock_status = false;

        [System.ComponentModel.Browsable(true)]
        public bool lock_status { get { return _lock_status; } set { if (_lock_status == value) return; _lock_status = value; Invalidate(); } }
        public float fontsize { get; set; } = 13;

        public QuickViewLockStatus()
        {
            //InitializeComponent();

            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);
            string locktxt = "";

            SKColor canvascolor;
            if ( lock_status == true )
            {
                locktxt = "Unlocked";
                canvascolor = SKColors.Empty;
            }
            else
            {
                locktxt = "Locked";
                canvascolor = SKColors.Red;
            }
            
            e2.Surface.Canvas.Clear(canvascolor);

            {
                Size extent = e.MeasureString(locktxt, new Font(this.Font.FontFamily, (float)fontsize, this.Font.Style)).ToSize();
                float xpos = (this.Size.Width / 2) - (extent.Width / 2);
                float ypos = (this.Size.Height / 2) - (extent.Height / 2);
                e.DrawString(locktxt, new Font(this.Font.FontFamily, fontsize, this.Font.Style), new SolidBrush(System.Drawing.SystemColors.Window), xpos, ypos);
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
