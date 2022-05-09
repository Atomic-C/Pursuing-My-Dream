using System.Collections;
using UnityEngine;

public class ConstantForceSpawn : MonoBehaviour
{
    public GameObject objToSpawn;
    public float spawnTimer, lifeSpan;

    private float timer;

    private void Start()
    {
        timer = spawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            StartCoroutine(InstantiateBall());
            timer = spawnTimer;
        }
    }

    IEnumerator InstantiateBall()
    {
        GameObject obj = objToSpawn;

        Instantiate(obj, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        yield return new WaitForSeconds(lifeSpan);

        Destroy(obj);
    }
}
