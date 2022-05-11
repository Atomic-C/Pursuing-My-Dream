using UnityEngine;

// Script that spawns an game object after every x seconds
public class ConstantForceSpawn : MonoBehaviour
{
    public GameObject objToSpawn;
    public float spawnTimer;

    private float timer;

    // Set the current timer at start
    private void Start()
    {
        timer = spawnTimer;
    }

    // Update is called once per frame
    // Decrement the timer as time passes by, instantiate the game object at this one position as the timer is depleted and reset it
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Instantiate(objToSpawn, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            timer = spawnTimer;
        }
    }
}
