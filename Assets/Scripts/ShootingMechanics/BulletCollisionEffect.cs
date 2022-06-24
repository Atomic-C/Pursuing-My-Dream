using UnityEngine;

/// <summary>
/// Class that controls the effect of the bullet collision
/// </summary>
public class BulletCollisionEffect : MonoBehaviour
{
    /// <summary>
    /// The effect object animator
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Bool that defines if this object belongs to a pooled bullet
    /// </summary>
    public bool fromPooledObject;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Destroy this game object after the collision effect animation finishes if it doesnt belong to a pooled bullet
    /// </summary>
    void Start()
    {
        if (!fromPooledObject)
            Invoke("DestroyObject", animator.GetCurrentAnimatorStateInfo(0).length);
    }

    /// <summary>
    /// Function used when this object belongs to a pooled bullet
    /// Instead of destroying it outright, simply deactivate it for next usage
    /// </summary>
    /// <param name="bulletPosition">The last position of the bullet before being deactivated</param>
    public void Activate(Vector2 bulletPosition)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = bulletPosition;

        Invoke("Deactivate", animator.GetCurrentAnimatorStateInfo(0).length);
    }

    /// <summary>
    /// Function that is called after the animation clip finished it animation
    /// Simply deactivate the object
    /// </summary>
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Destroy the game object
    /// </summary>
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
