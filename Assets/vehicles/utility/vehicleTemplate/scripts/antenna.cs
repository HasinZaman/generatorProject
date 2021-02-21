using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(antenna))]
public class antennaEditor : Editor
{
    antenna a;

    SerializedObject targetObject;

    SerializedProperty serializedJoints, serializedMaxRotation, serializedLastPosition, serializedLastVelocity, serializedBendFactor;
    SerializedProperty serializedRoots, serializedDamper, serializedSpringConstant, serializedRestCond;
    SerializedProperty serializedVelocityTriggerX, serializedVelocityTriggerY;

    Transform[] joints;
    double[] maxRotation;
    Vector3[] lastPosition, lastVelocity;

    float[] temp = new float[2] { 0, 0 };
    
    private void OnEnable()
    {
        //initalize modifiable variables
        a = (antenna) target;
        
        targetObject = new UnityEditor.SerializedObject(a);
        
        serializedJoints = targetObject.FindProperty("joints");
        serializedMaxRotation = targetObject.FindProperty("maxRotation");
        serializedLastPosition = targetObject.FindProperty("lastPosition");
        serializedLastVelocity = targetObject.FindProperty("lastVelocity");
        serializedBendFactor = targetObject.FindProperty("bendFactor");

        serializedRoots = targetObject.FindProperty("roots");
        serializedDamper = targetObject.FindProperty("damper");
        serializedSpringConstant = targetObject.FindProperty("springConstant");
        serializedRestCond = targetObject.FindProperty("restCond");

        serializedVelocityTriggerX = targetObject.FindProperty("velocityTriggerX");
        serializedVelocityTriggerY = targetObject.FindProperty("velocityTriggerY");
    }

