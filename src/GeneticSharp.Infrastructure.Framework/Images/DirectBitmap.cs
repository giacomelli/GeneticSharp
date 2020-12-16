using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GeneticSharp.Infrastructure.Framework.Images
{
    public class DirectBitmap : IDisposable, ICloneable
    {
        public Bitmap Bitmap { get; private set; }
        public int[] Bits { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }

        protected GCHandle BitsHandle { get; private set; }

        public DirectBitmap(int width, int height): this(width, height, new Int32[width * height])
        {
        }


        public DirectBitmap(int width, int height, int[] existing)
        {
            Width = width;
            Height = height;
            Bits = new List<int>(existing).ToArray();
            BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
            Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb,
                BitsHandle.AddrOfPinnedObject());
        }

        public void SetPixel(int x, int y, Color colour)
        {
            int index = x + (y * Width);
            int col = colour.ToArgb();

            Bits[index] = col;
        }

        public Color GetPixel(int x, int y)
        {
            int index = x + (y * Width);
            int col = Bits[index];
            Color result = Color.FromArgb(col);

            return result;
        }
      
       

        object ICloneable.Clone()
        {
            return Clone();
        }


        public DirectBitmap Clone()
        {
            return new DirectBitmap(Width, Height, Bits);
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
            BitsHandle.Free();
        }
    }
}