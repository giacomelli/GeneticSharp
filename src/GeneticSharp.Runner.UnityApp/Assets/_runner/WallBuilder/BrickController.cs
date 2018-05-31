using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class BrickController : MonoBehaviour
    {
        public bool HitFloor { get; private set; }
        public int HitBricksCount { get; private set; }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Floor")
            {
                HitFloor = true;
            }
            else 
            {
                HitBricksCount++;
            }
        }

        private void OnGetFromPool()
        {
            gameObject.SetActive(true);
            HitFloor = false;
            HitBricksCount = 0;
        }

        private void OnRelaseToPool()
        {
            gameObject.SetActive(false);
        }
    }
}