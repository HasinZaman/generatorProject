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

    /// <summary>
    ///     noise class is Noise object used height map generation using one/multiple noise algorithms
    /// </summary>
    class noise : Noise
    {
        Perlin2D perlin2D;

        public noise(float[][] templateVector, int seed, int[] perlinVectorDim)
        {
            perlin2D = new Perlin2D(templateVector, seed, perlinVectorDim);
            Debug.Log($"(0.5, 0.5) = {perlin2D.sample(new float[] { 0.5f, 0.5f })}");
            Debug.Log($"(0.25, 0.75) = {perlin2D.sample(new float[] { 0.25f, 0.75f })}");
            Debug.Log($"(0.1, 0.1) = {perlin2D.sample(new float[] { 0.1f, 0.1f })}");

            //Debug.Log($"(0.9, 0.5) = {perlin2D.sample(new float[] { 0.9f, 0.5f })}");
            //Debug.Log($"(1.1, 0.5) = {perlin2D.sample(new float[] { 1.1f, 0.5f })}");
        }

        public float sample(float[] pos)
        {
            float[] sample = new float[] { pos[0], pos[2]};
            float[] coord = new float[] { pos[3], pos[4], pos[5] };

            float tmp = perlin2D.sample(sample);

            return (tmp * 10f + 1) - coord[1];
        }

        public string toString()
        {
            return perlin2D.toString();
        }
    }

    void Start()
    {
        Noise n = new noise(NoiseVectors.TwoDimensionSet1, 0, new int[] { 3, 3 });

        NoiseHeightMapGenerator noiseHeightMapGenerator = new NoiseHeightMapGenerator(n);
        Debug.Log(n.toString());

        this.generate(noiseHeightMapGenerator);
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
    public void generate(HeightMapGenerator<Grid> HeightMapGenerator)
    {
        NoiseHeightMapGenerator.NoiseParam param = new NoiseHeightMapGenerator.NoiseParam();

        param.height = height;

        param.start = new float[] {0.1f, 0.1f, 0.1f};
        param.end = new float[] { 0.9f, 0.9f, 0.9f };
        param.dim = new int[] { samples[0], height, samples[1] };
        param.height = height;

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
                param.start = new float[] { start[0] + delta[0] * x1, 0, start[1] + delta[1] * y1 };
                param.end = new float[] { start[0] + delta[0] * (x1 + 1), 0, start[1] + delta[1] * (y1 + 1) };

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
