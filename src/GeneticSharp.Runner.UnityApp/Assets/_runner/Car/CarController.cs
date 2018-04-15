using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarController : MonoBehaviour
    {
        private static Rect? s_lastCameraRect;
        private PolygonCollider2D m_polygon;
        private Rigidbody2D m_rb;
        private CarChromosome m_chromosome;
        private TextMesh m_fitnessText;
        private FollowChromosomeCam m_cam;
        private Grayscale m_evaluatedEffect;
        public Vector2Int SimulationsGrid { get; set; }

        public float Distance { get; private set; }
    
        private void Awake()
        {
            m_polygon = GetComponent<PolygonCollider2D>();
            m_rb = GetComponent<Rigidbody2D>();
            m_fitnessText = GetComponentInChildren<TextMesh>();
        }

        void Start()
        {
            m_cam = GameObject.Find("SimulationGrid")
                      .GetComponent<SimulationGrid>()
                      .AddChromosome(gameObject);

            m_evaluatedEffect = m_cam.GetComponent<Grayscale>();
        }

		private void Update()
		{
            Distance = Vector2.Distance(Vector2.zero, transform.position);
            m_fitnessText.text = Distance.ToString("N2");
            m_fitnessText.transform.rotation = Quaternion.identity;
        
            if (m_rb.IsSleeping())
            {
                m_chromosome.Evaluated = true;
                m_evaluatedEffect.enabled = true;
            }
  		}

		public void SetChromosome(CarChromosome chromosome)
        {
            m_chromosome = chromosome;
            m_polygon.points = chromosome.GetVectors();

            if (m_cam != null)
            {
                m_cam.transform.position = transform.position;
                m_evaluatedEffect.enabled = false;
            }
        }
	}
}