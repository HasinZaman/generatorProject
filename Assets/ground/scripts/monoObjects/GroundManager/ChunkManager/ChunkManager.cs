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

    ChunkGridNode[] activeChunks;
    FreeChunkStack freeChunks = new FreeChunkStack();

    ChunkFileReader<NodeFactory.node, Node> chunkFileReader;

    /// <summary>
    ///     FreeChunkStack stores a statck of chunks
    /// </summary>
    class FreeChunkStack : LinkedStack<GameObject>
    {
        public static GameObject chunkPrefab;

        /// <summary>
        ///     pop returns top most element of stack. If stack is empty, then a new Chunk GameObject is returned
        /// </summary>
        /// <returns>ChunkGameObject</returns>
        public override GameObject pop()
        {
            if(this.isEmpty())
            {
                return UnityEngine.Object.Instantiate(FreeChunkStack.chunkPrefab);
            }
            GameObject elem = base.pop();
            elem.SetActive(true);
            return elem;
        }

        /// <summary>
        ///     Chunk GameObject is prepared and inserted into stack
        /// </summary>
        /// <param name="elem">Chunk GameObject</param>
        public override void push(GameObject elem)
        {
            elem.SetActive(false);
            base.push(elem);
        }
    }

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
        FreeChunkStack.chunkPrefab = chunkPrefab;
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
    ///     setLoadDist method sets load, deload dist and array activeChunk
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

        activeChunks = new ChunkGridNode[(int) (Mathf.Ceil(deLoadDist/chunkDist[0] * 2 + 1) * Mathf.Ceil(deLoadDist / chunkDist[2] * 2 + 1))];

        for(int i1 = 0; i1 < activeChunks.Length; i1++)
        {
            activeChunks[i1] = new ChunkGridNode();
        }

    }

    /// <summary>
    ///     update method loads and deloads chunks
    /// </summary>
    public void update()
    {
        //updating pos
        int[] posTmp = new int[2];
        if(centerObj != null)
        {
            posTmp[0] = Mathf.RoundToInt(centerObj.transform.position.x / chunkDist[0]);
            posTmp[1] = Mathf.RoundToInt(centerObj.transform.position.z / chunkDist[2]);
        }

        if(posTmp[0] != pos[0] || posTmp[1] != posTmp[1])
        {

        }
        //update rendered chunks
        
        //unload chunks
        //load new chunks

        //update loaded chunks
        //
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
    
    private int clamp(int i1, int min, int max)
    {
        if(i1 < min)
        {
            return min;
        }
        else if(i1 > max)
        {
            return max;
        }

        return i1;
    }

    private bool rangeCheck(int i1, int min, int max)
    {
        if (i1 < min)
        {
            return false;
        }
        else if (i1 > max)
        {
            return false;
        }

        return true;
    }

}
