using UnityEngine;

/// <summary>
/// Simple class used to animate any sprite, in a rotation motion based on one of the 3 axis: x, y or z
/// </summary>
public class Axis_Rotation : MonoBehaviour
{
    /// <summary>
    /// Enum used to dynamically set which axis to rotate
    /// </summary>
    public WhichAxis axis;

    /// <summary>
    /// The rotation speed
    /// </summary>
    public int speed = 200;

    /// <summary>
    /// Vector3 used in the Rotate function
    /// </summary>
    private Vector3 _rotation;

    /// <summary>
    /// Set what speed to rotate the respective axis
    /// </summary>
    private void FixedUpdate()
    {
        switch (axis)
        {
            case WhichAxis.X:
                _rotation.x = speed;
                break;
            case WhichAxis.Y:
                _rotation.y = speed;
                break;
            case WhichAxis.Z:
                _rotation.z = speed;
                break;
        }
    }

    /// <summary>
    /// Set the speed to rotate the axis, at start
    /// </summary>
    void Update()
    {
        this.gameObject.transform.Rotate(_rotation * Time.deltaTime);
    }

    /// <summary>
    /// Enum used to specify which axis to rotate
    /// </summary>
    public enum WhichAxis
    {
        X,
        Y,
        Z
    } 
}