    public override void OnInspectorGUI()
    {
        targetObject.Update();

        //create input feilds for anntenna variables
        serializedDamper.floatValue = EditorGUILayout.FloatField("damper", serializedDamper.floatValue);
        serializedSpringConstant.floatValue = EditorGUILayout.FloatField("Spring Constant", serializedSpringConstant.floatValue);

        //creates input feilds for min and max speeds which affect the bending of the antenna
        GUILayout.BeginHorizontal();
        GUILayout.Label("X:");
        for (int i1 = 0; i1 < 2; i1++)
        {
            serializedVelocityTriggerX.GetArrayElementAtIndex(i1).floatValue = EditorGUILayout.FloatField(serializedVelocityTriggerX.GetArrayElementAtIndex(i1).floatValue);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Y:");
        for (int i1 = 0; i1 < 2; i1++)
        {
            serializedVelocityTriggerY.GetArrayElementAtIndex(i1).floatValue = EditorGUILayout.FloatField(serializedVelocityTriggerY.GetArrayElementAtIndex(i1).floatValue);
        }
        GUILayout.EndHorizontal();

        //creates checkbox that shows if the a certian axis of the antenna is entering a rest position
        serializedRestCond.GetArrayElementAtIndex(0).boolValue = EditorGUILayout.Toggle("rest Cond X", serializedRestCond.GetArrayElementAtIndex(0).boolValue);
        serializedRestCond.GetArrayElementAtIndex(1).boolValue = EditorGUILayout.Toggle("rest Cond Y", serializedRestCond.GetArrayElementAtIndex(1).boolValue);

        //updates the roots of the resting function
        if (GUILayout.Button("Update Roots"))
        {
            updateArray(ref serializedRoots, rootFinder(0.01f));

            string temp = "";
            for (int i1 = 0; i1 < serializedRoots.arraySize; i1++)
            {
                temp += " " + serializedRoots.GetArrayElementAtIndex(i1).floatValue.ToString();
            }
            Debug.Log("New Roots:" + temp);
        }

        //creates a slider that visually expresses the antenna bend factor
        temp[0] = EditorGUILayout.Slider("X rotation", serializedBendFactor.vector2Value.x, -1, 1);
        temp[1] = EditorGUILayout.Slider("Y rotation", serializedBendFactor.vector2Value.y, -1, 1);
        
        serializedBendFactor.vector2Value = new Vector2(temp[0], temp[1]);

        //gets all the bones in the mesh
        if (GUILayout.Button("Update Joints"))
        {
            jointSetUp();
            maxRotation = new double[joints.Length];
            lastPosition = new Vector3[joints.Length];
            lastVelocity = new Vector3[joints.Length];
        }
        //gets all the bones and draws them onto the GUI
        else if (serializedJoints.arraySize > 0)
        {
            getArray(serializedJoints, ref joints);
            getArray(serializedMaxRotation, ref maxRotation);
            getArray(serializedLastPosition, ref lastPosition);
            getArray(serializedLastVelocity, ref lastVelocity);

            EditorGUILayout.LabelField("Antenna parts");

            for (int i1 = 0; i1 < joints.Length; i1++)
            {
                drawRow(i1);
            }
        }

        //update all the arrays in the antenna
        updateArray(ref serializedJoints, joints);
        updateArray(ref serializedMaxRotation, maxRotation);
        updateArray(ref serializedLastPosition, lastPosition);
        updateArray(ref serializedLastVelocity, lastVelocity);

        //save all the modifications
        targetObject.ApplyModifiedProperties();
        try
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(a);
                EditorSceneManager.MarkSceneDirty(a.gameObject.scene);
            }
        }
        catch
        {

        }
        if (Application.isEditor)
        {
            a.updateJoint(0);
        }
    }

    //sets up all the bones
    private void jointSetUp()
    {
        bool searchCond = true;

        List<Transform> jointTemp = new List<Transform> { };

        Transform previousJoint = null;

        Transform armature = a.transform.GetChild(1).GetChild(0);
        
        //goes through all the children the antenna and indexing all the bones
        while (searchCond)
        {
            if(armature.childCount > 0)
            {
                jointTemp.Add(armature.GetChild(0));
                armature = armature.GetChild(0);
                
                previousJoint = armature;
            }
            else
            {
                searchCond = false;
                break;
            }
        }

        jointTemp.Remove(jointTemp[jointTemp.Count - 1]);
        
        joints = jointTemp.ToArray();
    }

    //finds the roots of the rest fuction
    private float[] rootFinder(float step)
    {
        List<float> roots = new List<float> { 0 };

        float lastValX, lastValY, newVal;

        lastValX = 0;
        lastValY = antennaPostionFinderDerivative(lastValX);
        
        //for loop goes through the function starting at x=0 and when the dampner part of the function y=0
        for(float x = 0; x < Math.Abs(1 / serializedDamper.floatValue); x+= step)
        {
            newVal = antennaPostionFinderDerivative(x);
            
            //if the derivative equals 0; then a root has been found
            if (newVal == 0)
            {
                roots.Add(x);
            }
            //checks if the derivative function has crossed the x axis
            else if ((lastValY < 0) != (newVal < 0))
            {
                //finds the aproximate value of the nearest root
                roots.Add(x + lerp(newVal, lastValY, 0) * step);
            }
            lastValY = newVal;
        }

        return roots.ToArray();
    }

    private float antennaPostionFinderDerivative(float x)
    {
        return -2 * x * serializedSpringConstant.floatValue * (1 - serializedDamper.floatValue * x) * Mathf.Sin(Mathf.Pow(x, 2) * serializedSpringConstant.floatValue) - serializedDamper.floatValue * Mathf.Cos(Mathf.Pow(x, 2) * serializedSpringConstant.floatValue);
    }
    
    float lerp(double x1, double x2, double t)
    {
        return Convert.ToSingle((t - x1) / (x2 - x1));
    }

    //draws all settings for each joint 
    private void drawRow(int row)
    {
        GUILayout.BeginVertical("box");

        EditorGUILayout.ObjectField(joints[row], typeof(Transform), true);
        
        maxRotation[row] = EditorGUILayout.DoubleField("Max Rotation", maxRotation[row]);
        
        EditorGUILayout.Vector3Field("Current Position", joints[row].position);

        EditorGUILayout.Vector3Field("last Pos", lastPosition[row]);

        EditorGUILayout.Vector3Field("Last Velocity", lastVelocity[row]);

        GUILayout.EndVertical();
    }
    

    //converts the serrialized array into an array
    private void getArray(SerializedProperty array, ref double[] target)
    {
        target = new double[array.arraySize];
        for (int i1 = 0; i1 < array.arraySize; i1++)
        {
            target[i1] = array.GetArrayElementAtIndex(i1).doubleValue;
        }
    }

    private void getArray(SerializedProperty array, ref Vector3[] target)
    {
        target = new Vector3[array.arraySize];
        for (int i1 = 0; i1 < array.arraySize; i1++)
        {
            target[i1] = array.GetArrayElementAtIndex(i1).vector3Value;
        }
    }

    private void getArray(SerializedProperty array, ref Transform[] target)
    {
        target = new Transform[array.arraySize];
        
        for (int i1 = 0; i1 < array.arraySize; i1++)
        {
            target[i1] = (Transform) array.GetArrayElementAtIndex(i1).objectReferenceValue;
        }
    }

    //update a serialized array using the equlivent array
    private void updateArray(ref SerializedProperty array, double[] target)
    {
        array.ClearArray();
        for(int i1 = 0; i1 < target.Length; i1++)
        {
            array.InsertArrayElementAtIndex(array.arraySize);
            array.GetArrayElementAtIndex(array.arraySize - 1).doubleValue = target[i1];
        }
    }

    private void updateArray(ref SerializedProperty array, float[] target)
    {
        array.ClearArray();
        for (int i1 = 0; i1 < target.Length; i1++)
        {
            array.InsertArrayElementAtIndex(array.arraySize);
            array.GetArrayElementAtIndex(array.arraySize - 1).floatValue = target[i1];
        }
    }

    private void updateArray(ref SerializedProperty array, Vector3[] target)
    {
        array.ClearArray();
        for (int i1 = 0; i1 < target.Length; i1++)
        {
            array.InsertArrayElementAtIndex(array.arraySize);
            array.GetArrayElementAtIndex(array.arraySize - 1).vector3Value = target[i1];
        }
    }

    private void updateArray(ref SerializedProperty array, Transform[] target)
    {
        array.ClearArray();
        for (int i1 = 0; i1 < target.Length; i1++)
        {
            array.InsertArrayElementAtIndex(array.arraySize);
            array.GetArrayElementAtIndex(array.arraySize - 1).objectReferenceValue = target[i1];
        }
    }
}

