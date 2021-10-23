using System;
using UnityEngine;

public class ChunkManager
{
    float[] chunkDist = new float[3];

    float loadDist;
    float deLoadDist;
    float renderDist;
    float deRenderDist;

    public GameObject centerObj;
    public int[] pos = new int[2] { 0, 0 };

    GameObject[] renderedChunks;
    Grid[] loadedChunks;
    ChunkFileReader<NodeFactory.node, Node> chunkFileReader;

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
        renderedChunks = new GameObject[(int) Mathf.Max(circleArea(this.deRenderDist) / (chunkDist[0] * chunkDist[1]))];
        loadedChunks = new Grid[(int) Mathf.Max(circleArea(this.deLoadDist) / (chunkDist[0] * chunkDist[1]) - circleArea(this.deRenderDist) / (chunkDist[0] * chunkDist[1]))];
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

    /// <summary>
    ///     Utility method that calculates the area of a circle using Riemann sum. In which the Chunk x dist is length of each segment. The height of a rectangle is the height of cricle at x rounded up to the nearest number divisible by Chunk y dist. Method is used to calculate the number of chunks that touch or are within a circle of a given radius.
    /// </summary>
    /// <param name="radius">radius of circle</param>
    /// <returns>Area of circle</returns>
    private float circleArea(float radius)
    {
        float area = 0;

        for(float i1 = 0; i1 < radius; i1+= chunkDist[0])
        {
            area += chunkDist[0] * ceil(Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(floor(i1, chunkDist[0]), 2)), chunkDist[0]);
        }

        return area * 4;
    }

    /// <summary>
    ///     floor method is a utility method that rounds down to the nearest number divisible by b
    /// </summary>
    /// <param name="i">Number being floored</param>
    /// <param name="b">Divisibility base</param>
    /// <returns>float of floored number</returns>
    private float floor(float i, float b)
    {
        return Mathf.Floor(i / b) * b;
    }

    /// <summary>
    ///     ceil method is a utility method that rounds up to the nearest number divisible by b
    /// </summary>
    /// <param name="i">Number being ceiled</param>
    /// <param name="b">Divisibility base</param>
    /// <returns>float of ceiled number</returns>
    private float ceil(float i, float b)
    {
        return Mathf.Ceil(i / b) * b;
    }
}
