using System.Collections.Generic;
using System.Drawing;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Extensions.Drawing
{
    /// <summary>
    /// A chromosome that represents a bitmap.
    /// </summary>
    public sealed class BitmapChromosome : ChromosomeBase
    {
        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapChromosome"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public BitmapChromosome(int width, int height)
            : base(width * height)
        {
            Width = width;
            Height = height;

            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }
        #endregion

        #region Properties        
        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; private set; }
        #endregion

        #region Methods    
        /// <summary>
        /// Gets the pixels from bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>The pixels.</returns>
        public static IList<Color> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Color>();
            var width = bitmap.Width;
            var height = bitmap.Height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixels.Add(bitmap.GetPixel(x, y));
                }
            }

            return pixels;
        }

        /// <summary>
        /// Generates the gene.
        /// </summary>
        /// <param name="geneIndex">Index of the gene.</param>
        /// <returns>The new Gene.</returns>
        public override Gene GenerateGene(int geneIndex)
        {
            var rnd = RandomizationProvider.Current;

            return new Gene(Color.FromArgb(rnd.GetInt(0, 256), rnd.GetInt(0, 256), rnd.GetInt(0, 256)));
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>
        /// The new chromosome.
        /// </returns>
        public override IChromosome CreateNew()
        {
            return new BitmapChromosome(Width, Height);
        }

        /// <summary>
        /// Builds the bitmap from genes.
        /// </summary>
        /// <returns>The bitmap.</returns>
        public Bitmap BuildBitmap()
        {
            var result = new Bitmap(Width, Height);
            var geneIndex = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    result.SetPixel(x, y, (Color)GetGene(geneIndex++).Value);
                }
            }

            return result;
        }        
        #endregion
    }
}
