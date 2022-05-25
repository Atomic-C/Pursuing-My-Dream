using UnityEngine;

/// <summary>
/// The player will be jumping down and from below platforms with the Platform Effector
/// This activate these platforms Is Trigger property to TRUE, allowing the player to fall
/// And deactivate the collider of the platform, if jumping from below, to avoid jump animation bugs
/// This class is responsible for reseting the platform back to its default properties (is trigger = false and collider enabled = true)
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ResetPlatform : MonoBehaviour
{
    /// <summary>
    /// Hold the platform collider 2d
    /// </summary>
    private Collider2D thisCollider2d;

    /// <summary>
    /// Float uset in the Invoke function
    /// </summary>
    public float invokeTimer = 0.4f;

    /// <summary>
    /// Get the collider2d component
    /// </summary>
    private void Start()
    {
        thisCollider2d = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Reset the platform properties if is trigger or the collider is changed
    /// </summary>
    void Update()
    {
        if (thisCollider2d.isTrigger == true || thisCollider2d.enabled == false)
        {
            Invoke("ResetProperties", invokeTimer);
        }
    }

    /// <summary>
    /// Function to reset the properties
    /// </summary>
    private void ResetProperties()
    {
        thisCollider2d.isTrigger = false;
        thisCollider2d.enabled = true;
    }
}
