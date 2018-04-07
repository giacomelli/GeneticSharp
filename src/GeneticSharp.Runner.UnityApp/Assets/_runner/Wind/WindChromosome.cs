using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Chromosomes;
using UnityEngine;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using System.Collections.Generic;

namespace GeneticSharp.Runner.UnityApp.Wind
{
    public class WindChromosome : ChromosomeBase
    {
        private Vector2 m_minPosition, m_maxPosition;

        public WindChromosome(int windTurbineVertices, Vector2 minPosition, Vector2 maxPosition) : base(windTurbineVertices)
        {
            m_minPosition = minPosition;
            m_maxPosition = maxPosition;


            for (int i = 0; i < windTurbineVertices; i++)
            {
                ReplaceGene(i, new Gene(GetRandomVertice()));
            }
        }

        public string ID { get; } = System.Guid.NewGuid().ToString();

        public bool Evaluated { get; set; }
        public int Turns { get; set; }

        public override IChromosome CreateNew()
        {
            return new WindChromosome(Length, m_minPosition, m_maxPosition);
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(GetRandomVertice());
        }

        public Vector2[] GetVertices()
        {
            return GetGenes().Select(g => (Vector2)g.Value).ToArray();
        }


        Vector2 GetRandomVertice()
        {
            var rnd = RandomizationProvider.Current;

            return new Vector2(
                rnd.GetFloat(m_minPosition.x, m_maxPosition.x),
                rnd.GetFloat(m_minPosition.y, m_maxPosition.y));
        }
    }
}
