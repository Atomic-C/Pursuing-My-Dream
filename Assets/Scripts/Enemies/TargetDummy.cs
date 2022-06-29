using UnityEngine;

/// <summary>
/// Class that controls the target dummy
/// </summary>
public class TargetDummy : MonoBehaviour
{
    /// <summary>
    /// Dummy health before being destroyed
    /// </summary>
    public float dummyHealth;

    /// <summary>
    /// Gameobject representing the dummy healthbar
    /// </summary>
    public GameObject healthBar;

    /// <summary>
    /// Float used as a timer before it goes back to its iddle animations
    /// </summary>
    public float backToIddleTimer;

    /// <summary>
    /// Float used as a timer to hide the healthbar
    /// </summary>
    public float hideHealthBarTimer;

    /// <summary>
    /// Bool that determine if it is facing left, used in the Flip function 
    /// </summary>
    public bool facingLeft;

    /// <summary>
    /// The transform that will be manipulated to simulate the effect of the bar diminishing
    /// </summary>
    private GameObject _healthbarInside;

    /// <summary>
    /// The actual health of the dummy, that will be affected by the player shoots
    /// </summary>
    private float _actualDummyHealth;

    /// <summary>
    /// The actual timer to hide the healthbar, that will be affected by the passing of time
    /// </summary>
    public float _actualHideHealthBarTimer;

    /// <summary>
    /// Bool that controls the hide / show healthbar mechanic
    /// </summary>
    private bool _healthBarShown;

    /// <summary>
    /// Bool that determine if the dummy just got hit
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
    private TargetSpawner _targetSpawner;

    /// <summary>
    /// Its animator
    /// </summary>
    private Animator _dummyAnimator;

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
    private PolygonCollider2D _polygonCollider2D;

    /// <summary>
    /// The box collider2D positioned at the dummy head, to allow the player to stay on it. It is actually on a children game object, because the tag 'Plaftorm' is necessary to allow
    /// the player to stay on top and jump. The actual dummy already has another tag, making this necessary.
    /// </summary>
    private BoxCollider2D _headPlatformCollider2D;

    /// <summary>
    /// Its rigidbody2D
    /// </summary>
    private Rigidbody2D _thisRigidbody2D;

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
        _polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        _dummyAnimator = gameObject.GetComponent<Animator>();
        _headPlatformCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();
        _actualDummyHealth = dummyHealth;
        _xLocalScale = transform.localScale.x;
        _healthBar_XLocalScale = healthBar.transform.localScale.x;
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
        if(gameObject.scene.isLoaded)
            _targetSpawner.DummyDestroyed();
    }

    /// <summary>
    /// Function that controls the behavior of the dummy when hit by the player
    /// </summary>
    /// <param name="damage">The damage it took</param>
    /// <param name="bulletTransform">The position of the bullet that hit it</param>
    public void Hit(float damage, Transform bulletTransform)
    {
        // Just got hit
        _isHit = true;

        // Play the hit animation
        _dummyAnimator.SetInteger("AnimationState", 2);

        // Subtract from its health pool
        _actualDummyHealth -= damage;

        HealthBarHit();

        // Sets the bool depending on the direction of the bullet that hit it
        facingLeft = _polygonCollider2D.transform.position.x < bulletTransform.position.x ? true : false;

        // Then flips the sprite, if applicable
        Flip();

        // Fatal hit
        if(_actualDummyHealth <= 0)
        {
            _isDying = true;
            // Zero its gavity scale, because its colliders will be disabled to avoid keep soaking shoots
            _thisRigidbody2D.gravityScale = 0;
            // Disable its collider
            _polygonCollider2D.enabled = false;
            // Disable its head collider
            _headPlatformCollider2D.enabled = false;
            // Play the death animation
            _dummyAnimator.SetInteger("AnimationState", 3);
            AudioManager.instance.PlaySound("TargetDummyDeath", transform.position);
            Invoke("Destroyed", 1f);
        }
    }

    private void HealthBarHit()
    {
        // Show the healthbar
        healthBar.SetActive(true);

        // Healthbar is visible
        _healthBarShown = true;

        // Show the damage taken by manipulating the healthbar local scale
        _healthbarInside.transform.localScale = new Vector3(_actualDummyHealth / 10, _healthbarInside.transform.localScale.y);

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

                // Reset its timer
                _actualHideHealthBarTimer = hideHealthBarTimer;
            }
        }
    }

    /// <summary>
    /// Function used by the TargetSpawner: guarantee this object will have a cached reference to the spawner
    /// </summary>
    /// <param name="targetSpawner"></param>
    public void SetSpawner(TargetSpawner targetSpawner)
    {
        _targetSpawner = targetSpawner;
    }

    /// <summary>
    /// Destroy the game object
    /// </summary>
    private void Destroyed()
    {
        Destroy(gameObject);
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
    /// Randomly select from the two available iddle animations, setting its animator accordingly
    /// </summary>
    private void SetAnimationState()
    {
        // Random.Range has its max parameter as exclusive
        int animationState = Random.Range(0, 2);

        _dummyAnimator.SetInteger("AnimationState", animationState);
    }
}
