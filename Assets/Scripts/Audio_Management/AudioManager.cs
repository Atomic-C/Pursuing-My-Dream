using System;
using System.Threading.Tasks;
using UnityEngine;

// Script that controls, dynamically, the sounds and musics of the game
public class AudioManager : MonoBehaviour
{
    // As i understood, serialize field allows the access to this property in the editor
    // at the same time, not letting other scripts to access this directly
    [SerializeField]
    // Array of custom class that holds info about the sound to be played   
    private Sound[] sounds;
    // Instance of the audio manager
    public static AudioManager instance = null;

    // Guarantees there will always a single instance of this in each scene
    // using the singleton design pattern
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Create the same ammount of game objects, as the ammount of sounds array and make them children of this game object
    // Call the Sound function SetSource, passing to it a new AudioSource component, basically ensuring that all the sounds
    // of the array, have an audio source each, dynamically
    private void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject newSound = new GameObject("Sound_" + i + "_" + sounds[i].name);
            newSound.transform.SetParent(this.transform);
            sounds[i].SetSource(newSound.AddComponent<AudioSource>());
        }
        PlaySound("Music");
    }

    // Function that plays the sound with the respective parameter
    public void PlaySound(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == soundName)
            {
                sounds[i].Play();
                return;
            }
        }
    }

    // Function that stops the sound with the respective parameter
    public void StopSound(string soundName)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == soundName)
            {
                sounds[i].Stop();
                return;
            }
        }
    }
}

// System.Serializable allows the manipulation of custom classes in the editor
[System.Serializable]
// Custom class that holds info about the sounds in the game
public class Sound
{
    public string name;
    [Range(0f, 1f)]
    public float volume;
    public bool loop, isMusic, isPlaying;
    public AudioClip clip;
    private AudioSource source;

    // Set the properties below accordingly to our choices in the editor 
    public void SetSource(AudioSource audioSource)
    {
        source = audioSource;
        source.clip = clip;
        source.loop = loop;
    }

    // Function used to play the audio clip
    // If it is a music (isMusic bool), call the Play function
    // If its not a music (SFX), call the PlayOneShot function
    public void Play()
    {
        source.volume = volume;
        if (isMusic)
            source.Play();
        else
             if (!isPlaying)
        {
            source.PlayOneShot(clip);

            // Jump sound needs the delay to avoid the sound bug
            // All other sounds can repeat
            WaitForAudio(SetIsPlaying, name == "Jump" ? clip.length : 0f);
            SetIsPlaying();
        }
    }

    // Function used to stop the audio clip
    public void Stop()
    {
        source.Stop();
    }

    // Function used as a work-around fix for a bug (found with the jumping sound)
    // Basically, just using PlayOneShot would make the sound repeat more than one time, in quick sucessions
    // Since this class is not derivated from MonoBehaviour, its not possible to use Coroutines
    // So this function creates a delay before the jump sound can be played again, ensuring it will be played a single time
    // per jump
    public static async void WaitForAudio(Action action, float duration)
    {
        await Task.Delay(TimeSpan.FromSeconds(duration));
        action.Invoke();
    }

    // Function to reverse the bool isPlaying state
    public void SetIsPlaying()
    {
        isPlaying = !isPlaying;
    }
}