﻿using System.Collections;
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

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();

        meshFilter.mesh = mesh;
        collider = this.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        material = this.GetComponent<Renderer>().material;
        this.GetComponent<Renderer>().material = material;

        updateMesh();
    }

    void updateMesh()
    {
        meshData = meshGenerator.getMesh();

        mesh.Clear();

        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.triangles;

        mesh.RecalculateNormals();

        collider.sharedMesh = mesh;
    }

    public void setChunk(Grid grid, ComputeShader shader)
    {
        this.chunkGrid = grid;
        this.meshGenerator = new MeshGenerator(grid, shader, "getVertices", 1, 1);
    }
}
