using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public float MinWheelRadius = 0.1f;
        public CarSampleConfig Config;
        public float Distance { get; private set; }
        public float MaxDistance { get; private set; }
    
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

		private IEnumerator CheckTimeout()
        {
            var lastMaxDistance = MaxDistance;
            yield return new WaitForSeconds(Config.WarmupTime);
       
            do
            {
                if (MaxDistance - lastMaxDistance < Config.MinMaxDistanceDiff)
                {
                    m_rb.Sleep();

                    foreach(var rb in m_wheels.GetComponentsInChildren<Rigidbody2D>())
                    {
                        rb.Sleep();
                    }

                    m_chromosome.Evaluated = true;
                    m_evaluatedEffect.enabled = true;
                    break;
                }

                lastMaxDistance = MaxDistance;
                yield return new WaitForSeconds(Config.TimeoutNoBetterMaxDistance);
            } while (true);
        }

        private void Update()
		{
            Distance = transform.position.x;

            if (Distance > MaxDistance)
            {
                MaxDistance = Distance;
            }

            var formattedDistance = Distance.ToString("N2");
            var formattedMaxDistance = MaxDistance.ToString("N2");

            if (formattedDistance == formattedMaxDistance)
            {
                m_fitnessText.text = formattedDistance;
            }
            else
            {
                m_fitnessText.text = $"{formattedDistance} . {formattedMaxDistance}";
            }

            m_fitnessText.transform.rotation = Quaternion.identity;
       	}

		public void SetChromosome(CarChromosome chromosome)
        {
            MaxDistance = 0;
            Distance = 0;
            m_chromosome = chromosome;
            m_rb.velocity = Vector2.zero;
            m_rb.angularVelocity = 0;
            m_polygon.points = chromosome.GetVectors();

            var wheelIndexes = chromosome.GetWheelsIndexes();
            var wheelRadius = chromosome.GetWheelsRadius();

            for (int i = 0; i < wheelIndexes.Length; i++)
            {
                PrepareWheel(i, m_polygon.points[wheelIndexes[i]], wheelRadius[i]);
            }

            // The car mass should be greater than wheels sum mass, because the WheelJoint2d get crazy otherwise.
            // If we comment the line bellow and enable the crazy otherwise.\\r\\n            // If we comment the line bellow and enable the car mass should be greater than wheels sum mass, because the WheelJoint2d get crazy otherwise.\\\\\\\\\\\\\\\\r\\\\\\\\\\\\\\\\n            // If we comment the line bellow and enable the crazy otherwise.\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\r\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\n            // If we comment the line bellow and enable the "Auto mass" on car gameobject, with 10 vectors and 20 wheels count 
            // we can see all crazy behaviours.
            m_rb.mass = 1 +  m_polygon.points.Sum(p => p.magnitude) + wheelRadius.Sum();

            if (m_cam != null)
            {
                m_cam.transform.position = transform.position;
                m_evaluatedEffect.enabled = false;
            }

            StartCoroutine(CheckTimeout());
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

            var joint = wheel.GetComponent<AnchoredJoint2D>();
           
            joint.connectedBody = m_rb;
            joint.connectedAnchor = anchorPosition;
            joint.enabled = true;

            wheel.transform.localScale = new Vector3(radius, radius, 1);
            wheel.transform.localPosition = anchorPosition;
            wheel.SetActive(true);

            return wheel;
        }
	}
}