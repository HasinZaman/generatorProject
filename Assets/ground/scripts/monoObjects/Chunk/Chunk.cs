using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    int[] chunkPos;

    public Grid chunkGrid;
    public MeshGenerator meshGenerator;
    
    MeshData meshData;

    Material material;
    public MeshFilter meshFilter;
    public Mesh mesh;
    public MeshCollider collider;

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

    public void updateMesh()
    {
        if(meshGenerator == null)
        {
            throw new InvalidOperationException("meshGenerator not defined");
        }
        meshData = meshGenerator.getMesh();



        Debug.Log(meshData.vertices.Length);

        string str = $"({meshData.vertices[0].x},{meshData.vertices[0].y},{meshData.vertices[0].z})";
        for(int i1 = 1; i1 < meshData.vertices.Length; i1++)
        {
            if (i1 % 3 == 0)
            {
                str += "\n";
            }

            str += $" ({meshData.vertices[i1].x},{meshData.vertices[i1].y},{meshData.vertices[i1].z})";
            
        }
        Debug.Log(str);

        Debug.Log(meshData.triangles.Length);
        Debug.Log(String.Join(",", meshData.triangles));

        mesh.Clear();

        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;

        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
    }

    public void setChunk(Grid grid, ComputeShader shader)
    {
        this.chunkGrid = grid;
        this.meshGenerator = new MeshGenerator(grid, shader, "getVertices", 0.5f, 1);
    }
}
