using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script to destroy / release the game object that contains the explosion particle system
/// Also used to do area of effect damage with the guided bullet
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    /// <summary>
    /// Layer mask used to filter the target
    /// </summary>
    public LayerMask targetDummyMask;

    /// <summary>
    /// Bool that defines if this object belongs to a pooled bullet
    /// </summary>
    private bool _fromPooledObject;

    /// <summary>
    /// Bool used to determine which explosion sound will play
    /// </summary>
    private bool _alternateShoot;

    /// <summary>
    /// Collider set as trigger, used in the area of effect damage mechanic
    /// </summary>
    private CircleCollider2D _aoeRange;

    /// <summary>
    /// Cache the bullet strength of the one that instantiated this effect
    /// </summary>
    private float _bulletStrength;

    /// <summary>
    /// Cache the transform of the bullet that instantiated this effect
    /// </summary>
    private Transform _bulletTransform;

    /// <summary>
    /// This object particle system
    /// </summary>
    private ParticleSystem _thisParticleSystem;

    /// <summary>
    /// This object point effector 2D, if applicable
    /// </summary>
    private PointEffector2D _thisPointEffector2D;

    /// <summary>
    /// This object sprite renderer
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Cache the necessary variables
    /// </summary>
    private void Awake()
    {
        _thisParticleSystem = gameObject.GetComponent<ParticleSystem>();
        _aoeRange = gameObject.GetComponent<CircleCollider2D>();
        _thisPointEffector2D = gameObject.TryGetComponent<PointEffector2D>(out PointEffector2D pointEffector2D) ? pointEffector2D : null;
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    /// <summary>
    /// Call the DestroyFX function if this not belongs to a pooled bullet
    /// </summary>
    void Start()
    {
        if(!_fromPooledObject)
            StartCoroutine(DestroyFX());        
    }

    /// <summary>
    /// Routine that play the explosion sound and destroy this game object after the particle system animation ended
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyFX()
    {
        // This script is used for both the guided as its alternate projectiles, so the ternary is to differentiate which sound will play based on that
        AudioManager.instance.PlaySound(_alternateShoot ? "GuidedMiniExplosion" : "GuidedExplosion", transform.position);
        yield return new WaitForSeconds(.2f);
        if(_thisPointEffector2D != null)
            _thisPointEffector2D.enabled = false;
        yield return new WaitForSeconds(_thisParticleSystem.main.duration);
        Destroy(gameObject);
    }

    /// <summary>
    /// Function used when this object belongs to a pooled bullet
    /// Instead of destroying it outright, simply deactivate it for next usage
    /// </summary>
    /// <param name="bulletPosition">The last position of the bullet before being deactivated</param>
    public void Activate(Vector2 bulletPosition)
    {
        gameObject.transform.position = bulletPosition;
        AudioManager.instance.PlaySound(_alternateShoot ? "GuidedMiniExplosion" : "GuidedExplosion", transform.position);
        gameObject.SetActive(true);

        Invoke("Deactivate", _thisParticleSystem.main.duration);
    }

    /// <summary>
    /// Function that is called after the particle system finished it animation
    /// Simply deactivate the object
    /// </summary>
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Function that is responsible for setting up all the necessary variables / references this script needs
    /// </summary>
    /// <param name="fromPooledObject">The object that instantiated does belong to a pool?</param>
    /// <param name="alternateShoot">This object is being used as the main or alternate version of the bullet?</param>
    /// <param name="bulletStrength">The strength of the bullet that instantiated this</param>
    /// <param name="bulletTransform">The transform of the bullet that instantiated this</param>
    public void SetupReferences(bool fromPooledObject, bool alternateShoot, float bulletStrength, Transform bulletTransform)
    {
        _fromPooledObject = fromPooledObject;
        _alternateShoot = alternateShoot;
        _bulletStrength = bulletStrength;
        _bulletTransform = bulletTransform;
    }

    /// <summary>
    /// Function responsible for the target dummy area of effect damage
    /// By detecting all colliders (filtered by the selected layer mask) inside the radius of the aoeRange trigger collider, if there is any, call its Hit function
    /// </summary>
    public void AoEDamage()
    {
        // List that will hold all the detected colliders
        List<Collider2D> enemyColliders = new List<Collider2D>();

        ContactFilter2D contactFilter2D = new ContactFilter2D();
        // Set the contact filter layer mask
        contactFilter2D.SetLayerMask(targetDummyMask);

        // Detect all colliders in the radius that belongs to the selected layer
        _aoeRange.OverlapCollider(contactFilter2D, enemyColliders);

        // If there is any
        if(enemyColliders.Count != 0)
        {
            // Call its Hit function
            foreach(Collider2D collider in enemyColliders)
            {
                collider.GetComponent<EnemyHealth>().Hit(_bulletStrength, _bulletTransform);
            }
        }
    }

    /// <summary>
    /// Make use or not of the glow effect
    /// </summary>
    /// <param name="activate">Bool to activate / deactivate the effect</param>
    public void SetGlowEffect(bool activate, Color color, float intensity)
    {
        _spriteRenderer.material.SetInt("_UseEmission", activate ? 1 : 0);
        _spriteRenderer.material.SetColor("_GlowColor", color);
        _spriteRenderer.material.SetFloat("_EmissionIntensity", intensity);
    }
}
