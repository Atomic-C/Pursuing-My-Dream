using UnityEngine;

/// <summary>
/// Class that spawns an game object after every x seconds
/// </summary>
public class ConstantForceSpawn : MonoBehaviour
{
    /// <summary>
    /// Whih object to spawn
    /// </summary>
    public GameObject objToSpawn;

    /// <summary>
    /// Float used as a timer to spawn the object (fixed and set in the editor)
    /// </summary>
    public float spawnTimer;

    /// <summary>
    /// Float used as an actual timer (will be manipulated as times passes)
    /// </summary>
    private float timer;

    /// <summary>
    /// Set the current timer at start
    /// </summary>
    private void Start()
    {
        timer = spawnTimer;
    }

    /// <summary>
    /// Decrement the timer as time passes by, instantiate the game object at this one position as the timer is depleted and reset it
    /// </summary>
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Instantiate(objToSpawn, transform.position, Quaternion.identity);
            timer = spawnTimer;
        }
    }
}
