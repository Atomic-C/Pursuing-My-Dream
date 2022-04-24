using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCode : MonoBehaviour
{

    public bool freeRotation;
    public GameObject magnet;

    // Start is called before the first frame update
    void Start()
    {

        freeRotation = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (freeRotation == true)
        {
            magnet.gameObject.transform.Rotate(new Vector3(0, 0, 5 * Time.deltaTime));
        }
    }

    void OnTriggerEnter2D(Collider2D whoTriggered)
    {
        if (whoTriggered.gameObject.CompareTag("duck"))
        {
            freeRotation = true;
        }   
    }

    void OnTriggerExit2D(Collider2D whoTriggered)
    {
        if (whoTriggered.gameObject.CompareTag("duck"))
        {
            freeRotation = false;
        }
    }

}
