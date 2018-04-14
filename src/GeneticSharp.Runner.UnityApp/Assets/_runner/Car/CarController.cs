using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarController : MonoBehaviour
    {
        private PolygonCollider2D m_polygon;
        private Rigidbody2D m_rb;
        private CarChromosome m_chromosome;
        public float Distance { get; private set; }
    
        private void Awake()
        {
            m_polygon = GetComponent<PolygonCollider2D>();
            m_rb = GetComponent<Rigidbody2D>();
        }

		private void Update()
		{
            Distance = Vector2.Distance(Vector2.zero, transform.position);

            if (m_rb.IsSleeping())
            {
                m_chromosome.Evaluated = true;
            }
  		}

		public void SetChromosome(CarChromosome chromosome)
        {
            m_chromosome = chromosome;
            m_polygon.points = chromosome.GetVectors();
        }
	}
}