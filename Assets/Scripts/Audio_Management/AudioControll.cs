using UnityEngine;

public class AudioControll : MonoBehaviour
{
    private bool muted;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (muted)
            {
                this.GetComponent<AudioSource>().Play();
                muted = false;
            }
            else
            {
                this.GetComponent<AudioSource>().Pause();
                muted = true;
            }
        }
    }
}
