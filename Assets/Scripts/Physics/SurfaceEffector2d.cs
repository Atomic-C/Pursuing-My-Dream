using UnityEngine;

/// <summary>
/// Surface Effector 2d doesnt work when using the player rigidbody for movement
/// The Platform_Movement script sets the player rigidbody velocity directly
/// So this class is a work-around fix for this problem 
/// </summary>
public class SurfaceEffector2d : MonoBehaviour
{
    /// <summary>
    /// This game object surface effector 2d 
    /// </summary>
    private SurfaceEffector2D _surfaceEffector2d;

    /// <summary>
    /// The player rigidbody 2d
    /// </summary>
    private Rigidbody2D _playerRB;

    /// <summary>
    /// Initialize the surface effector 2d variable
    /// </summary>
    void Start()
    {
        _surfaceEffector2d = GetComponent<SurfaceEffector2D>();
    }
    
    /// <summary>
    /// If the player is standing on the surface effector 2d, move (in the x axis) its transform or rigidbody using the effector speed property
    /// </summary>
    void FixedUpdate()
    {
        if (_playerRB != null)
            _playerRB.transform.position += new Vector3(_surfaceEffector2d.speed, 0f, 0f) * Time.deltaTime;
            //playerRB.AddForce(new Vector2(surfaceEffector2d.speed * Time.deltaTime, 0f), ForceMode2D.Impulse);
    }

    /// <summary>
    /// On trigger with the player, cache its rigidbody
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _playerRB = collision.GetComponent<Rigidbody2D>();            
    }

    /// <summary>
    /// On trigger exit with the player, remove its rigidbody reference, stopping the x axis movement
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _playerRB = null;
    }

    // Affects the player collider transform when in contact to the surface effector 2d, changing its x position depending on the effector speed property
    /*private void OnTriggerStay2D(Collider2D collision)
    {
        if(playerRB != null)
        {
            if (collision.CompareTag("Player"))
            {
                collision.transform.localPosition += new Vector3(surfaceEffector2d.speed, 0f, 0f) * Time.deltaTime;
            }
        }
    }*/
}
