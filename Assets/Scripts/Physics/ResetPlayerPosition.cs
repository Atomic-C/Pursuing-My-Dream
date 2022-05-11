using UnityEngine;

// Solution from the 'inifite fall' if the player fall from the level platforms
// A trigger was set below the platforms and if the player hit it, the game camera will play a fade out /
// fade in animation while the player position is reseted to the last position it was before falling
public class ResetPlayerPosition : MonoBehaviour
{
    // The player game object
    public GameObject player;
    // The camera fade animator
    public Animator cameraFade;
    // Bool used to determine if the player fell to their doom
    private bool playerFell;

    // Update is called once per frame
    // If the player fell, call the fade out function and reset the bool
    void Update()
    {
        if (playerFell)
        {
            CameraFadeOut();
            playerFell = false;
        }
    }

    // Player fell and hit the trigger: set the bool to true
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerFell = true;
        }
    }

    // Play the camera fade out animation and after it finishes, call the CameraFadeIn function
    private void CameraFadeOut()
    {
        cameraFade.SetTrigger("FadeOut");
        Invoke("CameraFadeIn", cameraFade.GetCurrentAnimatorStateInfo(0).length);
    }

    // Play the camera fade in animation and call the ResetPosition function
    private void CameraFadeIn()
    {
        cameraFade.SetTrigger("FadeIn");
        ResetPosition();
    }

    // Set the player position to that last position before it fell and reset its rigibody2d velocity
    private void ResetPosition()
    {
        player.transform.position = player.GetComponent<Platform_Movement>().lastPosition;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
