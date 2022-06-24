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
    /// On collision with the player or its shoots, the ball explodes and its life span is set to 0
    /// </summary>
    /// <param name="collision">The other object collision 2d</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Bullet"))
        {
            lifeSpan = 0f;
        }
    }

    /// <summary>
    /// Timer is decremented as time passes and the ball explodes after it is depleted
    /// The ball is destroyed after the collision 
    /// </summary>
    void Update()
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0)
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
