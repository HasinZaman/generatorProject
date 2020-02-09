using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk
{
    public int seed;
    public string id;

    public string[] neghboringChunks;

    public double dist;
    public Vector3 startingPos;
    Vector3 chunkSize;


    public noise[] heightMaps;

    System.Random random;
    
	public chunk(int seed, int heightMapLayer, double dist, Vector3 chunkSize, Vector3 startingPos)
	{
        this.seed = seed;

        this.chunkSize = chunkSize;
        this.startingPos = startingPos;
        this.dist = dist;


        this.random = new System.Random(seed);

        this.heightMaps = new noise[heightMapLayer];

        for (int i1 = 0; i1 < heightMapLayer; i1++)
        {
            this.heightMaps[i1] = new noise(
                this.random.Next(),
                new double[][] {
                    new double[2]{0,1},
                    new double[2]{1,1},
                    new double[2]{1,0},
                    new double[2]{1,-1},
                    new double[2]{0,-1},
                    new double[2]{-1,-1},
                    new double[2]{-1,0},
                    new double[2]{-1,1}
                },
                Convert.ToInt32(chunkSize.x * Math.Pow(2,i1)),
                Convert.ToInt32(chunkSize.y * Math.Pow(2, i1))
            );
        }
    }

    public List<List<List<double[]>>>[] getGrid()
    {
        List<List<List<double[]>>>[] grids = new List<List<List<double[]>>>[heightMaps.Length];

        for (int i1 = 0; i1 < heightMaps.Length; i1++)
        {
            grids[i1] = heightMaps[i1].grid;
        }
        return grids;
    }

    private int chunkZone(int x, int y, int[] samplesPerCell, int[] sampleCount)
    {
        if (x < samplesPerCell[0] / 2 && y < samplesPerCell[1] / 2)
        {
            return 0;
        }
        else if ((samplesPerCell[0] / 2 < x && x < sampleCount[0] - samplesPerCell[0] / 2) && y < samplesPerCell[1] / 2)
        {
            return 1;
        }
        else if (sampleCount[0] - samplesPerCell[0] / 2 < x && y < samplesPerCell[1] / 2)
        {
            return 2;
        }
        else if (sampleCount[0] - samplesPerCell[0] / 2 < x && (samplesPerCell[1] / 2 < y && y < sampleCount[1] - samplesPerCell[1] / 2))
        {
            return 3;
        }
        else if (sampleCount[0] - samplesPerCell[0] / 2 < x && sampleCount[0] - samplesPerCell[0] / 2 < y)
        {
            return 4;
        }
        else if ((samplesPerCell[0] / 2 < x && x < sampleCount[0] - samplesPerCell[0] / 2) && sampleCount[0] - samplesPerCell[0] / 2 < y)
        {
            return 5;
        }
        else if (x < samplesPerCell[0] / 2 && sampleCount[0] - samplesPerCell[0] / 2 < y)
        {
            return 6;
        }
        else if (x < samplesPerCell[0] / 2 && (samplesPerCell[1] / 2 < y && y < sampleCount[1] - samplesPerCell[1] / 2))
        {
            return 7;
        }
        return -1;
    }

    public double[][][] getTerrain(int heightMapLayers, int[] samplesPerCell, double amplitude, List<List<List<double[]>>>[] surroundingChunks)
    {
        double[][][] terrainMap = new double[Convert.ToInt32(samplesPerCell[0] * this.chunkSize.x * Math.Pow(2, heightMapLayers -1)) + samplesPerCell[0] / 2][][];

        int zone;
        double sample;
        double[] offset = new double[]
        {
            0.1/samplesPerCell[0],
            0.1/samplesPerCell[1],
            0.1/samplesPerCell[2]
        };
        double xOffset = 0, zOffset = 0;
        for (int x = 0; x < terrainMap.Length; x++)
        {
            terrainMap[x] = new double[Convert.ToInt32(samplesPerCell[1] * this.chunkSize.y * Math.Pow(2, heightMapLayers - 1)) + samplesPerCell[1] / 2][];
            for (int y = 0; y < terrainMap[x].Length; y++)
            {
                terrainMap[x][y] = new double[Convert.ToInt32(samplesPerCell[2] * this.chunkSize.z * Math.Pow(2, heightMapLayers - 1)) + samplesPerCell[2] / 2];

                for (int z = 0; z < terrainMap[x][y].Length; z++)
                {
                    Debug.Log(x + "," + y + "," + z);

                    zone = chunkZone(x, z, samplesPerCell, new int[] { terrainMap.Length, terrainMap[x].Length, terrainMap[x][y].Length });

                    if (zone == -1)
                    {
                        Debug.Log("core");
                        sample = 0;
                        for (int i1  = 0; i1 <  heightMapLayers; i1++)
                        {
                            sample += this.heightMaps[i1].sample(
                                (x / samplesPerCell[0] - samplesPerCell[0]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[0],
                                (z / samplesPerCell[1] - samplesPerCell[1]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[2]
                                ) * amplitude / Math.Pow(2,heightMapLayers - i1 - 1);
                        }
                        terrainMap[x][y][z] = sample;
                    }
                    else if (surroundingChunks[zone] != null)
                    {
                        Debug.Log("edge");
                        sample = 0;

                        
                        for (int i1 = 0; i1 < heightMapLayers; i1++)
                        {
                            if (zone == 0 || zone == 1 || zone == 2)
                            {
                                zOffset = samplesPerCell[0] / 2;
                            }

                            if (zone == 0 || zone == 7 || zone == 6)
                            {
                                xOffset = samplesPerCell[0] / 2;
                            }
                            sample += this.heightMaps[i1].sampleEdge(
                                surroundingChunks,
                                ((x + xOffset) / samplesPerCell[0] - samplesPerCell[0]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[0],
                                ((z + zOffset) / samplesPerCell[1] - samplesPerCell[1]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[2]
                                ) * amplitude / Math.Pow(2, heightMapLayers - i1 - 1);
                        }
                        terrainMap[x][y][z] = sample;
                    }
                    else
                    {
                        Debug.Log("nothing");
                        terrainMap[x][y][z] = double.NaN;
                    }
                }
            }
        }
        

        return terrainMap;
    }

    public double[][][] getTerrain(int heightMapLayers, int[] samplesPerCell, double amplitude, double translation)
    {
        double[][][] terrainMap = new double[Convert.ToInt32(samplesPerCell[0] * this.chunkSize.x * Math.Pow(2, heightMapLayers - 1))][][];
        
        double sample;
        double[] offset = new double[]
        {
            0.1/samplesPerCell[0],
            0.1/samplesPerCell[1],
            0.1/samplesPerCell[2]
        };
        Debug.Log("samples per cell" + samplesPerCell[0] + "|" + samplesPerCell[1] + "|" + samplesPerCell[2]);
        for (int x = 0; x < terrainMap.Length; x++)
        {
            terrainMap[x] = new double[Convert.ToInt32(samplesPerCell[1] * this.chunkSize.y * Math.Pow(2, heightMapLayers - 1))][];
            for (int y = 0; y < terrainMap[x].Length; y++)
            {
                terrainMap[x][y] = new double[Convert.ToInt32(samplesPerCell[2] * this.chunkSize.z * Math.Pow(2, heightMapLayers - 1))];

                for (int z = 0; z < terrainMap[x][y].Length; z++)
                {
                    Debug.Log("core");
                    Debug.Log(x + "," + y + "," + z);

                    sample = 0;
                    for (int i1 = 0; i1 < heightMapLayers; i1++)
                    {
                        sample += this.heightMaps[i1].sample(
                            (Convert.ToDouble(x) / samplesPerCell[0]/* - samplesPerCell[0]*/) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[0],
                            (Convert.ToDouble(z) / samplesPerCell[2]/* - samplesPerCell[2]*/) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[2]
                            ) * amplitude / Math.Pow(2, heightMapLayers - i1 - 1) + translation;

                        Debug.Log
                        (
                            "x:" + ((Convert.ToDouble(x) / samplesPerCell[0]/* - samplesPerCell[0]*/) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[0]) +
                            "z:" + ((Convert.ToDouble(z) / samplesPerCell[2]/* - samplesPerCell[2]*/) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[2])
                        );
                    }
                    if(sample < y)
                    {
                        terrainMap[x][y][z] = sample - y * amplitude / Math.Pow(2, heightMapLayers - 1) + translation;
                    }
                    else
                    {
                        terrainMap[x][y][z] = 0;
                    }
                    Debug.Log("val:"+(float)terrainMap[x][y][z]);
                }
            }
        }


        return terrainMap;
    }

}
