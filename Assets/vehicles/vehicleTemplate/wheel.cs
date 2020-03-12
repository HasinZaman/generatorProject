using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wheel : MonoBehaviour
{
    public GameObject wheelMesh;
    public WheelCollider wheelCollider;
    public Mesh[] wheelMeshs;
    public int currentMesh = 0;

    [Header("steering")]
    public bool steerable;
    public float[] steeringRange;
    public float wheelAngle;
    public float targetAngle;
    public float rotationSpeed;

    [Header("movement")]
    public bool motor;
    public float[] wheelTorqueRange;
    public float torque = 0;
    public float targetTorque = 0;
    public float torqueAcceleration;


    [Header("wheel")]
    Vector3 pos;
    Quaternion rot;


    public void setTargetTorque(float newTarget)
    {
        if(newTarget < wheelTorqueRange[0])
        {
            targetTorque = wheelTorqueRange[0];
        }
        else if(newTarget > wheelTorqueRange[1])
        {
            targetTorque = wheelTorqueRange[1];
        }
        else
        {
            targetTorque = newTarget;
        }
    }
    public void setTargetWheelAngle(float newTarget)
    {
        if (newTarget < steeringRange[0])
        {
            targetAngle = steeringRange[0];
        }
        else if (newTarget > steeringRange[1])
        {
            targetAngle = steeringRange[1];
        }
        else
        {
            targetAngle = newTarget;
        }
    }

    private void accelerate()
    {
        if (torque == targetTorque)
        {
            return;
        }

        float deltaTorque = torqueAcceleration * Time.deltaTime;

        if (torque < targetTorque)
        {
            torque = Math.Min(torque + deltaTorque, targetTorque);
        }
        else// if(torque < targetTorque)
        {
            torque = Math.Max(torque - deltaTorque, targetTorque);
        }
        wheelCollider.motorTorque = torque;
    }
    
    private void steer()
    {
        if (targetAngle == wheelAngle)
        {
            return;
        }

        float deltaRotation = rotationSpeed * Time.deltaTime;
        
        if (torque < targetAngle)
        {
            wheelAngle = Math.Min(wheelAngle + deltaRotation, targetAngle);
        }
        else// if(torque < targetTorque)
        {
            wheelAngle = Math.Max(wheelAngle - deltaRotation, targetAngle);
        }

        wheelCollider.steerAngle = wheelAngle;
        Debug.Log("angle" + wheelCollider.steerAngle);
    }

    // Update is called once per frame
    void Update()
    {
        if (motor)
        {
            accelerate();
        }
        if (steerable)
        {
            steer();
        }

        //the next 3 lines lines up the wheel mesh and wheel coldier
        wheelCollider.GetWorldPose(out pos, out rot);

        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
        
    }
    


    

}
