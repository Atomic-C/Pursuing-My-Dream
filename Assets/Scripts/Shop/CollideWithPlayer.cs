using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple script to reactivate collisions with the player
/// </summary>
public class CollideWithPlayer : MonoBehaviour
{
    /// <summary>
    /// Bool to use or not the feature (usable only in the shop)
    /// </summary>
    bool _useFeature;

    private void Awake()
    {
        // In the shop? Activate the feature
        _useFeature = SceneManager.GetActiveScene().name == "Shop" ? true : false;
    }

    private void Start()
    {
        if (_useFeature)
            Invoke("ReactivateCollision", 0.5f);
    }
    
    /// <summary>
    /// Reactivate collisions with the player
    /// </summary>
    private void ReactivateCollision()
    {
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<CircleCollider2D>(), false);
    }
}
