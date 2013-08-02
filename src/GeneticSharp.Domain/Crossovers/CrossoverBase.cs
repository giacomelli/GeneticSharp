using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneticSharp.Domain.Chromosomes;
using HelperSharp;

namespace GeneticSharp.Domain.Crossovers
{
    public abstract class CrossoverBase : ICrossover
    {
        #region Constructors
        protected CrossoverBase(int parentsNumber, int childrenNumber)
        {
            ParentsNumber = parentsNumber;
			ChildrenNumber = childrenNumber;
        }
        #endregion

        public int ParentsNumber { get; private set; }
		public int ChildrenNumber  { get; private set; }

        public IList<IChromosome> Cross(IList<IChromosome> parents)
        {
            ExceptionHelper.ThrowIfNull("parents", parents);

            if (parents.Count != ParentsNumber)
            {
                throw new ArgumentOutOfRangeException("parents", "The number of parents should be the same of ParentsNumber.");
            }

            return PerformCross(parents);
        }

        protected abstract IList<IChromosome> PerformCross(IList<IChromosome> parents);
        
    }
}
