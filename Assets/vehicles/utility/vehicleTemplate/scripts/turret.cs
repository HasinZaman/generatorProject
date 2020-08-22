using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.SceneManagement;

//class that deals with rotations in a limited range
[System.Serializable]
public class limitedRotation
{
    public float[] angleRange;

    public float curentAngle;
    public float targetAngle;
    public float rotationSpeed;

    //initalizes varaiables
    public limitedRotation(float[] angleRange, float curentAngle, float targetAngle, float rotationSpeed)
    {
        this.angleRange = angleRange;
        this.curentAngle = curentAngle;
        this.targetAngle = targetAngle;
        this.rotationSpeed = rotationSpeed;
    }

    //sets target angle and checks if it's a valid target angle
    public void targetAngleSet(float newTarget)
    {
        targetAngle = Mathf.Min(Mathf.Max(angleRange[0], newTarget), angleRange[1]);
    }

    //updates angle
    public float updateAngle()
    {
        float deltaRotation = rotationSpeed * Time.deltaTime;

        if (targetAngle != curentAngle)
        {
            if (Math.Abs(targetAngle - curentAngle) > deltaRotation)
            {
                if (angleRange[0] <= targetAngle && targetAngle <= curentAngle)
                {
                    curentAngle -= deltaRotation;
                }
                else if (curentAngle <= targetAngle && targetAngle <= angleRange[1])
                {
                    curentAngle += deltaRotation;
                }
            }
            else
            {
                curentAngle = targetAngle;
            }
        }

        return curentAngle;
    }
}

//class that deals with rotations in a 360 degrees
[System.Serializable]
public class fullRotation
{
    public float curentAngle;
    public float targetAngle;
    public float rotationSpeed;

    //initalizes varaibles
    public fullRotation(float curentAngle, float targetAngle, float rotationSpeed)
    {
        this.curentAngle = curentAngle;
        this.targetAngle = targetAngle;
        this.rotationSpeed = rotationSpeed;
    }

    //sets target angle
    public void targetAngleSet(float newTarget)
    {
        float temp = newTarget;
        if(newTarget < 0)
        {
            temp = Mathf.Abs(360 + temp);
        }
        
        targetAngle = temp % 360;

    }

    //calculates the clock wise distance
    float clockwiseDist(float start, float end)
    {
        if (start < end)
        {
            return end - start;
        }
        return 360 - end + start;
    }

    //calculates the counter clockwise distance
    float counterClockwiseDist(float start, float end)
    {
        if (start < end)
        {
            return 360 - end + start;
        }
        return start - end;
    }

    //updates the angle in the fastest direction
    public float updateAngle()
    {
        float deltaRotation = rotationSpeed * Time.deltaTime;

        if (targetAngle != curentAngle)
        {
            //checks to see if the gap remaining from the current angle and the target angle is smaller than change in rotation
            if (Math.Abs(targetAngle - curentAngle) > deltaRotation)
            {
                //compares and finds which path is faster
                if (clockwiseDist(curentAngle, targetAngle) > counterClockwiseDist(curentAngle, targetAngle))
                {
                    curentAngle -= deltaRotation;
                }
                else
                {
                    curentAngle += deltaRotation;
                }
            }
            else
            {
                curentAngle = targetAngle;
            }
        }

        //ensures the angle is between 0 and 360 degress
        if (curentAngle < 0)
        {
            curentAngle = 360 - curentAngle % 360;
        }
        else if (curentAngle > 360)
        {
            curentAngle = curentAngle % 360;
        }

        return curentAngle;
    }
}

//class deals with rotating turrets/barrels
[System.Serializable]
public class turretRotationSystem
{
    public int rotationType = 0;

    [SerializeField] public fullRotation full = new fullRotation(0, 0, 10);

    [SerializeField] public limitedRotation limit = new limitedRotation(new float[2] { 0, 0 }, 0, 0, 10);
    
