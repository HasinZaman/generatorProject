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
    public float[] steeringRange = new float[] {0, 0};
    public float wheelAngle = 0;
    public float targetAngle = 0;
    public float rotationSpeed = 10;

    [Header("movement")]
    public bool motor;
    public float breakForce = 0;
    
    [Header("wheel")]
    Vector3 pos;
    Quaternion rot;
    
    //sets a target angle that doesn't lie outside the wheels minimum and maximum angle 
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
    
    //updates the wheel angle
    private void steer()
    {
        //if the wheel is already pointed in the target angle then the funtion is pre maturely ended
        if (targetAngle == wheelAngle)
        {
            return;
        }

        float deltaRotation = rotationSpeed * Time.deltaTime;
        
        if (deltaRotation < targetAngle)
        {
            wheelAngle = Math.Min(wheelAngle + deltaRotation, targetAngle);
        }
        else
        {
            wheelAngle = Math.Max(wheelAngle - deltaRotation, targetAngle);
        }

        wheelCollider.steerAngle = wheelAngle;
    }

    // Update is called once per frame
    void Update()
    {
        if (motor)
        {
            
        }
        if (steerable)
        {
            steer();
        }

        //the wheel mesh is positioned onto the location of the wheel collider
        wheelCollider.GetWorldPose(out pos, out rot);

        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
        
    }

    //draws the max and min rotation a wheel can undergo
    private void OnDrawGizmosSelected()
    {
        if (steerable)
        {
            Gizmos.color = Color.red;
            drawWheel(steeringRange[0]);

            Gizmos.color = Color.blue;
            drawWheel(steeringRange[1]);
        }
    }

    //draws the max and min rotation a wheel can undergo
    private void drawWheel(float angle)
    {
        double radian = angle * Math.PI / 180;

        float radius = wheelCollider.radius * 2;

        Vector3 center;
        Quaternion temp = new Quaternion();
        wheelCollider.GetWorldPose(out center, out temp);
        
        
        Vector3 endPoint = new Vector3
            (
                radius * Convert.ToSingle(Math.Sin(radian)),
                0,
                radius * Convert.ToSingle(Math.Cos(radian))
            );

        Gizmos.DrawLine(center - endPoint, center + endPoint);
    }
}
