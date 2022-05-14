using UnityEngine;

public class AudioControl : MonoBehaviour
{
    private bool muted;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (muted)
            {
                AudioManager.instance.PlaySound("Music", Vector3.zero);
                muted = false;
            }
            else
            {
                AudioManager.instance.StopSound("Music");
                muted = true;
            }
        }
    }
}
