using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class PathBuilder 
    {
        public float Height { get; set; } = 10f;
        private CarSampleConfig m_config;
        private GameObject m_obstacles;
        private PolygonCollider2D m_polygon;

        public Vector2 Build(CarSampleConfig config, RoadController road, Vector2 start, int pointsCount, int startPointIndex)
        {
            if (m_config == null)
            {
                m_config = config;

                // Creates the path game object.
                var path = Object.Instantiate(road.PathPrefab) as GameObject;
                path.layer = LayerMask.NameToLayer("Floor");
                path.transform.SetParent(road.transform, false);
                path.transform.rotation = Quaternion.Euler(0, 0, m_config.ZRotation);

                // Gets the polygon component.
                m_polygon = path.GetComponent<PolygonCollider2D>();
                m_polygon.pathCount = pointsCount;
            
                var xIndex = startPointIndex;

                // Gets the obstacles container game object.
                m_obstacles = path.transform.Find("Obstacles").gameObject;
                m_obstacles.transform.parent = path.transform;

                var points = new Vector2[pointsCount * 2];

                for (int i = 0; i < pointsCount; i++)
                {
                    var x = start.x + m_config.MaxPointsDistance * xIndex++;
                    points[i] = new Vector2(x, CalculateY(x, xIndex));

                    DeployObstacle(i, points[i], xIndex);
                }

                //  Closes the polygon.
                for (int i = pointsCount; i < points.Length; i++)
                {
                    var point = points[points.Length - i - 1];
                    points[i] = new Vector2(point.x, point.y - Height);
                }

                m_polygon.points = points;
            }
            else 
            {
                RedeployObstacles();
            }

            return m_polygon.points[pointsCount - 1];
        }

        private void DeployObstacle(int pointIndex, Vector2 point, int xIndex)
        {
            if (m_config.ObstaclesEachPoints > 0 && 
                point.x  >= m_config.ObstaclesStartPoint && 
                pointIndex % m_config.ObstaclesEachPoints == 0 &&
                pointIndex < m_config.PointsCount - 1)
            {
                for (int i = 0; i < m_config.MaxObstaclesPerPoint; i++)
                {
                    var obstacle = Object.Instantiate(m_config.ObstaclePrefab) as GameObject;
                    obstacle.transform.localScale = m_config.MaxObstacleSize * xIndex / m_config.PointsCount;
                    obstacle.GetComponent<ObstacleController>().Deploy(m_config, m_obstacles.transform, point + Vector2.up * obstacle.transform.localScale * (i + 1));
                    obstacle.GetComponent<Rigidbody2D>().mass = m_config.ObstaclesMass * xIndex / m_config.PointsCount;
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


        private float CalculateY(float x, int xIndex)
        {
            if(m_config.MaxHeight == 0)
            {
                return 0;
            }

            return (Mathf.Cos(x) / (m_config.PointsCount / xIndex) * m_config.MaxHeight);
        }
    }
}