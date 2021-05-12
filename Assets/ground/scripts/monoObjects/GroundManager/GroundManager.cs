using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public ComputeShaderList shaderList;
    public GameObject chunkPrefab;

    Chunk[] chunks;
    int[] chunkDim = new int[2] { 2, 2 };

    float[] nodeDistTemplate = new float[] { 1, 1, 1 };
    int[] gridDim = new int[] { 50, 50, 50 };
    void Start()
    {
        generate();
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

    public void generate()
    {
        GameObject gameObjectTemp;
        Chunk chunkTemp;

        chunks = new Chunk[chunkDim[0] * chunkDim[1]];
        TwoDimensionalNoiseHeightMap[] n = new TwoDimensionalNoiseHeightMap[chunkDim[0] * chunkDim[1]];

        Vector3 pos = new Vector3(0, 0, 0);

        for (int x = 0; x < chunkDim[0]; x++)
        {
            pos.x = x * gridDim[0] * nodeDistTemplate[0];

            for (int z = 0; z < chunkDim[1]; z++)
            {
                n[x + z * chunkDim[0]] = new TwoDimensionalNoiseHeightMap(NoiseVectors.TwoDimensionSet1, 25, gridDim, new uint[] { 5, 5 }, shaderList.Noise);

                pos.z = z * gridDim[2] * nodeDistTemplate[2];

                gameObjectTemp = Instantiate(chunkPrefab, pos, new Quaternion(0, 0, 0, 0), this.transform);
                chunkTemp = gameObjectTemp.GetComponent<Chunk>();

                chunkTemp.setChunk(n[x + z * chunkDim[0]].getHeightMap(), shaderList.MarchingCube, nodeDistTemplate);

                chunks[x + z * chunkDim[0]] = chunkTemp;
            }
        }
    }

    public void load(string file)
}
