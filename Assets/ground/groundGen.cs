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
    public chunk[] chunks;

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

    public int key = 3;


    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random(seed);

        mf = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;
        chunks = new chunk[] 
        {
            new chunk(random.Next(), 3, 5, new Vector3(3,3,3), new Vector3(0,0,0))
        };
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
            genrateGround();

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
        Debug.Log(
           "surface point" +
           " x1:" + x1 +
           " | x2:" + x2 +
           " threshold:" + t +
           " val:" + val
           );
        return val;
    }

    void genrateGround()
    {
        int[] pointsTemp;
        Vector3 pointTemp;
        marchingCube marching = new marchingCube();

        vertices = new List<Vector3>();
        triangles = new List<int>();

        double[][][] chunkTerrain;
        Debug.Log("starting the script bitch");
        Debug.Log(chunks.Length);

        for (int i1  = 0; i1 < chunks.Length; i1++)
        {
            chunkTerrain = chunks[i1].getTerrain(1, new int[] { 5, 5, 5 }, 5, 5);
            Debug.Log(chunkTerrain.Length);
            for (int x = 0; x < chunkTerrain.Length - 1; x++)
            {
                Debug.Log(chunkTerrain[x].Length);
                for (int y = 0; y < chunkTerrain[x].Length - 1; y++)
                {
                    Debug.Log(chunkTerrain[x][y].Length);
                    for (int z = 0; z <chunkTerrain[x][y].Length - 1; z++)
                    {
                        Debug.Log(x + "," + y + "," + z);
                        if (
                            true
                            )
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
                                    Debug.Log("x");
                                    pointTemp.x = Convert.ToSingle(surfaceCalc(chunkTerrain[x][y][z], chunkTerrain[x + 1][y][z], pointTemp.x, threshold) * chunks[i1].dist + chunks[i1].dist * x + chunks[i1].startingPos.x);

                                    Debug.Log("y");
                                    pointTemp.y = Convert.ToSingle(surfaceCalc(chunkTerrain[x][y][z], chunkTerrain[x][y + 1][z], pointTemp.y, threshold) * chunks[i1].dist + chunks[i1].dist * y + chunks[i1].startingPos.y);

                                    Debug.Log("z");
                                    pointTemp.z = Convert.ToSingle(surfaceCalc(chunkTerrain[x][y][z], chunkTerrain[x][y][z + 1], pointTemp.z, threshold) * chunks[i1].dist + chunks[i1].dist * z + chunks[i1].startingPos.z);
                                }
                                else
                                {
                                    pointTemp.x = Convert.ToSingle(pointTemp.x * chunks[i1].dist + chunks[i1].dist * x + chunks[i1].startingPos.x);

                                    pointTemp.y = Convert.ToSingle(pointTemp.y * chunks[i1].dist + chunks[i1].dist * y + chunks[i1].startingPos.y);

                                    pointTemp.z = Convert.ToSingle(pointTemp.z * chunks[i1].dist + chunks[i1].dist * z + chunks[i1].startingPos.z);
                                }
                                if (vertices.Contains(pointTemp) == false)
                                {
                                    //adds to vertice list7
                                    vertices.Add(pointTemp);
                                    triangles.Add(vertices.Count - 1);
                                }
                                else
                                {
                                    //finds verticy and adds
                                    triangles.Add(vertices.FindIndex(point => point == pointTemp));
                                }
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
