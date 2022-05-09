using UnityEngine;

// Script to simulate a moving platform using the Slider Joint 2d
public class MoveSliderJoint : MonoBehaviour
{
    private SliderJoint2D slider;
    private JointMotor2D auxiliarymotor;

    private Rigidbody2D rb;

    public float PositiveSpeed = 1f;
    public float NegativeSpeed = -1f;

    // Initialize the variables
    void Start()
    {
        slider = GetComponent<SliderJoint2D>();
        auxiliarymotor = slider.motor;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the slider reached its lower limit
        if(slider.limitState == JointLimitState2D.LowerLimit)
        {
            // Set its motor speed to a positive value
            auxiliarymotor.motorSpeed = PositiveSpeed;
            slider.motor = auxiliarymotor;
        }

        // If the slider reached its upper limit
        if (slider.limitState == JointLimitState2D.UpperLimit)
        {
            // Set its motor speed to a negative value
            auxiliarymotor.motorSpeed = NegativeSpeed;
            slider.motor = auxiliarymotor;
        }
    }

    // Trying to find a solution as to make the player position follow the platform
    // After some research, found the solution below, but only works when the platform rigid body is set to Kinematic
    // Which is not ideal, since the idea is to make use of the slider joint motor function, that works only when the rigid body is set to Dynamic
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }*/

    // Solution that works while still using the slider joint motor
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.localPosition += new Vector3(rb.velocity.x, rb.velocity.y, 0f) * Time.deltaTime;
        }
    }
}
