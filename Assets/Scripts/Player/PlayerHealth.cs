using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Script that controls everything related to the player health, damage and life bar setup
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public bool canStart;

    [Header("Setup")]
    /// <summary>
    /// The player maximum health
    /// </summary>
    public int maxHealth;

    /// <summary>
    /// The color the player sprite assumes when being hit (flashing effect)
    /// </summary>
    public Color flashingColor;

    /// <summary>
    /// The heart prefab used to build the player lifebar
    /// </summary>
    public Heart heart;
    
    /// <summary>
    /// Holds a fixed position where the first heart will be drawn in the UI canvas
    /// </summary>
    public Vector3 firstHeartPosition;

    /// <summary>
    /// Space offset used between each heart in the player lifebar
    /// </summary>
    public float heartsSpaceOffset;

    /// <summary>
    /// Holds a reference to the UI canvas, to build the player lifebar
    /// </summary>
    public Canvas canvas;

    [Header("Game over screen objects")]
    /// <summary>
    /// The game over UI text
    /// </summary>
    public UIFadeEffect gameOverText;

    /// <summary>
    /// The retry UI text
    /// </summary>
    public UIFadeEffect retryText;

    [Header("Stun effect")]
    /// <summary>
    /// Bool that dictates if the stun effect will be used or not
    /// </summary>
    public bool useStunEffect;

    /// <summary>
    /// Timer used for the stun effect, if applicable
    /// Affects both the duration of the player damage animation and the stun (loss of control), when this stun is being used
    /// Leaving this at 0, means the player damage animation length will be used instead
    /// </summary>
    public float hitStunTimer;

    /// <summary>
    /// The ammount of space the player sprite will move after being hit
    /// </summary>
    public float hitDistance;

    [Header("Invincibility mechanic")]
    /// <summary>
    /// Value used for the invincibility frame mechanic
    /// This is the total time of the invincibility effect after being hit
    /// </summary>
    public float invincibilityTimer;

    /// <summary>
    /// Value used for the invincibility frame mechanic
    /// This is deducted from the invincibilityTimer, dictating how many times the player sprite will flash, indicating the invincibility effect
    /// </summary>
    public float invincibilityDeltaTime;

    /// <summary>
    /// The player animator
    /// </summary>
    private Animator _animator;

    /// <summary>
    /// The player maximum health, that will be affected by health up upgrades
    /// </summary>
    private int _maxHealth;

    /// <summary>
    /// The player current health, that will be deducted when taking damage
    /// </summary>
    private int _currentHealth;

    /// <summary>
    /// List that will hold all the player hearts, used for manipulating its sprites
    /// </summary>
    private List<Heart> _hearts;

    /// <summary>
    /// Bool that determines if the player took a hit or not
    /// </summary>
    private bool _tookHit;

    /// <summary>
    /// The player rigidbody 2D
    /// </summary>
    private Rigidbody2D _rigidbody2D;

    /// <summary>
    /// Holds a reference of the floating magical gem
    /// </summary>
    private GameObject _magicalGem;

    /// <summary>
    /// The player sprite renderer, used in the flashing effect when damaged
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Holds the player sprite original color
    /// </summary>
    private Color _originalColor;

    /// <summary>
    /// Bool used in the invincibility frame mechanic, to determine which sprite color will be used
    /// </summary>
    private bool _isFlashing;

    /// <summary>
    /// Bool used after the player death, to set its rigidbody to static
    /// </summary>
    private bool _isDead;

    /// <summary>
    /// Holds a reference to the player Platform_Movement script
    /// </summary>
    private Platform_Movement _platformMovement;

    // Start is called before the first frame update
    void Start()
    {
        if (canStart)
        {
            // Initialize the hearts list
            _hearts = new List<Heart>();
            SetHealthBar();
        }
    }

    /// <summary>
    /// Cache all the necessary variables
    /// </summary>
    private void Awake()
    {
        if (canStart)
        {
            retryText.gameObject.SetActive(false);

            _maxHealth = maxHealth;
            _currentHealth = _maxHealth;
            _animator = GetComponent<Animator>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _platformMovement = GetComponent<Platform_Movement>();
            _originalColor = _spriteRenderer.color;
        }
    }

    /// <summary>
    /// Touch damage mechanic used in enemies explosions (area of effect that is trigger based)
    /// </summary>
    /// <param name="collision">The other object collider 2D</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            TakeHit(collision.transform);
    }

    /// <summary>
    /// Touch damage mechanic used in all enemies. Touching enemies hurts the player
    /// Also, change the player rigidbody to static after its death and its collider touches the ground (to make the player sprite stay exactly in contact with the ground)
    /// </summary>
    /// <param name="collision">The other object collider 2D</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Enemy"))
            TakeHit(collision.transform);

        if (collision.collider.CompareTag("ground") && _isDead)
            StaticRigidBody();

    }

    /// <summary>
    /// If the player remains colliding with enemies, it will take damage after the invincibility effect ends
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
            TakeHit(collision.transform);
    }

    public void Initialize()
    {
        Awake();
        Start();
    }

    /// <summary>
    /// Function that controls the health pickup mechanic (used by the CollectableManager)
    /// </summary>
    public void PickedUpHealth(float value)
    {
        // Play the health up sound
        AudioManager.instance.PlaySound("HealthUp", transform.position);

        // If the current health is lower than the max health (to avoid increasing the current health value above the maximum) and is different that 0 (dead cant pick health)
        if (_currentHealth < _maxHealth && _currentHealth != 0)
        {
            // Increase the current health
            _currentHealth += (int)value;

            // If the current health goes above the maximum health, set it to the maximum health
            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;

            // Loop using the value ammount of times
            for (int i = 1; i <= (int)value; i++)
            {
                // Iterates over all the hearts
                for (int x = 0; x <= _hearts.Count - 1; x++)
                {
                    // Finds the first that is depleted
                    if (_hearts[x].isDepleted)
                    {
                        // Call its GotHeart function and exit the loop
                        _hearts[x].GotHeart();
                        break;
                    }
                    // Or if it was the last heart of the list and it wasnt depleted, makes no sense to repeat the loop again or x ammount of times depending on the parameter 'value'
                    // So, just exit the function
                    else if (x == _hearts.Count - 1)
                        return;
                }
            }
        }
    }

    /// <summary>
    /// Function that controls the health bar behavior when a health upgrade is picked up
    /// </summary>
    public void PickedUpHealthUpgrade()
    {
        // Increase the _maxHealth
        _maxHealth++;

        // Instantiate the heart object and make it a child of the UI canvas
        GameObject newHeart = Instantiate(heart.gameObject);
        newHeart.transform.SetParent(canvas.transform);

        // Iterate the hearts list
        for (int i = 0; i <= _hearts.Count - 1; i++)
        {
            // Check the first heart that is depleted (is there previous damage?)
            if (_hearts[i].isDepleted)
            {
                // Set the new heart position at the depleted one's position
                newHeart.transform.localPosition = _hearts[i].transform.localPosition;

                // Insert the new heart behind the depleted one in the list
                _hearts.Insert(i, newHeart.GetComponent<Heart>());

                // Iterate the hearts list again, but beginning from the depleted heart found index (thats why its being used the int x = 'i+1')
                for (int x = i+1; x <= _hearts.Count - 1; x++)
                {
                    // And move the heart to the right, using the heartsSpaceOffset
                    _hearts[x].transform.localPosition = _hearts[x].transform.localPosition.WithAxis(Axis.X, _hearts[x].transform.localPosition.x + heartsSpaceOffset);
                }

                // Exit the function
                return;
            }
        }

        // If there's no heart depleted (no damage was taken), set its position to the right of the last heart and add it to the end of the list
        newHeart.transform.localPosition = _hearts[_hearts.Count - 1].transform.localPosition;
        newHeart.transform.localPosition = newHeart.transform.localPosition.WithAxis(Axis.X, _hearts[_hearts.Count - 1].transform.localPosition.x + heartsSpaceOffset);
        _hearts.Add(newHeart.GetComponent<Heart>());
    }

    /// <summary>
    /// Build the player health bar, instantiating the necessary ammount of hearts and making all of them a child of the UI canvas
    /// Calculation for distance between them: the first heart is instantiate in the firstHeartPosition Vector2 position. All others are instantiate in the same position but with
    /// a distance to the right, in the x axis, that is calculated with the formula for i variable times the heartsSpaceOffset value
    /// </summary>
    private void SetHealthBar()
    {
        for (int i = 0; i < _maxHealth; i++)
        {
            GameObject newHeart = Instantiate(heart.gameObject);
            newHeart.transform.SetParent(canvas.transform);
            newHeart.transform.localPosition = firstHeartPosition;

            if(i != 0)
                newHeart.transform.localPosition = newHeart.transform.localPosition.WithAxis(Axis.X, firstHeartPosition.x + (heartsSpaceOffset * i));

            _hearts.Add(newHeart.GetComponent<Heart>());
        }
    }

    /// <summary>
    /// Function that controls the damage taken by the player
    /// Responsible to reduce the ammount of hearts and controlling the player damage / death animation
    /// </summary>
    /// <param name="enemyTransform">The transform of the enemy</param>
    private void TakeHit(Transform enemyTransform)
    {
        // If the player took damage
        if (!_tookHit)
        {
            // Iterates over all the hearts in the list, but backwards
            for (int i = _hearts.Count - 1; i >= 0; i--)
            {
                // The first one that is full
                if (_hearts[i].isDepleted == false)
                {
                    // Deduce the player current health
                    _currentHealth--;

                    _tookHit = true;

                    // Call that heart TookHit function 
                    _hearts[i].TookHit();

                    // Checks in which direction the hit came
                    bool hitLeft = transform.position.x < enemyTransform.position.x ? true : false;

                    // Then move the player transform, simulating a knockback effect, to that direction (in the x axis)
                    transform.position = new Vector2(transform.position.x + (hitLeft ? -hitDistance : hitDistance), transform.position.y);

                    // Play the damage animation
                    _animator.SetBool("TookDamage", true);

                    // Or if the player just lost its last heart
                    if (_currentHealth == 0)
                    {
                        // Call the Death function and exit this function
                        Death();
                        return;
                    }
                    else
                    {
                        // If the stun effect is being used
                        if (useStunEffect)
                        {
                            // Set the isHit bool from the player Platform_Movement script, preventing movement
                            _platformMovement.isHit = true;
                        }
                            
                        // Play the damage sound
                        AudioManager.instance.PlaySound("PlayerDamage", transform.position);

                        StartCoroutine("InitiateInvincibilityFrames");
                        
                        // Call the ResetPlayerTookHit function after the hisStunTimer variable value in seconds or if it is 0, after the damage animation ends
                        Invoke("ResetPlayerTookHit", hitStunTimer == 0 ? _animator.GetCurrentAnimatorStateInfo(0).length : hitStunTimer);

                        return;
                    }
                    
                }
            }
        }
    }

    /// <summary>
    /// Resets the player animation back to its default
    /// Also, gives the player its movement back, if using the stun effect
    /// </summary>
    private void ResetPlayerTookHit()
    {
        _animator.SetBool("TookDamage", false);
        if(useStunEffect)
            _platformMovement.isHit = false;
    }

    /// <summary>
    /// Coroutine that controls the invincibility and sprite flashing effect
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitiateInvincibilityFrames()
    {
        // Use a for loop using both the invincibility variables
        for (float i = 0f; i < invincibilityTimer; i += invincibilityDeltaTime)
        {
            // If the player sprite is the flashing one
            if (_isFlashing)
                // Revert to its original color
                _spriteRenderer.color = _originalColor;
            else
                // Or sets the flashing color
                _spriteRenderer.color = flashingColor;
            // Then revert the bool state
            _isFlashing = !_isFlashing;
            // And waits for the invincibilityDeltaTime value in seconds
            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        // After the invincibility is over, set the _tookHit bool to false
        _tookHit = false;

        // And stop this coroutine
        StopCoroutine("InitiateInvincibilityFrames");
    }

    /// <summary>
    /// Function that initiate the player death animation
    /// </summary>
    private void Death()
    {
        // Get the MagicalGemController and deactivate it (disabling the shooting mechanics)
        GameObject.Find("MagicalGemController").GetComponent<MagicalGemController>().enabled = false;

        // Get the magical gem reference
        _magicalGem = GameObject.Find("MagicalGem(Clone)");

        // Disable its animator to make the gem stop midair
        _magicalGem.GetComponent<Animator>().enabled = false;

        // Set the player rigidbody velocity to zero in the x axis, to stop it where it took the last hit (still susceptible to other objects influences though)
        // But still maintains its velocity in the y axis (if the hit was took in midair, the player will still fall to the ground)
        _rigidbody2D.velocity = new Vector2(0f, _rigidbody2D.velocity.y);

        // Stop the music
        AudioManager.instance.StopSound("Music");

        // Set the Platform_Movement script isDead bool to true, preventing all movement inputs
        _platformMovement.isDead = true;

        // Play the death sound
        AudioManager.instance.PlaySound("PlayerDeath", transform.position);

        // Call the Pop function after the death sound ended
        Invoke("Pop", AudioManager.instance.GetSound("PlayerDeath").clip.length);
    }

    /// <summary>
    /// Next step of the death animation: affects the magical gem 
    /// </summary>
    private void Pop()
    {
        // Deactivate the magical gem range limit, located in its child object (must be deactivated to avoid a bug where this collider interacts with the camera box collider)
        _magicalGem.GetComponentInChildren<CircleCollider2D>().enabled = false;
        // Deactivate the magical gem light component
        _magicalGem.GetComponent<Light2D>().enabled = false;
        // Enable its polygon collider
        _magicalGem.GetComponent<PolygonCollider2D>().enabled = true;
        // And add a rigibody 2D component, kicking in the engine physics, making the gem fall to the ground
        _magicalGem.AddComponent<Rigidbody2D>();

        // Disable the damage animation
        _animator.SetBool("TookDamage", false);
        // And activate the death animation
        _animator.SetBool("Dead", true);

        // Play the slime pop sound
        AudioManager.instance.PlaySound("PlayerPop", transform.position);

        // After everything is done, set the _isDead bool to true
        _isDead = true;

        // Activate retry text UI game object
        retryText.gameObject.SetActive(true);

        // Call the fade in effect for both game over text's
        gameOverText.StartFadeEffect(true);
        retryText.StartFadeEffect(true);
    }

    /// <summary>
    /// Simply set the player rigidbody 2D to static, preventing more influence from other objects
    /// </summary>
    private void StaticRigidBody()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
    }
}
