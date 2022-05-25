using System.Collections;
using UnityEngine;

/// <summary>
/// Class used by the exploding ball game object
/// The idea is simple: the ball explodes after a set time (lifeSpan) and if it collides with the player
/// </summary>
public class ExplodingBall : MonoBehaviour
{
    /// <summary>
    /// Game object that contains the particle system that simulates the explosion
    /// </summary>
    public GameObject destroyEffect;

    /// <summary>
    /// Timer before the ball explodes
    /// </summary>
    public float lifeSpan;

    /// <summary>
    /// Bool to detect the player collision
    /// </summary>
    private bool collidedWithPlayer = false;

    /// <summary>
    /// On collision, the ball explodes and the bool is set to true
    /// </summary>
    /// <param name="collision">The other object collision 2d</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            DestroyBall();
            collidedWithPlayer = true;
        }
    }

    /// <summary>
    /// Timer is decremented as time passes and the ball explodes after it is depleted, without colliding with the player
    /// The ball is destroyed after the collision 
    /// </summary>
    void Update()
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0 && collidedWithPlayer == false)
        {
            DestroyBall();
        }
    }

    /// <summary>
    /// Instantiate the explosion effect, play the sound explosion and destroy the ball
    /// </summary>
    private void DestroyBall()
    {
        Instantiate(destroyEffect, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), Quaternion.identity);
        AudioManager.instance.PlaySound("Explosion", gameObject.transform.position);
        Destroy(gameObject);
    }
}
