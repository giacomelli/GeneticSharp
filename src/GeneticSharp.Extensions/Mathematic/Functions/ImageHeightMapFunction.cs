using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Infrastructure.Framework.Images;

namespace GeneticSharp.Extensions.Mathematic.Functions
{
    public class ImageHeightMapFunction : NamedEntity, IKnownFunction, IDisposable
    {
        private DirectBitmap _targetImage;

        public Image TargetImage
        {
            get => _targetImage.Bitmap;
            set
            {
                _targetImage = value.MakeGrayscaleImage();
            }
        }

        public Func<double[], double> Function => ComputeValue;

        public Func<int, IList<(double min, double max)>> Ranges => i =>
        {
            var drawRange = new[] {(0.0, (double) _targetImage.Width - 1), (0.0, (double) _targetImage.Height - 1)};
            if (i<=2)
            {
                return drawRange.ToList();
            }

            var extraCoordsRanges = Enumerable.Repeat((-1000.0, 1000.0), i - 2);
            return drawRange.Union(extraCoordsRanges).ToList();
        };
        public Func<double[],double, double> Fitness => (coords, d) => d;


        private double ComputeValue(double[] coords)
        {
            (double x, double y) = (coords[0], coords[1]);
            if (x<0 || y<0 || x> _targetImage.Width-1 || y>_targetImage.Height-1 )
            {
                throw new ArgumentException("coords outside of image size range");
            }
            (int xDraw, int yDraw) = ((int) Math.Floor(x), (int) Math.Floor(y));

            double value = _targetImage.GetPixel(xDraw, yDraw).R;
            if (x > xDraw && y > yDraw)
            {
                var b00 = _targetImage.GetPixel(xDraw, yDraw).R;
                var b01 = _targetImage.GetPixel(xDraw + 1, yDraw).R;
                var b10 = _targetImage.GetPixel(xDraw, yDraw + 1).R;
                var b11 = _targetImage.GetPixel(xDraw + 1, yDraw + 1).R;
                var d00 = Math.Sqrt((x - xDraw) * (x - xDraw) + (y - yDraw) * (y - yDraw));
                var d01 = Math.Sqrt((x - xDraw - 1) * (x - xDraw - 1) + (y - yDraw) * (y - yDraw));
                var d10 = Math.Sqrt((x - xDraw) * (x - xDraw) + (y - yDraw - 1) * (y - yDraw - 1));
                var d11 = Math.Sqrt((x - xDraw - 1) * (x - xDraw - 1) + (y - yDraw - 1) * (y - yDraw - 1));
                value = (b00 / d00 + b01 / d01 + b10 / d10 + b11 / d11) / (1/d00+1/d01+1/d10+1/d11);
            }

            return value;

        }


        public void Dispose()
        {
            _targetImage?.Dispose();
        }
    }
}