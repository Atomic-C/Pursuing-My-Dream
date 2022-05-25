using UnityEngine;

/// <summary>
/// The idea: to have a game object just outside of the camera view, so to create a cool effect of plant pollen that comes flying towards the player
/// The tweak: this class is used to make this game object (which has a particle system attached, responsible for creating the flying pollen)
/// have a fixed Y axis position, but still follows the camera in the X axis
/// </summary>
public class Fixed_Y_Position : MonoBehaviour
{
    /// <summary>
    /// The fixed Y axis position the game object will stay
    /// </summary>
    public float yPosition;

    /// <summary>
    /// The camera position
    /// </summary>
    public Transform cameraPosition;

    /// <summary>
    /// Initialize the game object position 
    /// </summary>
    void Start()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(gameObject.transform.position.x, yPosition, gameObject.transform.position.z), Quaternion.identity);    
    }

    /// <summary>
    /// Maintain the position in the Y axis, as dictated by the float yPosition
    /// </summary>
    void Update()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(gameObject.transform.position.x, yPosition, gameObject.transform.position.z), Quaternion.identity);
    }
}
 