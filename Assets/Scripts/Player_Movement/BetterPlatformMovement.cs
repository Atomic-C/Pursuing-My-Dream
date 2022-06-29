using UnityEngine;

/// <summary>
/// Class that is responsible for adding the Mario old games jumping feels
/// Quick jump button press > low jump
/// Holding the jump button > high jump
/// </summary>
public class BetterPlatformMovement : MonoBehaviour
{
    /// <summary>
    /// Float used as a multiplier for the gravity, when the player is falling down after a high jump (holding the jump button)
    /// </summary>
    public float fallMultiplier = 2.5f;

    /// <summary>
    /// Float used as a multiplier for the gravity when the player quickly pressed the jump button, making it do a low jump
    /// </summary>
    public float lowJumpMultiplier = 2f;

    /// <summary>
    /// This object rigidbody2D
    /// </summary>
    private Rigidbody2D _rb;

    /// <summary>
    /// Cache the player rigidbody 2d
    /// </summary>
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Check if the player is falling and if it is, apply the fallMultiplier variable to its rigibody2d y velocity, making it fall faster
    /// Or check if the player is jumping but not holding the jump button and if it is, apply the lowJumpMultiplier to its rigibody2d y velocity, making it jump lower
    /// </summary>
    private void Update()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (_rb.velocity.y > 0 && !Input.GetButton("Jump"))
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }
}
