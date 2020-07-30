using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vehicleDoor : MonoBehaviour
{
    public float[] doorRange;

    public float curentAngle;
    public float targetAngle;

    public int rotationAxis;

    public float rotationSpeed;

    float[] startRotation;

    private void Start()
    {
        startRotation = new float[] { transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z };
    }

    //checks if the target angle is within the door's rotation range
    public void targetAngleSet(float newTarget)
    {
        if (newTarget < doorRange[0])
        {
            targetAngle = doorRange[0];
        }
        else if (newTarget > doorRange[1])
        {
            targetAngle = doorRange[1];
        }
        else
        {
            targetAngle = newTarget;
        }
    }

    //updates the angles inorder to between 360 - 0 degrees
    private float angleConverter(float angle)
    {
        if (angle > 0)
        {
            return angle;
        }
        return 360 + angle;
    }

    //updates the dooors angle
    private void updateAngle()
    {
        float deltaRotation = rotationSpeed * Time.deltaTime;
        
        float[] newAngle = startRotation;
        
        if (targetAngle != curentAngle && doorRange[0] <= targetAngle && targetAngle <= doorRange[1])
        {
            if (Math.Abs(targetAngle - curentAngle) > deltaRotation)
            {
                if(doorRange[0] <= targetAngle && targetAngle <= curentAngle)
                {
                    curentAngle -= deltaRotation;
                }
                else if(curentAngle <= targetAngle && targetAngle <= doorRange[1])
                {
                    curentAngle += deltaRotation;
                }
                newAngle[rotationAxis] = angleConverter(curentAngle);
                
            }
            else
            {
                curentAngle = targetAngle;
                newAngle[rotationAxis] = angleConverter(targetAngle);
            }

            this.transform.localEulerAngles = new Vector3(newAngle[0], newAngle[1], newAngle[2]);
            
        }
    }

    // Update is called once per frame
    private void Update()
    {
        updateAngle();
    }
}
