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
        public Grid grid;
        public GameObject chunk;
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
        int deltaX = 0;
        int deltaY = 0;
        int startX = 0;
        int startY = 0;

        //determine current chunkPos
        if (centerObj != null)
        {
            posTmp[0] = Mathf.RoundToInt(centerObj.transform.position.x / chunkDist[0]);
            posTmp[1] = Mathf.RoundToInt(centerObj.transform.position.z / chunkDist[2]);
        }

        if(posTmp[0] != pos[0] || posTmp[1] != pos[1])
        {
            deltaX = clamp(posTmp[0] - pos[0], 0, 1);
            deltaY = clamp(posTmp[1] - pos[1], 0, 1);

            switch (deltaX)
            {
                case -1:
                    startX = activeChunksDim[0] - 1;
                    break;
                case 1:
                    startX = 0;
                    break;
            }

            switch (deltaY)
            {
                case -1:
                    startY = activeChunksDim[0] - 1;
                    break;
                case 1:
                    startY = 0;
                    break;
            }

            for (int x = startX; rangeCheck(x, -1, activeChunksDim[0]); x+= deltaX)
            {
                for (int y = startY; rangeCheck(y, -1, activeChunksDim[1]); y += deltaY)
                {
                    if (rangeCheck(x + deltaX, -1, activeChunksDim[0]) || rangeCheck(y + deltaY, -1, activeChunksDim[1]))//if out of bounds
                    {//loadChunk(x + delta, y)
                        activeChunks[x + y * activeChunksDim[0]].grid = null;
                    }
                    else
                    {
                        // update grid 
                        // (new grid is null) AND (Position of chunk is less than or equal do the chunk load distance)
                        if(activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].grid == null && x * x + y * y <= loadDist * loadDist)
                        {
                            activeChunks[x + y * activeChunksDim[0]].grid = loadChunk(x + deltaX, y + deltaY);
                        }
                        else
                        {
                            activeChunks[x + y * activeChunksDim[0]].grid = activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].grid;
                        }

                        if(x * x + y * y > deRenderDist * deRenderDist)//deload check
                        {
                            if (activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].chunk != null)
                            {
                                freeChunks.push(activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].chunk);
                                activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].chunk = null;
                            }
                        }
                        else
                        {
                            if (x * x + y * y <= renderDist * renderDist && activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].chunk == null)//load new chunk check
                            {
                                activeChunks[x + y * activeChunksDim[0]].chunk = freeChunks.pop();
                                activeChunks[x + y * activeChunksDim[0]].chunk.GetComponent<Chunk>().setGrid(activeChunks[x + y * activeChunksDim[0]].grid);
                            }
                            else//move rendered chuncks to new position
                            {
                                activeChunks[x + y * activeChunksDim[0]].chunk = activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].chunk;
                                activeChunks[x + deltaX + (y + startY) * activeChunksDim[0]].chunk = null;
                            }
                        }
                    }
                }
            }

            pos[0] = posTmp[0];
            pos[1] = posTmp[1];
        }
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
