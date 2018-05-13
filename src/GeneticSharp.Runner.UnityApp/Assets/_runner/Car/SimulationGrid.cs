using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    public class SimulationGrid : MonoBehaviour
    {
        private ConcurrentQueue<FollowChromosomeCam> m_availableCameras;

        public Object CameraPrefab;
        public CarSampleController Sample;

		private void Awake()
		{
            m_availableCameras = new ConcurrentQueue<FollowChromosomeCam>();

            for (int x = 0; x < Sample.SimulationsGrid.x; x++)
            { 
                for (int y = 0; y < Sample.SimulationsGrid.y; y++)
                {
                    var cam = ((GameObject)Instantiate(CameraPrefab, Vector3.zero, Quaternion.identity)).GetComponent<Camera>();
                    cam.transform.parent = transform.parent;
                
                    var width = cam.pixelRect.width / Sample.SimulationsGrid.x;
                    var height = cam.pixelRect.height / Sample.SimulationsGrid.y;

                    cam.pixelRect = new Rect(x * width, y * height, width, height);
                    m_availableCameras.Enqueue(cam.GetComponent<FollowChromosomeCam>());
                }
            }
		}

		public FollowChromosomeCam AddChromosome(GameObject chromosome)
        {
            FollowChromosomeCam cam;

            if (m_availableCameras.TryDequeue(out cam))
            {
                cam.StartFollowing(chromosome);
            }
            else
            {
                Debug.LogError("Cannot dequeue camera");    
            }

            return cam;
        }
    }
}