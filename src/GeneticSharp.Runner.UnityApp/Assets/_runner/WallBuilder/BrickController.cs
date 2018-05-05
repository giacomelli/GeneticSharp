using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class BrickController : MonoBehaviour
    {
        public bool HitFloor;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Floor")
            {
                HitFloor = true;
            }
        }

        private void OnGetFromPool()
        {
            gameObject.SetActive(true);
            HitFloor = false;
        }

        private void OnRelaseToPool()
        {
            gameObject.SetActive(false);
        }
    }
}