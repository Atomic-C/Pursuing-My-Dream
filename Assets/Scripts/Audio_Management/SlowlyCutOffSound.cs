using UnityEngine;

// As the name implies, this is a simple script to simulate the gradual loss of hearing
// Used by the black hole, i found it interesting to have this effect when the player enters into contact with the black hole
public class SlowlyCutOffSound : MonoBehaviour
{
    // The camera high pass filter
    public AudioHighPassFilter highFilter;
    public AudioLowPassFilter lowFilter;
    public float cutOffFrequency;
    public bool enteredCutOffZone;
    public WhichFilter whichFilter;


    public int changeNum;

    private int minCutOffFrequency = 10;
    private int maxCutOffFrequency = 22000;

    // Start is called before the first frame update
    void Start()
    {
        if (whichFilter == WhichFilter.HIGHPASS)
            cutOffFrequency = highFilter.cutoffFrequency;
        if (whichFilter == WhichFilter.LOWPASS)
            cutOffFrequency = lowFilter.cutoffFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if (whichFilter == WhichFilter.HIGHPASS)
            UsingHighPass();
        if (whichFilter == WhichFilter.LOWPASS)
            UsingLowPass();
    }

    private void UsingHighPass()
    {
        if (enteredCutOffZone)
        {
            if (cutOffFrequency != maxCutOffFrequency)
            {
                highFilter.cutoffFrequency = cutOffFrequency;
                cutOffFrequency += changeNum;
                if (cutOffFrequency > maxCutOffFrequency)
                {
                    cutOffFrequency = maxCutOffFrequency;
                    highFilter.cutoffFrequency = cutOffFrequency;
                }
            }
        }
        else
        {
            if (cutOffFrequency != minCutOffFrequency)
            {
                highFilter.cutoffFrequency = cutOffFrequency;
                cutOffFrequency -= changeNum * 20;
                if (cutOffFrequency < minCutOffFrequency)
                {
                    cutOffFrequency = minCutOffFrequency;
                    highFilter.cutoffFrequency = cutOffFrequency;
                }
            }
        }
    }

    private void UsingLowPass()
    {
        if (enteredCutOffZone)
        {
            if (cutOffFrequency != minCutOffFrequency)
            {
                lowFilter.cutoffFrequency = cutOffFrequency;
                cutOffFrequency -= changeNum;
                if (cutOffFrequency < minCutOffFrequency)
                {
                    cutOffFrequency = minCutOffFrequency;
                    lowFilter.cutoffFrequency = cutOffFrequency;
                }
            }
        }
        else
        {
            if (cutOffFrequency != maxCutOffFrequency)
            {
                lowFilter.cutoffFrequency = cutOffFrequency;
                cutOffFrequency += changeNum * 20;
                if (cutOffFrequency > maxCutOffFrequency)
                {
                    cutOffFrequency = maxCutOffFrequency;
                    lowFilter.cutoffFrequency = cutOffFrequency;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enteredCutOffZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        enteredCutOffZone = false;
    }

    public enum WhichFilter {
        HIGHPASS,
        LOWPASS
    }
}
