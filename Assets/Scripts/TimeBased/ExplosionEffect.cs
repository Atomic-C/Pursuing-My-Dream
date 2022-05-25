using System.Collections;
using UnityEngine;

/// <summary>
/// Simple script to remove the game object that contains the explosion particle system
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyFX());        
    }

    /// <summary>
    /// Routine that disables the point effector 2D, to avoid a persistent repulsion of the player (the idea is just a knock back)
    /// And destroy this game object after the particle system finishes it animation
    /// </summary>
    /// <returns></returns>
    IEnumerator DestroyFX()
    {
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<PointEffector2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
