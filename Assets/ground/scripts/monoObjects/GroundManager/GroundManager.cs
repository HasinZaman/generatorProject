using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     GroundManager handles Chunk generation and maintenance
/// </summary>
public class GroundManager : MonoBehaviour
{
    public ComputeShaderList shaderList;
    public GameObject chunkPrefab;

    Chunk[] chunks;
    int[] chunkDim = new int[2] { 5, 5 };

    float[] nodeDistTemplate = new float[] { 1, 1, 1 };
    int[] gridDim = new int[] { 4, 4, 4 };

    int[] samples = new int[] {10, 10};
    int height = 10;
    float[] start = new float[2] { 0.2f, 0.2f };
    float[] end = new float[2] { 1.8f, 1.8f };

    void Start()
    {
        TwoDimensionalNoiseHeightMap twoDimensionalNoiseHeightMap = new TwoDimensionalNoiseHeightMap(NoiseVectors.TwoDimensionSet1, 0, new int[] { 3, 3 });
        Debug.Log(twoDimensionalNoiseHeightMap.toString());

        this.generate(twoDimensionalNoiseHeightMap, samples, height, bias, amplitude, start, end);
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
        }else if( count < 100)
        {
            count++;
        }
    }

    /// <summary>
    ///     generate method creates the chunks required for the ground
    /// </summary>
    /// <param name="HeightMapGenerator">HeightMapGenerator object that generates height map</param>
    /// <param name="samplesPerChunk">int array of number samples & nodes along the x and z axis (Unity axis)</param>
    /// <param name="height">int of the number of nodes along the y axis</param>
    /// <param name="bias">A bias used to generate height node values</param>
    /// <param name="amplitude">A amplitude modifier used to generate height node values</param>
    /// <param name="start">float array of the intial sample values</param>
    /// <param name="end">float array of the final sample values</param>
    public void generate(HeightMapGenerator<Grid> HeightMapGenerator, int[] samplesPerChunk, int height, float bias, float amplitude, float[] start, float[] end )
    {
        TwoDimensionalNoiseHeightMap.GridParam param = new TwoDimensionalNoiseHeightMap.GridParam();

        param.setSamples(samplesPerChunk[0], samplesPerChunk[1]);
        param.height = height;

        param.bias = bias;
        param.amplitude = amplitude;

        float[] delta = new float[2];

        GameObject chunk;

        for (int i1 = 0; i1 < 2; i1++)
        {
            delta[i1] = (end[i1] - start[i1]) / (float)chunkDim[i1];
        }

        chunks = new Chunk[chunkDim[0] * chunkDim[1]];

        for (int x1 = 0; x1 < chunkDim[0]; x1++)
        {
            for (int y1 = 0; y1 < chunkDim[1]; y1++)
            {
                param.setStart(start[0] + delta[0] * x1, start[1] + delta[1] * y1);
                param.setEnd(start[0] + delta[0] * (x1 + 1), start[1] + delta[1] * (y1 + 1));

                chunk = Instantiate(chunkPrefab, this.transform);

                chunk.name = $"({x1},{y1})";
                chunk.transform.position = new Vector3(x1 * nodeDistTemplate[0] * (10 - 1), 0, y1 * nodeDistTemplate[2] * (10 - 1));

                chunks[x1 + y1 * chunkDim[0]] = chunk.GetComponent<Chunk>();

                chunks[x1 + y1 * chunkDim[0]].setChunk(HeightMapGenerator.getHeightMap(param), shaderList.MarchingCube, nodeDistTemplate);
            }
        }
    }

    /// <summary>
    ///     Load takes a file and generates chunks
    /// </summary>
    /// <param name="file"></param>
    public void load(string file)
    {
        throw new NotImplementedException();
    }
}
