using UnityEngine;

/// <summary>
/// This script simulates an timed explosion that has a knock back effect in the player (to be improved)
/// </summary>
public class TimeBasedRepulsion : MonoBehaviour
{
    /// <summary>
    /// Actual timer that will be affected by the passing of time
    /// </summary>
    public float activateTimer;

    /// <summary>
    /// Float that holds the value that the activateTimer will be reset too, after every explosion
    /// </summary>
    public float activateTimerSet = 2f;

    private CircleCollider2D circleCollider2D;

    private PointEffector2D pointEffector2D;

    private Animator animator;

    private void Awake()
    {
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        pointEffector2D = gameObject.GetComponent<PointEffector2D>();
        animator = gameObject.GetComponent<Animator>();
    }

    /// <summary>
    /// Initialize activateTimer variable
    /// </summary>
    private void Start()
    {
        activateTimer = activateTimerSet;
    }

    // Update is called once per frame
    /// <summary>
    /// ActivateTimer is deduced by the passing time and when it hits 0 or below, activate the explosion immediately and call the Deactivate function after the explosion
    /// animation ends. After that, reset the activateTimer variable to its initial value
    /// </summary>
    void Update()
    {
        activateTimer -= Time.deltaTime;

        if (activateTimer <= 0)
        {
            Activate();
            AudioManager.instance.PlaySound("Explosion", gameObject.transform.position);
            Invoke("Deactivate", animator.GetCurrentAnimatorStateInfo(0).length);
            activateTimer = activateTimerSet;
        } 
    }

    /// <summary>
    /// Activates the explosion animation and the Point Effector 2d, that is responsible for the knock back effect
    /// </summary>
    void Activate()
    {
        animator.SetBool("Activate", true);
        circleCollider2D.enabled = true;
        pointEffector2D.enabled = true;
    }

    /// <summary>
    /// Deactivate both
    /// </summary>
    void Deactivate()
    {
        animator.SetBool("Activate", false);
        circleCollider2D.enabled = false;
        pointEffector2D.enabled = false;
    }
}


