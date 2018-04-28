using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour {

    private Vector2 m_originalPosition;

    public void Deploy(CarSampleConfig config, Transform road, Vector2 point)
    {
        transform.position = point;
        m_originalPosition = transform.position;
        transform.SetParent(road, false);
        transform.localScale = new Vector3(config.MaxObstacleSize.x, config.MaxObstacleSize.y, 1);
    }
   
    public void Redeploy()
    {
        transform.position = m_originalPosition;
        transform.rotation = Quaternion.identity;
    }
}
