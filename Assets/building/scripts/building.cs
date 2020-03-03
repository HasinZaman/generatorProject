using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class buildingPoint : point
{
    public string pointMaterial;

    public buildingPoint(double val, string pointMaterial)
    {
        this.val = val;
        this.pointMaterial = pointMaterial;
    }
}

public class building : MonoBehaviour
{
    //declare object variables
    public buildingPoint[][][] buildingMap;

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
    
    public Material[] materials;
    public string[] materialId;

    public Renderer renderer;
    
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

        renderer.materials = this.materials;

        //sets up buildingMap
        buildingMap = new buildingPoint[Convert.ToInt32(buildingCellDim.x * samplesPerCell.x)][][];
        for (int x = 0; x < buildingMap.Length; x++)
        {
            buildingMap[x] = new buildingPoint[Convert.ToInt32(buildingCellDim.y * samplesPerCell.y)][];
            for(int y = 0; y < buildingMap[x].Length; y++)
            {
                buildingMap[x][y] = new buildingPoint[Convert.ToInt32(buildingCellDim.z * samplesPerCell.z)];
                for(int z = 0; z < buildingMap[x][y].Length; z++)
                {
                    buildingMap[x][y][z] = new buildingPoint(-5, "");
                }
            }
        }

        makeRoom(new int[] { 1, 1, 1 }, new int[] { 2, 2, 4 });

