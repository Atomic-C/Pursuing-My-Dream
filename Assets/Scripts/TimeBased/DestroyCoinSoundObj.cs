using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCoinSoundObj : MonoBehaviour
{
    private float time = 1.5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
            Destroy(this.gameObject);
    }
}
