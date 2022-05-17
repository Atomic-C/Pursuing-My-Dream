using UnityEngine;

// Since the camera and the shooting gem will both follow the player using the same code, to avoid code repetition, this helper class will be used instead
/// <summary>
/// Helper class that controls the aspects of the follow movement
/// </summary>
public class SmoothFollow : MonoBehaviour
{
    /// <summary>
    /// Static instance of this class, using singleton design pattern
    /// </summary>
    public static SmoothFollow instance = null;

    /// <summary>
    /// Singleton design pattern to guarantee only one instance of this exists
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Function that generates the smooth following movement
    /// </summary>
    /// <param name="objectFollowing">Transform of the object that is following</param>
    /// <param name="objectToFollow">The target object transform of the following one</param>
    /// <param name="followOffset">Vector3 used to control the exact positioning of the following object</param>
    /// <param name="smoothSpeed">Float that controls the smoothness of the movement</param>
    public void FollowObject(Transform objectFollowing, Transform objectToFollow, Vector3 followOffset, float smoothSpeed)
    {
        // The actual position the player is (plus offset)
        Vector3 desiredPosition = objectToFollow.position + followOffset;

        // Using the Lerp function, it is possible to create the smooth movement of the camera
        Vector3 smoothedPosition = Vector3.Lerp(objectFollowing.position, desiredPosition, smoothSpeed);

        // Pass the smoothedPosition to the camera position
        objectFollowing.position = smoothedPosition;
    }
}
