using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

[CustomEditor(typeof(vehicle))]
public class vehicleEditor : Editor
{
    // ui
    private int tab = 0;
    private string[] tabTitles = new string[] {"vehicle", "wheels","turrets","misc"};
    
    //vehicle
    private vehicle v;

    private SerializedObject targetObject;

    private SerializedProperty steeringMethod;

    private SerializedProperty miscSerialized;

    //engine settings
    private SerializedProperty centerOfMass, gearSpeeds, gearHorsePower, gearJerks, gear;

    //wheel settings    
    public GameObject wheelTemplate;

    private string[] steeringMethodMenu = new string[] { "normal", "fourWheel", "skid", "crab" };

    private bool showPowerd = true, showSteerable = true, showDogShit = true;
    private string customWheelSearch = "";

    private GameObject wheelMesh;

    private List<GameObject> poweredWheels = new List<GameObject> { };
    private List<GameObject> steerableWheels = new List<GameObject> { };
    private List<GameObject> dogShitWheels = new List<GameObject> { };//change name
    

    //turret Settings
    public GameObject turretTemplate;
    private GameObject turretMesh;
    private GameObject barrelMesh;
    private GameObject turretParent;

    //misc settings
    private bool[] miscDropDowns = new bool[] { false, false };//antenna Settings, lights

    private GameObject antennaProp;
    private GameObject miscParent;
    private GameObject lightProp;

    public void OnEnable()
    {
        v = (vehicle)target;

        targetObject = new UnityEditor.SerializedObject(v);

        steeringMethod = targetObject.FindProperty("steeringMethod");
        miscSerialized = targetObject.FindProperty("misc");
        
        centerOfMass = targetObject.FindProperty("centerOfMass");
        gearSpeeds = targetObject.FindProperty("gearSpeeds");
        gearHorsePower = targetObject.FindProperty("gearHorsePower");
        gearJerks = targetObject.FindProperty("gearJerks");
        gear = targetObject.FindProperty("gear");

    }

