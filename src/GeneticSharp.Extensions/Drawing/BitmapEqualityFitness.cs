using System;
using System.Collections.Generic;
using System.Drawing;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace GeneticSharp.Extensions.Drawing
{
    /// <summary>
    /// Bitmap equality fitness.
    /// </summary>
    public class BitmapEqualityFitness : IFitness
    {
        #region Fields
        private IList<Color> m_targetBitmapPixels;
        private int m_pixelsCount;
        #endregion

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapEqualityFitness"/> class.
        /// </summary>
        /// <param name="targetBitmap">The target bitmap.</param>
        public BitmapEqualityFitness(Bitmap targetBitmap)
			: this()
        {
            Initialize(targetBitmap);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapEqualityFitness"/> class.
        /// </summary>
        public BitmapEqualityFitness()
        {
        }
        #endregion

        #region Properties        
        /// <summary>
        /// Gets the width of the bitmap.
        /// </summary>
        /// <value>
        /// The width of the bitmap.
        /// </value>
        public int BitmapWidth { get; private set; }

        /// <summary>
        /// Gets the height of the bitmap.
        /// </summary>
        /// <value>
        /// The height of the bitmap.
        /// </value>
        public int BitmapHeight { get; private set; }
        #endregion

        #region Methods        
        /// <summary>
        /// Initializes the specified target bitmap.
        /// </summary>
        /// <param name="targetBitmap">The target bitmap.</param>
        public void Initialize(Bitmap targetBitmap)
        {
            BitmapWidth = targetBitmap.Width;
            BitmapHeight = targetBitmap.Height;

            m_targetBitmapPixels = BitmapChromosome.GetPixels(targetBitmap);
            m_pixelsCount = m_targetBitmapPixels.Count;
        }

        /// <summary>
        /// Evaluates the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>The chromosome fitness.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            double fitness = 0;

            for (int i = 0; i < m_pixelsCount; i++)
            {
                var targetPixel = m_targetBitmapPixels[i];
                var chromosomePixel = (Color)chromosome.GetGene(i).Value;

                fitness -= Math.Abs(targetPixel.R - chromosomePixel.R);
                fitness -= Math.Abs(targetPixel.G - chromosomePixel.G);
                fitness -= Math.Abs(targetPixel.B - chromosomePixel.B);
            }

            return fitness;
        }

        #endregion
    }
}