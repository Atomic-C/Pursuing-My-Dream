using UnityEngine;

/// <summary>
/// Class that controls the target dummy
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    /// <summary>
    /// Enemy max health
    /// </summary>
    public float maxHealth;

    [Header("Health bar control")]
    /// <summary>
    /// Float used as a timer before it goes back to its iddle animations
    /// </summary>
    public float backToIddleTimer;

    /// <summary>
    /// Float used as a timer to hide the healthbar
    /// </summary>
    public float hideHealthBarTimer;

    /// <summary>
    /// Gameobject representing the enemy healthbar
    /// </summary>
    public GameObject healthBar;

    [Header("Animation control")]
    /// <summary>
    /// Bool that determine if it is facing left, used in the Flip function 
    /// </summary>
    public bool facingLeft;

    /// <summary>
    /// How many iddle animations this enemy has
    /// </summary>
    public int iddleAnimationsCount;

    [Header("Drops")]
    /// <summary>
    /// The percentage this enemy has to drop anything
    /// </summary>
    public float dropChance;

    /// <summary>
    /// Bool that limit this enemy to drop a single item per drop (will keep checking for drop chances until the first one drops)
    /// </summary>
    public bool singleDrop;

    /// <summary>
    /// Array of game objects containing all the drops from this enemy
    /// </summary>
    public EnemyDrop[] enemyDrops;

    /// <summary>
    /// The actual timer to hide the healthbar, that will be affected by the passing of time
    /// </summary>
    private float _actualHideHealthBarTimer;

    /// <summary>
    /// The transform that will be manipulated to simulate the effect of the bar diminishing
    /// </summary>
    private GameObject _healthbarInside;

    /// <summary>
    /// The actual health of the enemy, that will be affected by the player shoots
    /// </summary>
    private float _actualHealth;

    /// <summary>
    /// Bool that controls the hide / show healthbar mechanic
    /// </summary>
    private bool _healthBarShown;

    /// <summary>
    /// Bool that determine if the enemy just got hit
    /// </summary>
    private bool _isHit;

    /// <summary>
    /// Float that will hold its transform x local scale
    /// </summary>
    private float _xLocalScale;

    /// <summary>
    /// Float that will hold the healthbar transform x local scale
    /// </summary>
    private float _healthBar_XLocalScale;

    /// <summary>
    /// Cache a reference of the target spawner
    /// </summary>
    private EnemySpawner _enemySpawner;

    /// <summary>
    /// Its animator
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// The actual timer of its iddle animations (will be affected by the passing of time)
    /// </summary>
    private float _actualBackToIddleTimer;

    /// <summary>
    /// Bool that determine if it is dying
    /// </summary>
    private bool _isDying;

    /// <summary>
    /// Its polygon collider 2D
    /// </summary>
    private Collider2D _collider2D;

    /// <summary>
    /// Its rigidbody2D
    /// </summary>
    private Rigidbody2D _thisRigidbody2D;

    private DamageIndicator _damageIndicator;

    /// <summary>
    /// Call the flip function to flip the sprite direction if needed (also make sure the healthbar is hidden)
    /// </summary>
    private void Start()
    {
        Flip();
        healthBar.SetActive(false);
    }

    /// <summary>
    /// Cache the necessary variables
    /// </summary>
    private void Awake()
    {
        _healthBarShown = false;
        _isHit = false;
        _healthbarInside = healthBar.transform.Find("HealthBar_Inside").gameObject;
        _actualHideHealthBarTimer = hideHealthBarTimer;
        _thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        _collider2D = gameObject.GetComponent<PolygonCollider2D>();
        _animator = gameObject.GetComponent<Animator>();
        _actualHealth = maxHealth;
        _xLocalScale = transform.localScale.x;
        _healthBar_XLocalScale = healthBar.transform.localScale.x;
        _damageIndicator = gameObject.GetComponent<DamageIndicator>();
    }

    /// <summary>
    /// If it is not dying, call the BackToIddleState function
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (!_isDying)
            BackToIddleState();

        HideHealthBar();
    }

    /// <summary>
    /// If it somehow fall from the platform, to avoid lefting it falling forever, destroy this game object when it touch the edge collider that is located below
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DestroyObject"))
            Destroy(gameObject);
    }

    /// <summary>
    /// When being destroyed, tell the spawner so that it can subtract from its spawn counter
    /// </summary>
    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
            _enemySpawner.EnemyDestroyed();
    }

    /// <summary>
    /// Function that controls the behavior of the enemy when hit by the player
    /// </summary>
    /// <param name="damage">The damage it took</param>
    /// <param name="bulletTransform">The position of the bullet that hit it</param>
    public void Hit(float damage, Transform bulletTransform)
    {
        // Just got hit
        _isHit = true;

        // Play the hit animation
        _animator.SetTrigger("Hit");

        // Subtract from its health pool
        _actualHealth -= damage;

        HealthBarHit();

        // Sets the bool depending on the direction of the bullet that hit it
        facingLeft = _collider2D.transform.position.x < bulletTransform.position.x ? true : false;

        // Then flips the sprite, if applicable
        Flip();

        // Show the damage inflicted
        _damageIndicator.ShowDamage(damage);

        // Fatal hit
        if(_actualHealth <= 0)
        {
            _isDying = true;
            // Zero its gavity scale, because its colliders will be disabled to avoid keep soaking shoots
            _thisRigidbody2D.gravityScale = 0;
            // Disable its collider
            _collider2D.enabled = false;
            // Play the death animation
            _animator.SetTrigger("Death");
            AudioManager.instance.PlaySound("TargetDummyDeath", transform.position);
            Invoke("Destroyed", 1f);
            // Check for drops
            DropOnDeath();
        }
    }

    private void HealthBarHit()
    {
        // Show the healthbar
        healthBar.SetActive(true);

        // Healthbar is visible
        _healthBarShown = true;

        // Show the damage taken by manipulating the healthbar local scale
        _healthbarInside.transform.localScale = new Vector3((float)_actualHealth / 10, _healthbarInside.transform.localScale.y);

        // If the healthbar x axis local scale get below zero, to avoid seeing the bar growing in the opposite direction, set its x scale to 0
        if (_healthbarInside.transform.localScale.x < 0f)
            _healthbarInside.transform.localScale = _healthbarInside.transform.localScale.WithAxis(Axis.X, 0f);
    }

    /// <summary>
    /// Hide the healthbar after a set time has passed without the dummy getting hit
    /// </summary>
    private void HideHealthBar()
    {
        // If the healthbar is visible 
        if (_healthBarShown)
        {
            // If it hasnt been hit recently
            if(!_isHit)
                // Start depleting the timer
                _actualHideHealthBarTimer -= Time.deltaTime;
            else
                // If not, then reset the timer (keep the timer still)
                _actualHideHealthBarTimer = hideHealthBarTimer;

            if (_actualHideHealthBarTimer < 0f)
            {
                // Timer depleted, then hide the healthbar
                healthBar.SetActive(false);

                // Health bar is hidden
                _healthBarShown = false;

                // Reset its timer
                _actualHideHealthBarTimer = hideHealthBarTimer;
            }
        }
    }

    /// <summary>
    /// Function used by the EnemySpawner: guarantee this object will have a cached reference to the spawner
    /// </summary>
    /// <param name="targetSpawner"></param>
    public void SetSpawner(EnemySpawner enemySpawner)
    {
        _enemySpawner = enemySpawner;
    }

    /// <summary>
    /// Destroy the game object
    /// </summary>
    private void Destroyed()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Function that controls the drops from this enemy
    /// </summary>
    private void DropOnDeath()
    {
        // First, check the drop chance from the enemy
        float chance = Random.Range(0.0f, 1.0f);

        // If successfull
        if(chance <= dropChance)
        {
            foreach (EnemyDrop drop in enemyDrops)
            {   
                // Check the drop chance from each item drop this enemy has
                float randomDrop = Random.Range(0.0f, 1.0f);

                // If sucessfull
                if (randomDrop <= drop.dropChance)
                {
                    // Call the EnemyDrop script SpawnDrop function
                    drop.SpawnDrop(transform.position);

                    // If using the single drop mechanic
                    if (singleDrop)
                        // Exit the loop
                        return;
                }
            }
        }
    }

    /// <summary>
    /// After the timer depletes, reset it and call the SetAnimationState function
    /// </summary>
    private void BackToIddleState()
    {
        _actualBackToIddleTimer -= Time.deltaTime;
        if (_actualBackToIddleTimer < 0f)
        {
            _actualBackToIddleTimer = backToIddleTimer;

            SetAnimationState();

            // A set ammount of time has passed after the last hit it took, so set the bool to false
            _isHit = false;
        }

    }

    /// <summary>
    /// Flips the target / healthbar sprite depending on the facingLeft bool
    /// </summary>
    private void Flip()
    {
        if (facingLeft)
        {
            transform.localScale = new Vector2(-_xLocalScale, transform.localScale.y);
            healthBar.transform.localScale = new Vector2(-_healthBar_XLocalScale, healthBar.transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector2(_xLocalScale, transform.localScale.y);
            healthBar.transform.localScale = new Vector2(_healthBar_XLocalScale, healthBar.transform.localScale.y);
        }
            
    }

    /// <summary>
    /// Randomly select from the available iddle animations, setting its animator accordingly
    /// </summary>
    private void SetAnimationState()
    {
        // Random.Range has its max parameter as exclusive, that why the +1
        int animationState = Random.Range(1, iddleAnimationsCount + 1);

        string trigger = "Iddle " + animationState;

        _animator.SetTrigger(trigger);
    }
}