    //gets target tag and rotates the target properly
    public void updateAngle(Transform target)
    {
        Vector3 rotation = target.localEulerAngles;
        if(target.tag == "turret")
        {
            rotation.y = updateAngle();
        }
        else
        {
            rotation.x = updateAngle();
        }
        target.transform.localEulerAngles = rotation;
    }

    //returns teh new updated the angle
    private float updateAngle()
    {
        switch (rotationType)
        {
            case 0:
                return full.updateAngle();
            case 1:
                return limit.updateAngle();
        }
        return 0;
    }
}

[CustomEditor(typeof(turret))]
public class turretEditor : Editor
{
    turret t;

    string[] rotationTypes = new string[] {"Full rotation", "limited rotation" };

    SerializedProperty shellEjector, barrel, muzzle, turretRotation, barrelElavation;
    
    List<GameObject> ammoList = new List<GameObject>() { };
    List<projectile> projectileList = new List<projectile>() { };

    GameObject ammoObjTemp;
    GameObject shellEjectorPrefab;
    GameObject newShellEjector;

    bool shellEjectorCond = true;

    bool updateTurret = false;

    SerializedObject targetObject;

    private void OnEnable()
    {
        t = (turret)target;

        ammoList = t.ammoLoop.ToList();
        
        targetObject = new UnityEditor.SerializedObject(t);

        shellEjector = targetObject.FindProperty("shellEjector");

        barrel = targetObject.FindProperty("barrel");

        muzzle = targetObject.FindProperty("muzzle");

        turretRotation = targetObject.FindProperty("turretRotation");

        barrelElavation = targetObject.FindProperty("barrelElevation");

    }
    
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Inspector Restart"))
        {
            OnInspectorGUI();
        }

        targetObject.Update();
        
        bool allowSceneObjects = EditorUtility.IsPersistent(target);

        label("");
        //creates a object field to input meshes/prefabs for parts of the turret
        shellEjector.objectReferenceValue = EditorGUILayout.ObjectField("shell Ejector", shellEjector.objectReferenceValue, typeof(GameObject), true);

        barrel.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Barrel", barrel.objectReferenceValue, typeof(GameObject), true);

        muzzle.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Muzzle", muzzle.objectReferenceValue, typeof(GameObject), true);

        //creates feilds iorder to edit rotation settings
        label("Turret Rotation");

        turretRotation.FindPropertyRelative("rotationType").intValue = EditorGUILayout.Popup(turretRotation.FindPropertyRelative("rotationType").intValue, rotationTypes);
        
        rotationTab
        (
            turretRotation.FindPropertyRelative("rotationType").intValue,
            turretRotation,
            t.gameObject
        );

        label("Barrel Elevation");

        barrelElavation.FindPropertyRelative("rotationType").intValue = EditorGUILayout.Popup(barrelElavation.FindPropertyRelative("rotationType").intValue, rotationTypes);

        rotationTab
        (
            barrelElavation.FindPropertyRelative("rotationType").intValue,
            barrelElavation,
            (GameObject)barrel.objectReferenceValue
        );

        //creates feilds to edit and add rounds that will fired in a certain cyclic order
        label("Default Amuntion Loop");
        
        for(int i1 = 0; i1 < ammoList.Count; i1++)
        {
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();

            GUILayout.Label(i1 + " - ");
            ammoList[i1] = (GameObject)EditorGUILayout.ObjectField(ammoList[i1], typeof(GameObject), true);

            GUILayout.EndHorizontal();

            ammoList[i1].GetComponent<projectile>().velocity.x = EditorGUILayout.FloatField("Forward Velocity:", ammoList[i1].GetComponent<projectile>().velocity.x);

            ammoList[i1].GetComponent<projectile>().gravitationalAcceleration = EditorGUILayout.FloatField("Gravitational Acceleration:", ammoList[i1].GetComponent<projectile>().gravitationalAcceleration);

            ammoList[i1].GetComponent<projectile>().segments = EditorGUILayout.IntSlider("Segments", ammoList[i1].GetComponent<projectile>().segments, 0, 100);

            if (GUILayout.Button("Remove Ammo"))
            {
                ammoList.Remove(ammoList[i1]);
            }

            GUILayout.EndVertical();
        }
        GUILayout.BeginHorizontal();

        

