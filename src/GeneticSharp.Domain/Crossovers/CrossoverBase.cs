using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelperSharp;
using GeneticSharp.Domain.Chromosomes;

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

		#region Properties
        public int ParentsNumber { get; private set; }
		public int ChildrenNumber  { get; private set; }
		#endregion

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
