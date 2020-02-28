using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Linq;

public class groundGen : MonoBehaviour
{

    //declaring variable

    // noise variables
    public int seed = 0;
    System.Random random;

    //chunk
    public List<chunk> chunks = new List<chunk> { };

    //chunk genration settings
    public Vector2 chunksMaxGenration = new Vector2(2, 2);

    //chunk default settings
    public int defaultChunkHeightMapLayers = 3;
    public Vector3 defaultChunkDist = new Vector3(5, 5, 5);
    public Vector3 defaultChunkGridSize = new Vector3(2, 2, 2);
    public int[] defaultChunkSamplesPerCell = new int[] { 4, 4, 4 };
    public double amplitude = 5;
    public double translation = 1;

    //mesh
    public MeshFilter mf;
    public Mesh mesh;
    public List<Vector2> uv = new List<Vector2>();

    public double threshold;

    public bool test = true;


    int kewlKewl(double x, double y, float maxX, float maxY)
    {
        if (x < 0 || y < 0 || x >= maxX || y >= maxY)
        {
            return -1;
        }
        return Convert.ToInt32(y * maxX + x);
    }

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random(seed);

        mf = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;

        for (double y = 0; y < chunksMaxGenration.y; y++)
        {
            for (double x = 0; x < chunksMaxGenration.x; x++)
            {
                chunks.Add
                (
                    new chunk(
                        random.Next(),
                        defaultChunkHeightMapLayers,
                        1,
                        defaultChunkDist,
                        defaultChunkGridSize,
                        new Vector3
                        (
                            Convert.ToSingle(defaultChunkDist.x * x),
                            0,
                            Convert.ToSingle(defaultChunkDist.z * y)
                        ),
                        Convert.ToInt32(x + y * chunksMaxGenration.x),
                        defaultChunkSamplesPerCell,
                        amplitude,
                        translation
                    )
                );

                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[0] = kewlKewl(x - 1, y - 1, chunksMaxGenration.x, chunksMaxGenration.y);
                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[1] = kewlKewl(x, y - 1, chunksMaxGenration.x, chunksMaxGenration.y);
                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[2] = kewlKewl(x + 1, y - 1, chunksMaxGenration.x, chunksMaxGenration.y);

                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[3] = kewlKewl(x - 1, y, chunksMaxGenration.x, chunksMaxGenration.y);
                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[4] = kewlKewl(x + 1, y, chunksMaxGenration.x, chunksMaxGenration.y);

                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[5] = kewlKewl(x - 1, y + 1, chunksMaxGenration.x, chunksMaxGenration.y);
                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[6] = kewlKewl(x, y + 1, chunksMaxGenration.x, chunksMaxGenration.y);
                chunks[Convert.ToInt32(x + y * chunksMaxGenration.x)].neghboringChunks[7] = kewlKewl(x + 1, y + 1, chunksMaxGenration.x, chunksMaxGenration.y);
            }
        }

        foreach (chunk c in chunks)
        {
            c.updateChunkGrids(chunks);
            c.chunkThread.Start();
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
        bool readyCond = chunks.Any(c => c.chunkThread.IsAlive == false);

        if (readyCond && test)
        {
            updateMesh();
        }
        if(chunks.TrueForAll(c => c.chunkThread.IsAlive == false))
        {
            test = false;
        }
    }
    int counter = 0;
    void updateMesh()
    {
        counter += 1;
        Vector3[] verticesTemp = new Vector3[0];
        int[] triangleTemp1;

        int[] triangleTemp2 = new int[0];
        int indexCount = 0;


        for(int i1 = 0; i1 < chunks.Count; i1++)
        {
            if(chunks[i1].vertices != null && chunks[i1].triangles != null)
            {
                try
                {
                    triangleTemp1 = new int[chunks[i1].triangles.Length];

                    for (int i2 = 0; i2 < chunks[i1].triangles.Length; i2++)
                    {
                        triangleTemp1[i2] = chunks[i1].triangles[i2] + indexCount;
                    }

                    triangleTemp2 = triangleTemp2.Concat(triangleTemp1).ToArray();
                    verticesTemp = verticesTemp.Concat(chunks[i1].vertices).ToArray();

                    indexCount = verticesTemp.Length;

                }
                finally
                {

                }

            }
        }

        mesh.Clear();

        mesh.vertices = verticesTemp;
        mesh.triangles = triangleTemp2;

        mesh.RecalculateNormals();
    }
}
