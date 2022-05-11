using System.Collections;
using UnityEngine;

// Simple script to remove the game object that contains the explosion particle system
public class ExplosionEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyFX());        
    }

    // Routine that disables the point effector 2D, to avoid a persistent repulsion of the player (the idea is just a knock back)
    // And destroy this game object after the particle system finishes it animation
    IEnumerator DestroyFX()
    {
        yield return new WaitForSeconds(0.4f);
        this.GetComponent<PointEffector2D>().enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
