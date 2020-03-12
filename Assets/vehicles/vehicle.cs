using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vehicle : MonoBehaviour
{
    public List<GameObject> wheels;
    public List<GameObject> steerableWheels;

    void wheelTorque(float targetTorque)
    {
        for(int i1 = 0; i1 < wheels.Count; i1++)
        {
            wheels[i1].GetComponent<wheel>().setTargetTorque(targetTorque);
        }
    }
    void wheelSteering(float targetAngle)
    {
        for (int i1 = 0; i1 < steerableWheels.Count; i1++)
        {
            steerableWheels[i1].GetComponent<wheel>().setTargetWheelAngle(targetAngle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            wheelTorque(9999);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            wheelTorque(-9999);
        }
        else
        {
            wheelTorque(0);
        }

        if (Input.GetKey(KeyCode.A))
        {
            wheelSteering(-90);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            wheelSteering(90);
        }
        else
        {
            wheelSteering(0);
        }
    }
}
