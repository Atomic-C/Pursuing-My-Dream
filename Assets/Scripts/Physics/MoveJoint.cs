using UnityEngine;

/// <summary>
/// Script to simulate a moving platform using the Slider Joint 2d
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MoveJoint : MonoBehaviour
{
    /// <summary>
    /// The object slider joint 2d
    /// </summary>
    private SliderJoint2D slider;

    /// <summary>
    /// The object hinge joint 2d
    /// </summary>
    private HingeJoint2D hinge;

    /// <summary>
    /// The slider joint 2d motor
    /// </summary>
    private JointMotor2D auxiliarymotor;

    /// <summary>
    /// Which joint 2d is being used by this object
    /// </summary>
    public JointType whichJoint;

    /// <summary>
    /// The platform rigidbody 2d
    /// </summary>
    private Rigidbody2D platformRB;

    /// <summary>
    /// Float used as positive speed for the motor
    /// </summary>
    public float PositiveSpeed = 1f;

    /// <summary>
    /// Float used as negative speed for the motor
    /// </summary>
    public float NegativeSpeed = -1f;

    /// <summary>
    /// Initialize the variables
    /// </summary>
    void Start()
    {
        switch (whichJoint)
        {
            case JointType.SLIDER:
                slider = GetComponent<SliderJoint2D>();
                auxiliarymotor = slider.motor;
                break;
            case JointType.HINGE:
                hinge = GetComponent<HingeJoint2D>();
                auxiliarymotor = hinge.motor;
                break;
        }
        platformRB = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// If the slider / hinge reached its lower limit, set its motor speed to a positive value
    /// If the slider / hinge reached its upper limit, set its motor speed to a negative value
    /// </summary>
    void Update()
    {
        switch (whichJoint)
        {
            case JointType.SLIDER:
                if (slider.limitState == JointLimitState2D.LowerLimit)
                {
                    auxiliarymotor.motorSpeed = PositiveSpeed;
                    slider.motor = auxiliarymotor;
                }

                if (slider.limitState == JointLimitState2D.UpperLimit)
                {
                    auxiliarymotor.motorSpeed = NegativeSpeed;
                    slider.motor = auxiliarymotor;
                }
                break;
            case JointType.HINGE:
                if (hinge.limitState == JointLimitState2D.LowerLimit)
                {
                    auxiliarymotor.motorSpeed = PositiveSpeed;
                    hinge.motor = auxiliarymotor;
                }

                if (hinge.limitState == JointLimitState2D.UpperLimit)
                {
                    auxiliarymotor.motorSpeed = NegativeSpeed;
                    hinge.motor = auxiliarymotor;
                }
                break;
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

    /// <summary>
    /// Solution that works while still using the slider joint motor, for making the player position follow the platform position
    /// </summary>
    /// <param name="collision">The other object collider 2d</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.localPosition += new Vector3(platformRB.velocity.x, platformRB.velocity.y, 0f) * Time.deltaTime;
        }
    }

    /// <summary>
    /// Enum used to determine which joint is being used
    /// </summary>
    public enum JointType
    {
        SLIDER,
        HINGE
    }
}
