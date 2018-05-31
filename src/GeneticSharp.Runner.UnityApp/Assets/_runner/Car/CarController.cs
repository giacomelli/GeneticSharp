using System.Collections;
using System.Linq;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class CarController : MonoBehaviour
    {
        private static Rect? s_lastCameraRect;
        private PolygonCollider2D m_polygon;
        private Rigidbody2D m_rb;
        private TextMesh m_fitnessText;
        private FollowChromosomeCam m_cam;
        private GameObject m_wheels;
        private CarSampleConfig m_config;
        private float m_startTime;
        private float m_currentVelocity;
        private float m_lastDistance;
        private float m_lastTime;
    

        public Object WheelPrefab;
        public float VectorMagnitudeMass = 2f;

        public float Distance { get; private set; }
        public float DistanceTime { get; set; }
        public CarChromosome Chromosome { get; private set; }

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

        }

      	private IEnumerator CheckTimeout()
        {
            var intervalLastDistance = Distance;
            var intervalLastTime = Time.time;
            yield return new WaitForSeconds(m_config.WarmupTime);
       
            do
            {
                // Check if car as the min velocity expected in the inteval.
                if(DistanceTime > 0 && CalculateVelocity(ref intervalLastDistance, ref intervalLastTime) < m_config.MinVelocity)
                {
                    StopEvaluation();
                    break;
                }
                yield return new WaitForSeconds(m_config.MinVelocityCheckTime);
            } while (true);
        }

        private float CalculateVelocity(ref float lastDistance, ref float lastTime)
        {
            var result = (Distance - lastDistance) / (Time.time - lastTime);

            lastDistance = Distance;
            lastTime = Time.time;

            return result;
        }

        private void StopEvaluation()
        {
            StopCoroutine("CheckTimeout");
            m_rb.Sleep();
            m_rb.isKinematic = true;

            foreach (var rb in m_wheels.GetComponentsInChildren<Rigidbody2D>())
            {
                rb.Sleep();
                rb.isKinematic = true;
            }

            foreach (var joint in m_wheels.GetComponentsInChildren<HingeJoint2D>())
            {
                joint.useMotor = false;
            }

            Chromosome.Evaluated = true;
            m_cam.StopFollowing();
        }

        void OnDestroy()
        {
            StopEvaluation();
        }

		private void OnCollisionEnter2D(Collision2D collision)
		{
            var other = collision.gameObject;

			if (other.tag == "DeadZone")
            {
                // When reach the road dead end, use the dead end position as max distance.
                if (transform.position.x > m_config.RoadMiddle && other.name.Equals("dead-end"))
                {
                    Chromosome.MaxDistance = other.transform.position.x;
                    UpdateFitnessText();
                }

                StopEvaluation();
            }
		}

     	private void Update()
		{
            if (!Chromosome.Evaluated)
            {
                CheckMaxDistance();
                UpdateFitnessText();
            }
        }

        private void UpdateFitnessText()
        {
            m_fitnessText.text = FormatFitnessText(Chromosome.MaxDistance, Chromosome.MaxDistanceTime);
            m_fitnessText.transform.rotation = Quaternion.identity;
        }

        private string FormatFitnessText(float distance, float time)
        {
            return time > 0
                ? $"{distance:N2}m - {(distance / time):N2}m/s\n{CalculateVelocity(ref m_lastDistance, ref m_lastTime):N2}m/s"
                   : "0m - 0m/s";
        }

        private void CheckMaxDistance()
        {
            Distance = transform.position.x;
            DistanceTime = Time.time - m_startTime;

            if (Distance > Chromosome.MaxDistance)
            {
                Chromosome.MaxDistance = Distance;
                Chromosome.MaxDistanceTime = DistanceTime;
            }
        }

		public void SetChromosome(CarChromosome chromosome, CarSampleConfig config)
        {
            Chromosome = chromosome;
            Chromosome.MaxDistance = 0;
            chromosome.MaxDistanceTime = 0;
            Distance = 0;
            DistanceTime = 0;
            m_startTime = Time.time;
            transform.rotation = Quaternion.identity;
           
            m_config = config;
            m_rb.isKinematic = false;
            m_rb.velocity = Vector2.zero;
            m_rb.angularVelocity = 0;

            var phenotypes = chromosome.GetPhenotypes();
            m_polygon.points = phenotypes.Select(p => p.Vector).ToArray();
            var wheelsMass = 0f;

            for (int i = 0; i < phenotypes.Length; i++)
            {
                var p = phenotypes[i];
                PrepareWheel(i, m_polygon.points[p.WheelIndex], p.WheelRadius);
                wheelsMass += p.WheelRadius;
            }

            // The car mass should be greater than wheels sum mass, because the WheelJoint2d get crazy otherwise.
            // If we comment the line bellow and enable the car mass should be greater than wheels sum mass, because the WheelJoint2d get crazy otherwise.
            m_rb.mass = 1 +  m_polygon.points.Sum(p => p.magnitude) * VectorMagnitudeMass + wheelsMass;

            if (m_cam != null)
            {
                m_cam.StartFollowing(gameObject);
            }

            StartCoroutine("CheckTimeout");
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

            var joint = wheel.GetComponent<HingeJoint2D>();

            joint.attachedRigidbody.isKinematic = false;
            joint.useMotor = true;
            joint.connectedBody = m_rb;
            joint.connectedAnchor = anchorPosition;
            joint.motor = new JointMotor2D { motorSpeed = m_config.MaxWheelSpeed * radius, maxMotorTorque = joint.motor.maxMotorTorque };
            joint.enabled = true;

            wheel.transform.localScale = new Vector3(radius, radius, 1);
            wheel.transform.localPosition = anchorPosition;
            wheel.SetActive(true);

            return wheel;
        }
	}
}