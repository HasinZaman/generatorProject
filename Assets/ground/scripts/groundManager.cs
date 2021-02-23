using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(groundManager))]
public class groundInput : Editor
{
    groundManager g;

    SerializedObject targetObject;

    //chunk mesh generation variables
    SerializedProperty serializedChunkPrefab;
    SerializedProperty serializedChunkDim, serializedDistPerNode, serializedDim, serializedAmplitude, serializedTranslation, serializedSeed;

    //managing chunks
    SerializedProperty serializedChunks;
    SerializedProperty serializedThreshold, serializedSaveFile, serializedComputeAlgorthim;

    SerializedProperty serializedBorderNoise;

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
        serializedBorderNoise = targetObject.FindProperty("borderNoise");


        switch (serializedComputeAlgorthim.stringValue)
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
        g.borderNoise = new int[3] { 30, 30, 30 };
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
        EditorGUILayout.BeginHorizontal();
        for (int i1 = 0; i1 < 3; i1++)
        {
            serializedDistPerNode.GetArrayElementAtIndex(i1).floatValue = EditorGUILayout.FloatField(serializedDistPerNode.GetArrayElementAtIndex(i1).floatValue);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Total Nodes per Chunk");
        EditorGUILayout.BeginHorizontal();
        for(int i1 = 0; i1 < 3; i1++)
        {
            serializedDim.GetArrayElementAtIndex(i1).intValue = EditorGUILayout.IntField(serializedDim.GetArrayElementAtIndex(i1).intValue);
        }
        EditorGUILayout.EndHorizontal();

        serializedAmplitude.floatValue = EditorGUILayout.FloatField("Amplitude", serializedAmplitude.floatValue);

        serializedThreshold.floatValue = EditorGUILayout.Slider("Threshold", serializedThreshold.floatValue, 0, 10);
        serializedTranslation.vector3Value = EditorGUILayout.Vector3Field("Translation", serializedTranslation.vector3Value);
        
        EditorGUILayout.LabelField("Noise Texture");
        EditorGUILayout.BeginHorizontal();
        for (int i1 = 0; i1 < 3; i1++)
        {
            serializedBorderNoise.GetArrayElementAtIndex(i1).intValue = EditorGUILayout.IntField(serializedBorderNoise.GetArrayElementAtIndex(i1).intValue);
        }
        EditorGUILayout.EndHorizontal();
        
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


    private System.Random r;
    private static int textureNoiseLevels = 2;
    public float[][][][] textureNoise;
    public int[] borderNoise = new int[3] { 30, 30, 30 };
    public float blendSharpness = 0.5f;


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
        float[] colourTemp;
        n = new noise
            (
                seed,
                noise.SET1,
                10,
                10
            );

        r = new System.Random(seed);
        
        textureNoise = new float[chunkDim.x * borderNoise[0] + 1][][][];

        for (int x = 0; x < textureNoise.Length; x++)
        {
            textureNoise[x] = new float[chunkDim.y * borderNoise[1] + 1][][];
            for (int y = 0; y < textureNoise[x].Length; y++)
            {
                textureNoise[x][y] = new float[chunkDim.z * borderNoise[2] + 1][];

                for (int z = 0; z < textureNoise[x][y].Length; z++)
                {
                    textureNoise[x][y][z] = noise.SET2[r.Next(noise.SET2.Length)];
                }
            }
        }
        
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
                    
                    temp2.chunkTextureDetails = new Texture3D[textureNoiseLevels];
                    for(int i2 = 0; i2 < textureNoiseLevels; i2++)
                    {
                        temp2.chunkTextureDetails[i2] = new Texture3D(borderNoise[0], borderNoise[1], borderNoise[2], TextureFormat.RGB24, false);
                        temp2.chunkTextureDetails[i2].filterMode = FilterMode.Point;

                        for (int x1 = 0; x1 < borderNoise[0]; x1++)
                        {
                            for (int y1 = 0; y1 < borderNoise[1]; y1++)
                            {
                                for (int z1 = 0; z1 < borderNoise[2]; z1++)
                                {
                                    colourTemp = textureNoise[x * (borderNoise[0] - 1) + x1][y * (borderNoise[1] - 1) + y1][z * (borderNoise[2] - 1) + z1];
                                    temp2.chunkTextureDetails[i2].SetPixel(x1, y1, z1, new Color((colourTemp[0] + 1) / 2, (colourTemp[1] + 1) / 2, (colourTemp[2] + 1) / 2));
                                }
                            }
                        }
                        temp2.chunkTextureDetails[i2].Apply();
                    }

                    temp2.m = temp1.GetComponent<Renderer>().material;
                    for (int i2 = 0; i2 < 4; i2++)
                    {
                        temp2.groundTextures[i2] = (Texture2D) temp2.m.GetTexture($"Texture2D_TopPrimaryColour_{i2}");
                    }
                    temp2.groundTextures[4] = (Texture2D) temp2.m.GetTexture($"Texture2D_TopCenterColour");
                    //Texture2D_SidePrimaryColour_
                    //Texture2D_TopPrimaryColour_0
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
        string[][] chunkTextureDetailsRaw;

        float[] nodesTemp;
        
        List<chunk> chunksTemp = new List<chunk> { };

        GameObject gameObjectTemp;
        chunk chunkTemp;

        Vector3Int minSize = Vector3Int.zero;
        Vector3Int maxSize = Vector3Int.zero;

        int i1 = 0;
        int i2 = 0;

        Texture3D texture3DTemp;
        Color colorTemp;

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

                    chunkTextureDetailsRaw = new string[2][];
                    for (i1 = 0; i1 < 2; i1++)
                    {
                        chunkTextureDetailsRaw[i1] = s2[2 + i1].Split(',');
                    }

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

                    int counter = 0;
                    chunkTemp.chunkTextureDetails = new Texture3D[2];
                    int i3;
                    for (i1 = 0; i1 < 2; i1++)
                    {
                        counter = 0;
                        i3 = 3;
                        texture3DTemp = new Texture3D(int.Parse(chunkTextureDetailsRaw[i1][0]), int.Parse(chunkTextureDetailsRaw[i1][1]), int.Parse(chunkTextureDetailsRaw[i1][2]), TextureFormat.RGB24, false);
                        texture3DTemp.filterMode = FilterMode.Point;
                        for (int x = 0; x < int.Parse(chunkTextureDetailsRaw[i1][0]); x++)
                        {
                            for(int y = 0; y < int.Parse(chunkTextureDetailsRaw[i1][1]); y++)
                            {
                                for(int z = 0; z < int.Parse(chunkTextureDetailsRaw[i1][2]); z++)
                                {
                                    colorTemp = new Color();
                                    colorTemp.r = float.Parse(chunkTextureDetailsRaw[i1][0+i3]);
                                    colorTemp.g = float.Parse(chunkTextureDetailsRaw[i1][1+i3]);
                                    colorTemp.b = float.Parse(chunkTextureDetailsRaw[i1][2+i3]);
                                    
                                    texture3DTemp.SetPixel(x, y, z, colorTemp);
                                    i3+=3;
                                    counter++;
                                }
                            }
                        }
                        texture3DTemp.Apply();
                        chunkTemp.chunkTextureDetails[i1] = texture3DTemp;
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
