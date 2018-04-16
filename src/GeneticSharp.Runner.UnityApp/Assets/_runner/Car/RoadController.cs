using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace GeneticSharp.Runner.UnityApp.Car
{

    [RequireComponent(typeof(PolygonCollider2D))]
    public class RoadController : MonoBehaviour
    {
        private PolygonCollider2D m_polygon;

        public int PathsCount = 100;
        public float MinPathSize = 2;
        public float MaxPathSize = 4;
        public float MaxHeight = 1f;
    
        private void Awake()
        {
            m_polygon = GetComponent<PolygonCollider2D>();
            var points = new Vector2[PathsCount * 2];
            var startX = transform.position.x;
            var startY = transform.position.y;

            for (int i = 0; i < PathsCount; i++)
            {
                points[i] = new Vector2(startX + MaxPathSize * i, i % 2 == 0 ? startY : startY + MaxHeight);
            }

            for (int i = PathsCount; i < points.Length; i++)
            {
                var p = points[points.Length - i - 1];
                points[i] = new Vector2(p.x, p.y - 0.5f);
            }


            m_polygon.points = points;
        }

    }
}