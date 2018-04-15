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
            m_lr.positionCount = m_polygon.points.Length;

            for (int i = 0; i < m_polygon.points.Length; i++)
            {
                m_lr.SetPosition(i, transform.TransformPoint(m_polygon.points[i]));
            }
        }
	}
}