using UnityEngine;

[CreateAssetMenu(menuName = "GeneticSharp/Car/CarSampleConfig", order = 1)]
public class CarSampleConfig : ScriptableObject {

    [Header("Road")]
    [Range(2, 1000)]
    public int PointsCount = 100;
    public float MaxPointsDistance = 4;
    public float MaxHeight = 1f;
    public float ZRotation = 0f;

    [Header("Gaps")]
    [Range(0, 1000)]
    public int GapsEachPoints = 0;
    public float MaxGapWidth = 1f;

    [Header("Obstacles")]
    public Vector2 MaxObstacleSize = new Vector2(5, 5);
    public int ObstaclesEachPoints = 5;
    public int MaxObstaclesPerPoint = 2;
    public float ObstaclesStartPoint = 10f;
    public float ObstaclesMass = 100f;
    public Object ObstaclePrefab;

    [Header("Evaluation")]
    public float WarmupTime = 10f;
    public float MinVelocityCheckTime = 5f;
    public float MinVelocity = 2f;

    [Header("Car")]
    [Range(3, 100)]
    public int VectorsCount = 8;

    [Range(2, 100)]
    public float MaxVectorSize = 10;

    [Header("Wheels")]
    [Range(0, 100)]
    public int WheelsCount = 2;

    [Range(1, 10)]
    public float MaxWheelRadius = 1;

    [Range(1, 1000)]
    public float MaxWheelSpeed = 800f;

    public float RoadLength
    {
        get 
        {
            return PointsCount * MaxPointsDistance;    
        }
    }

    public float RoadMiddle
    {
        get 
        {
            return RoadLength / 2f;
        }
    }
}
