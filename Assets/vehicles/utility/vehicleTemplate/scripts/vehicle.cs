using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

public class vehicle : MonoBehaviour
{
    public GameObject[] wheels;
    public GameObject[] turrets;
    public GameObject[] misc;

    public int steeringMethod;
    public Vector3 centerOfMass;
    
    public int gearPos = 0;
    public float[] speeds = new float[] { };
    public float[] horsePowers = new float[] { };

    public float targetSpeed = 0;
    public float targetSteering = 0;

    public bool brakeCond = false;
    public bool moveCond = false;

    private Vector3 lastP;

    public Vector3 velocityRelativeToForward = Vector3.zero;
    private void Start()
    {
        this.gameObject.GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        lastP = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wheel temp;
        
        //gets forward velocity
        velocityRelativeToForward = forwardDirection(velocity(this.transform.position, lastP));
        
        //updates every wheel
        for (int i1 = 0; i1 < wheels.Count(); i1++)
        {
            temp = wheels[i1].GetComponent<wheel>();
            
            //forward and backward movement
            if (temp.motor && moveCond)
            {
                temp.wheelCollider.motorTorque = temp.move(targetSpeed, velocityRelativeToForward.z * -1, horsePowers[gearPos]);
            }
            else
            {
                temp.wheelCollider.motorTorque = 0;
            }

            //steering
            if (temp.steerable)
            {
                temp.setTargetWheelAngle(targetSteering);
            }

            //brakes
            if (brakeCond)
            {
                temp.wheelCollider.brakeTorque = temp.brakeTorque;
            }
            else
            {
                temp.wheelCollider.brakeTorque = 0;
            }
        }

        //gets last postion for velocity calculations
        lastP = this.transform.position;
    }
    
    //toggles on and off all the lights
    public void toggleLights()
    {
        Transform temp;
        for(int i1 = 0; i1 < misc.Count(); i1++)
        {
            temp = misc[i1].transform;
            if(temp.GetChild(0).GetComponent<Light>() != null)
            {
                //toggles light on and off
                temp.GetChild(0).GetComponent<Light>().enabled = temp.GetChild(0).GetComponent<Light>().enabled == false;
            }
        }
    }

    //sets a target speed
    public void setTargetSpeed(float target)
    {
        targetSpeed = Mathf.Min(Mathf.Max(target, speeds[0]), speeds[speeds.Length - 1]);
    }

    //sets the gear that is within the gear range
    public int setGear(int gear)
    {
        return Mathf.Min(Mathf.Max(gear, 0), speeds.Length - 1);
    }

    //sets the gear based on the current speed
    public int setGear(float s)
    {
        for(int i1 = 0; i1 < speeds.Length - 1; i1++)
        {
            if(speeds[i1] < s && s < speeds[i1 + 1])
            {
                return i1;
            }
        }

        return Array.IndexOf(speeds, speeds.First(g => g == 0));
    }

    //calculates the velocity
    Vector3 velocity(Vector3 di, Vector3 df)
    {
        float t = Time.deltaTime;

        if (t == 0)
        {
            return Vector3.zero;
        }

        return (df - di) / Time.deltaTime / 1.5f;
    }
    
    //rotates forward vector relative to the forward direction
    Vector3 forwardDirection(Vector3 vector)
    {
        float yaw, pitch, roll;

        yaw = this.transform.localEulerAngles.y * Mathf.PI / 180;
        pitch = this.transform.localEulerAngles.x * Mathf.PI / 180;
        roll = this.transform.localEulerAngles.z * Mathf.PI / 180;

        float[] temp = new float[3] { vector.z, vector.x, vector.y };

        float cy, sy, cp, sp, cr, sr;

        cy = Mathf.Cos(yaw);
        sy = Mathf.Sin(yaw);

        cp = Mathf.Cos(pitch);
        sp = Mathf.Sin(pitch);

        cr = Mathf.Cos(roll);
        sr = Mathf.Sin(roll);

        float[][] temp3 = new float[][]
        {
            new float[]{ cy * cp, cy * sp * sr - sy * cr, cy * sp * cr + sy * sr },
            new float[]{ sy * cp, sy * sp * sr + cy * cr, sy * sp * cr - cy * sr },
            new float[]{ -1 * sp, cp * sr, cp * cr}
        };
        
        temp = dotProduct
        (
            temp3,
            temp
        );

        return new Vector3(temp[1], temp[2], temp[0]);
    }

    //calculates the dot product of a matrix
    float[][] dotProduct(float[][] matrix1, float[][] matrix2)
    {
        float[][] temp = new float[Mathf.Min(matrix2.Length, matrix1.Length)][];
        for (int y = 0; y < temp.Length; y++)
        {
            temp[y] = new float[Mathf.Min(matrix2[y].Length, matrix1[y].Length)];
            for (int x = 0; x < temp[y].Length; x++)
            {
                temp[y][x] = 0;

                for (int i1 = 0; i1 < matrix1[y].Length; i1++)
                {
                    temp[y][x] += matrix1[y][i1] * matrix2[i1][x];
                }

            }
        }
        return temp;
    }

    //calculates the dot product of a matrix
    float[] dotProduct(float[][] trigMatrix, float[] position)
    {
        float[] temp = new float[3];

        for (int i1 = 0; i1 < 3; i1++)
        {
            temp[i1] = trigMatrix[0][i1] * position[0] + trigMatrix[1][i1] * position[1] + trigMatrix[2][i1] * position[2];
        }

        return temp;
    }

    //draws the center of mass
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position + centerOfMass, 0.5f);
    }
}
