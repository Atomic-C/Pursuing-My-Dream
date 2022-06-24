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
    /// Float used as a timer before it goes back to its iddle animations
    /// </summary>
    public float backToIddleTimer;

    /// <summary>
    /// Bool that determine if it is facing left, used in the Flip function 
    /// </summary>
    public bool facingLeft;

    /// <summary>
    /// Float that will hold its transform x local scale
    /// </summary>
    private float xLocalScale;

    /// <summary>
    /// Cache a reference of the target spawner
    /// </summary>
    private TargetSpawner targetSpawner;

    /// <summary>
    /// Its animator
    /// </summary>
    private Animator dummyAnimator;

    /// <summary>
    /// The actual timer of its iddle animations (will be affected by the passing of time)
    /// </summary>
    private float actualBackToIddleTimer;

    /// <summary>
    /// The actual health of the dummy, that will be affected by the player shoots
    /// </summary>
    public float actualDummyHealth;

    /// <summary>
    /// Bool that determine if it is dying
    /// </summary>
    private bool isDying;

    /// <summary>
    /// Its polygon collider 2D
    /// </summary>
    private PolygonCollider2D polygonCollider2D;

    /// <summary>
    /// Its rigidbody2D
    /// </summary>
    private Rigidbody2D thisRigidbody2D;

    /// <summary>
    /// Call the flip function to flip the sprite direction if needed
    /// </summary>
    private void Start()
    {
        Flip();
    }

    /// <summary>
    /// Cache the necessary variables
    /// </summary>
    private void Awake()
    {
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        polygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
        dummyAnimator = gameObject.GetComponent<Animator>();
        actualDummyHealth = dummyHealth;
        xLocalScale = transform.localScale.x;
    }

    /// <summary>
    /// If it is not dying, call the BackToIddleState function
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (!isDying)
            BackToIddleState();
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
    /// Function that controls the behavior of the dummy when hit by the player
    /// </summary>
    /// <param name="damage">The damage it took</param>
    /// <param name="bulletTransform">The position of the bullet that hit it</param>
    public void Hit(float damage, Transform bulletTransform)
    {
        // Sets the bool depending on the direction of the bullet that hit it
        facingLeft = polygonCollider2D.transform.position.x < bulletTransform.position.x ? true : false;

        // Then flips the sprite, if applicable
        Flip();

        // Just got hit, play the hit animation
        dummyAnimator.SetInteger("AnimationState", 2);

        // Subtract from its health pool
        actualDummyHealth -= damage;

        // Fatal hit
        if(actualDummyHealth <= 0)
        {
            isDying = true;
            // Zero its gavity scale, because its colliders will be disabled to avoid keep soaking shoots
            thisRigidbody2D.gravityScale = 0;
            // Disable its collider
            polygonCollider2D.enabled = false;
            // Play the death animation
            dummyAnimator.SetInteger("AnimationState", 3);
            AudioManager.instance.PlaySound("TargetDummyDeath", transform.position);
            Invoke("Destroyed", 1f);
        }
    }

    /// <summary>
    /// Function used by the TargetSpawner: guarantee this object will have a cached reference to the spawner
    /// </summary>
    /// <param name="targetSpawner"></param>
    public void SetSpawner(TargetSpawner targetSpawner)
    {
        this.targetSpawner = targetSpawner;
    }

    /// <summary>
    /// When being destroyed, tell the spawner so that it can subtract from its spawn counter
    /// </summary>
    private void Destroyed()
    {
        targetSpawner.DummyDestroyed();
        Destroy(gameObject);
    }

    /// <summary>
    /// After the timer depletes, reset it and call the SetAnimationState function
    /// </summary>
    private void BackToIddleState()
    {
        actualBackToIddleTimer -= Time.deltaTime;
        if (actualBackToIddleTimer < 0f)
        {
            actualBackToIddleTimer = backToIddleTimer;

            SetAnimationState();
        }

    }

    /// <summary>
    /// Flips the sprite depending on the facingLeft bool
    /// </summary>
    private void Flip()
    {
        if (facingLeft)
            transform.localScale = new Vector2(-xLocalScale, transform.localScale.y);
        else
            transform.localScale = new Vector2(xLocalScale, transform.localScale.y);
    }

    /// <summary>
    /// Randomly select from the two available iddle animations, setting its animator accordingly
    /// </summary>
    private void SetAnimationState()
    {
        // Random.Range has its max parameter as exclusive
        int animationState = Random.Range(0, 2);

        dummyAnimator.SetInteger("AnimationState", animationState);
    }
}
