using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class chunk
{
    //declaring chunk variables
    public int seed;
    public int id;

    public int[] neghboringChunks = new int[27];
    public double[][][] terrainMap;

    public Vector3 dist;
    public Vector3 startingPos;
    Vector3 chunkSize;

    public Vector3[] vertices;
    public int[] triangles;

    List<List<List<double[]>>>[] neighboringChunksGrids;
    int heightMapLayer;
    int[] samplesPerCell;
    double amplitude, translation;

    public double threshold = 0;

    public Thread chunkThread;

    public noise[] heightMaps;

    System.Random random;

    public chunk(int seed,int totalHeightMaps, int heightMapLayer, Vector3 dist, Vector3 chunkSize, Vector3 startingPos, int id, int[] samplesPerCell, double amplitude, double translation)
    {
        this.seed = seed;

        this.chunkSize = chunkSize;
        this.startingPos = startingPos;
        this.dist = dist;

        this.id = id;

        this.random = new System.Random(seed);

        this.heightMaps = new noise[totalHeightMaps];

        this.heightMapLayer = heightMapLayer;
        this.samplesPerCell = samplesPerCell;
        this.amplitude = amplitude;
        this.translation = translation;

        //creates multiple height maps for added details
        for (int i1 = 0; i1 < totalHeightMaps; i1++)
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
                Convert.ToInt32(chunkSize.x * Math.Pow(2, i1)),
                Convert.ToInt32(chunkSize.y * Math.Pow(2, i1))
            );
        }
       chunkThread = new Thread(genrateMesh);
    }

    public void updateChunkGrids(List<chunk> chunkSearchList)
    {
        neighboringChunksGrids = getNeghboringHeightMaps(chunkSearchList);
    }

    public void genrateMesh()
    {
        this.terrainMap = new double[Convert.ToInt32(samplesPerCell[0] * this.chunkSize.x * Math.Pow(2, heightMapLayer - 1)) + samplesPerCell[0] / 2][][];

        double sample;
        double[] offset;

        double[] sampleCord = new double[2];

        for (int x = 0; x < terrainMap.Length; x++)
        {

            terrainMap[x] = new double[Convert.ToInt32(samplesPerCell[1] * this.chunkSize.y * Math.Pow(2, heightMapLayer - 1)) + samplesPerCell[1] / 2][];

            for (int y = 0; y < terrainMap[x].Length; y++)
            {

                terrainMap[x][y] = new double[Convert.ToInt32(samplesPerCell[2] * this.chunkSize.z * Math.Pow(2, heightMapLayer - 1)) + samplesPerCell[2] / 2];

                for (int z = 0; z < terrainMap[x][y].Length; z++)
                {
                    if (y == terrainMap[x].Length - 1)
                    {
                        sample = 0;
                    }
                    else
                    {
                        sample = 0;
                        for (int i1 = 0; i1 < heightMapLayer; i1++)
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
                                sampleCord[0] = Math.Floor((Convert.ToDouble(x - (float)samplesPerCell[0] / 2) / samplesPerCell[0]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[0]) + 0.5;
                            }
                            else
                            {
                                sampleCord[0] = (Convert.ToDouble(x - (float)samplesPerCell[0] / 2) / samplesPerCell[0]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[0];
                            }

                            if (z == 0)
                            {
                                sampleCord[1] = -0.5;
                            }
                            else if (z == terrainMap[x][y].Length - 1)
                            {
                                sampleCord[1] = Math.Floor((Convert.ToDouble(z - (float)samplesPerCell[2] / 2) / samplesPerCell[2]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[2]) + 0.5;
                            }
                            else
                            {
                                sampleCord[1] = (Convert.ToDouble(z - (float)samplesPerCell[2] / 2) / samplesPerCell[2]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[2];
                            }

                            sample += this.heightMaps[i1].sample(
                                sampleCord[0],
                                sampleCord[1],
                                neighboringChunksGrids
                                ) * amplitude / Math.Pow(2, heightMapLayer - i1 - 1) + translation;
                        }
                    }

                    if (sample < y)
                    {
                        terrainMap[x][y][z] = sample - y / Math.Pow(2, heightMapLayer - 1);
                    }
                    else
                    {
                        terrainMap[x][y][z] = 0;
                    }
                }
            }
        }
        meshPrep();
        Debug.Log("done");
    }

    //converts surronding chunk ids into chunk height map grids
    private List<List<List<double[]>>>[] getNeghboringHeightMaps(List<chunk> chunkSearchList)
    {
        List<List<List<double[]>>>[] grids = new List<List<List<double[]>>>[8];

        for (int i1 = 0; i1 < grids.Length; i1++)
        {
            if (chunkSearchList.Exists(c => c.id == neghboringChunks[i1]))
            {
                grids[i1] = chunkSearchList.Find(c => c.id == neghboringChunks[i1]).heightMaps[heightMapLayer - 1].grid;
            }
            else
            {
                grids[i1] = null;
            }
        }
        return grids;
    }

    public void meshPrep()
    {
        int[] pointsTemp;
        Vector3 pointTemp;
        marchingCube marching = new marchingCube();

        List<Vector3> verticesTemp = new List<Vector3>();
        List<int> trianglesTemp = new List<int>();
        
        //chunkTerrain = chunks[i1].getTerrain(1, new int[] { samplesFromCells, samplesFromCells, samplesFromCells }, 3, 1);

        Vector3 distPerSample = new Vector3
        (
            dist.x / (terrainMap.Length - 1),
            dist.y / (terrainMap[0].Length - 1),
            dist.z / (terrainMap[0][0].Length - 1)
        );

        for (int x = 0; x < terrainMap.Length - 1; x++)
        {
            for (int y = 0; y < terrainMap[x].Length - 1; y++)
            {
                for (int z = 0; z < terrainMap[x][y].Length - 1; z++)
                {


                    pointsTemp = marching.getPoint
                    (
                        new double[8]
                        {
                            terrainMap[x][y + 1][z + 1],
                            terrainMap[x+1][y + 1][z + 1],
                            terrainMap[x+1][y + 1][z],
                            terrainMap[x][y + 1][z],


                            terrainMap[x][y][z + 1],
                            terrainMap[x+1][y][z + 1],
                            terrainMap[x+1][y][z],
                            terrainMap[x][y][z]
                        },
                        threshold
                    );

                    for (int i2 = 0; i2 < pointsTemp.Length; i2++)
                    {
                        pointTemp = marching.points[pointsTemp[i2]];

                        pointTemp.x = Convert.ToSingle((pointTemp.x + x) * distPerSample[0] + startingPos.x);

                        pointTemp.y = Convert.ToSingle((pointTemp.y + y) * distPerSample[1] + startingPos.y);

                        pointTemp.z = Convert.ToSingle((pointTemp.z + z) * distPerSample[2] + startingPos.z);
                        
                        if (verticesTemp.Contains(pointTemp) == false)
                        {
                            verticesTemp.Add(pointTemp);
                            trianglesTemp.Add(verticesTemp.Count - 1);
                        }
                        else
                        {
                            trianglesTemp.Add(verticesTemp.FindIndex(point => point == pointTemp));
                        }
                    }
                }
            }
        }

        this.vertices = verticesTemp.ToArray();
        this.triangles = trianglesTemp.ToArray();

    }
}
    