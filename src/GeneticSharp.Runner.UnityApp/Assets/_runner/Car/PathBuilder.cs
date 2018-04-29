using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class PathBuilder 
    {
        private CarSampleConfig m_config;
        private GameObject m_obstacles;

        public void Build(CarSampleConfig config, RoadController road, Vector2 start)
        {
            if (m_config == null)
            {
                m_config = config;

                // Creates the path game object.
                var path = GameObject.Instantiate(road.PathPrefab) as GameObject;
                path.layer = LayerMask.NameToLayer("Floor");
                path.transform.SetParent(road.transform, false);
                path.transform.rotation = Quaternion.Euler(0, 0, m_config.ZRotation);

                var startX = start.x;
                var startY = start.y;
               
                // Gets the polygon component.
                var m_polygon = path.GetComponent<PolygonCollider2D>();


                var pathsCount = m_config.GapsRate > 0 ? Mathf.CeilToInt(m_config.PointsCount * m_config.GapsRate) : 1;
                m_polygon.pathCount = pathsCount;
                var pointsPerPathCount = m_config.PointsCount / pathsCount;

                var xIndex = 0;

                // Gets the obstacles container game object.
                m_obstacles = path.transform.Find("Obstacles").gameObject;
                m_obstacles.transform.parent = path.transform;

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
                    var obstacle = GameObject.Instantiate(m_config.ObstaclePrefab) as GameObject;
                    obstacle.GetComponent<ObstacleController>().Deploy(m_config, m_obstacles.transform, point + Vector2.up * (i + 1));
                    obstacle.GetComponent<Rigidbody2D>().mass = m_config.ObstaclesMass;
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
        }
    }
}