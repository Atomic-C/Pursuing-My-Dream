using UnityEngine;

// Surface Effector 2d doesnt work when using the player rigidbody for movement
// The Platform_Movement script sets the player rigidbody velocity directly
// So this script is a work-around fix for this problem
public class SurfaceEffector2d : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2d; 

    // Start is called before the first frame update
    void Start()
    {
        surfaceEffector2d = GetComponent<SurfaceEffector2D>();
    }

    // Affects the player collider transform when in contact to the surface effector 2d, changing its x position depending on the effector speed property
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.localPosition += new Vector3(surfaceEffector2d.speed, 0f, 0f) * Time.deltaTime;
        }
    }
}