public class antenna : MonoBehaviour
{
    //joint variables
    public Transform[] joints;
    public double[] maxRotation;
    public Vector3[] lastPosition, lastVelocity;//check if this can be converted into a vector3

    //rest function variables
    public Vector2 bendFactor = Vector2.zero;
    public float[] roots;
    public float[] t = new float[2] { 0, 0 };
    
    //spring variables
    public float damper = 1;
    public float springConstant = 1;

    public float[] springDirection = new float[2] { 1, 1 };

    public float[] velocityTriggerX = new float[2] { 0, 0 };
    public float[] velocityTriggerY = new float[2] { 0, 0 };
    private float[] target = new float[2] { 0, 0 };

    public bool[] restCond = new bool[] { false, false };

    private void Start()
    {
        //inializes all the positions of the bones
        for(int i1 = 0; i1 < lastPosition.Length; i1++)
        {
            lastPosition[i1] = joints[i1].position;
        }

        //sets the time value in the rest function when the damper part of the function == 0
        t[0] = Math.Abs(1 / damper);
        t[1] = Math.Abs(1 / damper);

        //returns the roots of the rest function
        if (Application.isEditor)
        {
            string temp = "";
            for (int i1 = 0; i1 < roots.Length; i1++)
            {
                temp += ", " + roots[i1];
            }
            Debug.Log(temp);
        }
    }

    int aga = 0;

