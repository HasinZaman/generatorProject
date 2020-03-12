using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret : MonoBehaviour
{
    public GameObject gunBarrel;
    public float gunElavation = 0;
    public float targetElavation = 0;
    public float elavationAcceleration = 0;
    public float[] gunElavationLimit = new float[2] { -25, 12 };

    public float turretRotationRate = 0;

    public void setTargetElavation(float newTarget)
    {
        if (newTarget < gunElavationLimit[0])
        {
            targetElavation = gunElavationLimit[0];
        }
        else if (newTarget > gunElavationLimit[1])
        {
            targetElavation = gunElavationLimit[1];
        }
        else
        {
            targetElavation = newTarget;
        }
    }
    float angleConverter(float angle)
    {
        if (angle > 0)
        {
            return angle;
        }
        return 360 + angle;
    }

    private void updateAngle()
    {
        float deltaRotation = elavationAcceleration * Time.deltaTime;
        Vector3 angle = gunBarrel.transform.localEulerAngles;
        
        if (targetElavation != angle.x)
        {
            if (Math.Abs(targetElavation - gunElavation) > deltaRotation)
            {
                gunElavation += Math.Abs(targetElavation - gunElavation) / (targetElavation - gunElavation) * deltaRotation;
                angle.x += Math.Abs(targetElavation - gunElavation) / (targetElavation - gunElavation) * deltaRotation;
            }
            else
            {
                gunElavation = targetElavation;
                angle.x = angleConverter(targetElavation);
            }

            gunBarrel.transform.localEulerAngles = angle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 turret = this.transform.localEulerAngles;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            setTargetElavation(targetElavation - 1);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            setTargetElavation(targetElavation + 1);
        }

        updateAngle();

        if (Input.GetKey(KeyCode.RightArrow))
        {
            turret.y += turretRotationRate * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            turret.y -= turretRotationRate * Time.deltaTime;
        }

        this.transform.localEulerAngles = turret;
    }
}
