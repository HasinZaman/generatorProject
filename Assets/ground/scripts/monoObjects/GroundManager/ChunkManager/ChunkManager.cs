using System;
using UnityEngine;

public class ChunkManager
{
    private GameObject chunkPrefab;

    float[] chunkDist = new float[3];

    float loadDist;
    float deLoadDist;
    float renderDist;
    float deRenderDist;

    public GameObject centerObj;
    public int[] pos = new int[2] { 0, 0 };

    ChunkGridNode[] activeChunks;
    LinkedStack<GameObject> freeChunks = new LinkedStack<GameObject>();

    ChunkFileReader<NodeFactory.node, Node> chunkFileReader;

    /// <summary>
    ///     ChunkGridNode stores a grid and assoicated gameObject
    /// </summary>
    struct ChunkGridNode
    {
        Grid grid;
        GameObject gameObject;
    }

    /// <summary>
    ///     Created ChunkManager constructor
    /// </summary>
    /// <param name="chunkPrefab">prefab</param>
    public ChunkManager(GameObject chunkPrefab)
    {
        this.chunkPrefab = chunkPrefab;
    }

    /// <summary>
    ///     setChunkDist method sets the width, height and length of a chunk
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void setChunkDist(float x, float y, float z)
    {
        if(x < 0 || y < 0 || z < 0)
        {
            throw new ArgumentException($"Paramaters must be: x({x}) > 0  and y({y}) > 0 and z({z}) > 0");
        }

        chunkDist[0] = x;
        chunkDist[1] = y;
        chunkDist[2] = z;
    }

    /// <summary>
    ///     setLoadDist method sets 
    /// </summary>
    /// <param name="renderDist"></param>
    /// <param name="deRenderDist"></param>
    /// <param name="loadDist"></param>
    /// <param name="deLoadDist"></param>
    public void setLoadDist(float renderDist, float deRenderDist, float loadDist, float deLoadDist)
    {
        if(renderDist > deRenderDist)
        {
            throw new ArgumentException($"renderDist({renderDist}) must be less than deRenderDist({deRenderDist})");
        }

        if(loadDist > deLoadDist)
        {
            throw new ArgumentException($"loadDist({loadDist}) must be less than deLoadDist({deLoadDist})");
        }

        if (deRenderDist >= loadDist)
        {
            throw new ArgumentException($"deRenderDist({deRenderDist}) must be less than loadDist({loadDist})");
        }

        this.renderDist = renderDist;
        this.deRenderDist = deRenderDist;
        this.loadDist = loadDist;
        this.deLoadDist = deLoadDist;

        //calculate size
        renderedChunks = new GameObject[(int) Mathf.Ceil(deRenderDist * deRenderDist)];
        loadedChunks = new Grid[(int)Mathf.Ceil(deLoadDist * deLoadDist) - renderedChunks.Length];
    }

    /// <summary>
    ///     update method loads and deloads chunks
    /// </summary>
    public void update()
    {

    }

    /// <summary>
    ///     loadChunk creates Grid from .chunk file
    /// </summary>
    /// <param name="x">x pos of chunk</param>
    /// <param name="y">y pos of chunk</param>
    /// <returns>Grid of .chunk file</returns>
    public Grid loadChunk(int x, int y)
    {
        ChunkParam fileParam = new ChunkParam();

        fileParam.fileName = $"C({x},{y})";

        return chunkFileReader.getHeightMap(fileParam);
    }

    public void convertChunk(GameObject c, Grid g)
    {
        throw new NotImplementedException();
    }

}
