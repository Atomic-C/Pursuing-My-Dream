using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakPoint : MonoBehaviour
{

    public DistanceJoint2D barrel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A))
        {

            barrel.breakForce = 0;

        }

    }
}
