using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.WallBuilder
{
    public class BrickController : MonoBehaviour
    {

        public int FloorHits;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag == "Floor")
            {
                FloorHits++;
            }
        }
    }
}