using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Player transform, used to get its position
    public Transform playerPosition;

    // Float used to give the sensation of the camera going to the player position
    // so its not instant
    public float smoothSpeed = 0.125f;

    private void FixedUpdate()
    {
        // The actual position the player is (plus offset)
        Vector3 desiredPosition = playerPosition.position + new Vector3(0,0,-1);

        // Using the Lerp function, it is possible to create the smooth movement of the camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Pass the smoothedPosition to the camera position
        transform.position = smoothedPosition;

        // The camera angle follows the player, useful for 3d games
        //transform.LookAt(target); 

        // Lil debug to see if the player "is jumping" through its animation state
        //Debug.Log(playerAnimator.GetBool("Jump"));
    }
}