    // Update is called once per frame
    void Update()
    {
        //get the kinomatic values at the current frame
        Vector2 newVelocity = velocity(Vector3.zero, forwardDirection(lastPosition[0] - transform.position));
        Vector2 newAcceleration = acceleration(Vector3.zero, forwardDirection(lastPosition[0] - transform.position), lastVelocity[0]);
        
        //gets the time/bend factor value
        updateJoint(ref bendFactor.x, ref t[0], ref springDirection[0], ref restCond[0], newVelocity.x, newAcceleration.x, ref velocityTriggerX);
        updateJoint(ref bendFactor.y, ref t[1], ref springDirection[1], ref restCond[1], newVelocity.y, newAcceleration.y, ref velocityTriggerY);

        //using the values updated in previous updateJoint functions to update the joints 
        updateJoint(0);

        //update and store the lastVelocity & position
        lastVelocity[0] = newVelocity;
        lastPosition[0] = transform.position;
    }

    //this version of the updatejoint updates time/bendfactor function
    private void updateJoint(ref float BendFactorRef, ref float tRef, ref float springDirectionRef, ref bool restCondRef, float newVelocity, float newAcceleration, ref float[] velocityTrigger)
    {
        //stops rest motion
        if (tRef > Math.Abs(1 / damper))
        {
            tRef = Math.Abs(1 / damper);

            BendFactorRef = springDirectionRef * antennaPostionFinder(tRef);
        }
        //in movement
        else if (velocityTrigger[0] <= Mathf.Abs(newVelocity) && Mathf.Abs(newVelocity) <= velocityTrigger[1])
        {
            springDirectionRef = stepFunction(newVelocity);
            BendFactorRef = antennaPostionFinder(newVelocity, velocityTrigger);
            restCondRef = false;
        }
        //procedural rest animation
        else if (restCondRef)
        {
            tRef += Time.deltaTime;

            BendFactorRef = springDirectionRef * antennaPostionFinder(tRef);
        }
        //starting the rest
        else if (newVelocity == 0 && newAcceleration == 0)
        {
            restCondRef = true;

            springDirectionRef = stepFunction(BendFactorRef);

            tRef = findStartPoint(BendFactorRef, springDirectionRef);
        }
    }

    //this version of the updatejoint update the bones in the antenna
    public void updateJoint(int jointIndex)
    {
        Vector3 rotationTemp;
        
        if(jointIndex < joints.Length)
        {
            if(maxRotation[jointIndex] != 0)
            {
                //update the rotations using bendfactor
                rotationTemp = joints[jointIndex].transform.localEulerAngles;
                rotationTemp.x = Convert.ToSingle(bendFactor.x * maxRotation[jointIndex]);
                rotationTemp.z = Convert.ToSingle(bendFactor.y * maxRotation[jointIndex]);

                joints[jointIndex].transform.localEulerAngles = rotationTemp;
            }

            updateJoint(jointIndex + 1);
        }

    }

    float stepFunction(float x)
    {
        if(x < 0)
        {
            return -1f;
        }
        return 1f;
    }
    
    //finds the start position before iniating the resting animation
    float findStartPoint(float target, float mod)
    {
        float y1, y2;

        float y3, y4;
        
        //goes though the for loop inorder check if the target is in between the local maximums/minimums;
        //inorder to locate a search zone to find the time value
        for(int i1 = 0; i1 < roots.Length; i1++)
        {
            y1 = mod * antennaPostionFinder(roots[i1]);
            y2 = mod * antennaPostionFinder(roots[Math.Min(i1+2,roots.Length-1)]);

            //if the target is a root then return the time value at the max/min of the function
            if (target == y1)
            {
                return roots[i1];
            }
            //if the target is inbetween to maximums/minimums of a function
            else if(Mathf.Max(y1, y2) > target && target > Mathf.Min(y1, y2))
            {
                //for loop searches through the search zone to find the start point
                for (float t = roots[i1]; t < roots[Math.Min(i1 + 1, roots.Length - 1)] && (mod * antennaPostionFinder(roots[i1]) < 0) == (mod * antennaPostionFinder(t) < 0); t += Time.deltaTime)
                {
                    y3 = mod * antennaPostionFinder(t);
                    y4 = mod * antennaPostionFinder(t + Time.deltaTime);
                    
                    if(Mathf.Max(y3,y4) >= target && target > Mathf.Min(y3, y4))
                    {
                        return t;
                    }
                }
            }
        }
        return 0f;
    }
    
