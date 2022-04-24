using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        this.gameObject.transform.Rotate(new Vector2(0, 200 * Time.deltaTime));

    }

}
