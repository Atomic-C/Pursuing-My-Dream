using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource sounds;
    public static AudioManager instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayAudio(AudioClip audioClip)
    {
        sounds.clip = audioClip;
        sounds.Play();
    }
}
