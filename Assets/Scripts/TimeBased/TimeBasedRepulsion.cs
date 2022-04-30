using UnityEngine;

public class TimeBasedRepulsion : MonoBehaviour
{
    public float activateTimer;
    
    public float activateTimerSet = 2f;

    private void Start()
    {
        activateTimer = activateTimerSet;
    }

    // Update is called once per frame
    void Update()
    {
        activateTimer -= Time.deltaTime;
        if (activateTimer <= 0)
        {
            Activate();
            Invoke("Deactivate", this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            activateTimer = activateTimerSet;
        } 
    }
    
    void Activate()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Activate", true);
        this.gameObject.GetComponent<PointEffector2D>().enabled = true;
    }

    void Deactivate()
    {
        this.gameObject.GetComponent<Animator>().SetBool("Activate", false);
        this.gameObject.GetComponent<PointEffector2D>().enabled = false;
    }
}


