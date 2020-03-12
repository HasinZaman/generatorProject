using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class vehicleSetUpWindow : EditorWindow
{
    List<Transform> wheels = new List<Transform> { };

    Vector2 scrollPosition = Vector2.zero;

    GameObject wheelTemplate;
    GameObject targetVehicle;
    GameObject wheelMesh;

    float defaultSpring;
    float defaultSpringDamper;
    float defaultSpringTargetPos;
    float defaultSuspensionDist;
    float defaultWheelRadius;
    float defaultMass;


    //steering
    bool defaultSteerable;
    float[] defaultSteeringRange = new float[2] { -90, 90 };
    float wheelAngle;
    float targetAngle;
    float defaultRotationSpeed;

    //movement
    bool defaultMotor = true;
    float[] defaultWheelTorqueRange = new float[2] { -90, 90 };
    float torque = 0;
    float defaultTorqueAcceleration;

    int mirrorAxis;

    [MenuItem("Window/vehicle SetUp Window")]
    static void ShowWindow()
    {
        GetWindow<vehicleSetUpWindow>("vehicle SetUp Window");
    }

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);



        GUILayout.Label("Wheel Settings", EditorStyles.boldLabel);

        GUILayout.Space(10);

        DrawWheelButtons();

        GUILayout.Space(10);

        GUILayout.Label("objects", EditorStyles.label);
        targetVehicle = (GameObject)EditorGUILayout.ObjectField("target vehicle", targetVehicle, typeof(GameObject), true);
        wheelTemplate = (GameObject)EditorGUILayout.ObjectField("wheel prefab", wheelTemplate, typeof(GameObject), true);
        wheelMesh = (GameObject)EditorGUILayout.ObjectField("target vehicle wheel Mesh", wheelMesh, typeof(GameObject), true);

        GUILayout.Space(10);

        //wheel colider
        GUILayout.Label("wheel colider", EditorStyles.label);

        GUILayout.Space(3);

        mirrorAxis = EditorGUILayout.IntSlider("mirror Axis", mirrorAxis, 0, 2);
        defaultSpring = EditorGUILayout.Slider("spring", defaultSpring,0,9999999);
        defaultSpringDamper = EditorGUILayout.Slider("damper", defaultSpringDamper, 0, 9999999);
        defaultSpringTargetPos = EditorGUILayout.Slider("target pos", defaultSpringTargetPos, 0, 9999999);
        defaultSuspensionDist = EditorGUILayout.Slider("suspension Dist", defaultSuspensionDist, 0, 9999999);
        defaultWheelRadius = EditorGUILayout.Slider("wheel Radius", defaultWheelRadius, 0, 9999999);
        defaultMass = EditorGUILayout.Slider("wheel mass", defaultMass, 0, 9999999);

        //steering
        GUILayout.Space(10);

        GUILayout.Label("steering", EditorStyles.label);

        GUILayout.Space(3);

        defaultSteerable = EditorGUILayout.ToggleLeft("steering cond", defaultSteerable);

        EditorGUILayout.LabelField("min:", defaultSteeringRange[0].ToString());
        EditorGUILayout.LabelField("max:", defaultSteeringRange[1].ToString());
        EditorGUILayout.LabelField("midVal:", ((defaultSteeringRange[0] - defaultSteeringRange[1]) / 2 + defaultSteeringRange[0]).ToString());
        EditorGUILayout.MinMaxSlider(ref defaultSteeringRange[0], ref defaultSteeringRange[1], -90, 90);

        defaultRotationSpeed = EditorGUILayout.Slider("steering speed", defaultRotationSpeed, -100, 100);
        //wheel movement
        GUILayout.Space(10);

        GUILayout.Label("wheel motor", EditorStyles.label);

        GUILayout.Space(3);

        defaultMotor = EditorGUILayout.ToggleLeft("wheel motor", defaultMotor);

        EditorGUILayout.LabelField("min:", defaultWheelTorqueRange[0].ToString());
        EditorGUILayout.LabelField("max:", defaultWheelTorqueRange[1].ToString());
        EditorGUILayout.LabelField("midVal:", ((defaultWheelTorqueRange[0] - defaultWheelTorqueRange[1]) / 2 + defaultWheelTorqueRange[0]).ToString());
        EditorGUILayout.MinMaxSlider(ref defaultWheelTorqueRange[0], ref defaultWheelTorqueRange[1], -999, 999);

        defaultTorqueAcceleration = EditorGUILayout.Slider("torque Acceleration", defaultTorqueAcceleration, -9999999, 9999999);

        GUILayout.Space(10);

        GUILayout.Label("Door Settings", EditorStyles.boldLabel);

        GUILayout.Label("Turret Settings", EditorStyles.boldLabel);

        GUILayout.EndScrollView();
    }

    void DrawWheelButtons()
    {
        if(GUILayout.Button("Add Wheel"))
        {
            wheels.Add(createWheel(defaultMass, defaultWheelRadius, defaultSuspensionDist, defaultSpring, defaultSpringTargetPos, defaultSpringDamper, defaultWheelTorqueRange, defaultTorqueAcceleration, defaultMotor, defaultSteerable, defaultSteeringRange, defaultRotationSpeed));
        }
        else if(GUILayout.Button("Mirror Wheel"))
        {
            mirrorWheels();
        }
        else if (GUILayout.Button("reset Wheel List"))
        {
            wheels = new List<Transform> { };
        }
    }

    Transform createWheel(float mass, float wheelRadius, float suspensionDist, float spring, float springTargetPos, float springDamper, float[] wheelTorqueRange, float torqueAcceleration, bool motor, bool steerable, float[] steeringRange, float rotationSpeed)
    {
        GameObject wheel = Instantiate(wheelTemplate, targetVehicle.transform.position, Quaternion.identity, targetVehicle.transform);

        wheel.GetComponent<WheelCollider>().mass = mass;
        wheel.GetComponent<WheelCollider>().radius = wheelRadius;
        wheel.GetComponent<WheelCollider>().suspensionDistance = suspensionDist;

        JointSpring jointTemp = new JointSpring();
        jointTemp.spring = spring;
        jointTemp.targetPosition = springTargetPos;
        jointTemp.damper = springDamper;
        
        wheel.GetComponent<WheelCollider>().suspensionSpring = jointTemp;
        
        wheel wheelTemp = wheel.GetComponent<wheel>();

        wheelTemp.steerable = false;
        wheelTemp.motor = false;
        wheel.name = "wheel:";
        if (steerable)
        {
            wheel.name += "S";
            
            wheelTemp.steerable = true;
            wheelTemp.steeringRange = steeringRange;
            wheelTemp.wheelAngle = 0;
            wheelTemp.rotationSpeed = rotationSpeed;
            
            targetVehicle.GetComponent<vehicle>().steerableWheels.Add( wheel );
        }
        if(motor)
        {
            wheel.name += "M";

            wheelTemp.motor = true;

            wheelTemp.torque = 0;
            wheelTemp.wheelTorqueRange = wheelTorqueRange;
            wheelTemp.torqueAcceleration = torqueAcceleration;

            targetVehicle.GetComponent<vehicle>().wheels.Add( wheel );
        }

        GameObject wheelMeshTemp = Instantiate(wheelMesh, wheel.transform.position, Quaternion.identity, wheel.transform);

        wheel.GetComponent<wheel>().wheelMesh = wheelMeshTemp;

        return wheel.transform;

    }

    Vector3 newPos(Vector3 pos)
    {
        Vector3 temp = pos;

        switch (mirrorAxis)
        {
            case 0:
                temp.x = temp.x * - 1;
                break;
            case 1:
                temp.y = temp.y * -1;
                break;
            case 2:
                temp.z = temp.z * -1;
                break;
        }

        return temp;

    }

    void mirrorWheels()
    {
        Transform childTemp;
        Transform[] wheelsTemp = wheels.ToArray();
        Vector3 scale = Vector3.one;

        GameObject newWheel;

        switch (mirrorAxis)
        {
            case 0:
                scale.x = -1;
                break;
            case 1:
                scale.y = -1;
                break;
            case 2:
                scale.z = -1;
                break;
        }
        

        for(int i1 = 0; i1 < wheelsTemp.Length; i1++)
        {
            childTemp = wheelsTemp[i1];
            newWheel = createWheel
            (
                childTemp.GetComponent<WheelCollider>().mass,
                childTemp.GetComponent<WheelCollider>().radius,
                childTemp.GetComponent<WheelCollider>().suspensionDistance,
                childTemp.GetComponent<WheelCollider>().suspensionSpring.spring,
                childTemp.GetComponent<WheelCollider>().suspensionSpring.targetPosition,
                childTemp.GetComponent<WheelCollider>().suspensionSpring.damper,
                childTemp.GetComponent<wheel>().wheelTorqueRange,
                childTemp.GetComponent<wheel>().torqueAcceleration,
                childTemp.GetComponent<wheel>().motor,
                childTemp.GetComponent<wheel>().steerable,
                childTemp.GetComponent<wheel>().steeringRange,
                childTemp.GetComponent<wheel>().rotationSpeed
            ).gameObject;

            newWheel.transform.position = newPos(childTemp.position);
            newWheel.transform.localScale = scale;

            wheels.Add(newWheel.transform);
        }
    }
}
