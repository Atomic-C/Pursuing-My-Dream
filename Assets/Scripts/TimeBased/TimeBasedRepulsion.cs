using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// This script simulates an timed explosion that has a knock back effect in the player (to be improved)
/// </summary>
public class TimeBasedRepulsion : MonoBehaviour
{
    /// <summary>
    /// Float that holds the value that the activateTimer will be reset too, after every explosion
    /// </summary>
    public float activateTimerSet = 2f;

    /// <summary>
    /// Actual timer that will be affected by the passing of time
    /// </summary>
    private float _activateTimer;

    /// <summary>
    /// Circle collider 2D responsible for destroying bullets that enter into contact with the explosion radius
    /// </summary>
    private CircleCollider2D _circleCollider2D;

    /// <summary>
    /// This object point effector 2D, responsible for the knock back effect
    /// </summary>
    private PointEffector2D _pointEffector2D;

    /// <summary>
    /// This object light2D (the explosion emits light)
    /// </summary>
    private Light2D _light2D;

    /// <summary>
    /// This object animator
    /// </summary>
    private Animator _animator;

    private void Awake()
    {
        _circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
        _pointEffector2D = gameObject.GetComponent<PointEffector2D>();
        _animator = gameObject.GetComponent<Animator>();
        _light2D = gameObject.GetComponent<Light2D>();
    }

    /// <summary>
    /// Initialize activateTimer variable
    /// </summary>
    private void Start()
    {
        _activateTimer = activateTimerSet;
        _circleCollider2D.enabled = false;
        _pointEffector2D.enabled = false;
        _light2D.enabled = false;
    }

    // Update is called once per frame
    /// <summary>
    /// ActivateTimer is deduced by the passing time and when it hits 0 or below, activate the explosion immediately and call the Deactivate function after the explosion
    /// animation ends. After that, reset the activateTimer variable to its initial value
    /// </summary>
    void Update()
    {
        _activateTimer -= Time.deltaTime;

        if (_activateTimer <= 0)
        {
            Activate();
            AudioManager.instance.PlaySound("Explosion", gameObject.transform.position);
            StartCoroutine(Deactivate());
            _activateTimer = activateTimerSet;
        } 
    }

    /// <summary>
    /// Activates the explosion animation and the Point Effector 2d, that is responsible for the knock back effect
    /// </summary>
    void Activate()
    {
        _animator.SetBool("Activate", true);
        _circleCollider2D.enabled = true;
        _pointEffector2D.enabled = true;
    }

    /// <summary>
    /// Deactivate both
    /// </summary>
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        _animator.SetBool("Activate", false);
        _circleCollider2D.enabled = false;
        _pointEffector2D.enabled = false;
        _light2D.enabled = true;
        yield return new WaitForSeconds(.15f);
        _light2D.enabled = false;

        StopCoroutine("Deactivate");
    }
}


