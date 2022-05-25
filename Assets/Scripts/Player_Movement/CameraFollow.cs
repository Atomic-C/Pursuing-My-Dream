using UnityEngine;

/// <summary>
/// Class that controls the camera movement 
/// Mainly, by following the player position
/// </summary>
public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// Player transform, used to get its position 
    /// </summary>
    public Transform targetPosition;

    /// <summary>
    /// Float used to give the sensation of the camera going to the player position so its not instant
    /// </summary>
    public float smoothSpeed = 0.125f;

    /// <summary>
    /// Follow the player with a small delay (defined in the smoothSpeed variable)
    /// </summary>
    private void FixedUpdate()
    {
        SmoothFollow.instance.FollowObject(transform, targetPosition, new Vector3(0, 0, -1), smoothSpeed);
        
        // The camera angle follows the player, useful for 3d games
        //transform.LookAt(target); 

        // Lil debug to see if the player "is jumping" through its animation state
        //Debug.Log(playerAnimator.GetBool("Jump"));
    }
}
