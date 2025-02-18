﻿using System;

namespace GeneticSharp.Extensions
{
    /// <summary>
    /// Fitness based on this paper: Genetic Algorithm for Solving Simple Mathematical Equality Problem.
    /// <see href="http://arxiv.org/ftp/arxiv/papers/1308/1308.4675.pdf" />
    /// </summary>
    public class EqualityFitness : IFitness
    {
        #region Methods  
        /// <summary>
        /// Gets the equation result using the chromosome's genes.
        /// </summary>
        /// <param name="equalityChromosome">The chromosome.</param>
        /// <returns>The equation result.</returns>
        public static int GetEquationResult(EquationChromosome equalityChromosome)
        {
            var genes = equalityChromosome.GetGenes();
            var a = (int)genes[0].Value!;
            var b = (int)genes[1].Value!;
            var c = (int)genes[2].Value!;
            var d = (int)genes[3].Value!;

            return a + (2 * b) + (3 * c) + (4 * d);
        }

        /// <summary>
        /// Evaluates the specified chromosome.
        /// </summary>
        /// <param name="chromosome">The chromosome.</param>
        /// <returns>The chromosome fitness.</returns>
        public double Evaluate(IChromosome chromosome)
        {
            // a + 2b + 3c + 4d = 30
            var equalityChromosome = chromosome as EquationChromosome;

            var fitness = Math.Abs(GetEquationResult(equalityChromosome!) - 30);

            return fitness * -1;
        }
        #endregion
    }
}
