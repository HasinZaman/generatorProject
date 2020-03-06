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

    public GameObject doorTemplate;

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
        
        makeRoom(new Vector3(1, 0, 1), new Vector3(4, 4, 4), 1, "floor/ceilling", "wall");

        makeWindow(new Vector3(2, 1, 1), new Vector3(4, 3, 1), new Vector3( 0, 0, 1), 1, new string[] { "wall", "floor/ceilling" }, "window");

        makeWindow(new Vector3(1, 4, 1), new Vector3(5, 4, 5), new Vector3(0, 1, 0), 1, new string[] { "floor/ceilling" }, "skyLight");

        meshUpdate();
    }
    
    float convertToMapIndex(double pos, float sampleAxisSize)
    {
        return convertToMapIndex(Convert.ToSingle(pos), sampleAxisSize);
    }
    float convertToMapIndex(int pos, float sampleAxisSize)
    {
        return convertToMapIndex(Convert.ToSingle(pos), sampleAxisSize);
    }
    float convertToMapIndex(float pos, float sampleAxisSize)
    {
        return pos * sampleAxisSize;
    }

    void makeSurface(Vector3 startPos, Vector3 endPos, int val, string material)
    {
        for (int x = Convert.ToInt32(startPos.x); x <= Convert.ToInt32(endPos.x); x++)
        {
            for (int y = Convert.ToInt32(startPos.y); y <= Convert.ToInt32(endPos.y); y++)
            {
                for (int z = Convert.ToInt32(startPos.z); z <= Convert.ToInt32(endPos.z); z++)
                {
                    buildingMap[x][y][z].val = val;
                    buildingMap[x][y][z].addMaterial( material );
                }
            }
        }
    }
    void removeSurface(Vector3 startPos, Vector3 endPos, string[] material)
    {
        for (int x = Convert.ToInt32(startPos.x); x <= Convert.ToInt32(endPos.x); x++)
        {
            for (int y = Convert.ToInt32(startPos.y); y <= Convert.ToInt32(endPos.y); y++)
            {
                for (int z = Convert.ToInt32(startPos.z); z <= Convert.ToInt32(endPos.z); z++)
                {
                    for(int i1 = 0; i1 < material.Count(); i1++)
                    {
                        buildingMap[x][y][z].pointMaterial.Remove(material[i1]);
                    }
                    if(buildingMap[x][y][z].pointMaterial.Count() == 0)
                    {
                        buildingMap[x][y][z].val = 0;
                    }
                }
            }
        }
    }
    float windowOffsetStart(float input)
    {
        if (input == 1)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }
    float windowOffsetEnd(float input)
    {
        if (input == 1)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
    void makeWindow(Vector3 startPos, Vector3 endPos, Vector3 axis ,int wallDepth, string[] targetMaterial, string windowMaterial)
    {
       removeSurface
       (
           new Vector3
            (
                convertToMapIndex(startPos.x, samplesPerCell.x) - wallDepth * windowOffsetStart(axis.x),
                convertToMapIndex(startPos.y, samplesPerCell.y) - wallDepth * windowOffsetStart(axis.y),
                convertToMapIndex(startPos.z, samplesPerCell.z) - wallDepth * windowOffsetStart(axis.z)
            ),
            new Vector3
            (
                convertToMapIndex(endPos.x, samplesPerCell.x) + wallDepth * windowOffsetEnd(axis.x),
                convertToMapIndex(endPos.y, samplesPerCell.y) + wallDepth * windowOffsetEnd(axis.y),
                convertToMapIndex(endPos.z, samplesPerCell.z) + wallDepth * windowOffsetEnd(axis.z)
            ),
           targetMaterial
       );

        makeSurface
        (
            new Vector3
            (
                convertToMapIndex(startPos.x, samplesPerCell.x) - wallDepth * windowOffsetStart(axis.x),
                convertToMapIndex(startPos.y, samplesPerCell.y) - wallDepth * windowOffsetStart(axis.y),
                convertToMapIndex(startPos.z, samplesPerCell.z) - wallDepth * windowOffsetStart(axis.z)
            ),
            new Vector3
            (
                convertToMapIndex(endPos.x, samplesPerCell.x) + wallDepth * windowOffsetEnd(axis.x),
                convertToMapIndex(endPos.y, samplesPerCell.y) + wallDepth * windowOffsetEnd(axis.y),
                convertToMapIndex(endPos.z, samplesPerCell.z) + wallDepth * windowOffsetEnd(axis.z)
            ),
            1,
            windowMaterial
        );
    }
    void makeRoom(Vector3 startPos, Vector3 roomSpec, int wallDepth, string floorMaterialId, string wallMaterialId)
    {
        for (int i1 = 0; i1 < 2; i1++)
        {
            //xy
            makeSurface
            (
                new Vector3
                (
                    convertToMapIndex(startPos.x, samplesPerCell.x),
                    convertToMapIndex(startPos.y, samplesPerCell.y),
                    convertToMapIndex(roomSpec.z * i1 + startPos.z, samplesPerCell.z)
                ),
                new Vector3
                (
                    convertToMapIndex(roomSpec.x + startPos.x, samplesPerCell.x) + 1,
                    convertToMapIndex(roomSpec.y + startPos.y, samplesPerCell.y) + wallDepth,
                    convertToMapIndex(roomSpec.z * i1 + startPos.z, samplesPerCell.z) + wallDepth
                ),
                1,
                wallMaterialId
            );

            //yz
            makeSurface
            (
                new Vector3
                (
                    convertToMapIndex(roomSpec.x * i1 + startPos.x, samplesPerCell.x),
                    convertToMapIndex(startPos.y, samplesPerCell.y),
                    convertToMapIndex(startPos.z, samplesPerCell.z)
                ),
                new Vector3
                (
                    convertToMapIndex(roomSpec.x * i1 + startPos.x, samplesPerCell.x) + wallDepth,
                    convertToMapIndex(roomSpec.y + startPos.y, samplesPerCell.y) + wallDepth,
                    convertToMapIndex(roomSpec.z + startPos.z, samplesPerCell.z) + 1
                ),
                1,
                wallMaterialId
            );
            //xz
            makeSurface
            (
                new Vector3
                (
                    convertToMapIndex(startPos.x, samplesPerCell.x) + wallDepth,
                    convertToMapIndex(roomSpec.y * i1 + startPos.y, samplesPerCell.y),
                    convertToMapIndex(startPos.z, samplesPerCell.z) + wallDepth
                ),
                new Vector3
                (
                    convertToMapIndex(roomSpec.x + startPos.x, samplesPerCell.x) - wallDepth + 1,
                    convertToMapIndex(roomSpec.y * i1 + startPos.y, samplesPerCell.y) + wallDepth,
                    convertToMapIndex(roomSpec.z + startPos.z, samplesPerCell.z) - wallDepth + 1
                ),
                1,
                floorMaterialId
            );
        }
    }

    double pointCalc(string material1, List<string> material2, double val)
    {
        switch (material1)
        {
            case "shadow":
                if (material2.Any(m2 => new List<string> { "window", "skyLight" }.Any(m => m == m2)))
                {
                    return -5;
                }else// if (material2.Any(m2 => new List<string> { "wall", "floor/ceilling", "floor", "ceilling" }.Any(m => m == m2)))
                {
                    return val;
                }
                break;
            case "wall":
                if (material2.Any(m2 => new List<string> { "wall"}.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling",  "skyLight", "window" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "floor":
                if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "wall", "window", "skyLight" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "window":
                if (material2.Any(m2 => new List<string> { "wall" }.Any(m => m == m2)))
                {
                    return 0;
                }
                else if (material2.Any(m2 => new List<string> {"window"}.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling", "skyLight" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "skyLight":
                if (material2.Any(m2 => new List<string> { "floor", "ceilling", "floor/ceilling", "wall" }.Any(m => m == m2)))
                {
                    return 0;
                }else if(material2.Any(m2 => new List<string> { "skyLight" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "wall", "window" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "ceilling":
                if (material2.Any(m2 => new List<string> {"floor", "ceilling", "floor/ceilling" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "wall", "window", "skyLight" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
        }
        return -5;
    }

    void meshUpdate()
    {
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
            for (int y = 0; y < buildingMap[x].Length - 1; y++)
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
                                if(buildingMap[x + x1][y + y1][z + z1].val == 1)
                                {
                                }
                            }
                        }
                    }
                    //shadow
                    pointsTemp = marching.getPoint
                    (
                        cubeVerticesShadow,
                        1
                    );

                    //adds the vertices from pointTemp to the triangle and vertice array
                    for (int i1 = 0; i1 < pointsTemp.Length; i1++)
                    {
                        pointTemp = pointsTemp[i1];

                        //converts the pointTemp form marching cube vertices into global vertices
                        pointTemp.x = Convert.ToSingle((pointTemp.x + x) * distPerSample[0]);

                        pointTemp.y = Convert.ToSingle((pointTemp.y + y) * distPerSample[1]);

                        pointTemp.z = Convert.ToSingle((pointTemp.z + z) * distPerSample[2]);

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

                        //Debug.Log(cubeMaterials.Count);
                        for (int m = 0; m < cubeMaterials.Count; m++)
                        {

                            
                            cubeVertices.Add(new double[2][][]);
                            /*if (cubeMaterials[m] == "window" && cubeMaterials.Contains("wall"))
                            {
                                continue;
                            }
                            if (cubeMaterials[m] == "skyLight" && cubeMaterials.Any(m1 => new List<string> { "floor", "ceilling", "floor/ceilling"}.Any(m2 => m1 == m2)))
                            {
                                continue;
                            }*/
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
                                pointTemp.x = Convert.ToSingle((pointTemp.x + x) * distPerSample[0]);

                                pointTemp.y = Convert.ToSingle((pointTemp.y + y) * distPerSample[1]);

                                pointTemp.z = Convert.ToSingle((pointTemp.z + z) * distPerSample[2]);

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
