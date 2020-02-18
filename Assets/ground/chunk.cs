using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chunk
{
    //declaring chunk variables
    public int seed;
    public int id;

    public int[] neghboringChunks = new int[27];

    public double dist;
    public Vector3 startingPos;
    Vector3 chunkSize;


    public noise[] heightMaps;

    System.Random random;
   
	public chunk(int seed, int heightMapLayer, double dist, Vector3 chunkSize, Vector3 startingPos, int id)
	{
        this.seed = seed;

        this.chunkSize = chunkSize;
        this.startingPos = startingPos;
        this.dist = dist;

        this.id = id;


        this.random = new System.Random(seed);

        this.heightMaps = new noise[heightMapLayer];
        
        //creates multiple height maps for added details
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

    //converts surronding chunk ids into chunk height map grids
    public List<List<List<double[]>>>[] getNeghboringHeightMaps(int heightMapLevel, List<chunk> chunkSearchList)
    {
        List<List<List<double[]>>>[] grids = new List<List<List<double[]>>>[8];
        
        for (int i1 = 0; i1 < grids.Length; i1++)
        {
            if (chunkSearchList.Exists(c => c.id == neghboringChunks[i1]))
            {
                grids[i1] = chunkSearchList.Find(c => c.id == neghboringChunks[i1]).heightMaps[heightMapLevel - 1].grid;
            }
            else
            {
                grids[i1] = null;
            }
        }
        return grids;
    }

    //calculates terrian using the chunks height maps
    public double[][][] getTerrain(int heightMapLayers, int[] samplesPerCell, double amplitude, double translation, List<List<List<double[]>>>[] surroundingChunks)
    {
        double[][][] terrainMap = new double[Convert.ToInt32(samplesPerCell[0] * this.chunkSize.x * Math.Pow(2, heightMapLayers -1)) + samplesPerCell[0] / 2][][];
        
        double sample;
        double[] offset;

        double[] sampleCord = new double[2];
        
        for (int x = 0; x < terrainMap.Length; x++)
        {

            terrainMap[x] = new double[Convert.ToInt32(samplesPerCell[1] * this.chunkSize.y * Math.Pow(2, heightMapLayers - 1)) + samplesPerCell[1] / 2][];

            for (int y = 0; y < terrainMap[x].Length; y++)
            {

                terrainMap[x][y] = new double[Convert.ToInt32(samplesPerCell[2] * this.chunkSize.z * Math.Pow(2, heightMapLayers - 1)) + samplesPerCell[2] / 2];
                
                for (int z = 0; z < terrainMap[x][y].Length; z++)
                {
                    if(y == terrainMap[x].Length - 1)
                    {
                        sample = 0;
                    }
                    else
                    {
                        sample = 0;
                        for (int i1 = 0; i1 < heightMapLayers; i1++)
                        {
                            offset = new double[]
                               {
                            0.1/samplesPerCell[0],
                            0.1/samplesPerCell[1],
                            0.1/samplesPerCell[2]
                               };
                            if (x == 0)
                            {
                                sampleCord[0] = -0.5;
                            }
                            else if (x == terrainMap.Length - 1)
                            {
                                sampleCord[0] = Math.Floor((Convert.ToDouble(x - (float)samplesPerCell[0] / 2) / samplesPerCell[0]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[0]) + 0.5;
                            }
                            else
                            {
                                sampleCord[0] = (Convert.ToDouble(x - (float)samplesPerCell[0] / 2) / samplesPerCell[0]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[0];
                            }

                            if (z == 0)
                            {
                                sampleCord[1] = -0.5;
                            }
                            else if (z == terrainMap[x][y].Length - 1)
                            {
                                sampleCord[1] = Math.Floor((Convert.ToDouble(z - (float)samplesPerCell[2] / 2) / samplesPerCell[2]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[2]) + 0.5;
                            }
                            else
                            {
                                sampleCord[1] = (Convert.ToDouble(z - (float)samplesPerCell[2] / 2) / samplesPerCell[2]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[2];
                            }
                            
                            sample += this.heightMaps[i1].sample(
                                sampleCord[0],
                                sampleCord[1],
                                surroundingChunks
                                ) * amplitude / Math.Pow(2, heightMapLayers - i1 - 1) + translation;
                        }
                    }
                    
                    if (sample < y)
                    {
                        terrainMap[x][y][z] = sample - y / Math.Pow(2, heightMapLayers - 1);
                    }
                    else
                    {
                        terrainMap[x][y][z] = 0;
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
        for (int x = 0; x < terrainMap.Length; x++)
        {
            terrainMap[x] = new double[Convert.ToInt32(samplesPerCell[1] * this.chunkSize.y * Math.Pow(2, heightMapLayers - 1))][];
            for (int y = 0; y < terrainMap[x].Length; y++)
            {
                terrainMap[x][y] = new double[Convert.ToInt32(samplesPerCell[2] * this.chunkSize.z * Math.Pow(2, heightMapLayers - 1))];

                for (int z = 0; z < terrainMap[x][y].Length; z++)
                {

                    sample = 0;
                    for (int i1 = 0; i1 < heightMapLayers; i1++)
                    {
                        sample += this.heightMaps[i1].sample(
                            (Convert.ToDouble(x) / samplesPerCell[0]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[0],
                            (Convert.ToDouble(z) / samplesPerCell[2]) * Math.Pow(2, heightMapLayers - 1 - i1) + offset[2]
                            ) * amplitude / Math.Pow(2, heightMapLayers - i1 - 1) + translation;
                        
                    }
                    if(sample < y)
                    {
                        terrainMap[x][y][z] = sample - y * amplitude / Math.Pow(2, heightMapLayers - 1) + translation;
                    }
                    else
                    {
                        terrainMap[x][y][z] = 0;
                    }
                }
            }
        }
        
        return terrainMap;
    }

}
