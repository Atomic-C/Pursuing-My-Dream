using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Player transform, used to get its position
    public Transform playerPosition;

    // Player animator, used to get the "Jump" bool parameter«
    // to know if the player jumped or not
    public Animator playerAnimator;

    // Float used to give the sensation of the camera going to the player position
    // so its not instant
    public float smoothSpeed = 0.125f;

    // Offset of the camera position. It is manipulated to create create an emphasis
    // in the player jump (the camera move a bit up)
    public Vector3 offset;

    // Weird bug with the camera that began happening outta nowhere.
    // The z axis of the camera transform began acting weird, "going behind" everything, making it unable to see anything
    private void Start()
    {
        offset.x = 0;
        offset.z = -2;
    }

    private void FixedUpdate()
    {
        // If the player jumped, manipulate the offset variable
        if (playerAnimator.GetBool("Jump") == true)
        {
            offset.y = 2;
        } else
        {
            offset.y = 1;
        }

        // The actual position the player is (plus offset)
        Vector3 desiredPosition = playerPosition.position + offset;

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