    public override void OnInspectorGUI()
    {
        targetObject.Update();

        //creates GUI tab
        tab = GUILayout.Toolbar(tab, tabTitles);
        
        switch (tab)
        {
            //basic vehicle stuff
            case 0:
                getVehicleValue();
                break;

            //wheel stuff
            case 1:
                //creates a drop down memnu with each steering method
                steeringMethod.intValue = EditorGUILayout.Popup("Steering Method", steeringMethod.intValue, steeringMethodMenu);

                //creates a feild to input prefab/mesh to create a new vehicle
                wheelMesh = (GameObject)EditorGUILayout.ObjectField("Wheel Mesh", wheelMesh, typeof(GameObject));

                //adds wheel
                if (GUILayout.Button("Add Wheel"))
                {
                    makeWheel(wheelMesh);
                }

                //mirrors wheel from +x side to the -x side
                if (GUILayout.Button("Mirror Wheel"))
                {
                    mirrorObj("wheel");
                }

                getWheels();
                break;

            //turret Stuff
            case 2:
                label("Adding Turret");

                //creates feilds for adding another turret
                turretMesh = (GameObject) EditorGUILayout.ObjectField("Turret mesh", turretMesh, typeof(GameObject));
                barrelMesh = (GameObject) EditorGUILayout.ObjectField("Barrel mesh", barrelMesh, typeof(GameObject));
                turretParent = (GameObject) EditorGUILayout.ObjectField("Turret Parent", turretParent, typeof(GameObject));

                //adds a turret
                if (GUILayout.Button("Add Turret"))
                {
                    GameObject turret = Instantiate(turretTemplate, Vector3.zero, new Quaternion(), turretParent.transform);
                    turret.name = "turret";
                    Instantiate(turretMesh, turret.transform.position, turret.transform.rotation, turret.transform);
                    Instantiate(barrelMesh, turret.transform.position, turret.transform.rotation, turret.transform);
                }

                //gets all the turret and displays turret information
                getTurrets();
                break;

            //misc stuff
            case 3:

                //creates a dropdown menu that shows antenna settings
                miscDropDowns[0] = EditorGUILayout.Foldout(miscDropDowns[0], "Antenna");
                
                if(miscDropDowns[0])
                {
                    //creates feild to imput prefab/meshs to add an antenna
                    antennaProp = (GameObject)EditorGUILayout.ObjectField("Antenna prefab", antennaProp, typeof(GameObject));
                    miscParent = (GameObject)EditorGUILayout.ObjectField("Parent", miscParent, typeof(GameObject));
                   
                    if (GUILayout.Button("Add Antenna"))
                    {
                        GameObject antennaTemp = Instantiate(antennaProp, Vector3.zero, new Quaternion(), miscParent.transform);
                        antennaProp.name = "antenna";
                        antennaProp.tag = "prop";
                        //checks if the input is a mesh with or without the antenna script
                        if (antennaTemp.GetComponent<antenna>() == null)
                        {
                            antenna component = antennaTemp.AddComponent(typeof(antenna)) as antenna;
                        }
                    }
                    
                }

                //creates a drop down menu with 
                miscDropDowns[1] = EditorGUILayout.Foldout(miscDropDowns[1], "Lights");

                if (miscDropDowns[1])
                {
                    lightProp = (GameObject)EditorGUILayout.ObjectField("Light Prefab", lightProp, typeof(GameObject));
                    miscParent = (GameObject)EditorGUILayout.ObjectField("Parent", miscParent, typeof(GameObject));

                    if(GUILayout.Button("Add Light"))
                    {
                        GameObject lightTemp = Instantiate(lightProp, Vector3.zero, new Quaternion(), miscParent.transform);
                        lightTemp.name = "light";
                        lightTemp.tag = "prop";
                    }

                    if(GUILayout.Button("Mirror Lights"))
                    {
                        mirrorObj("light");
                    }
                }

                //updates the misc array
                int miscCount = 0;
                miscSerialized.ClearArray();
                for (int i1 = 0; i1 < v.transform.childCount; i1++)
                {
                    if(v.transform.GetChild(i1).tag == "prop")
                    {
                        miscSerialized.InsertArrayElementAtIndex(miscCount);
                        miscSerialized.GetArrayElementAtIndex(miscCount).objectReferenceValue = v.transform.GetChild(i1).gameObject;
                        miscCount += 1;
                    }
                }
                break;
        }


        //update modified values
        targetObject.ApplyModifiedProperties();
        try
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(v);
                EditorSceneManager.MarkSceneDirty(v.gameObject.scene);
            }
        }
        catch
        {

        }
    }

    //gets vehicle engine information
    private void getVehicleValue()
    {
        //draws the feild to input the center of mass
        label("center of mass");
        centerOfMass.vector3Value = EditorGUILayout.Vector3Field("Center of Mass", centerOfMass.vector3Value);

        //draws the feilds for the gears
        label("Gears");
        for(int i1 = 0; i1 < gearSpeeds.arraySize; i1++)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(i1.ToString());
            
            GUILayout.Label("Max Speed");

            gearSpeeds.GetArrayElementAtIndex(i1).floatValue = EditorGUILayout.FloatField(gearSpeeds.GetArrayElementAtIndex(i1).floatValue/2);
            gearSpeeds.GetArrayElementAtIndex(i1).floatValue *= 2;


            GUILayout.Label("Horse Power");

            gearHorsePower.GetArrayElementAtIndex(i1).floatValue = EditorGUILayout.FloatField(gearHorsePower.GetArrayElementAtIndex(i1).floatValue/2);
            gearHorsePower.GetArrayElementAtIndex(i1).floatValue *= 2;

            GUILayout.Label("Jerk");

            gearJerks.GetArrayElementAtIndex(i1).floatValue = EditorGUILayout.FloatField(gearJerks.GetArrayElementAtIndex(i1).floatValue/2);
            gearJerks.GetArrayElementAtIndex(i1).floatValue *= 2;

            if (GUILayout.Button("Remove"))
            {
                gearSpeeds.DeleteArrayElementAtIndex(i1);
                gearHorsePower.DeleteArrayElementAtIndex(i1);
                gearJerks.DeleteArrayElementAtIndex(i1);
                break;
            }

            GUILayout.EndHorizontal();
        }

        //adds another gear
        if (GUILayout.Button("Add"))
        {
            gearSpeeds.InsertArrayElementAtIndex(gearSpeeds.arraySize);
            gearSpeeds.GetArrayElementAtIndex(gearSpeeds.arraySize - 1).floatValue = 0;

            gearHorsePower.InsertArrayElementAtIndex(gearHorsePower.arraySize);
            gearHorsePower.GetArrayElementAtIndex(gearHorsePower.arraySize - 1).floatValue = 0;

            gearJerks.InsertArrayElementAtIndex(gearJerks.arraySize);
            gearJerks.GetArrayElementAtIndex(gearJerks.arraySize - 1).floatValue = 0;
        }

    }

    //mirror a certian object
    private void mirrorObj(string tag)
    {
        GameObject newObj;

        Vector3 position;
        Vector3 scale = new Vector3(-1, 1, 1);
        Vector3 tempScale;
        Quaternion rotation;

        //searches through all the direct children and mirror every object with a certian tag
        int childCount = v.transform.childCount;

        for (int i1 = 0; i1 < childCount; i1++)
        {
            if (v.transform.GetChild(i1).tag == tag || v.transform.GetChild(i1).name == tag)
            {
                position = v.transform.GetChild(i1).position;
                position.x *= -1;

                rotation = v.transform.GetChild(i1).rotation;
                
                newObj = Instantiate(v.transform.GetChild(i1).gameObject, position, rotation, v.transform);
                newObj.name = newObj.name.Split('(')[0];
                
                tempScale = scale;

                tempScale.x *= newObj.transform.localScale.x;
                tempScale.y *= newObj.transform.localScale.y;
                tempScale.z *= newObj.transform.localScale.z;

                newObj.transform.localScale = tempScale;

            }
        }
    }

    //creates a wheel
    private void makeWheel(GameObject mesh)
    {
        GameObject wheel = Instantiate(wheelTemplate, Vector3.zero, new Quaternion(), v.transform);
        GameObject wheelMeshTemp = Instantiate(mesh, wheel.transform.position, wheel.transform.rotation, wheel.transform);

        wheel.name = "wheel:n";

        wheel.GetComponent<wheel>().wheelMesh = wheelMeshTemp;
    }

    //draws wheel setting gui
    private void getWheels()
    {
        //creates a filter wheels
        GUILayout.BeginHorizontal("Search");

        GUILayout.Label("powered ");
        showPowerd = EditorGUILayout.Toggle(showPowerd);
        GUILayout.Label(" | ");

        GUILayout.Label("Steerable ");
        showSteerable = EditorGUILayout.Toggle(showSteerable);
        GUILayout.Label(" | ");

        GUILayout.Label("Dog shit ");
        showDogShit = EditorGUILayout.Toggle(showDogShit);

        GUILayout.EndHorizontal();

        customWheelSearch = EditorGUILayout.TextField("Search", customWheelSearch);
        
        //draws a wheel's settings for gui
        Transform wheelTemp;
        
        for (int i1 = 0; i1 < v.transform.childCount; i1++)
        {
            wheelTemp = v.transform.GetChild(i1);

            if (v.transform.GetChild(i1).tag == "wheel")
            {
                if (wheelCheckCond(wheelTemp.name.Split(':')[1].Split(',')))
                {
                    drawWheelGUI(wheelTemp.gameObject);
                }
            }
        }
    }

    //checks if a wheel fits with the filter settings
    private bool wheelCheckCond(string[] wheelType)
    {
        if (wheelType.Contains("p") && showPowerd)
        {
            return true;
        }
        else if (wheelType.Contains("s") && showSteerable)
        {
            return true;
        }
        else if (wheelType.Contains("n") && showDogShit)
        {
            return true;
        }
        else if (wheelType.Contains(customWheelSearch) && customWheelSearch.Length > 0)
        {
            return true;
        }
        return false;
    }

    //draws wheel settings
    private void drawWheelGUI(GameObject wheelObj)
    {
        //wheel Object
        SerializedObject wheelTemp = new UnityEditor.SerializedObject(wheelObj.GetComponent<wheel>());
        SerializedProperty steerable, steeringRange, wheelAngle, targetAngle, rotationSpeed, motor, breakForce;

        steerable = wheelTemp.FindProperty("steerable");
        steeringRange = wheelTemp.FindProperty("steeringRange");
        wheelAngle = wheelTemp.FindProperty("wheelAngle");
        targetAngle = wheelTemp.FindProperty("targetAngle");
        rotationSpeed = wheelTemp.FindProperty("rotationSpeed");
        breakForce = wheelTemp.FindProperty("breakForce");

        motor = wheelTemp.FindProperty("motor");

        //wheel colider settings

        //create a wheel feild inorder to display wheel obj
        WheelCollider wheelColliderTemp = (WheelCollider)wheelTemp.FindProperty("wheelCollider").objectReferenceValue;
        JointSpring suspensionSpring = wheelColliderTemp.suspensionSpring;

        wheelTemp.Update();

        //creates a wheel feilds for spring susspenssion
        GUILayout.BeginVertical("box");

        label(wheelObj.name);

        EditorGUILayout.ObjectField(wheelObj, typeof(GameObject));
        
        header("Wheel Collider");

        wheelColliderTemp.mass = EditorGUILayout.FloatField("mass", wheelColliderTemp.mass);

        wheelColliderTemp.radius /= 2;
        wheelColliderTemp.radius = EditorGUILayout.Slider("radius", wheelColliderTemp.radius, 0, 25);
        wheelColliderTemp.radius *= 2;

        wheelColliderTemp.wheelDampingRate = EditorGUILayout.Slider("wheel Damping Rate", wheelColliderTemp.wheelDampingRate, 0, 1);

        wheelColliderTemp.suspensionDistance = EditorGUILayout.FloatField("suspension Distance", wheelColliderTemp.suspensionDistance);

        label("spring");

        suspensionSpring.spring = EditorGUILayout.FloatField("spring", suspensionSpring.spring);

        suspensionSpring.damper = EditorGUILayout.FloatField("damper", suspensionSpring.damper);

        suspensionSpring.targetPosition = EditorGUILayout.Slider("Target Position", suspensionSpring.targetPosition, 0, 1);

        wheelColliderTemp.suspensionSpring = suspensionSpring;

        wheelTemp.FindProperty("wheelCollider").objectReferenceValue = wheelColliderTemp;

        breakForce.floatValue = EditorGUILayout.Slider("Break Force", breakForce.floatValue, 0, 100);

        header("Wheel Tags");

        //creates a toggle feild for steerable or motor
        steerable.boolValue = EditorGUILayout.Toggle("Steerable ", steerable.boolValue);

        motor.boolValue = EditorGUILayout.Toggle("Motor ", motor.boolValue);

        //for loops get every caractersitics of the wheel
        List<string> nameTags = wheelObj.name.Split(':')[1].Split(',').ToList();
        int nameTagIndex = 0;
        for (int i1 = 0; i1 < wheelObj.name.Split(':')[1].Split(',').Length; i1++)
        {
            if (wheelObj.name.Split(':')[1].Split(',')[nameTagIndex] != "n" && wheelObj.name.Split(':')[1].Split(',')[nameTagIndex] != "p" && wheelObj.name.Split(':')[1].Split(',')[nameTagIndex] != "s")
            {
                GUILayout.BeginHorizontal();
                nameTags[nameTagIndex] = EditorGUILayout.TextField(nameTags[nameTagIndex]);

                if (GUILayout.Button("Remove Tag"))
                {
                    nameTags.Remove(nameTags[nameTagIndex]);
                    nameTagIndex -= 1;
                }

                GUILayout.EndHorizontal();
            }
            nameTagIndex++;
        }

        if (GUILayout.Button("add tag"))
        {
            nameTags.Add("");
        }

        wheelObj.name = "wheel:" + String.Join(",", nameTags);

        //draws steerable wheel settings
        if (steerable.boolValue)
        {
            label("steering");

            label("Steering Range");

            //creates feild for min and max steering
            steeringRange.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.Slider("Min Range", steeringRange.GetArrayElementAtIndex(0).floatValue, -90, steeringRange.GetArrayElementAtIndex(1).floatValue);
            steeringRange.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.Slider("Max Range", steeringRange.GetArrayElementAtIndex(1).floatValue, steeringRange.GetArrayElementAtIndex(0).floatValue, 90);

            label("start Position");

            //if game is in editor mode then it will allow the user modify the staring angle
            if (EditorApplication.isPlaying == false)
            {
                wheelAngle.floatValue = EditorGUILayout.Slider("Start Angle", wheelAngle.floatValue, steeringRange.GetArrayElementAtIndex(0).floatValue, steeringRange.GetArrayElementAtIndex(1).floatValue);
            }

            //crate feild for target wheel angel
            targetAngle.floatValue = EditorGUILayout.Slider("Target Angle", targetAngle.floatValue, steeringRange.GetArrayElementAtIndex(0).floatValue, steeringRange.GetArrayElementAtIndex(1).floatValue);

            rotationSpeed.floatValue = EditorGUILayout.Slider("Target Angle", rotationSpeed.floatValue, 0, 100);

            if (wheelObj.name.Split(':')[1].Split(',').Contains("s") == false)
            {
                wheelObj.name += ",s";
            }
        }
        else
        {
            //removes steerable caracter
            if (wheelObj.name.Split(':')[1].Split(',').Contains("s"))
            {
                List<string> tempTag = wheelObj.name.Split(':')[1].Split(',').ToList();
                tempTag.Remove("s");
                wheelObj.name = "wheel:" + string.Join(",", tempTag);
            }
        }

        //draws motor wheel settings
        if (motor.boolValue)
        {
            label("Motor");
            
            //checks if the wheel has the motor caracteristic
            if (wheelObj.name.Split(':')[1].Split(',').Contains("p") == false)
            {
                wheelObj.name += ",p";
            }
        }
        else
        {
            //removes if the wheel has the motor caracteristic
            if (wheelObj.name.Split(':')[1].Split(',').Contains("p"))
            {
                List<string> tempTag = wheelObj.name.Split(':')[1].Split(',').ToList();
                tempTag.Remove("p");
                wheelObj.name = "wheel:" + string.Join(",", tempTag);
            }
        }

        //if wheel has the steerable or motor caracteristic then the neutral wheel is removed
        if (steerable.boolValue || motor.boolValue)
        {
            if (wheelObj.name.Split(':')[1].Split(',').Contains("n"))
            {
                List<string> newName = wheelObj.name.Split(':')[1].Split(',').ToList();
                newName.Remove("n");

                wheelObj.name = "wheel:" + String.Join(",", newName.ToArray());
            }
        }
        //if wheel does not have the steerable or motor caracteristic then the neutral wheel is added
        else
        {
            List<string> newName = wheelObj.name.Split(':')[1].Split(',').ToList();
            newName.Remove("p");
            newName.Remove("s");

            if (wheelObj.name.Split(':')[1].Split(',').Contains("n") == false)
            {

                newName.Add("n");
                wheelObj.name = "wheel:" + String.Join(",", newName.ToArray());
            }
        }

        GUILayout.EndVertical();
        wheelTemp.ApplyModifiedProperties();
    }

    //turrets
    //gets all the turrets
    private void getTurrets()
    {
        Transform turretTemp;
        SerializedProperty turrets;

        turrets = targetObject.FindProperty("turrets");

        turrets.ClearArray();

        turretSearch(v.transform);
    }

    //searches through all children inorder to find turrets
    private void turretSearch(Transform searchItem)
    {
        SerializedProperty turrets;

        turrets = targetObject.FindProperty("turrets");
        
        for (int i1 = 0; i1 < searchItem.childCount; i1++)
        {
            if (searchItem.GetChild(i1).tag == "turret")
            {
                drawTurretGUI(searchItem.GetChild(i1).gameObject);
                turretSearch(searchItem.GetChild(i1));
                turrets.InsertArrayElementAtIndex(turrets.arraySize);
                turrets.GetArrayElementAtIndex(turrets.arraySize - 1).objectReferenceValue = (GameObject)searchItem.GetChild(i1).gameObject;
            }
        }
    }
    
    //draws turret settings on the GUI
    private void drawTurretGUI(GameObject turretObj)
    {
        SerializedObject wheelTemp = new UnityEditor.SerializedObject(turretObj.GetComponent<turret>());
        SerializedProperty turretRotation, barrelElevation;

        turretRotation = wheelTemp.FindProperty("turretRotation");
        barrelElevation = wheelTemp.FindProperty("barrelElevation");

        GUILayout.BeginVertical("box");

        EditorGUILayout.ObjectField(turretObj, typeof(GameObject));

        label("Turret Rotation");
        drawTurretInfo(turretRotation);
        label("Barrel Elavation");
        drawTurretInfo(barrelElevation);
        
        GUILayout.EndVertical();

    }

    //draws barrel or turret settings
    private void drawTurretInfo(SerializedProperty property)
    {
        SerializedProperty type, full, limit;

        type = property.FindPropertyRelative("rotationType");
        full = property.FindPropertyRelative("full");
        limit = property.FindPropertyRelative("limit");

        switch (type.intValue)
        {
            //if the obj has full 360 rotation
            case 0:

                GUILayout.Label("Rotation type : Full");

                EditorGUILayout.Slider("Target Angle", full.FindPropertyRelative("targetAngle").floatValue, 0f, 360f);
                EditorGUILayout.Slider("Current Angle", full.FindPropertyRelative("curentAngle").floatValue, 0f, 360f);

                break;
            //if the obj has a limited rotation
            case 1:
                GUILayout.Label("Rotation type : Ranged");

                EditorGUILayout.Slider("Target Angle", limit.FindPropertyRelative("targetAngle").floatValue, limit.FindPropertyRelative("angleRange").GetArrayElementAtIndex(0).floatValue, limit.FindPropertyRelative("angleRange").GetArrayElementAtIndex(1).floatValue);
                EditorGUILayout.Slider("Current Angle", limit.FindPropertyRelative("curentAngle").floatValue, limit.FindPropertyRelative("angleRange").GetArrayElementAtIndex(0).floatValue, limit.FindPropertyRelative("angleRange").GetArrayElementAtIndex(1).floatValue);

                break;
        }
    }

    //creates a label
    private void label(string label)
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(label);

        EditorGUILayout.Space();
    }

    //creates a header
    static void header(string headerText)
    {

        GUILayout.Space(5);
        GUILayout.Label(headerText);

        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        horizontalLine.fixedHeight = 1;

        var c = GUI.color;
        GUI.color = Color.grey;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = c;

        GUILayout.Space(2);
    }
}