        ammoObjTemp = (GameObject)EditorGUILayout.ObjectField(ammoObjTemp, typeof(GameObject), true);

        if (GUILayout.Button("Add Ammo") && ammoObjTemp !=  null)
        {
            ammoList.Add(ammoObjTemp);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Apply Ammo List"))
        {
            t.ammoLoop = ammoList.ToArray();
        }

        //toggles wether shells are ejected
        if (GUILayout.Button("Toggle Shell Ejector"))
        {
            shellEjectorCond = shellEjectorCond == false;
            
        }

        if (shellEjectorCond)
        {
            if (t.gameObject.transform.Find("shellEjector") == null)
            {
                newShellEjector = Instantiate(shellEjectorPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), t.gameObject.transform);
                newShellEjector.name = "shellEjector";
            }
        }
        else
        {
            if (t.gameObject.transform.Find("shellEjector") != null)
            {
                DestroyImmediate(t.gameObject.transform.Find("shellEjector").gameObject);
            }
        }
        
        //if the game is running then allow the player to fire the gun
        if (Application.isPlaying)
        {
            if (GUILayout.Button("fire"))
            {
                t.fire();
            }
        }
        targetObject.ApplyModifiedProperties();

        try
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(t);
                EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
            }
        }
        catch
        {

        }
    }
    
    //creates a input feilds inorder to edit rotation settings
    private void rotationTab(int type, SerializedProperty rotationTarget, GameObject rotationTargetPrefab)
    {
        Vector3 rotationTemp;

        SerializedProperty full = rotationTarget.FindPropertyRelative("full");
        SerializedProperty limit = rotationTarget.FindPropertyRelative("limit");
        
        SerializedProperty currentAngle, targetAngle, rotationSpeed;
        
        switch (type)
        {
            //creates input feilds for 360 degrees
            case 0:
                currentAngle = full.FindPropertyRelative("curentAngle");
                targetAngle = full.FindPropertyRelative("targetAngle");
                rotationSpeed = full.FindPropertyRelative("rotationSpeed");

                //creates input feilds if the game is not playing
                if (EditorApplication.isPlaying == false)
                {
                    currentAngle.floatValue = EditorGUILayout.Slider("Starting Angle", currentAngle.floatValue, 0, 360);
                
                    rotationTemp = rotationTargetPrefab.transform.localEulerAngles;

                    if (rotationTargetPrefab.tag == "turret")
                    {
                        rotationTemp.y = currentAngle.floatValue;
                    }
                    else
                    {
                        rotationTemp.x = currentAngle.floatValue;
                    }

                    rotationTargetPrefab.transform.localEulerAngles = rotationTemp;
                }
                //creates input feilds if the game is playing
                else
                {
                    EditorGUILayout.Slider("Current Angle", currentAngle.floatValue, 0, 360);
                }

                targetAngle.floatValue = EditorGUILayout.Slider("Target Angle", targetAngle.floatValue, 0, 360);

                rotationSpeed.floatValue = EditorGUILayout.Slider("Rotation Speed", rotationSpeed.floatValue, 0, 1000);
                
                break;
            
            //creates input feilds for limited range
            case 1:

                currentAngle = limit.FindPropertyRelative("curentAngle");
                targetAngle = limit.FindPropertyRelative("targetAngle");
                rotationSpeed = limit.FindPropertyRelative("rotationSpeed");
                SerializedProperty angleRange = limit.FindPropertyRelative("angleRange");
                
                EditorGUILayout.LabelField("Rotation Range");

                angleRange.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.Slider
                (
                    "min Range",
                    angleRange.GetArrayElementAtIndex(0).floatValue,
                    -360,
                    angleRange.GetArrayElementAtIndex(1).floatValue
                );

                angleRange.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.Slider
                (
                    "max Range",
                    angleRange.GetArrayElementAtIndex(1).floatValue,
                    angleRange.GetArrayElementAtIndex(0).floatValue, 
                    360
                );
                
                EditorGUILayout.Space();
                if (EditorApplication.isPlaying == false)
                {
                    currentAngle.floatValue = EditorGUILayout.Slider
                    (
                        "Starting Angle",
                        currentAngle.floatValue,
                        angleRange.GetArrayElementAtIndex(0).floatValue,
                        angleRange.GetArrayElementAtIndex(1).floatValue
                    );
                
                    rotationTemp = rotationTargetPrefab.transform.localEulerAngles;
                
                    if (rotationTargetPrefab.tag == "turret")
                    {
                        rotationTemp.y = currentAngle.floatValue;
                    }
                    else
                    {
                        rotationTemp.x = currentAngle.floatValue;
                    }

                    rotationTargetPrefab.transform.localEulerAngles = rotationTemp;
                }
                else
                {
                    EditorGUILayout.Slider("Starting Angle", currentAngle.floatValue, 0, 360);
                }


                targetAngle.floatValue = EditorGUILayout.Slider("Target Angle", targetAngle.floatValue, angleRange.GetArrayElementAtIndex(0).floatValue, angleRange.GetArrayElementAtIndex(1).floatValue);

                rotationSpeed.floatValue = EditorGUILayout.Slider("Rotation Speed", rotationSpeed.floatValue, 0, 1000);
              break;
        }
    }

    private void label(string label)
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(label);

        EditorGUILayout.Space();
    }
}

