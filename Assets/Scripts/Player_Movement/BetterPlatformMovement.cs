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
    private Rigidbody2D rb;

    /// <summary>
    /// Cache the player rigidbody 2d
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Check if the player is falling and if it is, apply the fallMultiplier variable to its rigibody2d y velocity, making it fall faster
    /// Or check if the player is jumping but not holding the jump button and if it is, apply the lowJumpMultiplier to its rigibody2d y velocity, making it jump lower
    /// </summary>
    private void Update()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
    }
}
