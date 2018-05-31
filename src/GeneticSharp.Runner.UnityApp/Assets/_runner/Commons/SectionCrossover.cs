using GeneticSharp.Domain.Chromosomes;
using System;
using System.Linq;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;
using GeneticSharp.Domain.Crossovers;

namespace GeneticSharp.Runner.UnityApp.Commons
{
    public class SectionCrossover : CrossoverBase
    {
        private int m_sectionSize;
        private bool m_randomParent;

        public SectionCrossover(int sectionSize, bool randomParent = true)
            :base(2, 2)
        {
            m_sectionSize = sectionSize;
            m_randomParent = randomParent;
            IsOrdered = false;
        }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var firstParent = parents[0];
            var secondParent = parents[1];
            var firstChild = firstParent.CreateNew();
            var secondChild = firstParent.CreateNew();
            var sectionsCount = firstParent.Length / m_sectionSize;

            for (int sectionIndex = 0; sectionIndex < sectionsCount; sectionIndex++)
            {
                CrossSection(firstParent, secondParent, firstChild, secondChild, sectionIndex);
            }

            return new List<IChromosome> { firstChild, secondChild };
        }

        protected void CrossSection(IChromosome firstParent, IChromosome secondParent, IChromosome firstChild, IChromosome secondChild, int sectionIndex)
        {
            var startIndex = sectionIndex * m_sectionSize;
            var useFirstParentFirst = false;

            if(m_randomParent)
            {
                useFirstParentFirst = RandomizationProvider.Current.GetInt(0, 2) == 0;
            }
            else 
            {
                useFirstParentFirst = sectionIndex % 2 == 0;    
            }

            if (useFirstParentFirst)
            {
                firstChild.ReplaceGenes(startIndex, firstParent.GetGenes().Skip(startIndex).Take(m_sectionSize).ToArray());
                secondChild.ReplaceGenes(startIndex, secondParent.GetGenes().Skip(startIndex).Take(m_sectionSize).ToArray());
            }
            else 
            {
                firstChild.ReplaceGenes(startIndex, secondParent.GetGenes().Skip(startIndex).Take(m_sectionSize).ToArray());
                secondChild.ReplaceGenes(startIndex, firstParent.GetGenes().Skip(startIndex).Take(m_sectionSize).ToArray());
            }
        }
    }
}