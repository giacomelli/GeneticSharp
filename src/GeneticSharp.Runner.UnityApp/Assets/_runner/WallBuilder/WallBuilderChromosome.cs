using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using GeneticSharp.Runner.UnityApp.Commons;
using System.Collections.Generic;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class WallBuilderChromosome : BitStringChromosome<BrickPhenotypeEntity>
    {
        private Vector3 m_minPosition, m_maxPosition;

        public WallBuilderChromosome(int bricksCount, Vector3 minPosition, Vector3 maxPosition)
        {
            BricksCount = bricksCount;
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
        public int BricksCount { get; private set; }
        public bool Evaluated { get; set; }
        public int FloorHits { get; set; }
        public int BrickHits { get; set; }
        public IList<Vector3> BricksEndPositions { get; set; } = new List<Vector3>();
     
        public override IChromosome CreateNew()
        {
            return new WallBuilderChromosome(BricksCount, m_minPosition, m_maxPosition);
        }
    }
}