using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControll : MonoBehaviour
{
    public AudioSource music;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.A))
        {

            music.Play();

        }

        if (Input.GetKeyDown(KeyCode.S))
        {

            music.Stop();

        }

        if (Input.GetKeyDown(KeyCode.P))
        {

            music.Pause();

        }

    }
}
