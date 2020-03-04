using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class buildingPoint : point
{
    public List<string> pointMaterial = new List<string> { };

    public buildingPoint(double val)
    {
        this.val = val;
    }

    public void addMaterial(string material)
    {
        if (pointMaterial.Contains(material) == false)
        {
            pointMaterial.Add(material);
        }
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
                    buildingMap[x][y][z] = new buildingPoint(-5);
                }
            }
        }

        makeRoom(new int[] { 1, 0, 1 }, new int[] { 4, 3, 4 });
        replaceMaterial(new int[] { 1, 0, 2 }, new int[] { 1, 2, 4 }, "wall", "window");


        replaceMaterial(new int[] { 2, 3, 2 }, new int[] { 3, 3, 3 }, "floor/ceilling", "skyLight");

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
                    buildingMap[Convert.ToInt32(startingPos[0] * samplesPerCell.x + x)][Convert.ToInt32(startingPos[1] * samplesPerCell.y + y)][Convert.ToInt32((roomSpec[2] * i1 + startingPos[2]) * samplesPerCell.z)].addMaterial("wall");
                }
            }

            //yz

            for (int y = 0; y < roomSpec[1] * samplesPerCell.y + 1; y++)
            {
                for (int z = 0; z < roomSpec[2] * samplesPerCell.z + 1; z++)
                {
                    buildingMap[Convert.ToInt32((roomSpec[0] * i1 + startingPos[0]) * samplesPerCell.x)][Convert.ToInt32(startingPos[1] * samplesPerCell.y + y )][Convert.ToInt32(startingPos[2] * samplesPerCell.z + z)].val = 1;
                    buildingMap[Convert.ToInt32((roomSpec[0] * i1 + startingPos[0]) * samplesPerCell.x)][Convert.ToInt32(startingPos[1] * samplesPerCell.y + y)][Convert.ToInt32(startingPos[2] * samplesPerCell.z + z)].addMaterial("wall");
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
                    buildingMap[Convert.ToInt32(startingPos[0] * samplesPerCell.x + x)][Convert.ToInt32((roomSpec[1] * i1 + startingPos[1]) * samplesPerCell.y)][Convert.ToInt32(startingPos[2] * samplesPerCell.z + z)].addMaterial("floor/ceilling");

                }
            }
        }
    }

    public void replaceMaterial(int[] startingPos, int[]endPos, string targetMaterial, string newMaterial)
    {
        for (int x = Convert.ToInt32(startingPos[0] * samplesPerCell.x); x < endPos[0] * samplesPerCell.x + 1; x++)
        {
            for (int y = Convert.ToInt32(startingPos[1] * samplesPerCell.y); y < endPos[1] * samplesPerCell.y + 1; y++)
            {
                for (int z = Convert.ToInt32(startingPos[2] * samplesPerCell.z); z < endPos[2] * samplesPerCell.z + 1; z++)
                {
                    if(buildingMap[x][y][z].pointMaterial.Contains(targetMaterial))
                    {
                        buildingMap[x][y][z].val = 1;
                        buildingMap[x][y][z].pointMaterial.Remove(targetMaterial);
                        buildingMap[x][y][z].addMaterial(newMaterial);
                    }
                }
            }
        }
    }

    double pointCalc(string material1, List<string> material2, double val)
    {
        switch (material1)
        {
            case "shadow":
                if(material2.Any(m2 => new List<string> { "wall", "floor/ceilling", "floor", "ceilling" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "window", "skyLight" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "wall":
                if (material2.Any(m2 => new List<string> { "wall", "window" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling", "skyLight" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "floor":
                if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling", "skyLight" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "wall", "window" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "window":
                if (material2.Any(m2 => new List<string> { "wall", "window" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling", "skyLight" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "skyLight":
                if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling", "skyLight" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "wall", "window" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "ceilling":
                if (material2.Any(m2 => new List<string> {"floor", "ceilling", "floor/ceilling", "skyLight" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "wall", "window" }.Any(m => m == m2)))
                {
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
        List<string> pMaterialTemp;

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
                                    p = buildingMap[x + x1][y + y1][z + z1];
                                    pMaterialTemp = p.pointMaterial.AsEnumerable().ToList();
                                    
                                    if (pMaterialTemp.Contains("floor/ceilling"))
                                    {
                                        pMaterialTemp.Remove("floor/ceilling");
                                        if (y1 == 0)
                                        {
                                            pMaterialTemp.Add("floor");
                                        }
                                        else
                                        {
                                            pMaterialTemp.Add("ceilling");
                                        }
                                    }

                                    for (int i1 = 0; i1 < pMaterialTemp.Count(); i1++)
                                    {
                                        if (cubeMaterials.Contains(pMaterialTemp[i1]) == false)
                                        {
                                            cubeMaterials.Add(pMaterialTemp[i1]);
                                        }
                                    }
                                }
                            }
                        }

                        
                        for (int m = 0; m < cubeMaterials.Count; m++)
                        {

                            
                            cubeVertices.Add(new double[2][][]);
                            if (cubeMaterials[m] == "window" && cubeMaterials.Contains("wall"))
                            {
                                continue;
                            }
                            if (cubeMaterials[m] == "skyLight" && cubeMaterials.Contains("floor/ceilling"))
                            {
                                continue;
                            }
                            for (int x1 = 0; x1 < 2; x1++)
                            {
                                
                                cubeVertices[m][x1] = new double[2][];
                                for (int y1 = 0; y1 < 2; y1++)
                                {
                                    cubeVertices[m][x1][y1] = new double[2];
                                    for (int z1 = 0; z1 < 2; z1++)
                                    {
                                        p = buildingMap[x + x1][y + y1][z + z1];
                                        pMaterialTemp = p.pointMaterial.AsEnumerable().ToList();

                                        if (pMaterialTemp.Contains("floor/ceilling"))
                                        {
                                            pMaterialTemp.Remove("floor/ceilling");
                                            if (y1 == 1)
                                            {
                                                pMaterialTemp.Add("floor");
                                            }
                                            else
                                            {
                                                pMaterialTemp.Add("ceilling");
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