    //finds the antenna postion when the parent is in motion
    float antennaPostionFinder(float speed, float[] triggerZone)
    {
        
        float bendTarget = stepFunction(speed) * lerp(triggerZone[0], triggerZone[1], Mathf.Min(triggerZone[1], Mathf.Abs(speed)));
        return bendTarget;
    }

    //finds the antenna postion when the antenna is entering a rest state
    float antennaPostionFinder(float x)
    {
        float d = 1 - damper * x;

        if (d < 0)
        {
            return 0;
        }

        return d * Mathf.Cos(springConstant * x * x);
    }

    float lerp(double x1, double x2, double speed)
    {
        return Convert.ToSingle((speed - x1) / (x2 - x1));
    }

    //gets the acceleration in the x and z axis
    Vector2 acceleration(Vector3 start, Vector3 end, Vector3 lastVelocity)
    {
        Vector2 temp = Vector2.zero;
        float t = Time.deltaTime;
        
        temp.x = acceleration(t, (end.x - start.x) / 1.5f, lastVelocity.x);
        
        temp.y = acceleration(t, (end.z - start.z) / 1.5f, lastVelocity.z);

        return temp;

    }

    float acceleration(float t, float d, float v)
    {

        float a = Convert.ToSingle(2 * (d - v * t) / Math.Pow(t, 2));

        if (a == float.NaN)
        {
            a = 0;
        }

        return a;
    }

    //gets the veloicty in the x and z axis
    Vector2 velocity(Vector3 start, Vector3 end)
    {
        Vector2 temp = Vector2.zero;
        float t = Time.deltaTime;
        
        temp.x = velocity(t, (end.x - start.x) / 1.5f);
        
        temp.y = velocity(t, (end.z - start.z) / 1.5f);

        return temp;
    }

    float velocity(float t, float d)
    {
        if (t == 0)
        {
            return 0;
        }

        float v = d / t;
        return v;
    }

    //roates the axies inorder to get the x, y & z position values relative to the forward direction
    Vector3 forwardDirection(Vector3 vector)
    {
        float yaw, pitch, roll;

        yaw = this.transform.localEulerAngles.y * Mathf.PI / 180;
        pitch = this.transform.localEulerAngles.x * Mathf.PI / 180;
        roll = this.transform.localEulerAngles.z * Mathf.PI / 180;
        
        float[] temp = new float[3] { vector.z, vector.x, vector.y};

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

        //gets position
        temp = dotProduct
        (
            temp3,
            temp
        );

        return new Vector3(temp[1], temp[2], temp[0]);
    }
    
    float[][] dotProduct(float[][] matrix1, float[][] matrix2)
    {
        float[][] temp = new float[Mathf.Min(matrix2.Length, matrix1.Length)][];
        for(int y = 0; y < temp.Length; y++)
        {
            temp[y] = new float[Mathf.Min(matrix2[y].Length, matrix1[y].Length)];
            for (int x = 0; x < temp[y].Length; x++)
            {
                temp[y][x] = 0;

                for(int i1 = 0; i1 < matrix1[y].Length; i1++)
                {
                    temp[y][x] += matrix1[y][i1] * matrix2[i1][x];
                }

            }
        }
        return temp;
    }

    float[] dotProduct(float[][] trigMatrix, float[] position)
    {
        float[] temp = new float[3];

        for(int i1 = 0; i1 < 3; i1++)
        {
            temp[i1] = trigMatrix[0][i1] * position[0] + trigMatrix[1][i1] * position[1] + trigMatrix[2][i1] * position[2];
        }

        return temp;
    }
}
