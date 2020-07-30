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

    public float[] gearSpeeds = new float[] { };
    public float[] gearHorsePower = new float[] { };
    public float[] gearJerks = new float[] { };
    public int gear = 0;
    
    private void Start()
    {
        this.gameObject.GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        toggleLights();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //draws the center of mass
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position + centerOfMass, 0.5f);
    }

    //gets forward velocity
    Vector3 getForwardVelocity()
    {
        return transform.InverseTransformDirection(this.GetComponent<Rigidbody>().velocity);
    }

    //toggles on and off all the lights
    void toggleLights()
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

}
