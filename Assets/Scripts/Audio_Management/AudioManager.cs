using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Class that controls, dynamically, the sounds and musics of the game
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Instance of the audio manager 
    /// </summary>
    public static AudioManager instance = null;

    // As i understood, serialize field allows the access to this property in the editor
    // at the same time, not letting other scripts to access this directly
    [SerializeField]
    /// <summary>
    /// Array of custom class that holds info about the sound to be played   
    /// </summary>
    private Sound[] _sounds;

    /// <summary>
    /// Singleton design pattern to guarantee only one instance of this exists
    /// </summary>
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

    /// <summary>
    /// Create the same ammount of game objects, as the ammount of sounds array and make them children of this game object
    /// Call the Sound function SetSource, passing to it a new AudioSource component, basically ensuring that all the sounds
    /// of the array, have an audio source each, dynamically
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < _sounds.Length; i++)
        {
            GameObject newSound = new GameObject("Sound_" + i + "_" + _sounds[i].name);
            newSound.transform.SetParent(this.transform);
            _sounds[i].SetSource(newSound.AddComponent<AudioSource>());
        }
        PlaySound("Music", Vector3.zero);
    }

    /// <summary>
    /// Function that plays the sound with the respective sound name and at a specific position
    /// </summary>
    /// <param name="soundName">The sound name to be played</param>
    /// <param name="positionToPlay">The position it will be played</param>
    public void PlaySound(string soundName, Vector3 positionToPlay)
    {
        for (int i = 0; i < _sounds.Length; i++)
        {
            if (_sounds[i].name == soundName)
            {
                _sounds[i].Play(positionToPlay);
                return;
            }
        }
    }

    /// <summary>
    /// Function that stops the sound with the respective sound name 
    /// </summary>
    /// <param name="soundName">The sound name to be stopped</param>
    public void StopSound(string soundName)
    {
        for (int i = 0; i < _sounds.Length; i++)
        {
            if (_sounds[i].name == soundName)
            {
                _sounds[i].Stop();
                return;
            }
        }
    }

    /// <summary>
    /// Function to get a specific sound from the audio manager
    /// </summary>
    /// <param name="soundName">The name of the sound</param>
    /// <returns></returns>
    public Sound GetSound(string soundName)
    {
        Sound sound = null;

        for (int i = 0; i < _sounds.Length; i++)
        {
            if (_sounds[i].name == soundName)
            {
                sound = _sounds[i];
            }
        }
        return sound;
    }
}

// System.Serializable allows the manipulation of custom classes in the editor
[System.Serializable]
/// <summary>
/// Custom class that holds info about a sound in the game
/// </summary>
public class Sound
{
    /// <summary>
    /// The sound name
    /// </summary>
    public string name;

    /// <summary>
    /// The sound volume
    /// </summary>
    [Range(0f, 1f)]
    public float volume = 0.7f;

    /// <summary>
    /// The sound pitch
    /// </summary>
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    /// <summary>
    /// Float that holds a value to be used as a randomizer to the sound volume, so that the sounds is always played at different volumes in order to not become tiresome
    /// </summary>
    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;

    /// <summary>
    /// Float that holds a value to be used as a randomizer to the sound pitch, so that the sounds is always played at different pitchs in order to not become tiresome
    /// </summary>
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    /// <summary>
    /// Bool used to control aspects of the sound
    /// </summary>
    public bool loop, isMusic;

    /// <summary>
    /// Bool used to determine if this sound is emitted by the player, directly or on touch
    /// If it is, there's no need to used the AudioSource.PlayClipAtPoint function
    /// </summary>
    public bool fromPlayer;

    /// <summary>
    /// The sound audio clip
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// Bool used to control aspects of the sound
    /// </summary>
    private bool _isPlaying;

    /// <summary>
    /// The sound audio source
    /// </summary>
    public AudioSource source { get; set; }

    /// <summary>
    /// Set the properties below accordingly to the choices in the editor 
    /// </summary>
    /// <param name="audioSource">The audio source that will be passed as this sound audio source*</param>
    public void SetSource(AudioSource audioSource)
    {
        source = audioSource;
        source.clip = clip;
        source.loop = loop;
    }

    /// <summary>
    /// Function used to play the audio clip
    /// If it is a music (isMusic bool), call the Play function
    /// If its not a music (SFX), call the PlayClipAtPoint function
    /// If the sound comes from the player, uses the PlayOneShot function since there is no need to play the sound at a determined location
    /// A simple fix to some sound bugs when the camera is focused distant from the player
    /// </summary>
    /// <param name="positionToPlay">Position that will be used in the PlayClipAtPoint function (SFX only)</param>
    public void Play(Vector3 positionToPlay)
    {
        source.volume = volume * (1 + UnityEngine.Random.Range(-randomVolume / 2f, randomVolume / 2f));
        source.pitch = pitch * (1 + UnityEngine.Random.Range(-randomPitch / 2f, randomPitch / 2f));
        
        if (isMusic)
            source.Play();
        else
             if (!_isPlaying)
        {
            if(fromPlayer)
                source.PlayOneShot(clip);
            else
            AudioSource.PlayClipAtPoint(clip, positionToPlay);

            // Jump sound needs the delay to avoid the sound bug
            // All other sounds can repeat
            WaitForAudio(SetIsPlaying, name == "Jump" ? clip.length : 0f);
            SetIsPlaying();
        }
    }

    /// <summary>
    /// Function used to stop the audio clip
    /// </summary>
    public void Stop()
    {
        source.Stop();
    }

    /// <summary>
    /// Function used as a work-around fix for a bug (found with the jumping sound)
    /// Basically, just using PlayOneShot would make the sound repeat more than one time, in quick sucessions
    /// Since this class is not derivated from MonoBehaviour, its not possible to use Coroutines
    /// So this function creates a delay before the jump sound can be played again, ensuring it will be played a single time
    /// per jump
    /// </summary>
    /// <param name="action">The action (function in this case) that will run</param>
    /// <param name="duration">The delay before it runs</param>
    public static async void WaitForAudio(Action action, float duration)
    {
        await Task.Delay(TimeSpan.FromSeconds(duration));
        action.Invoke();
    }

    /// <summary>
    /// Function to reverse the bool isPlaying state
    /// </summary>
    public void SetIsPlaying()
    {
        _isPlaying = !_isPlaying;
    }
}