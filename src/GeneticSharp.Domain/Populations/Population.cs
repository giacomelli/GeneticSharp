using System;
using System.Collections.Generic;
using System.Linq;
using HelperSharp;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using Amib.Threading;

namespace GeneticSharp.Domain.Populations
{
	public class Population
    {
        #region Fields
        private IChromosome m_adamChromosome;
        #endregion

        #region Constructors
        public Population(int minSize, 
                          int maxSize,
                          IChromosome adamChromosome,
		                  IFitness fitness, 
		                  ISelection selection, 
		                  ICrossover crossover,
		                  IMutation mutation)
		{
            if (minSize < 2)
            {
                throw new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes.");
            }

            if (maxSize < minSize)
            {
                throw new ArgumentOutOfRangeException("maxSize", "The maximum size for a population should be equal or greater than minimum size.");
            }

            ExceptionHelper.ThrowIfNull("adamChromosome", adamChromosome);
            ExceptionHelper.ThrowIfNull("fitness", fitness);
            ExceptionHelper.ThrowIfNull("selection", selection);
            ExceptionHelper.ThrowIfNull("crossover", crossover);
            ExceptionHelper.ThrowIfNull("mutation", mutation);

			MinSize = minSize;
            MaxSize = maxSize;
            m_adamChromosome = adamChromosome;
			Fitness = fitness;
			Selection = selection;
			Crossover = crossover;
			Mutation = mutation;

			Generations = new List<Generation> ();
			CrossoverProbability = 0.75f;
			MutationProbability = 0.1f;

		}
		#endregion

		#region Properties
		public IList<Generation> Generations { get; private set; }
		public Generation CurrentGeneration { get; private set; }
		public int MinSize { get; private set; }
        public int MaxSize { get; private set; }
		public IFitness Fitness { get; private set; }
		public ISelection Selection { get; private set; }
		public ICrossover Crossover { get; private set; }
		public float CrossoverProbability  { get; set; }
		public IMutation Mutation { get; private set; }
		public float MutationProbability  { get; set; }
        public IChromosome BestChromosome { get; private set; }
		#endregion

		#region Methods
		public void RunGeneration()
		{
			if (Generations.Count == 0) {
				CurrentGeneration = CreateNewGeneration (CreateInitialChromosomes ());
				EvaluateFitness ();
				CurrentGeneration.Chromosomes = Select ();
			} else {
				EvaluateFitness ();
				CurrentGeneration = CreateNewGeneration(Select ());
			}

			Cross ();
			Mutate ();

            FinalizeGeneration();
		}

		void FinalizeGeneration ()
		{
            CurrentGeneration.Chromosomes.Each((c) => c.Age++);
            CurrentGeneration.Chromosomes = CurrentGeneration.Chromosomes
                .OrderBy(c => c.Age)
                .ThenByDescending(c=> c.Fitness)
                .Take(MaxSize).ToList();

            BestChromosome = CurrentGeneration.Chromosomes.OrderByDescending(c => c.Fitness).First();
		}

        private IChromosome CreateChromosome()
        {
            var newOne = m_adamChromosome.CreateNew();

            return newOne;
        }

		private Generation CreateNewGeneration(IList<IChromosome> chromosomes)
		{
			return new Generation (Generations.Count + 1, chromosomes);
		}

		private IList<IChromosome> CreateInitialChromosomes ()
		{
			var chromosomes = new List<IChromosome> ();

			for(int i = 0; i < MinSize; i++)
			{
                var c = CreateChromosome();
				chromosomes.Add (c);
			}

			return chromosomes;
		}

		private void EvaluateFitness ()
		{
            using (var smartThreadPool = new SmartThreadPool())
            {

                if (Fitness.SupportsParallel)
                {
                    smartThreadPool.MinThreads = MinSize;
                    smartThreadPool.MaxThreads = MinSize;
                }
                else
                {
                    smartThreadPool.MaxThreads = 1;
                }

                var chromosomesWithoutFitness = CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue).ToList();
                var workItemResults = new IWorkItemResult[chromosomesWithoutFitness.Count];

                for (int i = 0; i < chromosomesWithoutFitness.Count; i++)
                {
                    var c = chromosomesWithoutFitness[i];

                    try
                    {

                        workItemResults[i] = smartThreadPool.QueueWorkItem(() =>
                        {
                            c.Fitness = Fitness.Evaluate(c);
                        });

                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Error executing Fitness.Evaluate for chromosome {0}: {1}".With(c.Id, ex.Message), ex);
                    }
                }

                SmartThreadPool.WaitAll(workItemResults);
                smartThreadPool.Shutdown();


                foreach (var c in chromosomesWithoutFitness)
                {
                    if (c.Fitness < 0 || c.Fitness > 1)
                    {
                        throw new InvalidOperationException("The {0}.Evaluate returns a fitness with value {1}. The fitness value should be between 0.0 and 1.0."
                                                             .With(Fitness.GetType(), c.Fitness));
                    }
                }
            }
		}

		private IList<IChromosome> Select ()
		{
			return Selection.SelectChromosomes (MinSize, CurrentGeneration);
		}

		private void Cross ()
		{
			var children = new List<IChromosome>();

			for ( int i = 0; i < MinSize; i += Crossover.ParentsNumber )
			{
				if (RandomizationProvider.Current.GetDouble() <= CrossoverProbability)
				{
					var child = Crossover.Cross (CurrentGeneration.Chromosomes.Skip(i).Take(Crossover.ParentsNumber).ToList());
					children.AddRange (child);
				}
			}

			foreach (var c in children) {
				CurrentGeneration.Chromosomes.Add (c);
			}
		}

		private void Mutate ()
		{
			foreach(var c in CurrentGeneration.Chromosomes)
			{
				if (RandomizationProvider.Current.GetDouble () <= MutationProbability)
				{
					Mutation.Mutate (c);
				}
			}
		}
		#endregion
	}
}