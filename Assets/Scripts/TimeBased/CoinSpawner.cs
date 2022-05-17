using UnityEngine;

/// <summary>
/// Script that spawns an game object (a coin) at the position of another game object (the hat) after every x seconds
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    /// <summary>
    /// Timer set at the editor
    /// </summary>
    public float timerNum;
    
    /// <summary>
    /// The actual timer 
    /// </summary>
    private float timer;

    /// <summary>
    /// The game object to be instantiated
    /// </summary>
    public GameObject coin;

    /// <summary>
    /// Random range used as force to the instantiated game object
    /// </summary>
    public float minRandomRange, maxRandomRange;

    /// <summary>
    /// The position the game object will be instantiated
    /// </summary>
    public Transform hatCoinSpawner;

    /// <summary>
    /// The player collider used to trigger the whole spawn proccess
    /// </summary>
    public CircleCollider2D playerCollider;

    /// <summary>
    /// When the player touches the trigger, the timer variable begins decrementing. After it reaches 0 or less, instantiate a coin at the hat position, but a little bit above it 
    /// (its an upside down hat, so makes sense the coins is being spit out from above, 'hatCoinSpawner.position.y + 1') and immediately after that, applies a force to this coin
    /// making it fly some distance from the hat (spit the coin out!) using random X and Y force from the range of minRandomRange and maxRandomRange. Reset the timer after that
    /// </summary>
    private void Update()
    {
        if (this.GetComponent<Collider2D>().IsTouching(playerCollider))
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Instantiate(coin, new Vector3(hatCoinSpawner.position.x, hatCoinSpawner.position.y + 1, hatCoinSpawner.position.z), Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(minRandomRange, maxRandomRange), Random.Range(minRandomRange, maxRandomRange)), ForceMode2D.Force);
                timer = timerNum;
            }
        }
    }
}
