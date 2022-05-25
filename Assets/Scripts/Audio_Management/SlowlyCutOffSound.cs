using UnityEngine;

/// <summary>
/// As the name implies, this is a simple script to simulate the gradual loss of hearing 
/// Used by the black hole, i found it interesting to have this effect when the player enters into contact with the black hole
/// </summary>
public class SlowlyCutOffSound : MonoBehaviour
{
    /// <summary>
    /// The camera high pass filter 
    /// </summary>
    public AudioHighPassFilter highFilter;

    /// <summary>
    /// The camera low pass filter 
    /// </summary>
    public AudioLowPassFilter lowFilter;

    /// <summary>
    /// Bool changed when the player enter this game object trigger collider
    /// </summary>
    public bool enteredCutOffZone;

    /// <summary>
    /// Enum that determines which audio filter will be used
    /// </summary>
    public WhichFilter whichFilter;

    /// <summary>
    /// Int used to increment / decrement the audio filter frequency over time
    /// </summary>
    [SerializeField]
    private int frequencyNum;

    /// <summary>
    /// Int used as a multiplier to boost the audio normalize speed, after the player exited the trigger area
    /// It is not used for when the player enter this area, because a faster audio "cut" is buggy, so to avoid this drastic change and the audio bug
    /// this is only used to normalize the audio
    /// </summary>
    [SerializeField]
    private int normalizeAudio;

    /// <summary>
    /// Int that holds the audio filter minimum frequency value
    /// </summary>
    private int minCutOffFrequency = 10;

    /// <summary>
    /// Int that holds the audio filter maximum frequency value
    /// </summary>
    private int maxCutOffFrequency = 22000;

    /// <summary>
    /// Call the respective function depending on which audio filter is being used
    /// </summary>
    void Update() 
    {
        if (whichFilter == WhichFilter.HIGHPASS)
            UsingHighPass();
        if (whichFilter == WhichFilter.LOWPASS)
            UsingLowPass();
    }

    /// <summary>
    /// Function using the high pass audio filter. Since the effects kicks at a higher frequency (22000 mutes the sound, basically), the audio filter frequency is incremented
    /// as time passes by a fixed ammount, when the player enters the trigger area. And stops after hiting the frequency cap of 22000. It is quickly decremented after the player
    /// exits the trigger area
    /// </summary>
    private void UsingHighPass()
    {
        if (enteredCutOffZone)
        {
            if (highFilter.cutoffFrequency < maxCutOffFrequency)
            {
                highFilter.cutoffFrequency += frequencyNum;
                if (highFilter.cutoffFrequency >= maxCutOffFrequency)
                {
                    highFilter.cutoffFrequency = maxCutOffFrequency;
                }
            }
        }
        else
        {
            if (highFilter.cutoffFrequency > minCutOffFrequency)
            {
                highFilter.cutoffFrequency -= frequencyNum * normalizeAudio;
                if (highFilter.cutoffFrequency <= minCutOffFrequency)
                {
                    highFilter.cutoffFrequency = minCutOffFrequency;
                }
            }
        }
    }

    /// <summary>
    /// Function using the low pass audio filter. Since the effects kicks at a lower frequency (10 mutes the sound, basically), the audio filter frequency is decremented
    /// as time passes by a fixed ammount, when the player enters the trigger area. And stops after hiting the frequency cap of 10. It is quickly incremented after the player
    /// exits the trigger area
    /// </summary>
    private void UsingLowPass()
    {
        if (enteredCutOffZone)
        {
            if (lowFilter.cutoffFrequency != minCutOffFrequency)
            {
                lowFilter.cutoffFrequency -= frequencyNum;
                if (lowFilter.cutoffFrequency < minCutOffFrequency)
                {
                    lowFilter.cutoffFrequency = minCutOffFrequency;
                }
            }
        }
        else
        {
            if (lowFilter.cutoffFrequency != maxCutOffFrequency)
            {
                lowFilter.cutoffFrequency += frequencyNum * normalizeAudio;
                if (lowFilter.cutoffFrequency > maxCutOffFrequency)
                {
                    lowFilter.cutoffFrequency = maxCutOffFrequency;
                }
            }
        }
    }

    /// <summary>
    /// Player entered the trigger area, so set the enteredCutOffZone bool to true
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        enteredCutOffZone = true;
    }

    /// <summary>
    /// Player exited the trigger area, so set the enteredCutOffZone bool to false
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        enteredCutOffZone = false;
    }

    /// <summary>
    /// Enum used to determine which audio filter will be used
    /// </summary>
    public enum WhichFilter {
        HIGHPASS,
        LOWPASS
    }
}
