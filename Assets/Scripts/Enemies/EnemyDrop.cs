using UnityEngine;

/// <summary>
/// Script that holds info about the item drop
/// </summary>
public class EnemyDrop : MonoBehaviour
{
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
