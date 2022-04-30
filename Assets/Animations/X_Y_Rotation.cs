using UnityEngine;

// Simple script used to animate any sprite, in a rotation motion 
// based on one of the 3 axis: x, y or z
public class X_Y_Rotation : MonoBehaviour
{
    // Enum used to dynamically set which axis to rotate
    public WhichAxis vector;

    public int speed = 200;

    // Vector3 used in the Rotate function
    private Vector3 rotation;

    void Start()
    {
        // Set the speed to rotate the axis, at start
        switch (vector)
        {
            case WhichAxis.X:
                rotation.x = speed;
                break;
            case WhichAxis.Y:
                rotation.y = speed;
                break;
            case WhichAxis.Z:
                rotation.z = speed;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the object using the selected axis
        this.gameObject.transform.Rotate(rotation * Time.deltaTime);


        this.GetComponent<PointEffector2D>().gameObject.SetActive(true);
    }

    // Enum used to specify which axis to rotate
    public enum WhichAxis
    {
        X,
        Y,
        Z
    } 

}
