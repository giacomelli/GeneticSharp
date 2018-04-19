using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    [RequireComponent(typeof(PolygonCollider2D))]
    [RequireComponent(typeof(LineRenderer))]
    public class DrawPolygon : MonoBehaviour
    {
        private PolygonCollider2D m_polygon;
        private LineRenderer m_lr;

		private void Awake()
		{
            m_polygon = GetComponent<PolygonCollider2D>();
            m_lr = GetComponent<LineRenderer>();
            m_lr.positionCount = 0;
		}

        private void Update()
        {
            m_lr.positionCount = m_polygon.GetTotalPointCount() + m_polygon.pathCount;
            var linePointIndex = 0;

            for (int i = 0; i < m_polygon.pathCount; i++)
            {
                var path = m_polygon.GetPath(i);

                for (int j = 0; j < path.Length; j++)
                {
                    m_lr.SetPosition(linePointIndex++, transform.TransformPoint(path[j]));
                }

                // Close the polygon connecting the last point with the first one.
                m_lr.SetPosition(path.Length, transform.TransformPoint(path[0]));
            }
        }
	}
}