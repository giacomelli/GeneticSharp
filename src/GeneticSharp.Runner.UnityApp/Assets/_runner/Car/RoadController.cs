using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class RoadController : MonoBehaviour
    {
        private PolygonCollider2D m_polygon;

        public RoadConfig Config;
    
        private void Awake()
        {
            m_polygon = GetComponent<PolygonCollider2D>();
            var startX = transform.position.x;
            var startY = transform.position.y;

            var pathsCount = Config.GapsRate > 0 ? Mathf.CeilToInt(Config.PointsCount * Config.GapsRate) : 1;
            m_polygon.pathCount = pathsCount;
            var pointsPerPathCount = Config.PointsCount / pathsCount;

            var xIndex = 0;

            for (int pathIndex = 0; pathIndex < pathsCount; pathIndex++)
            {
                var points = new Vector2[pointsPerPathCount * 2];
               
                for (int i = 0; i < pointsPerPathCount; i++)
                {
                    var x = startX + Config.MaxPointsDistance * xIndex++;
                    points[i] = new Vector2(x, startY + Mathf.Cos(x) * (Config.MaxHeight / Config.PointsCount) * xIndex);
                }

                startX += Config.MaxGapWidth;

                //  Closes the polygon.
                for (int i = pointsPerPathCount; i < points.Length; i++)
                {
                    var point = points[points.Length - i - 1];
                    points[i] = new Vector2(point.x, point.y - 0.5f);
                }

                m_polygon.SetPath(pathIndex, points);
            }
        }

    }
}