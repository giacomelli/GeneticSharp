using System;
public class TspCity
{
    public TspCity(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; }
    public float Y { get; }

    public double Distance(TspCity other)
    {
        return Math.Sqrt(
            Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2)
        );
    }
}