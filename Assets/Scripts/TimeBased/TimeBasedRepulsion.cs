using UnityEngine;

// This script simulates an timed explosion that has a knock back effect in the player (to be improved)
public class TimeBasedRepulsion : MonoBehaviour
{
    // Actual timer that will be affected by the passing of time
    public float activateTimer;
    
    // Float that holds the value that the activateTimer will be reset too, after every explosion
    public float activateTimerSet = 2f;

    // Initialize activateTimer variable
    private void Start()
    {
        activateTimer = activateTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        // activateTimer is deduced by the passing time
        activateTimer -= Time.deltaTime;

        // And when it hits 0 or below, activate the explosion immediately and...
        if (activateTimer <= 0)
        {
            Activate();

            // ... call the Deactivate function after the explosion animation ends
            Invoke("Deactivate", this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

            // Reset the activateTimer variable to its initial value
            activateTimer = activateTimerSet;
        } 
    }
    
    // Activates the explosion animation and the Point Effector 2d, that is responsible for the knock back effect
    void Activate()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Activate", true);
        this.gameObject.GetComponent<PointEffector2D>().enabled = true;
    }

    // Deactivate both
    void Deactivate()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Activate", false);
        this.gameObject.GetComponent<PointEffector2D>().enabled = false;
    }
}


