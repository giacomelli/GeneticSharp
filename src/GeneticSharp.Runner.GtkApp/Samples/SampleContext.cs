using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Gdk;
using GeneticSharp.Domain.Populations;
using HelperSharp;

namespace GeneticSharp.Runner.GtkApp.Samples
{
    public class SampleContext
    {
        #region Fields
        private int m_lastTextY = 0;
		private Gdk.Window m_window;
        #endregion

		public SampleContext(Gdk.Window window)
		{
			m_window = window;
		}

        public Gdk.GC GC { get; set; }
        public Pixmap Buffer { get; set; }
        public Pango.Layout Layout { get; set; }
        public Population Population { get; set; }
        public Rectangle DrawingArea { get; set; }
	
        public void Reset()
        {

            m_lastTextY = 0;
        }

        public void WriteText(string text, params object[] args)
        {
            Layout.SetMarkup("<span color='gray'>{0}</span>".With(String.Format(CultureInfo.InvariantCulture, text, args)));
            Buffer.DrawLayout(GC, 0, m_lastTextY, Layout);
            m_lastTextY += 20;
        }

		public Gdk.GC CreateGC(Gdk.Color foregroundColor)
		{
			var gc = new Gdk.GC(m_window);
			gc.RgbFgColor = foregroundColor;
			gc.RgbBgColor = new Gdk.Color(255, 255, 255);
			gc.Background = new Gdk.Color(255, 255, 255);
			gc.SetLineAttributes(1, LineStyle.OnOffDash, CapStyle.Projecting, JoinStyle.Round);

			return gc;
		}
    }
}
