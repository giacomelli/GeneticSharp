using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeneticSharp.Infrastructure.Framework.Images
{
    public static class ImageExtensions
    {

        public static DirectBitmap MakeGrayscaleImage(this Image original)
        {
            //create a blank bitmap the same size as original
            DirectBitmap newBitmap = new DirectBitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap.Bitmap))
            {

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                    {
                        new float[] {.3f, .3f, .3f, 0, 0},
                        new float[] {.59f, .59f, .59f, 0, 0},
                        new float[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                    });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                        0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        //todo: The following was to be used in DirectBitmap to create from bitmap/clone, but it has a memory leak, ImageConverter does not have the issue but it is not available in .Net core<3.0, and a move to SharpImage or other libs is recommended rather the compatibility System.Drawing

        //public static int[] ToIntArray(this Bitmap bmp)
        //{
        //    BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

        //    // number of bytes in the bitmap
        //    var byteCount = bData.Stride * (bmp.Height);

        //    int[] ints = new int[byteCount / 4];

        //    Marshal.Copy(bData.Scan0, ints, 0, byteCount / 4);

        //    // don't forget to unlock the bitmap!!
        //    bmp.UnlockBits(bData);

        //    return ints;
        //}


        /// <summary>
        /// Helper to build a RGB color from HSV definition
        /// </summary>
        /// <param name="h">Hue</param>
        /// <param name="s">Saturation</param>
        /// <param name="v"Value></param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        public static (double r, double g, double b) HsvToRgb(this (double h, double s, double v) hsv)
        {
            double r, g, b;
            if (Math.Abs(hsv.h - 1.0) <= double.Epsilon)
            {
                hsv.h = 0.0;
            }

            double step = 1.0 / 6.0;
            double vh = hsv.h / step;

            int i = (int)Math.Floor(vh);

            double f = vh - i;
            double p = hsv.v * (1.0 - hsv.s);
            double q = hsv.v * (1.0 - hsv.s * f);
            double t = hsv.v * (1.0 - hsv.s * (1.0 - f));

            switch (i)
            {
                case 0:
                {
                    r = hsv.v;
                    g = t;
                    b = p;
                    break;
                }
                case 1:
                {
                    r = q;
                    g = hsv.v;
                    b = p;
                    break;
                }
                case 2:
                {
                    r = p;
                    g = hsv.v;
                    b = t;
                    break;
                }
                case 3:
                {
                    r = p;
                    g = q;
                    b = hsv.v;
                    break;
                }
                case 4:
                {
                    r = t;
                    g = p;
                    b = hsv.v;
                    break;
                }
                case 5:
                {
                    r = hsv.v;
                    g = p;
                    b = q;
                    break;
                }
                default:
                {
                    // not possible - if we get here it is an internal error
                    throw new ArgumentException();
                }
            }

            return (r, g, b);
        }
    }
}