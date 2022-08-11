using UnityEngine;

/// <summary>
/// Script that maintain a given object in a fixed position
/// Controlled by the use of 3 variables, each for each axis (x, y and z)
/// </summary>
public class Fixed_Position : MonoBehaviour
{
    /// <summary>
    /// The fixed Y axis position the game object will stay
    /// </summary>
    public float yPosition;

    /// <summary>
    /// The fixed X axis position the game object will stay
    /// </summary>
    public float xPosition;

    /// <summary>
    /// The fixed Z axis position the game object will stay
    /// </summary>
    public float zPosition;

    /// <summary>
    /// Initialize the game object position 
    /// </summary>
    void Start()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(xPosition == 0 ? gameObject.transform.position.x : xPosition, 
                                                                yPosition == 0 ? gameObject.transform.position.y : yPosition, 
                                                                zPosition == 0 ? gameObject.transform.position.z : zPosition), Quaternion.identity);    
    }

    /// <summary>
    /// Maintain the position all axis, as dictated by their respective variables
    /// If the value is 0, use the same as the object this script is attached to
    /// </summary>
    void Update()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(xPosition == 0 ? gameObject.transform.position.x : xPosition,
                                                                yPosition == 0 ? gameObject.transform.position.y : yPosition,
                                                                zPosition == 0 ? gameObject.transform.position.z : zPosition), Quaternion.identity);
    }
}
 