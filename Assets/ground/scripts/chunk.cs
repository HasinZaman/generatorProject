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
    
    SerializedProperty serializedNodes, serializedPos, serializedMesh, serializedCollider, serializedVertices, serializedComputerShader, serializedManager, serializedm;

    SerializedProperty[] serializedGroundTextures = new SerializedProperty[2];

    int layer = 0;
    bool layerMode = false;

    string[] planes = new string[3] { "XY", "XZ", "YZ" };
    int plane = 0;

    bool neighborMode = false;
    GUIStyle nullVal, passVal, failVal;

    string[] textureDirections = new string[2] {"side", "top" };
    int textureDirection = 0;

    public void OnEnable()
    {
        c = (chunk)target;

        targetObject = new UnityEditor.SerializedObject(c);

        serializedNodes = targetObject.FindProperty("n");
        serializedPos = targetObject.FindProperty("pos");
        serializedMesh = targetObject.FindProperty("mesh");
        serializedCollider = targetObject.FindProperty("collider");
        serializedVertices = targetObject.FindProperty("vertices");

        serializedComputerShader = targetObject.FindProperty("shader");

        serializedManager = targetObject.FindProperty("manager");

        serializedm = targetObject.FindProperty("m");

        serializedGroundTextures[0] = targetObject.FindProperty("groundTopTextures");
        serializedGroundTextures[1] = targetObject.FindProperty("groundSideTextures");

        nullVal = new GUIStyle();
        nullVal.normal.background = textureMaker(1, 1, Color.yellow);
        passVal = new GUIStyle();
        passVal.normal.background = textureMaker(1, 1, Color.green);
        failVal = new GUIStyle();
        failVal.normal.background = textureMaker(1, 1, Color.red);
    }

    public override void OnInspectorGUI()
    {
        targetObject.Update();

        EditorGUILayout.Vector3IntField("Pos", serializedPos.vector3IntValue);
        serializedComputerShader.objectReferenceValue = EditorGUILayout.ObjectField("Shader", serializedComputerShader.objectReferenceValue, typeof(ComputeShader));
        EditorGUILayout.ObjectField("Shader", serializedManager.objectReferenceValue, typeof(groundManager));

        layerMode = EditorGUILayout.Toggle("Node Mode", layerMode);
        neighborMode = EditorGUILayout.Toggle("show neighbor Nodes", neighborMode);
        plane = EditorGUILayout.Popup("Display Mode", plane, planes);

        EditorGUILayout.LabelField("Chunk Texture");
        textureDirection = EditorGUILayout.Popup("Direction", textureDirection, textureDirections);
        chunkTextues(serializedGroundTextures[textureDirection]);
        //textureDirections;


        if (c.n != null)
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
    
    private void chunkTextues(SerializedProperty direction)
    {
        EditorGUILayout.BeginHorizontal();
        textureInput(direction.GetArrayElementAtIndex(Convert.ToInt32("01", 2)), EditorGUIUtility.currentViewWidth / 3);
        GUILayout.FlexibleSpace();
        textureInput(direction.GetArrayElementAtIndex(Convert.ToInt32("11", 2)), EditorGUIUtility.currentViewWidth / 3);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        textureInput(direction.GetArrayElementAtIndex(4), EditorGUIUtility.currentViewWidth / 3);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        textureInput(direction.GetArrayElementAtIndex(Convert.ToInt32("00", 2)), EditorGUIUtility.currentViewWidth / 3);
        GUILayout.FlexibleSpace();
        textureInput(direction.GetArrayElementAtIndex(Convert.ToInt32("10", 2)), EditorGUIUtility.currentViewWidth / 3);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Update Textures"))
        {
            for (int i1 = 0; i1 < 4; i1++)
            {
                c.m.SetTexture($"Texture2D_TopPrimaryColour_{i1}", (Texture2D) direction.GetArrayElementAtIndex(i1).objectReferenceValue);
            }
            c.m.SetTexture($"Texture2D_TopCenterColour", (Texture2D) direction.GetArrayElementAtIndex(4).objectReferenceValue);
        }

    }

    void drawTexture(Texture2D t, Vector2 size)
    {
        GUIStyle s = new GUIStyle();
        s.fixedWidth = size.x;
        s.fixedHeight = size.y;
        EditorGUILayout.BeginVertical(s);
        GUILayout.BeginVertical(t, s);
        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }
    void textureInput(SerializedProperty s, float width)
    {
        EditorGUILayout.BeginVertical();
        drawTexture((Texture2D)s.objectReferenceValue, new Vector2(100, 100));
        s.objectReferenceValue = (Texture2D) EditorGUILayout.ObjectField((Texture2D) s.objectReferenceValue, typeof(Texture2D), false, GUILayout.Width(100));
        EditorGUILayout.EndVertical();
    }
    void textureInput(Texture2D t, float width)
    {
        EditorGUILayout.BeginVertical();
        drawTexture(t, new Vector2(100, 100));
        t = (Texture2D) EditorGUILayout.ObjectField(t, typeof(Texture2D), false, GUILayout.Width(100));
        EditorGUILayout.EndVertical();
    }
    void drawNode()
    {
        int[] dim;
        float[] n;
        
        if (neighborMode)
        {
            dim = c.n.getDim(nodes.NODES);
            n = c.n.getNodes(nodes.NODES);
        }
        else
        {
            dim = c.n.getDim(nodes.CORE);
            n = c.n.getNodes(nodes.CORE);
        }
        GUILayout.BeginHorizontal();
        for (int i1 = 0; i1 < dim[0]; i1++)
        {
            GUILayout.BeginVertical();
            for(int i2 = 0; i2 < dim[2]; i2++)
            {
                if(n[indexCreator(plane, i1, i2, layer, dim)] <= -3.402823466e+38F + 10)
                {
                    GUILayout.BeginHorizontal(nullVal);
                }
                else if (n[indexCreator(plane, i1, i2, layer, dim)] > c.manager.threshold)
                {
                    GUILayout.BeginHorizontal(passVal);
                }
                else
                {
                    GUILayout.BeginHorizontal(failVal);
                }
                if(layerMode)
                {
                    n[indexCreator(plane, i1, i2, layer, dim)] = EditorGUILayout.FloatField(n[indexCreator(plane, i1, i2, layer, dim)]);
                }
                else
                {
                    EditorGUILayout.Toggle(n[indexCreator(plane, i1, i2, layer, dim)] > c.manager.threshold);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        layer = EditorGUILayout.IntSlider("Layer", layer, 0, dim[1] - 1);
    }

    private int indexCreator(int mode, int i1, int i2,int layer, int[] dim)
    {
        switch(mode)
        {
            case 0://xy
                return i1 + (i2 + layer * dim[1]) * dim[0];
                break;
            case 1://xz
                return i1 + (layer + i2 * dim[1]) * dim[0];
                break;
            case 2://yz
                return layer + (i1 + i2 * dim[1]) * dim[0];
                break;
        }
        throw new Exception("error");
    }

    private Texture2D textureMaker(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
public class chunk : MonoBehaviour
{
    //chunk manager
    public groundManager manager;

    //cube information
    public nodes n;

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

    public Texture3D edgeNoise;
    public Texture3D macroNoiseTexture;
    public Material m;

    public Texture2D[] groundTopTextures = new Texture2D[5];
    public Texture2D[] groundSideTextures = new Texture2D[5];


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
        collider = this.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        m = this.GetComponent<Renderer>().material;
        this.GetComponent<Renderer>().material = m;


        //creating mesh
        //even pos
        setMaterialNoise();

        updateMesh();
    }
    private void setMaterialNoise()
    {
        m.SetTexture("Texture3D_edgeNoise", edgeNoise);
        
        m.SetVector($"Vector3_NodesDim", new Vector3(manager.dim[0], manager.dim[1], manager.dim[2]));
        this.GetComponent<Renderer>().material = m;
    }

    public void updateMesh()
    {
        int[] dim;

        int kernelHandle = shader.FindKernel(manager.computeAlgorthim);

        //declaring Compute Buffers
        ComputeBuffer trianglesBuffer;
        ComputeBuffer triangleCountBuffer;
        ComputeBuffer nodeBuffer;

        //declaring output variables
        Triangle[] trianglesTemp;
        int[] triangleCountArray = { 0 };

        dim = n.getDim(nodes.NODES);

        trianglesBuffer = new ComputeBuffer(dim[0] * dim[1] * dim[2] * 15, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        triangleCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        nodeBuffer = new ComputeBuffer(dim[0] * dim[1] * dim[2], sizeof(float), ComputeBufferType.Default);

        //setting up shader variables
        shader.SetBuffer(kernelHandle, "triangles", trianglesBuffer);
        trianglesBuffer.SetCounterValue(0);

        shader.SetFloat("threshold", manager.threshold);
        shader.SetFloats("distPerNode", manager.distPerNode);

        shader.SetInts("dim", dim);

        nodeBuffer = new ComputeBuffer(dim[0] * dim[1] * dim[2], sizeof(float), ComputeBufferType.Default);
        shader.SetBuffer(kernelHandle, "nodes", nodeBuffer);
        nodeBuffer.SetCounterValue(0);
        nodeBuffer.SetData(n.getNodes(nodes.NODES));

        //starting computeBuffer
        shader.Dispatch(kernelHandle, (int) Math.Ceiling((float) dim[0] / 10), (int) Math.Ceiling((float)dim[1] / 10), (int) Math.Ceiling((float)dim[2] / 10));

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

        trianglesBuffer.Release();
        triangleCountBuffer.Release();
        nodeBuffer.Release();
    }

    //toString turns instances into a string
    public string toString()
    {
        //format
        //Chunk value|Node value|chunkTextureDetails
        //           ^ Line seperates chunk values and node values
        string temp = $"{this.transform.position.x},{this.transform.position.y},{this.transform.position.z},{this.pos.x},{this.pos.y},{this.pos.z}|{n.ToString()}|{chunkTextureDetailsToString()}";

        return temp;
    }

    private string chunkTextureDetailsToString()
    {
        string temp = "";

        int u, v, d;

        u = edgeNoise.width;
        v = edgeNoise.height;
        d = edgeNoise.depth;

        temp += $"{u},{v},{d},";

        for (int x = 0; x < u; x++)
        {
            for (int y = 0; y < v; y++)
            {
                for (int z = 0; z < d; z++)
                {
                    temp += $"{edgeNoise.GetPixel(x,y,z).r},{edgeNoise.GetPixel(x, y, z).g},{edgeNoise.GetPixel(x, y, z).b},";
                }
            }
        }
            
        temp = temp.Remove(temp.Length - 1);
        temp += "|";

        

        return temp;
    }

}

public class nodes
{
    //dim of core nodes
    private int[] dim;

    private float[] core;
    private float[] n;

    public const float NaN = -3.402823466e+38F + 10;
    public const int CORE = 0;
    public const int NODES = 1;
    //map of neighbor array

    // ^
    // |
    // z x ->
    // y goes up the layers

    //layer below core layer
    //  6   7   8
    //  3   4   5
    //  0   1   2

    //same layer as core
    //  14  15  16
    //  12  X   13
    //  9   10  11

    //layer above core layer
    //  23  24  25
    //  20  21  22
    //  17  18  19

    //index legend
    // X is the location of nodes relative to neghboring chunks the core nodes
    // 4, 10, 12, 13, 15, 21 are the indexes of neghboring chunks that share a face with these nodes
    // 1, 3, 5, 7, 9, 11, 14, 16, 18, 20, 22, 24 are the indexes of neghboring chunks that share a edge with these nodes
    // 0, 2, 6, 8, 17, 19, 23, 25 are the indexes of neghboring chunks that share a corner

    //neghbor array values
    // {0, 1, 3, 4, 9 , 10, 12}
    public nodes[] neighbor = new nodes[7];
    private Dictionary<int, int> neighborId = new Dictionary<int, int>
        {
            { 0, 0 },
            { 1, 1 },
            { 3, 2 },
            { 4, 3 },
            { 9, 4 },
            { 10, 5 },
            { 12, 6 }
        };


    public nodes(float[] n, int[] dim)
    {
        this.dim = dim;
        this.n = new float[(dim[0] + 1) * (dim[1] + 1) * (dim[2] + 1)];
        this.core = new float[dim[0] * dim[1] * dim[2]];
        //setting up n
        setNodes(NaN, new int[3] { -1, -1, -1 }, new int[3] { dim[0], 0, dim[2] }, NODES);
        setNodes(NaN, new int[3] { -1, -1, -1 }, new int[3] { 0, dim[1], dim[2] }, NODES);
        setNodes(NaN, new int[3] { -1, -1, -1 }, new int[3] { dim[0], dim[1], 0 }, NODES);

        setNodes(n, this.dim, new int[3] { 0, 0, 0 }, this.dim, CORE);
        setNodes(n, this.dim, new int[3] { 0, 0, 0 }, this.dim, NODES);
    }

    public nodes(float[] n, int[] dim, nodes[] neighbor)
    {
        this.dim = dim;
        this.n = new float[(dim[0] + 1) * (dim[1] + 1) * (dim[2] + 1)];
        this.core = new float[dim[0] * dim[1] * dim[2]];
        //setting up n
        setNodes(NaN, new int[3] { -1, -1, -1 }, new int[3] { dim[0], 0, dim[2] }, NODES);
        setNodes(NaN, new int[3] { -1, -1, -1 }, new int[3] { 0, dim[1], dim[2] }, NODES);
        setNodes(NaN, new int[3] { -1, -1, -1 }, new int[3] { dim[0], dim[1], 0 }, NODES);

        setNodes(n, this.dim, new int[3] { 0, 0, 0 }, this.dim, CORE);
        this.neighbor = neighbor;
        setNodes(n, dim, new int[3] { 0, 0, 0 }, this.dim, NODES);
    }

    public int[] getDim(int mode)
    {
        int[] tempInt = (int[]) dim.Clone();

        if (mode == NODES && n != null)
        {
            for (int i1 = 0; i1 < 3; i1++)
            {
                tempInt[i1]+=1;
            }
        }
        return tempInt;
    }

    //setting nodes
    public void setNodes(float[] vals, int[] valDim, int[] start, int[] end, int code)
    {
        int[] i1 = new int[3] { 0, 0, 0 };

        for (int x = start[0]; x < end[0]; x++)
        {
            for (int y = start[1]; y < end[1]; y++)
            {
                for (int z = start[2]; z < end[2]; z++)
                {
                    setNode(vals[i1[0] + (i1[1] + i1[2] * valDim[1]) * valDim[0]], x, y, z, code);

                    if(i1[2] + 1 < valDim[2])
                    {
                        i1[2]++;
                    }
                    else
                    {
                        i1[2] = 0;
                        if(i1[1] + 1 < valDim[1])
                        {
                            i1[1]++;
                        }
                        else
                        {
                            i1[1] = 0;
                            i1[0]++;
                        }
                    }
                }
            }
        }
    }
    public void setNodes(float val, int[] start, int[] end, int code)
    {
        for (int x = start[0]; x < end[0]; x++)
        {
            for (int y = start[1]; y < end[1]; y++)
            {
                for (int z = start[2]; z < end[2]; z++)
                {
                    setNode(val, x, y, z, code);
                }
            }
        }
    }

    //converts position into index
    public void setNode(float value, int x, int y, int z, int code)
    {
        int index;
        switch (code)
        {
            case CORE:
                index = x + (y + z * dim[1]) * dim[0];
                setNode(value, index, CORE);
                break;
            case NODES:
                index = (x + 1) + ((y + 1) + (z + 1) * (dim[1] + 1)) * (dim[0] + 1);
                setNode(value, index, NODES);
                if(x >= 0 && y >= 0 && z >= 0)
                {
                    index = x + (y + z * dim[1]) * dim[0];
                    setNode(value, index, CORE);
                }
                break;
        }
    }

    //most basic form of setNodes - in which one value is changed
    public void setNode(float value, int index, int code)
    {
        switch (code)
        {
            case CORE:
                core[index] = Math.Max(NaN, value);
                break;
            case NODES:
                n[index] = Math.Max(NaN, value);
                break;
        }
    }

    public void setNeighbor(nodes n, int index)
    {
        neighbor[neighborId[index]] = n;
        setNeighborNodes(index);
    }

    public void setNeighborNodes(int neighborIndex)
    {
        int[] dimTemp;
        int[] startTemp;
        int[] endTemp;

        if (neighbor[neighborId[neighborIndex]] == null)
        {
            switch (neighborIndex)
            {
                //neighbors that share faces
                case 4:
                    startTemp = new int[3] { 0, -1, 0 };
                    endTemp = (int[]) this.dim.Clone();
                    endTemp[1] = 0;
                    break;
                case 10:
                    startTemp = new int[3] { 0, 0, -1 };
                    endTemp = (int[]) this.dim.Clone();
                    endTemp[2] = 0;
                    break;
                case 12:
                    startTemp = new int[3] { -1, 0, 0 };
                    endTemp = (int[]) this.dim.Clone();
                    endTemp[0] = 0;
                    break;

                //neighbors that shared edges
                //1, 3, 5, 7, 9, 11, 14, 16, 18, 20, 22, 24

                case 1:
                    startTemp = new int[3] { 0, -1, -1 };
                    endTemp = new int[3] { this.dim[0], 0, 0 };
                    break;
                case 3:
                    startTemp = new int[3] { -1, -1, 0 };
                    endTemp = new int[3] { 0, 0, this.dim[2] };
                    break;
                case 9:
                    startTemp = new int[3] { -1, 0, -1 };
                    endTemp = new int[3] { 0, this.dim[1], 0 };
                    break;
                
                //corner values
                case 0:
                    setNode(NaN, -1, -1, -1, NODES);
                    return;
                default:
                    if (neighborIndex > 25 || neighborIndex < 0)
                    {
                        throw new ArgumentException("neighbor index out of range");
                    }
                    else if (neighbor[neighborId[neighborIndex]] == null)
                    {
                        throw new Exception("Neighbor is not set");
                    }
                    throw new Exception("Error");

            }
            setNodes(NaN, startTemp, endTemp, NODES);
        }


        switch (neighborIndex)
        {
            //neighbors that share faces
            case 4:
                dimTemp = neighbor[neighborId[4]].getDim(CORE);
                dimTemp[1] = 1;
                startTemp = new int[3] { 0, -1, 0 };
                endTemp = (int[]) this.dim.Clone();
                endTemp[1] = 0;
                break;
            case 10:
                dimTemp = neighbor[neighborId[10]].getDim(CORE);
                dimTemp[2] = 1;
                startTemp = new int[3] { 0, 0, -1 };
                endTemp = (int[]) this.dim.Clone();
                endTemp[2] = 0;
                break;
            case 12:
                dimTemp = neighbor[neighborId[12]].getDim(CORE);
                dimTemp[0] = 1;
                startTemp = new int[3] { -1, 0, 0 };
                endTemp = (int[]) this.dim.Clone();
                endTemp[0] = 0;
                break;
            //neighbors that shared edges
            //1, 3, 5, 7, 9, 11, 14, 16, 18, 20, 22, 24

            case 1:
                dimTemp = new int[3] { neighbor[neighborId[1]].getDim(CORE)[0], 1, 1 };
                startTemp = new int[3] { 0, -1, -1 };
                endTemp = new int[3] { this.dim[0], 0, 0 };
                break;
            case 3:
                dimTemp = new int[3] { 1, 1, neighbor[neighborId[3]].getDim(CORE)[2] };
                startTemp = new int[3] { -1, -1, 0 };
                endTemp = new int[3] { 0, 0, this.dim[2] };
                break;
            case 9:
                dimTemp = new int[3] { 1, neighbor[neighborId[9]].getDim(CORE)[1], 1 };
                startTemp = new int[3] { -1, 0, -1 };
                endTemp = new int[3] { 0, this.dim[1], 0 };
                break;
            
            //corner values
            case 0:
                setNode(getNeighbor(neighborIndex)[0], -1, -1, -1, NODES);
                return;

            default:
                if(neighborIndex > 25 || neighborIndex < 0)
                {
                    throw new ArgumentException("neighbor index out of range");
                }
                else if(neighbor[neighborId[neighborIndex]] == null)
                {
                    throw new Exception("Neighbor is not set");
                }
                throw new Exception("Error");
                
        }

        setNodes(getNeighbor(neighborIndex), dimTemp, startTemp, endTemp, NODES);
    }

    public void setNeighborNodes()
    {
        for(int i1 = 0; i1 < neighbor.Length; i1++)
        {
            setNeighborNodes(i1);
        }
    }

    public float[] getNeighbor(int neighborIndex)
    {
        switch(neighborIndex)
        {
            //neighbors that share faces
            case 4:
                return neighbor[neighborId[4]].getFace(0, neighbor[neighborId[4]].getDim(CORE)[1] - 1);
                break;
            case 10:
                return neighbor[neighborId[10]].getFace(1, neighbor[neighborId[10]].getDim(CORE)[2] - 1);
                break;
            case 12:
                return neighbor[neighborId[12]].getFace(2, neighbor[neighborId[12]].getDim(CORE)[0] - 1);
                break;
            
            //neighbors that shared edges
            //1, 3, 5, 7, 9, 11, 14, 16, 18, 20, 22, 24

            case 1:
                return neighbor[neighborId[1]].getLine(0, new int[3] { 0, neighbor[neighborId[1]].getDim(CORE)[1] - 1, neighbor[neighborId[1]].getDim(CORE)[2] - 1 });
                break;
            case 3:
                return neighbor[neighborId[3]].getLine(2, new int[3] { neighbor[neighborId[3]].getDim(CORE)[0] - 1, neighbor[neighborId[3]].getDim(CORE)[1] - 1, 0 });
                break;
            case 9:
                return neighbor[neighborId[9]].getLine(1, new int[3] { neighbor[neighborId[9]].getDim(CORE)[0] - 1, 0, neighbor[neighborId[9]].getDim(CORE)[2] - 1 });
                break;
            
            //corner values
            case 0:
                return neighbor[neighborId[0]].getCorner(6);
                break;
        }
        return null;
    }

    public float[] getFace(int plane, int layer)
    {
        //0 = XZ
        //1 = XY
        //2 = YZ
        switch (plane)
        {
            case 0:
                return getNodes(new int[3] { 0, layer, 0 }, new int[3] { dim[0], layer + 1, dim[2] });
                break;
            case 1:
                return getNodes(new int[3] { 0, 0, layer }, new int[3] { dim[0], dim[1], layer + 1 });
                break;
            case 2:
                return getNodes(new int[3] { layer, 0, 0 }, new int[3] { layer + 1, dim[1], dim[2] });
                break;
        }
        throw new ArgumentException("Parameter must be 0,1 or 2");
    }

    public float[] getLine(int parrelAxis, int[] p)
    {
        // 0 = x
        // 1 = y
        // 2 = z
        switch(parrelAxis)
        {
            case 0:
                return getNodes(new int[3] { 0, p[1], p[2] }, new int[3] { dim[0], p[1] + 1, p[2] + 1 });
                break;
            case 1:
                return getNodes(new int[3] { p[0], 0, p[2] }, new int[3] { p[0] + 1, dim[1], p[2] + 1 });
                break;
            case 2:
                return getNodes(new int[3] { p[0], p[1], 0 }, new int[3] { p[0] + 1, p[1] + 1, dim[2] });
                break;
        }
        throw new ArgumentException("Parameter must be 0,1 or 2");
    }

    public float[] getCorner(int corner)
    {
        float[] temp = new float[1];
        switch(corner)
        {
            case 0:
                temp[0] = core[0 + (0 + 0 * dim[1]) * dim[0]];
                break;
            case 1:
                temp[0] = core[dim[0] - 1 + (0 + 0 * dim[1]) * dim[0]];
                break;
            case 2:
                temp[0] = core[dim[0] - 1 + (0 + (dim[2] - 1) * dim[1]) * dim[0]];
                break;
            case 3:
                temp[0] = core[0 + (0 + (dim[2] - 1) * dim[1]) * dim[0]];
                break;
            case 4:
                temp[0] = core[0 + (dim[1]-1 + 0 * dim[1]) * dim[0]];
                break;
            case 5:
                temp[0] = core[dim[0] - 1 + (dim[1] - 1 + 0 * dim[1]) * dim[0]];
                break;
            case 6:
                temp[0] = core[dim[0] - 1 + (dim[1] - 1 + (dim[2] - 1) * dim[1]) * dim[0]];
                break;
            case 7:
                temp[0] = core[0 + (dim[1] - 1 + (dim[2] - 1) * dim[1]) * dim[0]];
                break;
        }
        return temp;
    }

    //needs to updated so it can discrimnate between nodes and core
    public float[] getNodes(int[] start, int[] end)
    {
        int deltaX, deltaY, deltaZ;

        deltaX = end[0] - start[0];
        deltaY = end[1] - start[1];
        deltaZ = end[2] - start[2];

        float[] temp = new float[deltaX * deltaY * deltaZ];

        for(int x = start[0]; x < end[0]; x++)
        {
            for(int y = start[1]; y < end[1]; y++)
            {
                for(int z = start[2]; z < end[2]; z++)
                {
                    temp[x - start[0] + (y - start[1] + (z - start[2]) * deltaY) * deltaX] = core[x + (y + z * dim[1]) * dim[0]];
                }
            }
        }

        return temp;
    }

    public float[] getNodes(int mode)
    {
        if(mode == NODES && n != null)
        {
            return n;
        }
        else
        {
            return core;
        }
    }

    public override string ToString()
    {
        return string.Join(",", core);
    }
}