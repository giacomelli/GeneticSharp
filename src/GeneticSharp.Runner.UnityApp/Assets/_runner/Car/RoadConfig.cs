using UnityEngine;

[CreateAssetMenu(menuName = "GeneticSharp/Car/RoadConfig", order = 1)]
public class RoadConfig : ScriptableObject {

    public int PointsCount = 100;
    public float MinPointsDistance = 2;
    public float MaxPointsDistance = 4;

    public float MaxHeight = 1f;
    public float GapsRate = 0.1f;
    public float MaxGapWidth = 1f;

}
