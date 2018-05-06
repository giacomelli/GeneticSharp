using System.Linq;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class WallController : MonoBehaviour
    {
        private WallBuilderChromosome m_chromosome;

        public void SetChromosome(WallBuilderChromosome chromosome)
        {
            m_chromosome = chromosome;
        }

		private void Update()
        {
            if (!m_chromosome.Evaluated && GetComponentsInChildren<Rigidbody>().All(r => r.IsSleeping()))
            {
                m_chromosome.Evaluated = true;
            }
        }
    }
}