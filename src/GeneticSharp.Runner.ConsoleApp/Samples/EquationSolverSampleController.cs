using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;
using Jace;
using Jace.Tokenizer;

namespace GeneticSharp.Runner.ConsoleApp.Samples
{
    [DisplayName("Equation solver")]
    public class EquationSolverSampleController : SampleControllerBase
    {
        #region Fields
        private EquationSolverFitness m_fitness;
        private string m_equation;
        private string m_equationLeftPart;
        private string m_equationRightPart;
        private int m_equationResult;
        private List<string> m_variables;
        #endregion

        #region Methods        
        /// <summary>
        /// Creates the chromosome.
        /// </summary>
        /// <returns>The sample chromosome.</returns>
        public override IChromosome CreateChromosome()
        {
            return new EquationChromosome(m_equationResult, m_variables.Count);
        }

        /// <summary>
        /// Creates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        public override IFitness CreateFitness()
        {
            m_fitness = new EquationSolverFitness(m_equationResult, GetEquationResult);

            return m_fitness;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            Console.WriteLine("Type the equation:");
            Console.WriteLine("Samples:");
            Console.WriteLine("     3*x - 2*y + 4*z = 7");
            Console.WriteLine("     x * y - 3 * z + t = 8");
            Console.WriteLine("     a + b * c + d - f / g = 1000");
            Console.WriteLine("     a*x^2 + b * x + c = 100");
            Console.WriteLine("     a^3 - 4*b^2 + 3*c - 4 = 0");

            m_equation = Console.ReadLine();

            var equationParts = m_equation.Split('=');
            m_equationLeftPart = equationParts[0];
            m_equationRightPart = equationParts[1];

            var reader = new TokenReader();
            var tokens = reader.Read(m_equationLeftPart);
            m_variables = new List<string>();

            foreach (var token in tokens)
            {
                if (token.TokenType == TokenType.Text)
                {
                    var value = token.Value as string;

                    if (!m_variables.Contains(value))
                    {
                        m_variables.Add(value);
                    }
                }
            }

            var engine = new CalculationEngine();
            m_equationResult = (int)engine.Calculate(m_equationRightPart);
        }

        /// <summary>
        /// Draws the specified best chromosome.
        /// </summary>
        /// <param name="bestChromosome">The best chromosome.</param>
        public override void Draw(IChromosome bestChromosome)
        {
            Console.WriteLine("Equation: {0}", m_equation);
            var best = bestChromosome as EquationChromosome;

            var genes = best.GetGenes();

            for (int i = 0; i < m_variables.Count; i++)
            {
                Console.WriteLine("{0} = {1}", m_variables[i], genes[i]);
            }

            Console.WriteLine("Result: {0}", GetEquationResult(genes));
        }

        /// <summary>
        /// Creates the termination.
        /// </summary>
        /// <returns>
        /// The termination.
        /// </returns>
        public override ITermination CreateTermination()
        {
            return new FitnessThresholdTermination(0);
        }

        private int GetEquationResult(Gene[] genes)
        {
            var variableValues = new Dictionary<string, double>();

            for (int i = 0; i < m_variables.Count; i++)
            {
                variableValues.Add(m_variables[i], (int)genes[i].Value);
            }

            var engine = new CalculationEngine();
            var result = engine.Calculate(m_equationLeftPart, variableValues);

            return (int)result;
        }
        #endregion
    }
}
