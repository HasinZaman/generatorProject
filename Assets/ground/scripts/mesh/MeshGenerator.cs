using System;
using UnityEngine;

public class MeshGenerator
{
    /// <summary>
    ///     shader is a instance that deals with cpu <-> gpu computeShader calculations
    /// </summary>
    private ComputeShader shader;
    
    /// <summary>
    ///     computeAlgorthim is a string that will be used to calculate the mesh
    /// </summary>
    private string computeAlgorthim;

    /// <summary>
    ///     grid is a refrence to a grid that is used calculate a mesh
    /// </summary>
    private Grid grid;

    /// <summary>
    ///     threshold is used to determine the which nodes are active and are not
    /// </summary>
    private float threshold;

    /// <summary>
    ///     distPerNode is the distance between nodes
    /// </summary>
    private float[] distPerNode;

    /// <summary>
    ///     kernelhandle is the id of the algorthim used to generate a mesh
    /// </summary>
    private int kernelHandle;

    /// <summary>
    ///     Triangle struct is used to convert gpu buffer into a useable struct
    /// </summary>
    struct Triangle
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
    }

    /// <summary>
    ///     Constructor initializes MeshGenerator
    /// </summary>
    /// <param name="grid">grid paramater initalizes grid refrence</param>
    /// <param name="shader">shader paramater initalizes shader instance</param>
    /// <param name="algorthim">algorthim paramater initalizes computeAlgorthim instance</param>
    /// <param name="threshold">threshold paramater initalizes threshold instance</param>
    /// <param name="distPerNode"></param>
    public MeshGenerator(Grid grid, ComputeShader shader, string algorthim, float threshold, float distPerNode)
    {
        setGrid(grid);
        setComputeShader(shader, algorthim);

        this.threshold = threshold;
        this.distPerNode = new float[] { distPerNode, distPerNode, distPerNode } ;
    }

    /// <summary>
    ///     Constructor initializes MeshGenerator
    /// </summary>
    /// <param name="grid">grid paramater initalizes grid refrence</param>
    /// <param name="shader">shader paramater initalizes shader instance</param>
    /// <param name="algorthim">algorthim paramater initalizes computeAlgorthim instance</param>
    /// <param name="threshold">threshold paramater initalizes threshold instance</param>
    /// <param name="distPerNode"></param>
    public MeshGenerator(Grid grid, ComputeShader shader, string algorthim, float threshold, float[] distPerNode)
    {
        setGrid(grid);
        setComputeShader(shader, algorthim);



        this.threshold = threshold;
        this.distPerNode = distPerNode;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    public void setGrid(Grid grid)
    {
        this.grid = grid;
    }

    /// <summary>
    ///     setComputeShader sets shader and algorthim
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="algorthim"></param>
    public void setComputeShader(ComputeShader shader, string algorthim)
    {
        if(shader == null)
        {
            throw new ArgumentNullException();
        }

        this.shader = shader;
        this.computeAlgorthim = algorthim;

        int kernelHandle = shader.FindKernel(computeAlgorthim);
    }

    /// <summary>
    ///     GetMesh returns a MeshData instance with every nth nodes from grid
    /// </summary>
    /// <param name="n">n is an int that define which nodes that are selected</param>
    /// <returns>
    ///     Meshdata of a mesh of grid
    /// </returns>
    public MeshData getMesh(int n)
    {
        if(this.grid == null)
        {
            throw new InvalidOperationException("Grid not set");
        }
        //Debug.Log(grid.toString());
        int[] dim = grid.getDim();
        
        //declaring Compute Buffers
        ComputeBuffer trianglesBuffer;
        ComputeBuffer triangleCountBuffer;
        ComputeBuffer nodeBuffer;
        
        //declaring variables that store information provided by the gpu
        Triangle[] trianglesTemp;
        int[] triangleCountArray = { 0 };

        //outputed mesh data variables
        int[] triangleFinal;
        Vector3[] verticesFinal;


        trianglesBuffer = new ComputeBuffer(dim[0] * dim[1] * dim[2] * 15, sizeof(float) * 3 * 3, ComputeBufferType.Append);
        triangleCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        nodeBuffer = new ComputeBuffer(dim[0] * dim[1] * dim[2], sizeof(float), ComputeBufferType.Default);

        //setting up shader variables
        shader.SetBuffer(kernelHandle, "triangles", trianglesBuffer);
        trianglesBuffer.SetCounterValue(0);

        
        shader.SetFloat("threshold", threshold);
        shader.SetFloats("distPerNode", distPerNode);

        shader.SetInts("dim", dim);

        nodeBuffer = new ComputeBuffer(dim[0] * dim[1] * dim[2], sizeof(float), ComputeBufferType.Default);
        shader.SetBuffer(kernelHandle, "nodes", nodeBuffer);
        nodeBuffer.SetCounterValue(0);
        nodeBuffer.SetData(grid.toArray(n));

        //starting computeBuffer
        shader.Dispatch(kernelHandle, (int)Math.Ceiling((float)dim[0] / 10), (int)Math.Ceiling((float)dim[1] / 10), (int)Math.Ceiling((float)dim[2] / 10));

        //getting triangle information from Compute shader
        ComputeBuffer.CopyCount(trianglesBuffer, triangleCountBuffer, 0);

        triangleCountBuffer.GetData(triangleCountArray);


        trianglesTemp = new Triangle[triangleCountArray[0]];
        trianglesBuffer.GetData(trianglesTemp, 0, 0, triangleCountArray[0]);

        triangleFinal = new int[triangleCountArray[0] * 3];
        verticesFinal = new Vector3[triangleCountArray[0] * 3];

        //convert rawData into triangle and vertices array for material
        for (int i1 = 0; i1 < trianglesTemp.Length; i1++)
        {
            for (int i2 = 0; i2 < 3; i2++)
            {
                triangleFinal[i1 * 3 + i2] = i1 * 3 + i2;
                switch (i2)
                {
                    case 0:
                        verticesFinal[i1 * 3] = trianglesTemp[i1].p1;
                        break;
                    case 1:
                        verticesFinal[i1 * 3 + 1] = trianglesTemp[i1].p2;
                        break;
                    case 2:
                        verticesFinal[i1 * 3 + 2] = trianglesTemp[i1].p3;
                        break;
                }
            }

        }
        trianglesBuffer.Release();
        triangleCountBuffer.Release();
        nodeBuffer.Release();

        return new MeshData(verticesFinal, triangleFinal);
    }

    /// <summary>
    ///     GetMesh returns a MeshData instance without any nodes from grid being skipped
    /// </summary>
    /// <returns>
    ///     Meshdata of a mesh of grid
    /// </returns>
    public MeshData getMesh()
    {
        return getMesh(1);
    }

    /// <summary>
    ///     toString method returns a string representation grid
    /// </summary>
    /// <returns>string representation of class</returns>
    public string toString()
    {
        return grid.toString();
    }

    /// <summary>
    ///     repr method is a string representation of grid used for saving files
    /// </summary>
    /// <returns>string representation of grid used for saving files</returns>
    public string repr()
    {
        return $"{grid.toString("|", "|", "", ",")}";
    }
}
