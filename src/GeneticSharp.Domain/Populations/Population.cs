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
        public event EventHandler GenerationRan;
		public event EventHandler BestChromosomeChanged;
        #endregion

        #region Fields
        private IChromosome m_adamChromosome;
		private SmartThreadPool m_threadPool;
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
		public void RunGeneration(int timeout = 0)
		{
			if (Generations.Count == 0) {
				CurrentGeneration = CreateNewGeneration (CreateInitialChromosomes ());
				EvaluateFitness (timeout);
				CurrentGeneration.Chromosomes = SelectParents ();
			} else {
				EvaluateFitness (timeout);
				CurrentGeneration = CreateNewGeneration(SelectParents ());
			}
			
			Mutate (Cross ());
            EvaluateFitness(timeout);
            ElectBestChromosome();
            FinalizeGeneration();

            if (GenerationRan != null)
            {
                GenerationRan(this, EventArgs.Empty);
            }
		}

		public void RunGenerations(int generations, int timeoutPerGeneration = 0)
		{
			for (var i = 0; i < generations; i++) {
				RunGeneration (timeoutPerGeneration);
			}
		}

		public void AbortGeneration (int timeout = 60000)
		{
			if (m_threadPool != null) {
				m_threadPool.Shutdown (true, timeout);
			}
		}

		void FinalizeGeneration ()
		{
			if(CurrentGeneration.Chromosomes.Count > MaxSize)
			{
                CurrentGeneration.Chromosomes = Selection.SelectChromosomes(MaxSize, CurrentGeneration);

				if (!CurrentGeneration.Chromosomes.Any (c => c == CurrentGeneration.BestChromosome)) {
					CurrentGeneration.Chromosomes.RemoveAt (CurrentGeneration.Chromosomes.Count - 1);
					CurrentGeneration.Chromosomes.Add (CurrentGeneration.BestChromosome);
				}
			}
		}

        private IChromosome CreateChromosome()
        {
            var newOne = m_adamChromosome.CreateNew();

            return newOne;
        }

		private Generation CreateNewGeneration(IList<IChromosome> chromosomes)
		{
			var g = new Generation (Generations.Count + 1, chromosomes);
			Generations.Add (g);

			return g;
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

        private void EvaluateFitness(int timeout)
        {
            if (Fitness.SupportsParallel)
            {
                EvaluateFitnessParallel(timeout);
            }
            else
            {
                EvaluateFitnessLinear(timeout);
            }
        }

        private void EvaluateFitnessLinear(int timeout)
        {
            var chromosomesWithoutFitness = CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue);
             
			foreach(var c in chromosomesWithoutFitness)
            {
                c.Fitness = Fitness.Evaluate(c);

                if (c.Fitness < 0 || c.Fitness > 1)
                {
                    throw new FitnessException(Fitness, "The {0}.Evaluate returns a fitness with value {1}. The fitness value should be between 0.0 and 1.0."
                                                        .With(Fitness.GetType(), c.Fitness));
                }
            }          
        }

		private void EvaluateFitnessParallel (int timeout)
		{
			m_threadPool = new SmartThreadPool();

			try {
	            m_threadPool.MinThreads = MinSize;
				m_threadPool.MaxThreads = MinSize;	            
	            var chromosomesWithoutFitness = CurrentGeneration.Chromosomes.Where(c => !c.Fitness.HasValue).ToList();
	            var workItemResults = new IWorkItemResult[chromosomesWithoutFitness.Count];

	            for (int i = 0; i < chromosomesWithoutFitness.Count; i++)
	            {
	                var c = chromosomesWithoutFitness[i];

	                try
	                {
						workItemResults[i] = m_threadPool.QueueWorkItem(new WorkItemCallback(RunEvaluateFitness), c);
	                }
	                catch (Exception ex)
	                {
	                    throw new InvalidOperationException("Error executing Fitness.Evaluate for chromosome {0}: {1}".With(c.Id, ex.Message), ex);
	                }
	            }                
				
                m_threadPool.Start ();                

				if(!m_threadPool.WaitForIdle (timeout == 0 ? int.MaxValue : timeout))
				{
					throw new TimeoutException("The RunGeneration reach the {0} milliseconds timeout.".With(timeout));
				}

                foreach (var wi in workItemResults)
                {
                    Exception ex;
                    wi.GetResult(out ex);

                    if (ex != null)
                    {
                        throw ex;
                    }
                }

				foreach (var c in chromosomesWithoutFitness)
				{
					if (c.Fitness < 0 || c.Fitness > 1)
					{
						throw new FitnessException(Fitness, "The {0}.Evaluate returns a fitness with value {1}. The fitness value should be between 0.0 and 1.0."
						                                    .With(Fitness.GetType(), c.Fitness));
					}
				}
			}
			finally {
				m_threadPool.Shutdown(true, 1000);
			}
		}

		private void ElectBestChromosome()
		{
			var newBestChromosome = CurrentGeneration.Chromosomes.OrderByDescending(c => c.Fitness.Value).First();
			ValidateBestChromosome (newBestChromosome);
			CurrentGeneration.BestChromosome = newBestChromosome;
		
			if (newBestChromosome != BestChromosome) {
				BestChromosome = newBestChromosome;
				if (BestChromosomeChanged != null) {
					BestChromosomeChanged (this, EventArgs.Empty);
				}
			}
		}

		private void ValidateBestChromosome(IChromosome chromosome)
		{
			if (!chromosome.Fitness.HasValue) {
				throw new InvalidOperationException (
					"There is unknown problem in current population, because BestChromosome should have a Fitness value. BestChromosome: Id:{0}, age: {1} and length: {2}"
					.With (chromosome.Id, chromosome.Age, chromosome.Length));
			}
		}


		private object RunEvaluateFitness(object state)
		{
			var c = state as IChromosome;

            try
            {
                c.Fitness = Fitness.Evaluate(c);
            }
            catch (Exception ex)
            {
                throw new FitnessException(Fitness, "Error executing Fitness.Evaluate for chromosome {0}: {1}".With(c.Id, ex.Message), ex);
            }

			return c.Fitness;
		}

		private IList<IChromosome> SelectParents ()
		{
			return Selection.SelectChromosomes (MinSize, CurrentGeneration);
		}

		private IList<IChromosome> Cross ()
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

			return children;
		}

		private void Mutate (IList<IChromosome> chromosomes)
		{
			foreach(var c in chromosomes)
			{
				Mutation.Mutate (c, MutationProbability);
			}
		}
		#endregion
	}
}