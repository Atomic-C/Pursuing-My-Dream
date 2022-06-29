using UnityEngine;

/// <summary>
/// Simple script that allows changing a single axis of a vector 3, without the need to set all three
/// </summary>
public static class VectorsExtension
{
    public static Vector3 WithAxis(this Vector3 vector, Axis axis, float value)
    {
        return new Vector3(
            axis == Axis.X ? value : vector.x,
            axis == Axis.Y ? value : vector.y,
            axis == Axis.Z ? value : vector.z
            );
    }
}

public enum Axis
{
    X, Y, Z
}
