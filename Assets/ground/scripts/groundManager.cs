using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(groundManager))]
public class groundInput : Editor
{
    groundManager g;

    SerializedObject targetObject;

    SerializedProperty serializedThreshold, serializedSeed, serializedChunkDim, serializedDistPerNode, serializedDim, serializedChunkPrefab, serializedChunks, serializedAmplitude, serializedTranslation, serializedSaveFile, serializedComputeAlgorthim;

    bool lerpCond = false;

    private int computeAlgorthimIntVal = 0;
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
        serializedSaveFile = targetObject.FindProperty("saveFile");
        serializedComputeAlgorthim = targetObject.FindProperty("computeAlgorthim");

        switch(serializedComputeAlgorthim.stringValue)
        {
            case "getVertices":
                lerpCond = false;
                break;
            case "getVerticesLerp":
                lerpCond = true;
                break;
        }
    }

    public override void OnInspectorGUI()
    {
        targetObject.Update();

        label("Chunk");

        serializedChunkPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Chunk Prefab", serializedChunkPrefab.objectReferenceValue, typeof(GameObject));

        
        label("Noise");
        serializedSeed.intValue = EditorGUILayout.IntField("seed", serializedSeed.intValue);

        label("Generation settings");

        lerpCond = EditorGUILayout.Toggle("Lerp Cond:",lerpCond);
        if(lerpCond)
        {
            serializedComputeAlgorthim.stringValue = "getVerticesLerp";
        }
        else
        {
            serializedComputeAlgorthim.stringValue = "getVertices";
        }

        serializedChunkDim.vector3IntValue = EditorGUILayout.Vector3IntField("Chunk Dimenstions:", serializedChunkDim.vector3IntValue);
        

        EditorGUILayout.LabelField("Distance Per Nodes");

        serializedDistPerNode.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.FloatField("x", serializedDistPerNode.GetArrayElementAtIndex(0).floatValue);

        serializedDistPerNode.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.FloatField("y", serializedDistPerNode.GetArrayElementAtIndex(1).floatValue);

        serializedDistPerNode.GetArrayElementAtIndex(2).floatValue = EditorGUILayout.FloatField("z", serializedDistPerNode.GetArrayElementAtIndex(2).floatValue);



        
        EditorGUILayout.LabelField("Total Nodes per Chunk");

        serializedDim.GetArrayElementAtIndex(0).intValue = EditorGUILayout.IntField("x", serializedDim.GetArrayElementAtIndex(0).intValue);

        serializedDim.GetArrayElementAtIndex(1).intValue = EditorGUILayout.IntField("y", serializedDim.GetArrayElementAtIndex(1).intValue);

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

        if (GUILayout.Button("Reset Chunks"))
        {
            for (int i1 = serializedChunks.arraySize - 1; i1 >= 0 ; i1--)
            {
                DestroyImmediate((GameObject)serializedChunks.GetArrayElementAtIndex(i1).objectReferenceValue);
            }
            serializedChunks.ClearArray();
        }
        if(GUILayout.Button("Randomize Nodes"))
        {
            g.randomizeNodes();
        }

        serializedSaveFile.stringValue = EditorGUILayout.TextField("Save File Name:", serializedSaveFile.stringValue);
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Save Ground"))
        {
            g.saveMesh();
        }
        if(GUILayout.Button("Load Ground"))
        {
            g.loadMesh();
        }
        GUILayout.EndHorizontal();
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
    public string saveFile = "ground.world";

    public string computeAlgorthim = "getVertices";

    private Dictionary<Vector3Int, int> chunkLine = new Dictionary<Vector3Int, int>();


    public noise n;
    public GameObject[] chunks = new GameObject[] { };

   public float[] generateNodes(int[] chunkOffset)
    {
        float[] temp = new float[dim[0] * dim[1] * dim[2]];
        float[] offset = new float[3];

        for(int i1 = 0; i1 < 3; i1++)
        {
            offset[i1] = 0.1f / dim[i1] + chunkOffset[i1] * distPerNode[i1] * dim[i1];
        };

        float sample;

        for (int x = 0; x < dim[0]; x++)
        {
            for (int z = 0; z < dim[2]; z++)
            {
                sample = Convert.ToSingle(n.sample(Convert.ToDouble((x + offset[0]) / dim[0] + translation.x), Convert.ToDouble((z + offset[1]) / dim[2]) + translation.z)) *  amplitude + translation.y;
                for(int y = 0; y < dim[1]; y++)
                {
                    temp[x + (y + z * dim[1]) * dim[0]] = sample  - y;
                }
            }
        }

        return temp;
    }

    public void generateChunks()
    {
        int i1;
        
        chunks = new GameObject[Convert.ToInt32(chunkDim.x * chunkDim.y * chunkDim.z)];

        GameObject temp1;
        chunk temp2;
        Vector3 pos;
        float[] nodesTemp;

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


        for (int x = 0; x < chunkDim.x; x++)
        {
            for (int y = 0; y < chunkDim.y; y++)
            {
                for (int z = 0; z < chunkDim.z; z++)
                {
                    pos = Vector3.zero;
                    pos.x = dim[0] * distPerNode[0] * x;
                    pos.y = dim[1] * distPerNode[1] * y;
                    pos.z = dim[2] * distPerNode[2] * z;

                    temp1 = Instantiate(chunkPrefab, this.transform);

                    temp1.GetComponent<Transform>().position = pos;
                    temp2 = temp1.GetComponent<chunk>();
                    temp2.pos = new Vector3Int(x, y, z);

                    nodesTemp = generateNodes(new int[] { x, z, y });

                    temp2.n = new nodes(nodesTemp, dim);
                    

                    i1 = findChunk(new Vector3(x - 1, y, z));
                    if (i1 != -1)
                    {
                        temp2.n.setNeighbor(chunks[i1].GetComponent<chunk>().n, 12);

                    }

                    i1 = findChunk(new Vector3(x, y, z - 1));
                    if (i1 != -1)
                    {
                        temp2.n.setNeighbor(chunks[i1].GetComponent<chunk>().n, 10);
                    }
                    i1 = findChunk(new Vector3(x - 1, y, z - 1));
                    if (i1 != -1)
                    {
                        temp2.n.setNeighbor(chunks[i1].GetComponent<chunk>().n, 9);
                    }
                    
                    
                    temp2.manager = this;
                    chunks[Convert.ToInt32(x + y * chunkDim.x + z * chunkDim.x * chunkDim.y)] = temp1;
                }
            }
        }
    }

    public int findChunk(Vector3 pos)
    {
        for(int i1 = 0; i1 < chunks.Length; i1++)
        {
            if(chunks[i1] != null)
            {
                if (chunks[i1].GetComponent<chunk>().pos == pos)
                {
                    return i1;
                }
            }
        }
        return -1;
    }
    public void randomizeNodes()
    {
        chunk temp;
        float[] tempNodes;
        int[] tempDim;
        for(int i1 = 0; i1 < chunks.Length; i1++)
        {
            temp = chunks[i1].GetComponent<chunk>();
            tempNodes = generateNodes(new int[] { temp.pos.x, temp.pos.y, temp.pos.z });
            tempDim = temp.n.getDim(nodes.CORE);
            temp.n.setNodes(tempNodes, tempDim, new int[3] { 0, 0, 0 }, tempDim, nodes.CORE);
        }
    }

    //method saves ground
    public bool saveMesh()
    {
        bool errorCond = false;

        //checks if the worlds folder exists
        if(!Directory.Exists("worlds"))
        {
            Directory.CreateDirectory("worlds");
        }

        using (StreamWriter temp = File.CreateText($"worlds\\{saveFile}"))
        {
            //writes groundManager instances
            temp.WriteLine($"{threshold},{seed},{distPerNode[0]},{distPerNode[1]},{distPerNode[2]},{dim[0]},{dim[1]},{dim[2]},{computeAlgorthim}");
        
            //writes chunk instances
            for(int i1 = 0; i1 < chunks.Length; i1++)
            {
                temp.WriteLine(chunks[i1].GetComponent<chunk>().toString());
            }
        }
        return errorCond;
    }

    
    public bool loadMesh()
    {
        string s1;
        string[] s2;
        string[] chunkInstancesRaw;
        string[] chunkNodesRaw;

        float[] nodesTemp;
        
        List<chunk> chunksTemp = new List<chunk> { };

        GameObject gameObjectTemp;
        chunk chunkTemp;

        Vector3Int minSize = Vector3Int.zero;
        Vector3Int maxSize = Vector3Int.zero;

        int i1 = 0;
        int i2 = 0;

        //checks if file exists
        bool errorCond = File.Exists($"worlds\\{saveFile}");

        if (errorCond)
        {
            chunkLine.Clear();
            using (StreamReader temp = File.OpenText($"worlds\\{saveFile}"))
            {
                //setting up ground manager
                s1 = temp.ReadLine();
                s2 = s1.Split(',');


                threshold = float.Parse(s2[0]);

                seed = Int32.Parse(s2[1]);

                distPerNode = new float[3] { float.Parse(s2[2]), float.Parse(s2[3]), float.Parse(s2[4]) };
                dim = new int[3] { Int32.Parse(s2[5]), Int32.Parse(s2[6]), Int32.Parse(s2[7]) };
                this.computeAlgorthim = s2[8];

                //re-initialize chunks
                for (int i = 0; i < chunks.Length; i++)
                {
                    Destroy(chunks[i]);
                }

                chunks = new GameObject[File.ReadAllLines($"worlds\\{saveFile}").Length - 1];

                //setting up chunks
                s1 = temp.ReadLine();
                while (s1 != null)
                {
                    s2 = s1.Split('|');

                    chunkInstancesRaw = s2[0].Split(',');
                    chunkNodesRaw = s2[1].Split(',');

                    gameObjectTemp = Instantiate(chunkPrefab, this.transform);
                    chunkTemp = gameObjectTemp.GetComponent<chunk>();
                    chunkTemp.manager = this;

                    gameObjectTemp.transform.position = new Vector3(float.Parse(chunkInstancesRaw[0]), float.Parse(chunkInstancesRaw[1]), float.Parse(chunkInstancesRaw[2]));

                    chunkTemp.pos = new Vector3Int(Int32.Parse(chunkInstancesRaw[3]), Int32.Parse(chunkInstancesRaw[4]), Int32.Parse(chunkInstancesRaw[5]));

                    minSize.x = Math.Min(minSize.x, chunkTemp.pos.x);
                    minSize.y = Math.Min(minSize.y, chunkTemp.pos.y);
                    minSize.z = Math.Min(minSize.z, chunkTemp.pos.z);

                    maxSize.x = Math.Max(minSize.x, chunkTemp.pos.x);
                    maxSize.y = Math.Max(minSize.y, chunkTemp.pos.y);
                    maxSize.z = Math.Max(minSize.z, chunkTemp.pos.z);

                    nodesTemp = new float[dim[0] * dim[1] * dim[2]];

                    //fills node array with values
                    i1 = 0;
                    for (int z = 0; z < dim[2]; z++)
                    {
                        for (int y = 0; y < dim[1]; y++)
                        {
                            for (int x = 0; x < dim[0]; x++)
                            {
                                nodesTemp[x + (y + z * dim[1]) * dim[0]] = float.Parse(chunkNodesRaw[i1]);
                                i1++;
                            }
                        }
                    }


                    chunkTemp.n = new nodes(nodesTemp, dim);

                    chunks[i2] = gameObjectTemp;
                    i2++;

                    chunkLine.Add(chunkTemp.pos, i2 + 1);

                    s1 = temp.ReadLine();
                }
            }
        }

        
        for(i2 = 0; i2 < chunks.Length; i2++)
        {
            chunkTemp = chunks[i2].GetComponent<chunk>();
            i1 = findChunk(new Vector3(chunkTemp.pos.x - 1, chunkTemp.pos.y, chunkTemp.pos.z));
            if (i1 != -1)
            {
                chunkTemp.n.setNeighbor(chunks[i1].GetComponent<chunk>().n, 12);
            }

            i1 = findChunk(new Vector3(chunkTemp.pos.x, chunkTemp.pos.y, chunkTemp.pos.z - 1));
            if (i1 != -1)
            {
                chunkTemp.n.setNeighbor(chunks[i1].GetComponent<chunk>().n, 10);
            }
            i1 = findChunk(new Vector3(chunkTemp.pos.x - 1, chunkTemp.pos.y, chunkTemp.pos.z - 1));
            if (i1 != -1)
            {
                chunkTemp.n.setNeighbor(chunks[i1].GetComponent<chunk>().n, 9);
            }
        }
        
        //gets chunkDim
        this.chunkDim = maxSize - minSize + Vector3Int.one;

        return false;
    }
}