        meshUpdate();
    }

    void makeRoom(int[] startingPos, int[] roomSpec)
    {
        for(int i1 = 0; i1 < 2; i1++)
        {
            //xy

            for (int x = 0; x < roomSpec[0] * samplesPerCell.x + 1; x++)
            {
                for (int y = 0; y < roomSpec[1] * samplesPerCell.y + 1; y++)
                {
                    buildingMap[Convert.ToInt32(startingPos[0] * samplesPerCell.x + x)][Convert.ToInt32(startingPos[1] * samplesPerCell.y + y )][Convert.ToInt32((roomSpec[2] * i1 + startingPos[2]) * samplesPerCell.z)].val = 1;
                    buildingMap[Convert.ToInt32(startingPos[0] * samplesPerCell.x + x)][Convert.ToInt32(startingPos[1] * samplesPerCell.y + y)][Convert.ToInt32((roomSpec[2] * i1 + startingPos[2]) * samplesPerCell.z)].pointMaterial = "wall";
                }
            }

            //yz

            for (int y = 0; y < roomSpec[1] * samplesPerCell.y + 1; y++)
            {
                for (int z = 0; z < roomSpec[2] * samplesPerCell.z + 1; z++)
                {
                    buildingMap[Convert.ToInt32((roomSpec[0] * i1 + startingPos[0]) * samplesPerCell.x)][Convert.ToInt32(startingPos[1] * samplesPerCell.y + y )][Convert.ToInt32(startingPos[2] * samplesPerCell.z + z)].val = 1;
                    buildingMap[Convert.ToInt32((roomSpec[0] * i1 + startingPos[0]) * samplesPerCell.x)][Convert.ToInt32(startingPos[1] * samplesPerCell.y + y)][Convert.ToInt32(startingPos[2] * samplesPerCell.z + z)].pointMaterial = "wall";
                }
            }
        }
        for(int i1 = 0; i1 < 2; i1++)
        {
            //xz
            for (int x = 0; x < roomSpec[0] * samplesPerCell.x + 1; x++)
            {
                for (int z = 0; z < roomSpec[2] * samplesPerCell.z + 1; z++)
                {
                    buildingMap[Convert.ToInt32(startingPos[0] * samplesPerCell.x + x)][Convert.ToInt32((roomSpec[1] * i1 + startingPos[1]) * samplesPerCell.y)][Convert.ToInt32(startingPos[2] * samplesPerCell.z + z)].val = 1;
                    buildingMap[Convert.ToInt32(startingPos[0] * samplesPerCell.x + x)][Convert.ToInt32((roomSpec[1] * i1 + startingPos[1]) * samplesPerCell.y)][Convert.ToInt32(startingPos[2] * samplesPerCell.z + z)].pointMaterial = "floor/ceilling";

                }
            }
        }
    }

    double pointCalc(string material1, string material2, double val)
    {
        

        switch (material1)
        {
            case "shadow":
                switch (material2)
                {
                    case "wall": case "floor/ceilling":
                        return val;
                    case "": case "window":
                        return -5;
                }
                break;
            case "wall":
                switch (material2)
                {
                    case "wall": case "floor": case "ceilling": case "floor/ceilling":
                        return val;
                    case "": case "window":
                        return -5;
                }
                break;
            case "floor":
                switch (material2)
                {
                    case "floor": case "ceilling":
                        return val;
                    case "wall": case "window": case "":
                        return -5;
                }
                break;
            case "window":
                switch (material2)
                {
                    case "wall": case "floor": case "ceilling": case "window":
                        return val;
                    case "":
                        return -5;
                }
                break;
            case "ceilling":
                switch (material2)
                {
                    case "floor": case "ceilling":
                        return val;
                    case "wall": case "window": case "":
                        return -5;
                }
                break;
        }
        return -5;
    }

    void meshUpdate()
    {
        //new method
        Vector3[] pointsTemp;
        Vector3 pointTemp;

        buildingPoint p;
        string pMaterialTemp;

        List<Vector3> meshVertices = new List<Vector3>();
        List<int>[] subMeshTriangles = new List<int>[materials.Length];

        for(int i1 = 0; i1 < subMeshTriangles.Count(); i1++)
        {
            subMeshTriangles[i1] = new List<int>();
        }
        
        List<Vector3> verticesShadowTemp = new List<Vector3>();
        List<int> trianglesShadowTemp = new List<int>();
        double[][][] cubeVerticesShadow;


        List<double[][][]> cubeVertices;
        List<string> cubeMaterials;


        Vector3 distPerSample = new Vector3
        (
            cellDim.x / samplesPerCell.x,
            cellDim.y / samplesPerCell.y,
            cellDim.z / samplesPerCell.z
        );

        for (int x = 0; x < buildingMap.Length - 1; x++)
        {
            for (int y = 0; y < Math.Min(buildingMap[x].Length - 1, (level + 1) * samplesPerCell.y); y++)
            {
                for (int z = 0; z < buildingMap[x][y].Length - 1; z++)
                {
                    cubeVerticesShadow = new double[2][][];
                    for (int x1 = 0; x1 < 2; x1++)
                    {
                        cubeVerticesShadow[x1] = new double[2][];
                        for (int y1 = 0; y1 < 2; y1++)
                        {
                            cubeVerticesShadow[x1][y1] = new double[2];
                            for (int z1 = 0; z1 < 2; z1++)
                            {
                                cubeVerticesShadow[x1][y1][z1] = pointCalc("shadow", buildingMap[x + x1][y + y1][z + z1].pointMaterial, buildingMap[x + x1][y + y1][z + z1].val);
                            }
                        }
                    }
                    //shadow
                    pointsTemp = marching.getPoint
                    (
                        cubeVerticesShadow,
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
                        if (verticesShadowTemp.Contains(pointTemp) == false)
                        {
                            verticesShadowTemp.Add(pointTemp);
                            trianglesShadowTemp.Add(verticesShadowTemp.Count - 1);
                        }
                        else
                        {
                            trianglesShadowTemp.Add(verticesShadowTemp.FindIndex(point => point == pointTemp));
                        }
                    }

                    cubeVertices = new List<double[][][]>();
                    cubeMaterials = new List<string> {};
                    if (y < level * samplesPerCell.y)
                    {
                        for (int x1 = 0; x1 < 2; x1++)
                        {
                            for (int y1 = 0; y1 < 2; y1++)
                            {
                                for (int z1 = 0; z1 < 2; z1++)
                                {
                                    pMaterialTemp = buildingMap[x + x1][y + y1][z + z1].pointMaterial;

                                    if (pMaterialTemp == "floor/ceilling")
                                    {
                                        if (y1 == 0)
                                        {
                                            pMaterialTemp = "floor";
                                        }
                                        else
                                        {
                                            pMaterialTemp = "ceilling";
                                        }
                                    }

                                    if (cubeMaterials.Contains(pMaterialTemp) == false && pMaterialTemp != "")
                                    {
                                        cubeMaterials.Add(pMaterialTemp);
                                    }
                                }
                            }
                        }

                        for (int m = 0; m < cubeMaterials.Count; m++)
                        {
                            cubeVertices.Add(new double[2][][]);
                            for (int x1 = 0; x1 < 2; x1++)
                            {
                                cubeVertices[m][x1] = new double[2][];
                                for (int y1 = 0; y1 < 2; y1++)
                                {
                                    cubeVertices[m][x1][y1] = new double[2];
                                    for (int z1 = 0; z1 < 2; z1++)
                                    {
                                        p = buildingMap[x + x1][y + y1][z + z1];
                                        pMaterialTemp = p.pointMaterial;

                                        if (pMaterialTemp == "floor/ceilling")
                                        {
                                            if(y1 == 0)
                                            {
                                                pMaterialTemp = "floor";
                                            }
                                            else
                                            {
                                                pMaterialTemp = "ceilling";
                                            }
                                        }
                                        
                                        if (y + y1 == level * samplesPerCell.y)
                                        {
                                            cubeVertices[m][x1][y1][z1] = -1;
                                        }
                                        else
                                        {
                                            cubeVertices[m][x1][y1][z1] = pointCalc(cubeMaterials[m], pMaterialTemp, p.val);
                                        }

                                    }
                                }
                            }

                            pointsTemp = marching.getPoint
                            (
                                cubeVertices[m],
                                0
                            );
                            
                            for (int i1 = 0; i1 < pointsTemp.Count(); i1++)
                            {
                                pointTemp = pointsTemp[i1];

                                //converts the pointTemp form marching cube vertices into global vertices
                                pointTemp.x = Convert.ToSingle((pointTemp.x + x) * distPerSample[0] + startingPos.x);

                                pointTemp.y = Convert.ToSingle((pointTemp.y + y) * distPerSample[1] + startingPos.y);

                                pointTemp.z = Convert.ToSingle((pointTemp.z + z) * distPerSample[2] + startingPos.z);

                                //checks if the vertice exists in the  vertice array
                                //if the vertice was found then the vertice would be shared rather than a new one being made
                                if (meshVertices.Contains(pointTemp) == false)
                                {
                                    meshVertices.Add(pointTemp);
                                    subMeshTriangles[Array.IndexOf(materialId, cubeMaterials[m])].Add(meshVertices.Count - 1);
                                }
                                else
                                {
                                    subMeshTriangles[Array.IndexOf(materialId, cubeMaterials[m])].Add(meshVertices.FindIndex(point => point == pointTemp));
                                }

                            }
                        }
                    }
                }
            }
        }


        
        mesh.Clear();

        mesh.subMeshCount = subMeshTriangles.Count();

        this.vertices = meshVertices.ToArray();

        mesh.vertices = this.vertices;
        for (int i1 = 0; i1 < subMeshTriangles.Count(); i1++)
        {
            this.triangles = this.triangles.Concat(subMeshTriangles[i1]).ToArray();
        }

        for (int i1 = 0; i1 < subMeshTriangles.Count(); i1++)
        {
            if (subMeshTriangles[i1].Count > 0)
            {
                mesh.SetTriangles(subMeshTriangles[i1].ToArray(), i1);

            }
        }

        mesh.RecalculateNormals();


        buildingShadowMesh.Clear();

        buildingShadowMesh.vertices = verticesShadowTemp.ToArray();
        buildingShadowMesh.triangles = trianglesShadowTemp.ToArray();
        buildingShadowMesh.RecalculateNormals();
        
    }

    // Update is called once per frame
    void Update()
    {

        //meshUpdate();
    }
}
