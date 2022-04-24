using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D objecthit)
    {

        if (objecthit.gameObject.CompareTag("target"))
        {
            Destroy(objecthit.gameObject);
        }

    }
}
