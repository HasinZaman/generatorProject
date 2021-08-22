using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Chunk MonoBehaviour script handles ChunkGameObject
/// </summary>
public class Chunk : MonoBehaviour
{
    /// <summary>
    ///     pos instances stores the position of the Chunk relative to center
    /// </summary>
    [SerializeField]
    private int[] pos;

    /// <summary>
    ///     meshGenerator object handles mesh generating
    /// </summary>
    private MeshGenerator meshGenerator;
    
    /// <summary>
    ///     meshData strut stores the current mesh information
    /// </summary>
    private MeshData meshData;

    // object mesh instances
    Material material;
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private MeshCollider collider;

    /// <summary>
    ///     start method sets up Chunk objects
    /// </summary>
    public void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        meshFilter.mesh = mesh;
        collider = this.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        material = this.GetComponent<Renderer>().material;
        this.GetComponent<Renderer>().material = material;
    }

    /// <summary>
    ///     updatMesh method updates the chunks mesh
    /// </summary>
    public void updateMesh()
    {
        if(meshGenerator == null)
        {
            throw new InvalidOperationException("meshGenerator not defined");
        }
        meshData = meshGenerator.getMesh();
        mesh.Clear();

        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;

        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
    }

    /// <summary>
    ///     updatMesh method updates the chunks mesh
    /// </summary>
    /// <param name="n">n paramater defines the nodes that will be skipped when generating mesh</param>
    public void updateMesh(int n)
    {
        if (meshGenerator == null)
        {
            throw new InvalidOperationException("meshGenerator not defined");
        }
        meshData = meshGenerator.getMesh(n);
        mesh.Clear();

        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;

        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
    }

    /// <summary>
    ///     setChunk method initates chunk
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="shader"></param>
    /// <param name="nodeDist"></param>
    public void setChunk(Grid grid, ComputeShader shader, float nodeDist)
    {
        if(nodeDist < 0)
        {
            throw new ArgumentException("nodeDist cannot be greater than 0");
        }
        this.chunkGrid = grid;
        this.meshGenerator = new MeshGenerator(grid, shader, "getVertices", 0.5f, 1);
    }

    /// <summary>
    ///     setChunk method initates chunk
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="shader"></param>
    /// <param name="nodeDist"></param>
    public void setChunk(Grid grid, ComputeShader shader, float[] nodeDist)
    {
        if (nodeDist.Length != 3)
        {
            throw new ArgumentException("nodeDist requires 3 values in array");
        }

        for(int i1 = 0; i1 < 3; i1++)
        {
            if (nodeDist[i1] < 0)
            {
                throw new ArgumentException("nodeDist cannot be greater than 0");
            }
        }
        
        this.chunkGrid = grid;
        this.meshGenerator = new MeshGenerator(grid, shader, "getVertices", 0.5f, nodeDist);
    }
}
