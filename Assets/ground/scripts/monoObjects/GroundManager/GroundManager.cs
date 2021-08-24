using System;
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
    int[] gridDim = new int[] { 4, 4, 4 };
    void Start()
    {
        TwoDimensionalNoiseHeightMap twoDimensionalNoiseHeightMap = new TwoDimensionalNoiseHeightMap(NoiseVectors.TwoDimensionSet1, 0, new int[] { 3, 3 }, shaderList.Noise);
        Debug.Log(twoDimensionalNoiseHeightMap.toString());

        TwoDimensionalNoiseHeightMap.GridParam param = new TwoDimensionalNoiseHeightMap.GridParam();

        param.setSamples(10, 10);
        param.height = 5;

        param.bias = 1;
        param.amplitude = 3f / 4f;

        float[] start = new float[2] { 0.2f, 0.2f };
        float[] current = new float[2];
        float[] end = new float[2] { 1.8f, 1.8f };
        float[] delta = new float[2];

        GameObject chunk;

        for (int i1 = 0; i1 < 2; i1++)
        {
            current[i1] = start[i1];
            delta[i1] = (end[i1] - start[i1]) / (float) chunkDim[i1];
        }

        chunks = new Chunk[chunkDim[0] * chunkDim[1]];

        for (int x1 = 0; x1 < chunkDim[0]; x1++)
        {
            for(int y1 = 0; y1 < chunkDim[1]; y1++)
            {
                param.setStart(start[0] + delta[0] * x1, start[1] + delta[1] * y1);
                param.setEnd(start[0] + delta[0] * (x1 + 1), start[1] + delta[1] * (y1 + 1));

                chunk = Instantiate(chunkPrefab, this.transform);

                chunk.name = $"({x1},{y1})";
                chunk.transform.position = new Vector3(x1 * nodeDistTemplate[0] * (10 - 1), 0, y1 * nodeDistTemplate[2] * (10 - 1));
                
                chunks[x1 + y1 * chunkDim[0]] = chunk.GetComponent<Chunk>();

                chunks[x1 + y1 * chunkDim[0]].setChunk(twoDimensionalNoiseHeightMap.getHeightMap(param), shaderList.MarchingCube, nodeDistTemplate);
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
        }else if( count < 100)
        {
            count++;
        }
    }

    /// <summary>
    ///     generate method creates the chunks required for the ground
    /// </summary>
    /*
    public void generate()
    {
        GameObject gameObjectTemp;
        Chunk chunkTemp;

        chunks = new Chunk[chunkDim[0] * chunkDim[1]];

        TwoDimensionalNoiseHeightMap[] n = new TwoDimensionalNoiseHeightMap[chunkDim[0] * chunkDim[1]];

        Debug.Log("OG");
        for (int y = 0; y < chunkDim[1]; y++)
        {
            for (int x = 0; x < chunkDim[0]; x++)
            {
                n[x + y * chunkDim[0]] = new TwoDimensionalNoiseHeightMap(NoiseVectors.TwoDimensionSet1, x + y * chunkDim[1] + 1, gridDim, new uint[] { 3, 3 }, shaderList.Noise);
                Debug.Log($"{x},{y}");
                //n[x + y * chunkDim[1]].gridDebug();

                                            //cornerXY
                TwoDimensionalNoiseHeightMap corner00 = null;
                TwoDimensionalNoiseHeightMap corner01 = null;
                TwoDimensionalNoiseHeightMap corner10 = null;
                TwoDimensionalNoiseHeightMap corner11 = n[x + y * chunkDim[0]];
                
                if (x - 1 >= 0)
                {
                    corner01 = n[(x - 1) + y * chunkDim[0]];
                    TwoDimensionalNoiseHeightMap.setVerticalEdge(corner01, corner11);
                }

                if(y - 1 >= 0)
                {
                    corner10 = n[x + (y - 1) * chunkDim[0]];
                    TwoDimensionalNoiseHeightMap.setHorizontalEdge(corner11, corner10);
                }

                if (x - 1 >= 0 && y - 1 >= 0)
                {
                    corner00 = n[(x - 1) + (y - 1) * chunkDim[0]];
                    TwoDimensionalNoiseHeightMap.setCorner(corner00, corner01, corner10, corner11, 0);
                }
            }
        }

        Debug.Log("After Sharing Nodes");
        for (int y = 0; y < chunkDim[1]; y++)
        {
            for (int x = 0; x < chunkDim[0]; x++)
            {
                Debug.Log($"{x},{y}");
                n[x + y * chunkDim[0]].gridDebug();
            }
        }
        Debug.Log("FINAL GRID");

        string tmp = "";
        TwoDimensionalNoiseHeightMap tmpNoise;
        for (int chunkY = chunkDim[1] - 1; chunkY >= 0; chunkY--)
        {
            for (int y = 2; y >= 0; y--)
            {
                for (int chunkX = 0; chunkX < chunkDim[0]; chunkX++)
                {

                    tmpNoise = n[chunkX + chunkY * chunkDim[0]];
                    for (int x = 0; x < 3; x++)
                    {
                        tmp += $"({tmpNoise.perlinNoiseVectors[x + y * 3][0]},{tmpNoise.perlinNoiseVectors[x + y * 3][1]})\t";
                    }
                    tmp += "|\t";
                }
                tmp += "\n";
            }
            for (int chunkX = 0; chunkX < chunkDim[0]; chunkX++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if(x == 1)
                    {
                        tmp += $"[{chunkX},{chunkY}]\t";
                    }
                    else
                    {
                        tmp += "-----\t";
                    }
                }
                tmp += "+\t";
            }
            tmp += "\n";

        }
        Debug.Log(tmp);

        Vector3 pos = new Vector3(0, 0, 0);

        for (int x = 0; x < chunkDim[0]; x++)
        {
            pos.x = x * (gridDim[0] - 1) * nodeDistTemplate[0];

            for (int z = 0; z < chunkDim[1]; z++)
            {
                pos.z = z * (gridDim[2] - 1) * nodeDistTemplate[2];

                gameObjectTemp = Instantiate(chunkPrefab, pos, new Quaternion(0, 0, 0, 0), this.transform);
                gameObjectTemp.name = $"Chunk:{x},{z}";
                chunkTemp = gameObjectTemp.GetComponent<Chunk>();

                chunkTemp.setChunk(n[x + z * chunkDim[0]].getHeightMap(), shaderList.MarchingCube, nodeDistTemplate);

                chunks[x + z * chunkDim[0]] = chunkTemp;
            }
        }
    }*/

    /// <summary>
    ///     Load takes a file and generates chunks
    /// </summary>
    /// <param name="file"></param>
    public void load(string file)
    {
        throw new NotImplementedException();
    }
}
