using UnityEngine;

// The player will be jumping down and from below platforms with the Platform Effector
// This activate these platforms Is Trigger property to TRUE, allowing the player to fall
// And deactivate the collider of the platform, if jumping from below, to avoid jump animation bugs
// This scrip is responsible for reseting the platform back to its default properties (is trigger = false and collider enabled = true)
public class ResetPlatform : MonoBehaviour
{
    // Hold the platform collider 2d
    private Collider2D thisCollider2d;
    // Float uset in the Invoke function
    public float invokeTimer = 0.4f;

    // Get the collider2d component
    private void Start()
    {
        thisCollider2d = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    // Reset the platform properties if is trigger or the collider is changed
    void Update()
    {
        if (thisCollider2d.isTrigger == true || thisCollider2d.enabled == false)
        {
            Invoke("ResetProperties", invokeTimer);
        }
    }

    // Function to reset the properties
    private void ResetProperties()
    {
        thisCollider2d.isTrigger = false;
        thisCollider2d.enabled = true;
    }
}
