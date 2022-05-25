using UnityEngine;

/// <summary>
/// Solution from the 'inifite fall' if the player fall from the level platforms
/// A trigger was set below the platforms and if the player hit it, the game camera will play a fade out
/// fade in animation while the player position is reseted to the last position it was before falling
/// </summary>
public class ResetPlayerPosition : MonoBehaviour
{
    /// <summary>
    /// The player game object
    /// </summary>
    public GameObject player;

    /// <summary>
    /// The camera fade animator
    /// </summary>
    public Animator cameraFade;

    /// <summary>
    /// Bool used to determine if the player fell to their doom
    /// </summary>
    private bool playerFell;

    /// <summary>
    /// If the player fell, call the fade out function and reset the bool
    /// </summary>
    void Update()
    {
        if (playerFell)
        {
            CameraFadeOut();
            playerFell = false;
        }
    }

    /// <summary>
    /// Player fell and hit the trigger: set the bool to true
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerFell = true;
        }
    }

    /// <summary>
    /// Play the camera fade out animation and after it finishes, call the CameraFadeIn function
    /// </summary>
    private void CameraFadeOut()
    {
        cameraFade.SetTrigger("FadeOut");
        Invoke("CameraFadeIn", cameraFade.GetCurrentAnimatorStateInfo(0).length);
    }

    /// <summary>
    /// Play the camera fade in animation and call the ResetPosition function
    /// </summary>
    private void CameraFadeIn()
    {
        cameraFade.SetTrigger("FadeIn");
        ResetPosition();
    }

    /// <summary>
    /// Set the player position to that last position before it fell and reset its rigibody2d velocity
    /// </summary>
    private void ResetPosition()
    {
        player.transform.position = player.GetComponent<Platform_Movement>().lastPosition;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
