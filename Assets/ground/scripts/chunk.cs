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

    //sets up the chunk
    public chunk(int seed,int totalHeightMaps, int heightMapLayer, Vector3 dist, Vector3 chunkSize, Vector3 startingPos, int id, int[] samplesPerCell, double amplitude, double translation)
    {
        //assigns the chunk variables
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

        //chunkThread creates a new thread to run beside the main thread and to speed up the script
        chunkThread = new Thread(genrateMesh);
    }

    //updates near by grids
    public void updateChunkGrids(List<chunk> chunkSearchList)//remove function
    {
        neighboringChunksGrids = getNeghboringHeightMaps(chunkSearchList);
    }

    //genrateMesh creates sets up the terrainMap
    public void genrateMesh()
    {
        //declares function variables
        double sample;
        //offset exists to ensure that sample coord is never a whole number
        double[] offset;

        double[] sampleCord = new double[2];

        //sets up the terrainMap
        this.terrainMap = new double[Convert.ToInt32(samplesPerCell[0] * this.chunkSize.x * Math.Pow(2, heightMapLayer - 1)) + samplesPerCell[0]][][];
        for (int x = 0; x < terrainMap.Length; x++)
        {

            terrainMap[x] = new double[Convert.ToInt32(samplesPerCell[1] * this.chunkSize.y * Math.Pow(2, heightMapLayer - 1)) + samplesPerCell[1]][];

            for (int y = 0; y < terrainMap[x].Length; y++)
            {

                terrainMap[x][y] = new double[Convert.ToInt32(samplesPerCell[2] * this.chunkSize.z * Math.Pow(2, heightMapLayer - 1)) + samplesPerCell[2]];

                for (int z = 0; z < terrainMap[x][y].Length; z++)
                {
                    sample = 0;
                    //if y == terrainMap[x].Length - 1 then the function is at the top and sample is set to 0 inorder to ensure the top is rendered
                    if (y != terrainMap[x].Length - 1)
                    {
                        for (int i1 = 0; i1 < heightMapLayer; i1++)
                        {
                            offset = new double[]
                            {
                                0.1/samplesPerCell[0],
                                0.1/samplesPerCell[1],
                                0.1/samplesPerCell[2]
                            };

                            //if any coordinate is at the edge of then the sample coord is assigned inorder to ensure the seems of every chunk align
                            if (x == 0)
                            {
                                sampleCord[0] = -0.5;
                            }
                            else if (x == terrainMap.Length - 1)
                            {
                                sampleCord[0] = Math.Floor((Convert.ToDouble((float)x - (float)samplesPerCell[0]) / samplesPerCell[0]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[0]) + 0.5;
                            }
                            else
                            {
                                sampleCord[0] = (Convert.ToDouble((float)x - (float)samplesPerCell[0]) / samplesPerCell[0]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[0];
                            }

                            if (z == 0)
                            {
                                sampleCord[1] = -0.5;
                            }
                            else if (z == terrainMap[x][y].Length - 1)
                            {
                                sampleCord[1] = Math.Floor((Convert.ToDouble((float)z - (float)samplesPerCell[2]) / samplesPerCell[2]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[2]) + 0.5;
                            }
                            else
                            {
                                sampleCord[1] = (Convert.ToDouble((float)z - (float)samplesPerCell[2]) / samplesPerCell[2]) * Math.Pow(2, heightMapLayer - 1 - i1) + offset[2];
                            }
                            
                            sample += this.heightMaps[i1].sample(
                                sampleCord[0],
                                sampleCord[1],
                                neighboringChunksGrids
                                );
                            
                            if (sample != 0)
                            {
                                sample= sample * amplitude / Math.Pow(2, heightMapLayer - i1 - 1) + translation;
                            }
                        }
                    }
                    terrainMap[x][y][z] = sample - y / Math.Pow(2, heightMapLayer - 1);
                }
            }
        }
        //meshPrep sets up the triangles and vertice arrays inorder to be used in the ground mesh
        meshPrep();
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

    //mesh prep sets up the chunks triangles and vertices
    public void meshPrep()
    {
        //declares function varriables
        Vector3[] pointsTemp;
        Vector3 pointTemp;
        double[][][] cubeVertices;
        marchingCube marching = new marchingCube();

        List<Vector3> verticesTemp = new List<Vector3>();
        List<int> trianglesTemp = new List<int>();


        marching.lerpCond = true;

        //distPerSamples get the distance between samples inorder to ensure chunk has a width, lenght and height that is exactly dist
        Vector3 distPerSample = new Vector3
        (
            dist.x / (terrainMap.Length - 1),
            dist.y / (terrainMap[0].Length - 1),
            dist.z / (terrainMap[0][0].Length - 1)
        );

        //goes through terrainMap and gets the cubes triangles and meshes
        for (int x = 0; x < terrainMap.Length - 1; x++)
        {
            for (int y = 0; y < terrainMap[x].Length - 1; y++)
            {
                for (int z = 0; z < terrainMap[x][y].Length - 1; z++)
                {
                    //gets the vertices for the current cube
                    cubeVertices = new double[2][][];
                    for(int x1 = 0; x1 < 2; x1++)
                    {
                        cubeVertices[x1] = new double[2][];
                        for(int y1 = 0; y1 < 2; y1++)
                        {
                            cubeVertices[x1][y1] = new double[2];
                            for(int z1 = 0; z1 < 2; z1++)
                            {
                                cubeVertices[x1][y1][z1] = terrainMap[x + x1][y + y1][z + z1];
                            }
                        }
                    }

                    //gets the vertices for the current cube
                    pointsTemp = marching.getPoint
                    (
                        cubeVertices,
                        threshold
                    );

                    //adds the vertices from pointTemp to the triangle and vertice array
                    for (int i1 = 0; i1 < pointsTemp.Length; i1++)
                    {
                        pointTemp = pointsTemp[i1];

                        //converts the pointTemp form marching cube vertices into global vertices
                        pointTemp.x = Convert.ToSingle((pointTemp.x + x) * distPerSample[0] + startingPos.x);

                        pointTemp.y = Convert.ToSingle((pointTemp.y + y) * distPerSample[1] + startingPos.y);

                        pointTemp.z = Convert.ToSingle((pointTemp.z + z) * distPerSample[2] + startingPos.z);
                        
                        //checks if the vertice exists in the  vertice array
                        //if the vertice was found then the vertice would be shared rather than a new one being made
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
    