using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(groundManager))]
public class groundInput : Editor
{
    groundManager g;

    SerializedObject targetObject;

    SerializedProperty serializedThreshold, serializedSeed, serializedChunkDim, serializedDistPerNode, serializedDim, serializedChunkPrefab, serializedChunks, serializedAmplitude, serializedTranslation;
    public void OnEnable()
    {
        g = (groundManager) target;

        targetObject = new UnityEditor.SerializedObject(g);

        serializedThreshold = targetObject.FindProperty("threshold");

        serializedSeed = targetObject.FindProperty("seed");
        serializedChunkDim = targetObject.FindProperty("chunkDim");
        serializedDistPerNode = targetObject.FindProperty("distPerNode");
        serializedDim = targetObject.FindProperty("dim");
        serializedChunkPrefab = targetObject.FindProperty("chunkPrefab");
        serializedChunks = targetObject.FindProperty("chunks");
        serializedAmplitude = targetObject.FindProperty("amplitude");
        serializedTranslation = targetObject.FindProperty("translation");
    }

    public override void OnInspectorGUI()
    {
        targetObject.Update();

        label("Chunk");

        serializedChunkPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Chunk Prefab", serializedChunkPrefab.objectReferenceValue, typeof(GameObject));

        
        label("Noise");
        serializedSeed.intValue = EditorGUILayout.IntField("seed", serializedSeed.intValue);

        label("Generation settings");

        serializedChunkDim.vector3IntValue = EditorGUILayout.Vector3IntField("Chunk Dimenstions:", serializedChunkDim.vector3IntValue);
        


        EditorGUILayout.LabelField("Distance Per Nodes");

        serializedDistPerNode.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.FloatField("x", serializedDistPerNode.GetArrayElementAtIndex(0).floatValue);

        serializedDistPerNode.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.FloatField("y", serializedDistPerNode.GetArrayElementAtIndex(1).floatValue);

        serializedDistPerNode.GetArrayElementAtIndex(2).floatValue = EditorGUILayout.FloatField("z", serializedDistPerNode.GetArrayElementAtIndex(2).floatValue);



        
        EditorGUILayout.LabelField("Total Nodes per Chunk");

        //EditorGUILayout.LabelField("x");
        serializedDim.GetArrayElementAtIndex(0).intValue = EditorGUILayout.IntField("x", serializedDim.GetArrayElementAtIndex(0).intValue);

        //EditorGUILayout.LabelField("y");
        serializedDim.GetArrayElementAtIndex(1).intValue = EditorGUILayout.IntField("y", serializedDim.GetArrayElementAtIndex(1).intValue);

        //EditorGUILayout.LabelField("z");
        serializedDim.GetArrayElementAtIndex(2).intValue = EditorGUILayout.IntField("z", serializedDim.GetArrayElementAtIndex(2).intValue);

        serializedAmplitude.floatValue = EditorGUILayout.FloatField("Amplitude", serializedAmplitude.floatValue);

        serializedThreshold.floatValue = EditorGUILayout.Slider("Threshold", serializedThreshold.floatValue, 0, 10);
        serializedTranslation.vector3Value = EditorGUILayout.Vector3Field("Translation", serializedTranslation.vector3Value);



        EditorGUILayout.LabelField("Chunks");
        GUILayout.BeginVertical("box");
        for (int i1 = 0; i1 < serializedChunks.arraySize; i1++)
        {
            EditorGUILayout.ObjectField(serializedChunks.GetArrayElementAtIndex(i1).objectReferenceValue, typeof(GameObject));
        }
        GUILayout.EndVertical();

        if (GUILayout.Button("GenerateChunks"))
        {
            g.generateChunks();
        }

        if (GUILayout.Button("Reset nodes"))
        {
            for (int i1 = serializedChunks.arraySize - 1; i1 >= 0 ; i1--)
            {
                DestroyImmediate((GameObject)serializedChunks.GetArrayElementAtIndex(i1).objectReferenceValue);
                serializedChunks.DeleteArrayElementAtIndex(i1);
            }
        }
        if(GUILayout.Button("Randomize Nodes"))
        {
            g.randomizeNodes();
        }
        targetObject.ApplyModifiedProperties();
    }

    private void randomizeNodes()
    {
        chunk c;
        SerializedObject chunkObj;
    }

    private void label(string label)
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField(label);

        EditorGUILayout.Space();
    }
}

public class groundManager : MonoBehaviour
{
    public float threshold = 0;
    public int seed = 0;
    public Vector3Int chunkDim = new Vector3Int(3,3,1);
    public float[] distPerNode = new float[3] { 1, 1, 1 };
    public int[] dim = new int[3] { 10, 10, 10 };
    public float amplitude = 1;
    public Vector3 translation = Vector3.zero;
    public GameObject chunkPrefab;

    public noise n;
    public GameObject[] chunks = new GameObject[] { };

   public float[][][] generateNodes(int[] chunkOffset)
    {
        float[][][] temp = new float[dim[0]][][];
        float[] offset = new float[3];

        for(int i1 = 0; i1 < 3; i1++)
        {
            offset[i1] = 0.1f / dim[i1] + chunkOffset[i1] * distPerNode[i1] * dim[i1];
        };

        float sample;

        for (int x = 0; x < temp.Length; x++)
        {
            temp[x] = new float[dim[1]][];

            for (int y = 0; y < temp[x].Length; y++)
            {
                temp[x][y] = new float[dim[2]];

                sample = Convert.ToSingle(n.sample(Convert.ToDouble((x + offset[0]) / dim[0] + translation.x), Convert.ToDouble((y + offset[1]) / dim[1]) + translation.y)) *  amplitude + translation.z;
                for(int z = 0; z < temp[x][y].Length; z++)
                {
                    temp[x][y][z] = sample  - z;
                }
            }
        }

        return temp;
    }

    public void generateChunks()
    {
        n = new noise
            (
                seed,
                new double[][] {
                    new double[2]{0,1},
                    new double[2]{1,1},
                    new double[2]{1,0},
                    new double[2]{1,-1},
                    new double[2]{0,-1},
                    new double[2]{-1,-1},
                    new double[2]{-1,0},
                    new double[2]{-1,1}
                },
                10,
                10
            );

        chunks = new GameObject[Convert.ToInt32(chunkDim.x * chunkDim.y * chunkDim.z)];

        GameObject temp1;
        chunk temp2;
        Vector3 pos;

        for (int x = 0; x < chunkDim.x; x++)
        {
            for(int y = 0; y < chunkDim.y; y++)
            {
                for(int z = 0; z < chunkDim.z; z++)
                {
                    pos = Vector3.zero;
                    pos.x = dim[0] * distPerNode[0] * x;
                    pos.y = dim[1] * distPerNode[1] * y;
                    pos.z = dim[2] * distPerNode[2] * z;

                    temp1 = Instantiate(chunkPrefab, this.transform);

                    temp1.GetComponent<Transform>().localPosition = pos;
                    temp2 = temp1.GetComponent<chunk>();
                    temp2.pos = new Vector3Int(x, y, z);

                    temp2.nodes = generateNodes(new int[] { x, y, z });

                    temp2.manager = this;
                    
                    chunks[Convert.ToInt32(x + y * chunkDim.x + z * chunkDim.x * chunkDim.y)] = temp1;
                }
            }
        }
    }

    public void randomizeNodes()
    {
        chunk temp;
        for(int i1 = 0; i1 < chunks.Length; i1++)
        {
            temp = chunks[i1].GetComponent<chunk>();
            temp.nodes = generateNodes(new int[] { temp.pos.x, temp.pos.y, temp.pos.z });
        }
    }
}
