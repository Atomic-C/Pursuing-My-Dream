using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X_Y_Axis_Movement : MonoBehaviour
{

    public float speed = 10f; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // using the old method
        // with keyboard
        float V = Input.GetAxis("Vertical") * speed;
        float H = Input.GetAxis("Horizontal") * speed;

        transform.Translate(new Vector3(H * Time.deltaTime, V * Time.deltaTime, 0));
        
        // with mouse
        // float mX = Input.GetAxis("Mouse X");
        // float mY = Input.GetAxis("Mouse Y");

        // transform.Translate(new Vector3(mX * Time.deltaTime, mY * Time.deltaTime, 0));

    }
}
