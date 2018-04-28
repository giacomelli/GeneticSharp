using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class RoadController : MonoBehaviour
    {
        private PolygonCollider2D m_polygon;
        private CarSampleConfig m_config;
        private GameObject m_obstacles;

        public Object ObstaclePrefab;
    
        public void Build(CarSampleConfig config)
        {
            if (m_config == null)
            {
                m_config = config;
                m_polygon = GetComponent<PolygonCollider2D>();
                var startX = transform.position.x;
                var startY = transform.position.y;
                transform.rotation = Quaternion.Euler(0, 0, config.ZRotation);

                var pathsCount = m_config.GapsRate > 0 ? Mathf.CeilToInt(m_config.PointsCount * m_config.GapsRate) : 1;
                m_polygon.pathCount = pathsCount;
                var pointsPerPathCount = m_config.PointsCount / pathsCount;

                var xIndex = 0;

                m_obstacles = transform.Find("Obstacles").gameObject;

                for (int pathIndex = 0; pathIndex < pathsCount; pathIndex++)
                {
                    var points = new Vector2[pointsPerPathCount * 2];

                    for (int i = 0; i < pointsPerPathCount; i++)
                    {
                        var x = startX + m_config.MaxPointsDistance * xIndex++;
                        points[i] = new Vector2(x, CalculateY(i, x, xIndex));

                        DeployObstacle(i, points[i]);
                    }

                    startX += m_config.MaxGapWidth;

                    //  Closes the polygon.
                    for (int i = pointsPerPathCount; i < points.Length; i++)
                    {
                        var point = points[points.Length - i - 1];
                        points[i] = new Vector2(point.x, point.y - 0.5f);
                    }

                    m_polygon.SetPath(pathIndex, points);
                }
            }
            else 
            {
                RedeployObstacles();
            }
        }

        private void DeployObstacle(int pointIndex, Vector2 point)
        {
            if (m_config.ObstaclesEachPoints > 0 && 
                point.x  >= m_config.ObstaclesStartPoint && 
                pointIndex % m_config.ObstaclesEachPoints == 0)
            {
                for (int i = 0; i < m_config.MaxObstaclesPerPoint; i++)
                {
                    var obstacle = Instantiate(ObstaclePrefab) as GameObject;
                    obstacle.GetComponent<ObstacleController>().Deploy(m_config, transform, point + Vector2.up * (i + 1));
                }
            }
        }

        private void RedeployObstacles()
        {
            for (int i = 0; i < m_obstacles.transform.childCount; i++)
            {
                m_obstacles.transform.GetChild(i).GetComponent<ObstacleController>().Redeploy();
            }
        }


        private float CalculateY(int pointIndex, float x, int xIndex)
        {
            return  Mathf.Cos(x) * (m_config.MaxHeight / m_config.PointsCount) * xIndex;
            // https://en.wikipedia.org/wiki/Sine_wave
            //return Mathf.Sin(x / Config.MaxHeight);
        }
    }
}