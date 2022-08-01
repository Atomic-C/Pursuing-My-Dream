using System.Collections;
using UnityEngine;

/// <summary>
/// Script that holds info about the item drop
/// </summary>
public class EnemyDrop : MonoBehaviour
{
    [Header("Spawn setup")]
    /// <summary>
    /// Random range used as force to the instantiated game object
    /// </summary>
    public float minRandomRange, maxRandomRange;

    /// <summary>
    /// Float used to determine the value that will be incremented, when picked up, in the case of health, energy and coin types
    /// </summary>
    public float incrementValue;

    /// <summary>
    /// Percentage chance this item has to drop (0.0f > 0%, 1.0f > 100%)
    /// </summary>
    public float dropChance;

    /// <summary>
    /// Enum that determines the type of this drop
    /// </summary>
    public DropType dropType;

    [Header("Vanish setup")]
    /// <summary>
    /// Life span of this drop
    /// </summary>
    public float lifeSpan;

    /// <summary>
    /// Timer used in the vanishing function
    /// </summary>
    public float vanishTime;

    /// <summary>
    /// Delta timer used in vanishing function
    /// </summary>
    public float vanishDeltaTime;

    /// <summary>
    /// The color the drop assumes when vanishing
    /// </summary>
    public Color flashingColor;

    /// <summary>
    /// Bool used to determine which color the drop sprite will use
    /// </summary>
    private bool _isFlashing;

    /// <summary>
    /// The drop sprite renderer
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// The drop sprite renderer original color
    /// </summary>
    private Color _originalColor;

    [Header("Drop from shop")]
    /// <summary>
    /// Determines if this drop is from the shop
    /// </summary>
    public bool fromShop;

    /// <summary>
    /// Cache all references
    /// </summary>
    private void Awake()
    {
        _isFlashing = false;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
    }

    /// <summary>
    /// Start with the Coroutine DepleteLifeSpan
    /// </summary>
    private void Start()
    {
        if(!fromShop)
            StartCoroutine("DepleteLifeSpan");
    }

    /// <summary>
    /// As the life span of the drop depletes, initiate the vanishing effect
    /// </summary>
    /// <returns>Wait for seconds</returns>
    private IEnumerator DepleteLifeSpan()
    {
        for (float i = 0f; i < lifeSpan; i += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine("StartVanishing");

        StopCoroutine("DepleteLifeSpan");
    }

    /// <summary>
    /// Start flashing the drop sprite and after the vanishTime variable is reached, destroy the object
    /// </summary>
    /// <returns>Wait for seconds</returns>
    private IEnumerator StartVanishing()
    {
        // Use a for loop using both the invincibility variables
        for (float i = 0f; i < vanishTime; i += vanishDeltaTime)
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
            yield return new WaitForSeconds(vanishDeltaTime);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Function that will spawn this item in the position where the enemy died
    /// </summary>
    /// <param name="position">The enemy position</param>
    public void SpawnDrop(Vector2 position)
    {
        Instantiate(gameObject, position, Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(minRandomRange, maxRandomRange),
                                                                                     Random.Range(minRandomRange, maxRandomRange)), ForceMode2D.Force);
    }

    /// <summary>
    /// Enum that determines item drop types
    /// </summary>
    public enum DropType 
    {
        Health,
        Energy,
        Upgrade,
        Coin
    }
}
