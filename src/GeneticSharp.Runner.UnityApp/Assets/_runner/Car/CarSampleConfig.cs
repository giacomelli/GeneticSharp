using UnityEngine;

[CreateAssetMenu(menuName = "GeneticSharp/Car/CarSampleConfig", order = 1)]
public class CarSampleConfig : ScriptableObject {

    public int PointsCount = 100;
    public float MinPointsDistance = 2;
    public float MaxPointsDistance = 4;

    public float MaxHeight = 1f;
    public float GapsRate = 0.1f;
    public float MaxGapWidth = 1f;

    public float WarmupTime = 10f;
    public float TimeoutNoBetterMaxDistance = 5f;
    public float MinMaxDistanceDiff = 1f;
}
