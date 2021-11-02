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

    int[] activeChunksDim = new int[2];
    ChunkGridNode[] activeChunks;
    FreeChunkStack freeChunks = new FreeChunkStack();

    ChunkFileReader<NodeFactory.node, Node> chunkFileReader;

    /// <summary>
    ///     FreeChunkStack stores a statck of chunks
    /// </summary>
    class FreeChunkStack : LinkedStack<GameObject>
    {
        public static GameObject chunkPrefab;
        public static Transform groundObject;
        /// <summary>
        ///     pop returns top most element of stack. If stack is empty, then a new Chunk GameObject is returned
        /// </summary>
        /// <returns>ChunkGameObject</returns>
        public override GameObject pop()
        {
            if(this.isEmpty())
            {
                return UnityEngine.Object.Instantiate(FreeChunkStack.chunkPrefab, FreeChunkStack.groundObject);
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
    public ChunkManager(GameObject chunkPrefab, Transform groundObject)
    {
        FreeChunkStack.chunkPrefab = chunkPrefab;
        FreeChunkStack.groundObject = groundObject;
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
        activeChunksDim[0] = (int) Mathf.Ceil(deLoadDist / chunkDist[0] * 2 + 1);
        activeChunksDim[1] = (int) Mathf.Ceil(deLoadDist / chunkDist[2] * 2 + 1);
        for (int i1 = 0; i1 < activeChunks.Length; i1++)
        {
            activeChunks[i1] = new ChunkGridNode();
        }

    }

    /// <summary>
    ///     update method loads and deloads chunks
    /// </summary>
    public void update()
    {
        int[] posTmp = new int[2];
        int delta = 0;
        int start = 0;
        //determine current chunkPos
        if (centerObj != null)
        {
            posTmp[0] = Mathf.RoundToInt(centerObj.transform.position.x / chunkDist[0]);
            posTmp[1] = Mathf.RoundToInt(centerObj.transform.position.z / chunkDist[2]);
        }

        //horizontal shift
        if(posTmp[0] != pos[0])
        {
            delta = clamp(posTmp[0] - pos[0], 0, 1);
            
            switch(delta)
            {
                case -1:
                    start = activeChunksDim[0] - 1;
                    break;
                case 1:
                    start = 0;
                    break;
            }

            for(int x = start; rangeCheck(x, -1, activeChunksDim[0]); x+= delta)
            {
                for(int y = 0; y < activeChunksDim[1]; y++)
                {
                    if (x + delta < 0 || x + delta >= activeChunksDim[0])//if out of bounds 
                    {
                        activeChunks[x + y * activeChunksDim[0]].grid = loadChunk(x + delta, y);
                    }
                    else
                    {
                        activeChunks[x + y * activeChunksDim[0]].grid = activeChunks[x + delta + y * activeChunksDim[0]].grid;
                    }
                }
            }
            delta = posTmp[0] - pos[0];
        }

        //vertical shift
        if(posTmp[1] != posTmp[1])
        {
            delta = clamp(posTmp[1] - pos[1], 0, 1);

            switch (delta)
            {
                case -1:
                    start = activeChunksDim[0] - 1;
                    break;
                case 1:
                    start = 0;
                    break;
            }

            for(int y = start; rangeCheck(y, -1, activeChunksDim[1]); y+=delta)
            {
                for(int x  = 0; x < activeChunksDim[0]; x++)
                {
                    if (y + delta < 0 || y + delta >= activeChunksDim[0])//if out of bounds 
                    {
                        activeChunks[x + y * activeChunksDim[0]].grid = loadChunk(x, y + delta);
                    }
                    else
                    {
                        activeChunks[x + y * activeChunksDim[0]].grid = activeChunks[x + (y + delta) * activeChunksDim[0]].grid;
                    }
                }
            }

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
