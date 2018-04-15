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
        private GameObject m_wheels;
      
        public Object WheelPrefab;

        public Vector2Int SimulationsGrid { get; set; }
        public float Distance { get; private set; }
    
        private void Awake()
        {
            m_polygon = GetComponent<PolygonCollider2D>();
            m_rb = GetComponent<Rigidbody2D>();
            m_fitnessText = GetComponentInChildren<TextMesh>();
            m_wheels = transform.Find("Wheels").gameObject;
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
            Distance = transform.position.x;
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

            var wheelIndexes = chromosome.GetWheelsIndexes();
            var wheelRadius = chromosome.GetWheelsRadius();

            for (int i = 0; i < wheelIndexes.Length; i++)
            {
                PrepareWheel(i, m_polygon.points[wheelIndexes[i]], wheelRadius[i]);
            }

            if (m_cam != null)
            {
                m_cam.transform.position = transform.position;
                m_evaluatedEffect.enabled = false;
            }
        }

        private GameObject PrepareWheel(int index, Vector2 anchorPosition, float radius)
        {
            GameObject wheel;
            Transform wheelTransform = m_wheels.transform.childCount > index
                                               ? m_wheels.transform.GetChild(index)
                                               : null;

            if (wheelTransform == null)
            {
                wheel = Instantiate(WheelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                wheel.transform.SetParent(m_wheels.transform, false);
            }
            else
            {
                wheel = wheelTransform.gameObject;    
            }



            if (radius <= 0)
            {
                wheel.SetActive(false);
            }
            else
            {
                wheel.SetActive(true);
                wheel.transform.localScale = new Vector3(radius, radius, 1);
                var joint = wheel.GetComponent<WheelJoint2D>();
                joint.connectedBody = m_rb;
                joint.connectedAnchor = anchorPosition;
            }

            return wheel;
        }
	}
}