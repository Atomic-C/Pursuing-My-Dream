using UnityEngine;

/// <summary>
/// Found a nice black hole effect but since its kinda advanced (shader manipulation), i didnt knew how to fix a problem
/// The effect is always pointing at the camera, which in a 2d environment get weird (i think it was made with 3d in mind)
/// So, the work-around is simple: the effect only activates when the player enter in contact with it, which, at a close distance
/// the effect works alright
/// </summary>
public class DistortionEffectWorkAround : MonoBehaviour
{
    /// <summary>
    /// This game object mesh renderer
    /// </summary>
    private MeshRenderer _distortionEffect;

    /// <summary>
    /// Initialize the distortionEffect variable
    /// </summary>
    void Start()
    {
        _distortionEffect = gameObject.GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Enable the distortion effect on trigger with the player
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _distortionEffect.enabled = true;
    }

    /// <summary>
    /// Disable the distortion effect on trigger exit with the player
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _distortionEffect.enabled = false;
    }
}
