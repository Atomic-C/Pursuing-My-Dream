using UnityEngine;

/// <summary>
/// Class responsible to control the player movement when on top of a moving platform
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform_x_Player : MonoBehaviour
{
    /// <summary>
    /// The platform rigid body 2d
    /// </summary>
    private Rigidbody2D _platformRB;

    /// <summary>
    /// Initialize the platform rigidbody 2d variable
    /// </summary>
    private void Start()
    {
        _platformRB = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// On trigger stay with the player, moves its position alongside the platform's
    /// </summary>
    /// <param name="collision">The other object collider</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position += new Vector3(_platformRB.velocity.x, 0f, 0f) * Time.deltaTime;
        }
    }
}