public class turret : MonoBehaviour
{
    public GameObject barrel;
    public GameObject muzzle;
    public GameObject shellEjector;
    
    public GameObject[] ammoLoop;
    public int ammoRotation = 0;

    public turretRotationSystem turretRotation = new turretRotationSystem();
    public turretRotationSystem barrelElevation = new turretRotationSystem();
    
    public float loadingTime;
    private float loadingTimeLeft = 0;
    
    // Update is called once per frame
    void Update()
    {
        //updates the turret angle
        turretRotation.updateAngle(this.transform);
        barrelElevation.updateAngle(barrel.transform);
    }

    private void OnDrawGizmos()
    {
        try
        {
            //draws starting round's trejectory
            Gizmos.color = Color.red;
            Gizmos.DrawRay(muzzle.transform.position, muzzle.transform.forward * -5);
        }
        catch{ }
    }

    //creates a round gives it the intial values
    public void fire()
    {
        Vector3 rotation =  barrel.transform.localEulerAngles;
        rotation.z *= -1;
        rotation = transform.TransformDirection(rotation);
        
        GameObject shot = Instantiate(ammoLoop[ammoRotation], muzzle.transform.position, barrel.transform.rotation);

        shot.transform.rotation *= Quaternion.AngleAxis(180, transform.up);

        shellEjector.GetComponent<ParticleSystem>().Play();

        muzzle.GetComponent<muzzleFlash>().flashCond = true;
        
        ammoRotation += 1;
        if(ammoRotation >= ammoLoop.Count())
        {
            ammoRotation = 0;
        }
    }

