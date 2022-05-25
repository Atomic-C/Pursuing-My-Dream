using UnityEngine;

/// <summary>
/// Work around class used to fix a problem that was created with the camera target object creation.
/// The problem: the distortion effect created at the center of the black hole is made using a shader present in another object mesh renderer. This effect is always pointing towards
/// the camera. Probably because it was created it 3d games in mind. And since the camera is following an object slightly above the player, the effect dont work as intended
/// The solution: when the player touches the black hole, the camera target object has its position changed to be the same as the player, having the distortion effect working properly
/// and after the player leaves the black hole proximity, it goes back to its original position
/// </summary>
public class BlackHoleDistortion : MonoBehaviour
{
    /// <summary>
    /// On trigger enter 2d with the player, find the camera target object (since its a children of the player object) and changes its position to be the same as the player's
    /// </summary>
    /// <param name="collision">The other object collider2d</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform[] childrenTransform = collision.GetComponentsInChildren<Transform>();
            // I could have cached the object position as a Vector3 or Transform, but couldnt make it work. This resulted in weird values being passed to the camera target object
            // position, so i opted to this approach. Not the best, but it works.
            for (int i = 0; i < childrenTransform.Length; i++)
            {
                if (childrenTransform[i].CompareTag("CameraTarget"))
                {
                    childrenTransform[i].position = collision.transform.position;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// On trigger exit 2d with the player, find the camera target object (since its a children of the player object) and changes its position to its original one
    /// </summary>
    /// <param name="collision">The other object collider2d</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Transform[] childrenTransform = collision.GetComponentsInChildren<Transform>();
            // The same as above
            for (int i = 0; i < childrenTransform.Length; i++)
            {
                if (childrenTransform[i].CompareTag("CameraTarget"))
                {
                    childrenTransform[i].position = new Vector2(collision.transform.position.x, collision.transform.position.y + 1.7f);
                    break;
                }
            }
        }
    }
}
