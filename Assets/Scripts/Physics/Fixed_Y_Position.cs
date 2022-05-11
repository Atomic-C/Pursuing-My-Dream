using UnityEngine;

// The idea: to have a game object just outside of the camera view, so to create a cool effect of plant pollen that comes flying towards the player
// The tweak: this script is used to make this game object (which has a particle system attached, responsible for creating the flying pollen)
// have a fixed Y axis position, but still follows the camera in the X axis
public class Fixed_Y_Position : MonoBehaviour
{
    // The fixed Y axis position the game object will stay
    public float yPosition;
    // The camera position
    public Transform cameraPosition;

    // Start is called before the first frame update
    // Initialize the game object position and...
    void Start()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(gameObject.transform.position.x, yPosition, gameObject.transform.position.z), Quaternion.identity);    
    }

    // Update is called once per frame
    // ...maintain the position in the Y axis, as dictated by the float yPosition
    void Update()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(gameObject.transform.position.x, yPosition, gameObject.transform.position.z), Quaternion.identity);
    }
}
 