    //creates a circle/sector to visualize the range in which the turret and barrel can rotate
    private void OnDrawGizmosSelected()
    {
        float radius = 5;

        //rotation
        Gizmos.color = Color.blue;
        switch (turretRotation.rotationType)
        {
            case 0:
                circleGizmos(radius, 1, this.transform);
                break;
            case 1:
                sectorGizmos(radius, 1, turretRotation.limit.angleRange[0], turretRotation.limit.angleRange[1], this.transform);
                break;
        }

        //elavation
        try
        {
            Gizmos.color = Color.green;

            switch (barrelElevation.rotationType)
            {
                case 0:
                    circleGizmos(radius, 0, barrel.transform);
                    break;
                case 1:
                    sectorGizmos(radius, 0, barrelElevation.limit.angleRange[0], barrelElevation.limit.angleRange[1], barrel.transform);
                    break;
            }
        }
        catch { }
    }
    
    //draws a sector
    private void sectorGizmos(float radius, int axis, float startAngle, float endAngle, Transform target)
    {
        float[][] pos = new float[Convert.ToInt32(endAngle) - Convert.ToInt32(startAngle)][];

        Vector3 tempAxis1 = Vector3.zero;
        Vector3 tempAxis2 = Vector3.zero;

        double tempAngle;

        for (int r = 0; r < Convert.ToInt32(endAngle) - Convert.ToInt32(startAngle); r++)
        {

            tempAngle = r + startAngle;
            if(tempAngle < 0)
            {
                tempAngle += 360;
            }

            pos[r] = new float[2]
            {
                Convert.ToSingle(Math.Cos(tempAngle * Math.PI / 180)*radius),
                Convert.ToSingle(Math.Sin(tempAngle * Math.PI / 180)*radius),
            };
        }

        switch (axis)
        {
            case 0:
                tempAxis1.z = 1;
                tempAxis2.y = 1;
                break;
            case 1:
                tempAxis1.x = 1;
                tempAxis2.z = 1;
                break;
            case 2:
                tempAxis1.x = 1;
                tempAxis2.y = 1;
                break;
        }

        Gizmos.DrawLine
        (
            target.position,
            this.transform.TransformPoint(tempAxis1 * pos[0][0] + tempAxis2 * pos[0][1])
        );
        Gizmos.DrawLine
        (
            target.position,
            this.transform.TransformPoint(tempAxis1 * pos[pos.Count() - 1][0] + tempAxis2 * pos[pos.Count() - 1][1])
        );

        for (int i1 = 1; i1 < pos.Count(); i1++)
        {
            Gizmos.DrawLine
            (
                this.transform.TransformPoint(tempAxis1 * pos[i1 - 1][0] + tempAxis2 * pos[i1 - 1][1]),
                this.transform.TransformPoint(tempAxis1 * pos[i1][0] + tempAxis2 * pos[i1][1])
            );
        }
    }

    //draws a circle with a certain radius
    private void circleGizmos(float radius, int axis, Transform target)
    {
        float[][] pos =  new float[360][];
        
        Vector3 tempAxis1 = Vector3.zero;
        Vector3 tempAxis2 = Vector3.zero;

        for (int r = 0; r < 360; r++)
        {
            pos[r] = new float[2]
            {
                Convert.ToSingle(Math.Cos(r * Math.PI / 180)*radius),
                Convert.ToSingle(Math.Sin(r * Math.PI / 180)*radius),
            };
        }

        switch (axis)
        {
            case 0:
                tempAxis1.z = 1;
                tempAxis2.y = 1;
                Gizmos.DrawRay(target.position, target.up * radius);
                break;
            case 1:
                tempAxis1.x = 1;
                tempAxis2.z = 1;
                Gizmos.DrawRay(target.position, target.forward * radius);
                break;
            case 2:
                tempAxis1.x = 1;
                tempAxis2.y = 1;
                Gizmos.DrawRay(target.position,target.right * radius);
                break;
        }

        

        for (int i1 = 1; i1 < pos.Count(); i1++)
        {
            Gizmos.DrawLine
            (
                target.TransformPoint(tempAxis1 * pos[i1 - 1][0] + tempAxis2 * pos[i1 - 1][1]),
                target.TransformPoint(tempAxis1 * pos[i1][0] + tempAxis2 * pos[i1][1])
            );
        }
    }
}
