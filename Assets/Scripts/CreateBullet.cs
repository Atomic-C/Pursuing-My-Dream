using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBullet : MonoBehaviour
{

    public GameObject bullet;
    public GameObject firingPoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bullet, new Vector3(firingPoint.transform.position.x, firingPoint.transform.position.y, firingPoint.transform.position.z), firingPoint.transform.rotation);
        }

    }
}
