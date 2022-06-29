using System.Collections;
using UnityEngine;

/// <summary>
/// Simple script that destroy this game object after the explosion animation ended
/// </summary>
public class Explosion_KnockBack : MonoBehaviour
{
    /// <summary>
    /// This object point effector 2D, if applicable
    /// </summary>
    private PointEffector2D _thisPointEffector2D;

    /// <summary>
    /// This object particle system
    /// </summary>
    private ParticleSystem _thisParticleSystem;

    private void Awake()
    {
        _thisParticleSystem = GetComponent<ParticleSystem>();
        _thisPointEffector2D = GetComponent<PointEffector2D>();
    }

    /// <summary>
    /// Call the DestroyFX function
    /// </summary>
    void Start()
    {
        StartCoroutine(DestroyFX());
    }

    /// <summary>
    /// Routine that play the explosion sound and destroy this game object after the particle system animation ended
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyFX()
    {
        yield return new WaitForSeconds(.2f);
        _thisPointEffector2D.enabled = false;
        yield return new WaitForSeconds(_thisParticleSystem.main.duration);
        Destroy(gameObject);
    }
}
