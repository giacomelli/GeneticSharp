using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using GeneticSharp.Runner.UnityApp.Commons;
using System.Collections.Generic;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class WallBuilderChromosome : BitStringChromosome<BrickPhenotypeEntity>
    {
        private int m_bricksCount;
        private Vector3 m_minPosition, m_maxPosition;

        public WallBuilderChromosome(int bricksCount, Vector3 minPosition, Vector3 maxPosition)
        {
            m_bricksCount = bricksCount;
            m_minPosition = minPosition;
            m_maxPosition = maxPosition;

            var phenotypeEntities = new BrickPhenotypeEntity[bricksCount];

            for (int i = 0; i < phenotypeEntities.Length; i++)
            {
                phenotypeEntities[i] = new BrickPhenotypeEntity(minPosition, maxPosition);
            }

            SetPhenotypes(phenotypeEntities);
            CreateGenes();
        }

        public string ID { get; } = System.Guid.NewGuid().ToString();

        public bool Evaluated { get; set; }
        public int FloorHits { get; set; }
        public int BrickHits { get; set; }
     
        public override IChromosome CreateNew()
        {
            return new WallBuilderChromosome(m_bricksCount, m_minPosition, m_maxPosition);
        }
    }
}