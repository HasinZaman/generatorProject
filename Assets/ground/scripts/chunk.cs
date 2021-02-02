using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(chunk))]
public class chunkInput : Editor
{
    chunk c;

    SerializedObject targetObject;
    
    SerializedProperty serializedNodes, serializedPos, serializedMesh, serializedCollider, serializedVertices, serializedComputerShader, serializedManager;

    int layer = 0;
    bool layerMode = false;
    public void OnEnable()
    {
        c = (chunk)target;

        targetObject = new UnityEditor.SerializedObject(c);

        serializedNodes = targetObject.FindProperty("nodes");
        serializedPos = targetObject.FindProperty("pos");
        serializedMesh = targetObject.FindProperty("mesh");
        serializedCollider = targetObject.FindProperty("collider");
        serializedVertices = targetObject.FindProperty("vertices");

        serializedComputerShader = targetObject.FindProperty("shader");

        serializedManager = targetObject.FindProperty("manager");

    }

    public override void OnInspectorGUI()
    {
        targetObject.Update();
        EditorGUILayout.Vector3IntField("Pos", serializedPos.vector3IntValue);
        serializedComputerShader.objectReferenceValue = EditorGUILayout.ObjectField("Shader", serializedComputerShader.objectReferenceValue, typeof(ComputeShader));
        EditorGUILayout.ObjectField("Shader", serializedManager.objectReferenceValue, typeof(groundManager));

        EditorGUILayout.LabelField("Node Mode");
        layerMode = EditorGUILayout.Toggle(layerMode);


        if (c.nodes != null)
        {
            EditorGUILayout.LabelField("Nodes");
            drawNode();
        }
        targetObject.ApplyModifiedProperties();


        if(GUILayout.Button("UpdateMesh"))
        {
            c.updateMesh();
        }
    }

    void drawNode()
    {
        GUILayout.BeginHorizontal();
        for (int x = 0; x < c.manager.dim[0]; x++)
        {
            GUILayout.BeginVertical();
            for(int y = 0; y < c.manager.dim[1]; y++)
            {
                if(layerMode)
                {
                    c.nodes[x + (y + layer * c.manager.dim[1]) * c.manager.dim[0]] = EditorGUILayout.FloatField(c.nodes[x + (y + layer * c.manager.dim[1]) * c.manager.dim[0]]);
                }
                else
                {
                    EditorGUILayout.Toggle(c.nodes[x + (y + layer * c.manager.dim[1]) * c.manager.dim[0]] > c.manager.threshold);
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        layer = EditorGUILayout.IntSlider("Layer", layer, 0, c.manager.dim[2] - 1);
    }
}
public class chunk : MonoBehaviour
{
    //chunk manager
    public groundManager manager;

    //cube information
    public float[] nodes;

    public Vector3Int pos;
    //shaders
    public ComputeShader shader;

    //mesh
    public MeshFilter mf;
    public Mesh mesh;
    public MeshCollider collider;

    public Vector3[] vertices;
    public int[] triangles;
    public List<Vector2> uv = new List<Vector2>();

    struct Triangle
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
    }

    private void Start()
    {
        //setting up material
        mf = GetComponent<MeshFilter>();
        mesh = new Mesh();

        mf.mesh = mesh;
        collider.sharedMesh = mesh;

        //creating mesh
        updateMesh();
    }

    public void updateMesh()
    {
        int kernelHandle = shader.FindKernel("getVertices");

        //declaring Compute Buffers
        ComputeBuffer trianglesBuffer = new ComputeBuffer(manager.dim[0] * manager.dim[1] * manager.dim[2] * 15, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        ComputeBuffer triangleCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        ComputeBuffer nodeBuffer = new ComputeBuffer(manager.dim[0] * manager.dim[1] * manager.dim[2], sizeof(float), ComputeBufferType.Default);

        //declaring output variables
        Triangle[] trianglesTemp;
        int[] triangleCountArray = { 0 };

        //setting up shader variables
        shader.SetBuffer(kernelHandle, "triangles", trianglesBuffer);
        trianglesBuffer.SetCounterValue(0);

        shader.SetFloat("threshold", manager.threshold);
        shader.SetFloats("distPerNode", manager.distPerNode);

        shader.SetInts("dim", manager.dim);

        nodeBuffer = new ComputeBuffer(manager.dim[0] * manager.dim[1] * manager.dim[2], sizeof(float), ComputeBufferType.Default);
        shader.SetBuffer(kernelHandle, "nodes", nodeBuffer);
        nodeBuffer.SetCounterValue(0);
        nodeBuffer.SetData(nodes);

        //starting computeBuffer
        shader.Dispatch(kernelHandle, (int) Math.Ceiling((float) manager.dim[0] / 10), (int) Math.Ceiling((float)manager.dim[1] / 10), (int) Math.Ceiling((float)manager.dim[2] / 10));

        //getting triangle information from Compute shader
        ComputeBuffer.CopyCount(trianglesBuffer, triangleCountBuffer, 0);

        triangleCountBuffer.GetData(triangleCountArray);


        trianglesTemp = new Triangle[triangleCountArray[0]];
        trianglesBuffer.GetData(trianglesTemp, 0, 0, triangleCountArray[0]);

        triangles = new int[triangleCountArray[0] * 3];
        vertices = new Vector3[triangleCountArray[0] * 3];

        //convert rawData into triangle and vertices array for material
        for (int i1 = 0; i1 < trianglesTemp.Length; i1++)
        {
            for(int i2 = 0; i2 < 3; i2++)
            {
                triangles[i1 * 3 + i2] = i1 * 3 + i2;
                switch(i2)
                {
                    case 0:
                        vertices[i1 * 3] = trianglesTemp[i1].p1;
                        break;
                    case 1:
                        vertices[i1 * 3 + 1] = trianglesTemp[i1].p2;
                        break;
                    case 2:
                        vertices[i1 * 3 + 2] = trianglesTemp[i1].p3;
                        break;
                }
            }

        }
        //update mesh
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
    }

    //toString turns instances into a string
    public string toString()
    {
        //format
        //Chunk value|Node value
        //           ^ Line seperatoes chunk values and node values
        string temp = $"{this.transform.position.x},{this.transform.position.y},{this.transform.position.z},{this.pos.x},{this.pos.y},{this.pos.z}|";

        //for loop gets the nodes value
        for (int z = 0; z < manager.dim[2]; z++)
        {
            for (int y = 0; y < manager.dim[1]; y++)
            {
                for (int x = 0; x < manager.dim[0]; x++)
                {
                    temp += $"{nodes[x + (y + z * manager.dim[1]) * manager.dim[0]]},";
                }
            }    
        }
        //gets rid of last comma
        temp = temp.Remove(temp.Length - 1);

        return temp;
    }
}
