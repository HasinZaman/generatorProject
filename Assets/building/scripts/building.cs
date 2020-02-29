using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class building : MonoBehaviour
{
    //declare object variables
    public double[][][] buildingMap;

    public Vector3 startingPos;

    public Vector3 buildingCellDim;
    public Vector3 samplesPerCell;
    public Vector3 cellDim;
    
    public Vector3[] vertices;
    public int[] triangles;

    public double level = 0;

    marchingCube marching = new marchingCube();

    public Mesh mesh;
    public MeshFilter mf;
    
    private Mesh buildingShadowMesh;
    private MeshFilter buildingShadowMF;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        buildingShadowMesh = new Mesh();
        mf.mesh = mesh;

        buildingShadowMF = transform.GetChild(0).GetComponent<MeshFilter>();
        buildingShadowMF.mesh = buildingShadowMesh;

        marching.lerpCond = true;

        //sets up buildingMap
        buildingMap = new double[Convert.ToInt32(samplesPerCell.x * samplesPerCell.x)][][];
        for (int x = 0; x < buildingMap.Length; x++)
        {
            buildingMap[x] = new double[Convert.ToInt32(samplesPerCell.y * samplesPerCell.y)][];
            for(int y = 0; y < buildingMap[x].Length; y++)
            {
                buildingMap[x][y] = new double[Convert.ToInt32(samplesPerCell.z * samplesPerCell.z)];
                for(int z = 0; z < buildingMap[x][y].Length; z++)
                {
                    buildingMap[x][y][z] = -1;
                }
            }
        }

        makeRoom(new int[] { 1, 1, 1 }, new int[] { 2, 2, 2 });

        meshUpdate();
    }

    void makeRoom(int[] startingPos, int[] roomSpec)
    {
        for(int i1 = 0; i1 < 2; i1++)
        {
            //xz
            for(int x = 0; x < roomSpec[0] * samplesPerCell.x + 1; x++)
            {
                for(int z = 0; z < roomSpec[2] * samplesPerCell.z + 1; z++)
                {
                    buildingMap[startingPos[0] + x][Convert.ToInt32(roomSpec[1] * samplesPerCell.y * i1 + startingPos[1])][startingPos[2] + z] = 1;
                }
            }

            //xy
            for (int x = 0; x < roomSpec[0] * samplesPerCell.x + 1; x++)
            {
                for (int y = 0; y < roomSpec[1] * samplesPerCell.y + 1; y++)
                {
                    buildingMap[startingPos[0] + x][startingPos[1] + y][Convert.ToInt32(roomSpec[2] * samplesPerCell.z * i1 + startingPos[2])] = 1;
                }
            }
            //yz

            for (int y = 0; y < roomSpec[1] * samplesPerCell.y + 1; y++)
            {
                for (int z = 0; z < roomSpec[2] * samplesPerCell.z + 1; z++)
                {
                    buildingMap[Convert.ToInt32(roomSpec[0] * samplesPerCell.x * i1 + startingPos[0])][startingPos[1] + y][startingPos[2] + z] = 1;
                }
            }
        }
    }

    void meshUpdate()
    {
        Vector3[] pointsTemp;
        Vector3 pointTemp;
        double[][][] cubeVertices;
        
        Vector3 distPerSample = new Vector3
        (
            cellDim.x / samplesPerCell.x,
            cellDim.y / samplesPerCell.y,
            cellDim.z / samplesPerCell.z
        );
        List<Vector3> verticesTemp = new List<Vector3>();
        List<int> trianglesTemp = new List<int>();

        bool meshUpdateCond = true;

        for (int x = 0; x < buildingMap.Length - 1; x++)
        {
            //stop level * samplesPerCell.y
            for (int y = 0; y < Math.Min(buildingMap[x].Length - 1, level * samplesPerCell.y); y++)
            {
                for(int z = 0; z < buildingMap[x][y].Length - 1; z++)
                {

                    cubeVertices = new double[2][][];
                    for (int x1 = 0; x1< 2; x1++)
                    {
                        cubeVertices[x1] = new double[2][];
                        for (int y1 = 0; y1 < 2; y1++)
                        {
                            cubeVertices[x1][y1] = new double[2];
                            for (int z1 = 0; z1 <2; z1++)
                            {
                                if(y + y1 == Math.Min(buildingMap[x].Length - 1, level * samplesPerCell.y))
                                {
                                    cubeVertices[x1][y1][z1] = -1;
                                }
                                else
                                {
                                    cubeVertices[x1][y1][z1] = buildingMap[x + x1][y + y1][z + z1];
                                }
                            }
                        }
                    }

                    //gets the vertices for the current cube
                    pointsTemp = marching.getPoint
                    (
                        cubeVertices,
                        0
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

        mesh.Clear();

        mesh.vertices = this.vertices;
        mesh.triangles = this.triangles;

        mesh.RecalculateNormals();

        meshUpdateCond = false;

        for (int x = 0; x < buildingMap.Length - 1; x++)
        {
            //stop level * samplesPerCell.y
            for (int y = Convert.ToInt32(Math.Min(buildingMap[x].Length - 1, level * samplesPerCell.y)) - 1; y < buildingMap[x].Length - 1; y++)
            {
                for (int z = 0; z < buildingMap[x][y].Length - 1; z++)
                {

                    cubeVertices = new double[2][][];
                    for (int x1 = 0; x1 < 2; x1++)
                    {
                        cubeVertices[x1] = new double[2][];
                        for (int y1 = 0; y1 < 2; y1++)
                        {
                            cubeVertices[x1][y1] = new double[2];
                            for (int z1 = 0; z1 < 2; z1++)
                            {
                                if (y + y1 == Math.Min(buildingMap[x].Length - 1, level * samplesPerCell.y))
                                {
                                    cubeVertices[x1][y1][z1] = -1;
                                }
                                else
                                {
                                    cubeVertices[x1][y1][z1] = buildingMap[x + x1][y + y1][z + z1];
                                }
                            }
                        }
                    }

                    //gets the vertices for the current cube
                    pointsTemp = marching.getPoint
                    (
                        cubeVertices,
                        0
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

        buildingShadowMesh.Clear();

        buildingShadowMesh.vertices = verticesTemp.ToArray();
        buildingShadowMesh.triangles = trianglesTemp.ToArray();
        buildingShadowMesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {

        //meshUpdate();
    }
}
