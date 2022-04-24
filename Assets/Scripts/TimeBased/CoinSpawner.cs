using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public float time, timeNum;
    public GameObject coin;
    public float minRandomRange, maxRandomRange;
    public Transform hatCoinSpawner;
    public CircleCollider2D playerCollider;

    void Start()
    {
        
    }

    private void Update()
    {
        if (this.GetComponent<Collider2D>().IsTouching(playerCollider))
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                Instantiate(coin, new Vector3(hatCoinSpawner.position.x, hatCoinSpawner.position.y + 1, hatCoinSpawner.position.z), Quaternion.identity).GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(minRandomRange, maxRandomRange), Random.Range(minRandomRange, maxRandomRange)), ForceMode2D.Force);
                time = timeNum;
            }
        }
    }

}
