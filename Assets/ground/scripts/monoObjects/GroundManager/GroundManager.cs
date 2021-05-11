using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public ComputeShaderList shaderList;
    public GameObject chunkPrefab;

    Chunk[] chunks;
    int[] chunkDim = new int[2] { 1, 1 };

    void Start()
    {
        GameObject gameObjectTemp;
        Chunk chunkTemp;

        chunks = new Chunk[chunkDim[0] * chunkDim[1]];
        TwoDimensionalNoiseHeightMap n = new TwoDimensionalNoiseHeightMap(NoiseVectors.TwoDimensionSet1, 25, new int[3] { 50, 50, 50 }, new uint[] { 5, 5 }, shaderList.Noise);
        
        for (int x = 0; x < chunkDim[0]; x++)
        {
            for(int y = 0; y < chunkDim[1]; y++)
            {
                gameObjectTemp = Instantiate(chunkPrefab, new Vector3( x, y, 0), new Quaternion(0,0,0,0), this.transform);
                chunkTemp = gameObjectTemp.GetComponent<Chunk>();

                chunkTemp.setChunk(n.getHeightMap(), shaderList.MarchingCube, new float[] { 1, 1, 1 });

                chunks[x + y * chunkDim[0]] = chunkTemp;
            }
        }
    }

    private int count = 0;

    private void Update()
    {
        if(count == 100)
        {
            for (int i1 = 0; i1 < chunks.Length; i1++)
            {
                chunks[i1].updateMesh();
            }

            count++;
        }
        else
        {
            count++;
        }
    }
}
