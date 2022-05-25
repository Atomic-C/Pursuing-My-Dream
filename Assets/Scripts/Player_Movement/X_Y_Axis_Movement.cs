using UnityEngine;

/// <summary>
/// Simple class that controls movement as a topdown game
/// </summary>
public class X_Y_Axis_Movement : MonoBehaviour
{
    /// <summary>
    /// Movement speed
    /// </summary>
    public float speed = 10f; 

    /// <summary>
    /// Controls the player movement using the keyboard or the mouse
    /// </summary>
    void FixedUpdate()
    {
        // with keyboard
        float V = Input.GetAxis("Vertical") * speed;
        float H = Input.GetAxis("Horizontal") * speed;
        transform.Translate(new Vector3(H * Time.deltaTime, V * Time.deltaTime, 0));
        
        // with mouse
        /*float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");
        transform.Translate(new Vector3(mX * Time.deltaTime, mY * Time.deltaTime, 0));*/

    }
}
