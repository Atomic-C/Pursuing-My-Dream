using UnityEngine;

/// <summary>
/// Simple class that controls the main music of the game
/// Mute / unmute with the "M" key
/// </summary>
public class AudioControl : MonoBehaviour
{
    /// <summary>
    /// Bool that define if the music is muted or not
    /// </summary>
    private bool muted;

    /// <summary>
    /// Mute / unmute the music with the "M" Key press
    /// </summary>
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
