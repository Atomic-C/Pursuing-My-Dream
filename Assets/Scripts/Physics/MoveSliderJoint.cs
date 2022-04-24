using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSliderJoint : MonoBehaviour
{
    public SliderJoint2D slider;
    public JointMotor2D auxiliarymotor;

    public float yPositiveSpeed;
    public float yNegativeSpeed;
    void Start()
    {
        auxiliarymotor = slider.motor;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(slider.limitState == JointLimitState2D.LowerLimit)
        {

            auxiliarymotor.motorSpeed = yPositiveSpeed;
            slider.motor = auxiliarymotor;

        }

        if (slider.limitState == JointLimitState2D.UpperLimit)
        {

            auxiliarymotor.motorSpeed = yNegativeSpeed;
            slider.motor = auxiliarymotor;

        }

    }
}
