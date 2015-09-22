using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Extensions.Mathematic
{
    /// <summary>
    /// Function builder chromosome.
    /// </summary>
    public sealed class FunctionBuilderChromosome : ChromosomeBase
    {
        #region Constants
        /// <summary>
        /// The max integer operation.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "Is right in this context")]
        public const int MaxIntOperation = 9;
        #endregion

        #region Fields
        private ReadOnlyCollection<string> m_availableOperations;
        private int m_maxOperations;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneticSharp.Extensions.Mathematic.FunctionBuilderChromosome"/> class.
        /// </summary>
        /// <param name="availableOperations">Available operations.</param>
        /// <param name="maxOperations">Max operations.</param>
        public FunctionBuilderChromosome(ReadOnlyCollection<string> availableOperations, int maxOperations) : base(maxOperations)
        {
            m_availableOperations = availableOperations;
            m_maxOperations = maxOperations;

            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Builds the available operations.
        /// </summary>
        /// <returns>The available operations.</returns>
        /// <param name="parametersCount">Parameters count.</param>
        public static ReadOnlyCollection<string> BuildAvailableOperations(int parametersCount)
        {
            var availableOperations = new List<string>(new string[] { string.Empty, "+", "-", "/", "*", "__INT__" });
            availableOperations.AddRange(GetParameterNames(parametersCount));

            return availableOperations.AsReadOnly();
        }

        /// <summary>
        /// Gets the parameter names.
        /// </summary>
        /// <returns>The parameter names.</returns>
        /// <param name="parametersCount">Parameters count.</param>
        public static string[] GetParameterNames(int parametersCount)
        {
            string[] parameterNames = new string[parametersCount];

            for (int i = 0; i < parametersCount; i++)
            {
                parameterNames[i] = ((char)(i + 65)).ToString();
            }

            return parameterNames;
        }

        /// <summary>
        /// Builds the function.
        /// </summary>
        /// <returns>The function.</returns>
        public string BuildFunction()
        {
            var builder = new StringBuilder();

            foreach (var g in GetGenes())
            {
                var op = g.Value.ToString();

                if (!string.IsNullOrEmpty(op))
                {
                    builder.AppendFormat("{0} ", op);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates a new chromosome using the same structure of this.
        /// </summary>
        /// <returns>The new chromosome.</returns>
        public override IChromosome CreateNew()
        {
            return new FunctionBuilderChromosome(m_availableOperations, m_maxOperations);
        }

        /// <summary>
        /// Generates the gene for the specified index.
        /// </summary>
        /// <returns>The gene.</returns>
        /// <param name="geneIndex">Gene index.</param>
        public override Gene GenerateGene(int geneIndex)
        {
            var rnd = Domain.Randomizations.RandomizationProvider.Current;
            var op = m_availableOperations[rnd.GetInt(0, m_availableOperations.Count)];

            if (op.Equals("__INT__", StringComparison.OrdinalIgnoreCase))
            {
                op = rnd.GetInt(0, MaxIntOperation + 1).ToString(CultureInfo.InvariantCulture);
            }

            return new Gene(op);
        }
        #endregion
    }
}
