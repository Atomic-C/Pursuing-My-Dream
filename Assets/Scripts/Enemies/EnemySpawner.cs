using UnityEngine;

/// <summary>
/// Class that controls the target spawner
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// Max ammount of object to spawn
    /// </summary>
    public int spawnLimit;

    /// <summary>
    /// Timer before each spawn
    /// </summary>
    public float spawnTimer;

    /// <summary>
    /// The object to spawn
    /// </summary>
    public EnemyHealth enemy;

    /// <summary>
    /// Actual counter of objects spawned
    /// </summary>
    private int _enemyCounter;

    /// <summary>
    /// Actual timer before each spawn
    /// </summary>
    private float _actualSpawnTimer;

    private void Start()
    {
        _actualSpawnTimer = spawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnTargetDummy();
    }

    /// <summary>
    /// Spawn a new object, if the spawn limit has not been reached and after the spawn timer depleted
    /// </summary>
    private void SpawnTargetDummy()
    {
        if(_enemyCounter < spawnLimit)
        {
            _actualSpawnTimer -= Time.deltaTime;
            if (_actualSpawnTimer < 0f)
            {
                EnemyHealth enemy = Instantiate(this.enemy, transform.position, Quaternion.identity);
                // Guarante the object spawned has a reference to this
                enemy.SetSpawner(this);
                // Spawn the dummy randomly facing right or left
                bool facingLeft = Random.Range(0f, 1f) == 0 ? false : true;
                enemy.facingLeft = facingLeft;
                _actualSpawnTimer = spawnTimer;
                _enemyCounter++;
            }
        }
    }

    /// <summary>
    /// One object has been destroyed, so this class will subtract from its counter
    /// </summary>
    public void EnemyDestroyed()
    {
        _enemyCounter--;
    }
}
