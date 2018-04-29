using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class RoadController : MonoBehaviour
    {
        private PathBuilder m_pathBuilder = new PathBuilder();
        public Object PathPrefab;

        public void Build(CarSampleConfig config)
        {
            m_pathBuilder.Build(config, this, transform.position);
        }
    }
}