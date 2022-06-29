using UnityEngine;

/// <summary>
/// Class that controls the guided bullet alternate projectile
/// </summary>
public class MiniGuidedBullet : MonoBehaviour
{
    /// <summary>
    /// This projectile statistics
    /// </summary>
    public float strenght, speed;

    /// <summary>
    /// Life span timer
    /// </summary>
    public float shootTimer;

    /// <summary>
    /// The position of the target detected
    /// </summary>
    public Vector2 target;

    /// <summary>
    /// The prefab of its explosion effect
    /// </summary>
    public ExplosionEffect explosionEffectPrefab;

    /// <summary>
    /// Bool that determine if this belongs to a pooled object
    /// </summary>
    public bool fromPooledObject;

    /// <summary>
    /// Make use or not of the glow effect
    /// </summary>
    public bool useEmission;

    [Header("Echo effect is not perfomance friendly")]
    /// <summary>
    /// Enum used to define which trail effect the guided shoot will have
    /// </summary>
    public Bullet.TrailType trailType;

    /// <summary>
    /// Its rigidbody 2D
    /// </summary>
    private Rigidbody2D _thisRigidbody2D;

    /// <summary>
    /// Its rotation controlling script
    /// </summary>
    private Axis_Rotation _axis_Rotation;

    /// <summary>
    /// Variable that will hold the instantiated explosion prefab
    /// </summary>
    private ExplosionEffect _explosionEffect;

    /// <summary>
    /// Its echo effect script
    /// </summary>
    private EchoEffect _echoEffect;

    /// <summary>
    /// Its trail renderer component
    /// </summary>
    private TrailRenderer _trailRenderer;

    /// <summary>
    /// The actual timer that will dictate its life span
    /// </summary>
    private float _actualShootTimer;

    /// <summary>
    /// Bool used in finetuning the behavior of the guided shoot variant 
    /// </summary>
    private bool _foundTarget;

    /// <summary>
    /// This object sprite renderer
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Cache the necessary variables
    /// </summary>
    // Start is called before the first frame update
    void Awake()
    {
        _thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _actualShootTimer = Random.Range(shootTimer / 2, shootTimer * 2);
        _axis_Rotation = gameObject.GetComponent<Axis_Rotation>();
        _echoEffect = gameObject.GetComponent<EchoEffect>();
        _echoEffect.enabled = false;
        _trailRenderer = gameObject.GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Instantiate its explosion effect, if it belongs to a pooled object (main guided bullet)
    /// </summary>
    private void Start()
    {
        if (fromPooledObject)
        {
            ExplosionEffect explosionEffect = Instantiate(explosionEffectPrefab);
            explosionEffect.SetupReferences(fromPooledObject, true, strenght, transform);
            _explosionEffect = explosionEffect;
            _explosionEffect.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TrackTarget();
        GuidedShootLifeSpan();
    }

    /// <summary>
    /// This projectile is instantiated in a random radius from the main guided bullet. Initially was using a collider, but caused some problems when hitting a target
    /// The projectile was being instantiated inside the target, making it have weird behavior (e.g. teleporting inside other objects)
    /// So, by using a trigger collider, this problem can be avoided while making it work as intended
    /// </summary>
    /// <param name="collision">The other object collider</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // It will only be destroyed when touching anything but the range limit
        // Its already inside the trigger circle collider, so this is needed in order to avoid it exploding instantly
        if (!collision.CompareTag("RangeLimit"))
            DestroyBehavior();      
    }

    /// <summary>
    /// Explodes when exiting the range trigger collider
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("RangeLimit"))
            DestroyBehavior();
    }

    /// <summary>
    /// On destroy behavior
    /// If it belong to a pooled object: destroy its pre-instantiated explosion effect, if it exists
    /// If not: instantiate the explosion effect, set all its necessary variables and references and call its area of effect damage function
    /// </summary>
    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            if (fromPooledObject && _explosionEffect != null)
                Destroy(_explosionEffect.gameObject);
            else if (!fromPooledObject)
            {
                ExplosionEffect explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
                explosionEffect.SetupReferences(fromPooledObject, true, strenght, transform);
                explosionEffect.AoEDamage();
                _explosionEffect = explosionEffect;
            }   
        }
    }

    /// <summary>
    /// Call the DestroyBehavior function when its life span is depleted
    /// </summary>
    private void GuidedShootLifeSpan()
    {
        _actualShootTimer -= Time.deltaTime;
        if (_actualShootTimer <= 0f)
        {
            DestroyBehavior();
        }
    }

    /// <summary>
    /// Its destroy behavior
    /// If it belongs to a pooled object: reset its variables (timers, target, etc), activate its explosion effect, call its area of effect damage function and deactiva this object
    /// </summary>
    private void DestroyBehavior()
    {
        if (fromPooledObject)
        {
            ResetMiniBullet();
            _explosionEffect.Activate(transform.position);
            _explosionEffect.AoEDamage();
            gameObject.SetActive(false);
        } else
            Destroy(gameObject);
    }

    /// <summary>
    /// Target tracking function, the same as the guided bullet one
    /// </summary>
    private void TrackTarget()
    {
        Transform closestTarget = Radar.instance.TargetPosition();

        if (closestTarget != null)
        {
            target = closestTarget.position;
            _foundTarget = true;
            // Doubles the rotation animation speed
            _axis_Rotation.speed = 800;
            // Zero the rigidbody velocity. Since this dont use the built-in physics, this is to prevent the move towards function struggling with the rigidbody velocity
            _thisRigidbody2D.velocity = Vector2.zero;
            transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, SpeedBoost() * Time.deltaTime);
        }

        if (!_foundTarget)
        {
            _axis_Rotation.speed = 400;
            _thisRigidbody2D.velocity = target.normalized * this.speed;
        }


        // Activate / deactivate the selected trail effect
        switch (trailType)
        {
            case Bullet.TrailType.TRAILRENDERER:
                _trailRenderer.enabled = _foundTarget;
                break;
            case Bullet.TrailType.ECHOEFFECT:
                _echoEffect.enabled = _foundTarget;
                break;
        }
    }

    /// <summary>
    /// Return the double of its speed
    /// </summary>
    /// <returns>Doubled speed</returns>
    private float SpeedBoost()
    {
        return speed * 2;
    }

    /// <summary>
    /// Activate this object is a random radius from the main bullet
    /// </summary>
    /// <param name="bulletPosition">The position of the main bullet</param>
    /// <param name="spreadOffSet">Used as a random position in a radius from the main bullet</param>
    public void Activate(Vector2 bulletPosition, float spreadOffSet)
    {
        transform.position = new Vector2(Random.Range(bulletPosition.x - spreadOffSet, bulletPosition.x + spreadOffSet),
                                         Random.Range(bulletPosition.y - spreadOffSet, bulletPosition.y + spreadOffSet));
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Reset its necessary variables
    /// No target detected and randomly sets its life span
    /// </summary>
    public void ResetMiniBullet()
    {
        _foundTarget = false;
        _actualShootTimer = Random.Range(shootTimer / 2, shootTimer * 2);
    }

    /// <summary>
    /// Initially set all of its variables
    /// </summary>
    /// <param name="target">Its target</param>
    /// <param name="range">Projectile range</param>
    /// <param name="speed">Projectile speed</param>
    /// <param name="strenght">Projectile strenght</param>
    /// <param name="fromPooledObject">If it belongs to a pooled object</param>
    public void SetupMiniBullet(Vector2 target, float speed, float strenght, bool fromPooledObject)
    {
        this.target = target;
        this.speed = speed;
        this.strenght = strenght;
        this.fromPooledObject = fromPooledObject;
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
