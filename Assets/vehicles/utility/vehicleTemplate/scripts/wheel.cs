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

    [Header("brakes")]
    public float brakeTorque = 1;
    
    [Header("wheel")]
    Vector3 pos;
    Quaternion rot;
    
    //sets a target angle that doesn't lie outside the wheels minimum and maximum angle 
    public void setTargetWheelAngle(float newTarget)
    {
        targetAngle = Mathf.Min(Mathf.Max(steeringRange[0], newTarget), steeringRange[1]);
    }
    
    //updates the wheel angle
    private void steer()//replace with the limitedRotation class
    {
        //if the wheel is already pointed in the target angle then the funtion is pre maturely ended
        if (targetAngle == wheelAngle)
        {
            return;
        }
        
        float deltaRotation = rotationSpeed * Time.deltaTime;

        if (Math.Abs(targetAngle - wheelAngle) > deltaRotation)
        {
            if (steeringRange[0] <= targetAngle && targetAngle <= wheelAngle)
            {
                wheelAngle -= deltaRotation;
            }
            else if (wheelAngle <= targetAngle && targetAngle <= steeringRange[1])
            {
                wheelAngle += deltaRotation;
            }
        }
        else
        {
            wheelAngle = targetAngle;
        }

        wheelCollider.steerAngle = wheelAngle;
    }

    //calculats the torque inroder to achive a target velocity
    public float move(float targetSpeed, float currentSpeed, float horsePower)
    {
        float torque, RPM;
        float speedDiffrence = targetSpeed - currentSpeed;
        float mod = 1;

        if(speedDiffrence == 0)
        {
            return 0f;
        }
        else if(targetSpeed < 0)
        {
            mod = -1;
            speedDiffrence *= -1;
        }

        RPM = speedDiffrence / wheelCollider.radius * 9.5488f;


        torque = horsePower * 0.75f * RPM * mod;

        return torque;
    }

    // Update is called once per frame
    void Update()
    {
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
