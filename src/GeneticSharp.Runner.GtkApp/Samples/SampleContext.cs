﻿using System.Globalization;
using Gdk;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Infrastructure.Framework.Texts;

namespace GeneticSharp.Runner.GtkApp.Samples
{
    public class SampleContext
    {
        #region Fields
        private int m_lastTextY = 0;
        #endregion

        public SampleContext(Window gdkWindow, Gtk.Window gtkWindow)
        {
            GdkWindow = gdkWindow;
            GtkWindow = gtkWindow;
        }

        public GeneticAlgorithm GA { get; set; }

        public Window GdkWindow { get; private set; }

        public Gtk.Window GtkWindow { get; private set; }

        public GC GC { get; set; }

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
            Layout.SetMarkup("<span color='gray'>{0}</span>".With(string.Format(CultureInfo.InvariantCulture, text, args)));
            Buffer.DrawLayout(GC, 0, m_lastTextY, Layout);
            m_lastTextY += 20;
        }

        public GC CreateGC(Color foregroundColor)
        {
            var gc = new GC(GdkWindow)
            {
                RgbFgColor = foregroundColor,
                RgbBgColor = new Color(255, 255, 255),
                Background = new Color(255, 255, 255)
            };
            gc.SetLineAttributes(1, LineStyle.OnOffDash, CapStyle.Projecting, JoinStyle.Round);

            return gc;
        }
    }
}
