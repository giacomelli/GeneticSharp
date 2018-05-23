using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class RoadController : MonoBehaviour
    {
        private PathBuilder[] m_pathBuilders;
        private int m_pointsPerPath;
        public Object PathPrefab;
        public Object DeadEndPrefab;

        public void Build(CarSampleConfig config)
        {
            // Creates the path builders.
            if (m_pathBuilders == null)
            {
                var pathsCount = 1;

                if (config.GapsEachPoints > 0)
                {
                    pathsCount = Mathf.CeilToInt(config.PointsCount / config.GapsEachPoints) * 2;
                    m_pointsPerPath = config.GapsEachPoints;
                }
                else
                {
                    m_pointsPerPath = config.PointsCount / pathsCount;
                }

                m_pathBuilders = new PathBuilder[pathsCount];

                for (int i = 0; i < pathsCount; i++)
                {
                    m_pathBuilders[i] = new PathBuilder();
                }

                // Call each path builder to build the path.
                var start = (Vector2)transform.position;
                var end = start;
                var startPointIndex = 0;

                for (int i = 0; i < m_pathBuilders.Length; i++)
                {
                    end = m_pathBuilders[i].Build(config, this, start, m_pointsPerPath, startPointIndex);
                    start += new Vector2(config.MaxGapWidth * (i + 1) / m_pathBuilders.Length, 0);
                    startPointIndex += m_pointsPerPath - 1;
                }

                // Creates the dead-end.
                var deadEnd = Instantiate(DeadEndPrefab, end, Quaternion.identity) as GameObject;
                deadEnd.transform.SetParent(transform, false);
                deadEnd.name = "dead-end";
            }
        }
    }
}