using UnityEngine;

namespace GeneticSharp.Runner.UnityApp.Car
{
    [RequireComponent(typeof(WheelJoint2D))]
    [RequireComponent(typeof(LineRenderer))]
    public class DrawJoint : MonoBehaviour
    {
        private WheelJoint2D m_joint;
        private LineRenderer m_lr;

		private void Awake()
		{
            m_joint = GetComponent<WheelJoint2D>();
            m_lr = GetComponent<LineRenderer>();
            m_lr.positionCount = 2;
		}

        private void Update()
        {
            m_lr.SetPosition(0, m_joint.connectedBody.transform.TransformPoint(m_joint.connectedAnchor));
            m_lr.SetPosition(1, m_joint.transform.position);
        }
	}
}