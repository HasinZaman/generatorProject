using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class groundGen : MonoBehaviour
{
    //declaring variable

    // noise variables
    public int seed = 0;
    System.Random random;

    //chunk
    public List<chunk> chunks = new List<chunk> { };

    //mesh
    public MeshFilter mf;
    public Mesh mesh;
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> uv = new List<Vector2>();

    public double threshold;
    public GameObject point;
    public bool pointVisible=false;

    public bool test = true;
    public bool linearAproxCond = true;
    

    int kewlKewl(double x, double y, int maxX, int maxY)
    {
        if(x < 0 || y < 0 || x >= maxX || y >= maxY)
        {
            return -1;
        }
        return Convert.ToInt32(y*maxX+x);
    }

    // Start is called before the first frame update
    void Start()
    {
        noise aa = new noise(
                1,
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
                Convert.ToInt32(2),
                Convert.ToInt32(2)
            );

        random = new System.Random(seed);

        mf = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;

        int xMax = 4;
        int yMax = 4;

        for(double y = 0; y < yMax; y++)
        {
            for(double x = 0; x < xMax; x++)
            {
                chunks.Add(new chunk(random.Next(), 3, 5, new Vector3(2, 2, 2), new Vector3(Convert.ToSingle(11.25 * x), 0, Convert.ToSingle(11.25 * y)), Convert.ToInt32(x + y * xMax)));

                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[0] = kewlKewl(x-1,y-1, xMax, yMax);
                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[1] = kewlKewl(x, y - 1, xMax, yMax);
                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[2] = kewlKewl(x + 1, y - 1, xMax, yMax);

                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[3] = kewlKewl(x - 1, y, xMax, yMax);
                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[4] = kewlKewl(x + 1, y, xMax, yMax);

                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[5] = kewlKewl(x - 1, y + 1, xMax, yMax);
                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[6] = kewlKewl(x, y + 1, xMax, yMax);
                chunks[Convert.ToInt32(x + y * xMax)].neghboringChunks[7] = kewlKewl(x + 1, y + 1, xMax, yMax);
            }
        }


        /*noise = new noise(
            seed,
            new double[][] {
                new double[2]{0,1},
                new double[2]{1,1},
                new double[2]{1,0},
                new double[2]{1,-1},
                new double[2]{0,-1},
                new double[2]{-1,-1},
                new double[2]{-1,0},
                new double[2]{-1,1}
                
                new double[3]{1,1,0},
                new double[3]{-1,1,0},
                new double[3]{1,-1,0},
                new double[3]{-1,-1,0},
                new double[3]{1,0,1},
                new double[3]{-1,0,1},
                new double[3]{1,0,-1},
                new double[3]{-1,0,-1},
                new double[3]{0,1,1},
                new double[3]{0,-1,1},
                new double[3]{0,1,-1},
                new double[3]{0,-1,-1},
            },
            8,
            8,
            8
        );*/



    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            test = false;
            genrateGround(4);

        }
    }

    double surfaceCalc(double x1, double x2, double mode, double t)
    {
        if(mode == 0 || x1 == threshold)
        {
            return 0;
        }
        else if(mode == 1 || x2 == threshold)
        {
            return 1;
        }else if(x1 == x2)
        {
            return 1;
        }else if(x1 > x2)
        {
            return surfaceCalc(x2, x1, mode, 1 - t);
        }
        double val = 1 / (Math.Max(x1, x2) - Math.Min(x1, x2)) * (t-Math.Min(x1,x2));
        
        if(val > 1)
        {
            return 1;
        }else if (val < 0)
        {
            return 0;
        }

        return val;
    }

    void genrateGround(int samplesFromCells)
    {
        int[] pointsTemp;
        Vector3 pointTemp;
        marchingCube marching = new marchingCube();

        vertices = new List<Vector3>();
        triangles = new List<int>();

        double[][][] chunkTerrain;

        double dist;

        for (int i1  = 0; i1 < chunks.Count; i1++)
        {
            dist = chunks[i1].dist / samplesFromCells;

            chunkTerrain = chunks[i1].getTerrain(1, new int[] { samplesFromCells, samplesFromCells, samplesFromCells }, 5, 5, chunks[i1].getNeghboringHeightMaps(1,chunks));
            //chunkTerrain = chunks[i1].getTerrain(1, new int[] { samplesFromCells, samplesFromCells, samplesFromCells }, 3, 1);
            
            for (int x = 0; x < chunkTerrain.Length - 1; x++)
            {
                for (int y = 0; y < chunkTerrain[x].Length - 1; y++)
                {
                    for (int z = 0; z <chunkTerrain[x][y].Length - 1; z++)
                    {
                        

                        pointsTemp = marching.getPoint
                        (
                            new double[8]
                            {
                                chunkTerrain[x][y + 1][z + 1],
                                chunkTerrain[x+1][y + 1][z + 1],
                                chunkTerrain[x+1][y + 1][z],
                                chunkTerrain[x][y + 1][z],


                                chunkTerrain[x][y][z + 1],
                                chunkTerrain[x+1][y][z + 1],
                                chunkTerrain[x+1][y][z],
                                chunkTerrain[x][y][z]
                            },
                            threshold
                        );

                        for (int i2 = 0; i2 < pointsTemp.Length; i2++)
                        {
                            pointTemp = marching.points[pointsTemp[i2]];

                            if (linearAproxCond)
                            {
                                pointTemp.x = Convert.ToSingle((surfaceCalc(chunkTerrain[x][y][z], chunkTerrain[x + 1][y][z], pointTemp.x, threshold) + x) * dist + chunks[i1].startingPos.x);
                                
                                pointTemp.y = Convert.ToSingle((surfaceCalc(chunkTerrain[x][y][z], chunkTerrain[x][y + 1][z], pointTemp.y, threshold) + y) * dist + chunks[i1].startingPos.y);
                                
                                pointTemp.z = Convert.ToSingle((surfaceCalc(chunkTerrain[x][y][z], chunkTerrain[x][y][z + 1], pointTemp.z, threshold) + z) * dist + chunks[i1].startingPos.z);
                            }
                            else
                            {
                                pointTemp.x = Convert.ToSingle((pointTemp.x + x) * dist + chunks[i1].startingPos.x);

                                pointTemp.y = Convert.ToSingle((pointTemp.y + y) * dist + chunks[i1].startingPos.y);

                                pointTemp.z = Convert.ToSingle((pointTemp.z + z) * dist + chunks[i1].startingPos.z);
                            }
                            if (vertices.Contains(pointTemp) == false)
                            {
                                vertices.Add(pointTemp);
                                triangles.Add(vertices.Count - 1);
                            }
                            else
                            {
                                triangles.Add(vertices.FindIndex(point => point == pointTemp));
                            }
                        }
                    }
                }
            }
        }
        
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }
}
