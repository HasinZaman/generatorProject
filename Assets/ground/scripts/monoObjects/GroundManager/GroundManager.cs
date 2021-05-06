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

        TwoDimensionalNoiseHeightMap n = new TwoDimensionalNoiseHeightMap(NoiseVectors.TwoDimensionSet1, 25, new int[3] { 10,10,10 }, new uint[] { 2, 2 }, shaderList.Noise);
        
        for (int x = 0; x < chunkDim[0]; x++)
        {
            for(int y = 0; y < chunkDim[1]; y++)
            {
                gameObjectTemp = Instantiate(chunkPrefab, this.transform);
                chunkTemp = gameObjectTemp.GetComponent<Chunk>();

                chunkTemp.setChunk(n.getHeightMap(), shaderList.MarchingCube);
            }
        }
    }
}
