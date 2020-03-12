using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class door : MonoBehaviour
{
    public buildingPoint[][][] buildingMap;

    public Vector3 samplesPerCell;
    public Vector3 cellDim;
    
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    [Header("door angles")]
    public int rotationAxis;
    public float[] angleRange;
    
    public float curentAngle;
    public float targetAngle;
    public float rotationSpeed;
    
    private marchingCube marching = new marchingCube();

    public Material[] materials;
    public string[] materialId;

    public Mesh mesh;
    public MeshFilter mf;
    

    // Start is called before the first frame update
    public void Start()
    {
        mesh = new Mesh();
        mf.mesh = mesh;
        
        marching.lerpCond = true;

        Debug.Log("x:" + buildingMap.Count() + " y:" + buildingMap[0].Count() + " Z:" + buildingMap[0][0].Count());

        meshUpdate();
    }


    double pointCalc(string material1, List<string> material2, double val)
    {
        switch (material1)
        {
            case "window":
                if (material2.Any(m2 => new List<string> { "wall" }.Any(m => m == m2)))
                {
                    return 0;
                }
                else if (material2.Any(m2 => new List<string> { "window" }.Any(m => m == m2)))
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
                }
                else if (material2.Any(m2 => new List<string> { "skyLight" }.Any(m => m == m2)))
                {
                    return val;
                }
                else if (material2.Any(m2 => new List<string> { "wall", "window" }.Any(m => m == m2)))
                {
                    return -5;
                }
                break;
            case "door":
                if (material2.Any(m2 => new List<string> { "door" }.Any(m => m == m2)))
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

    public void meshUpdate()
    {
        Vector3[] pointsTemp;
        Vector3 pointTemp;

        buildingPoint p;
        List<string> pMaterialTemp;

        List<Vector3> meshVertices = new List<Vector3>();
        List<int>[] subMeshTriangles = new List<int>[materials.Length];

        for (int i1 = 0; i1 < subMeshTriangles.Count(); i1++)
        {
            subMeshTriangles[i1] = new List<int>();
        }
        

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
                    
                    cubeVertices = new List<double[][][]>();
                    cubeMaterials = new List<string> { };
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
                                    cubeVertices[m][x1][y1][z1] = pointCalc(cubeMaterials[m], pMaterialTemp, p.val);
                                    

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
                            pointTemp.x = Convert.ToSingle((pointTemp.x + x - 1) * distPerSample[0]);

                            pointTemp.y = Convert.ToSingle((pointTemp.y + y - 1) * distPerSample[1]);

                            pointTemp.z = Convert.ToSingle((pointTemp.z + z - 1) * distPerSample[2]);


                            //checks if the vertice exists in the  vertice array
                            //if the vertice was found then the vertice would be shared rather than a new one being made
                            if (meshVertices.Contains(pointTemp) == false)
                            {
                                Vector2 uv = new Vector2(pointTemp.z/ distPerSample[2], pointTemp.y/ distPerSample[1]);
                                
                                uvs = uvs.Concat(new Vector2[] { uv }).ToArray();
                                

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
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        

    }
    
    public void targetAngleSet(float newTarget)
    {
        if (newTarget < angleRange[0])
        {
            targetAngle = angleRange[0];
        }
        else if(newTarget > angleRange[1])
        {
            targetAngle = angleRange[1];
        }
        else
        {
            targetAngle = newTarget;
        }
    }

    float clockwiseDist(float a, float b)
    {
        if(a < b)
        {
            return b - a;
        }
        return 360 - a + b;
    }

    float counterClockwiseDist(float a, float b)
    {
        if (a < b)
        {
            return 360 - a + b;
        }
        return a - b;
    }

    float angleConverter(float angle)
    {
        if (angle > 0)
        {
            return angle;
        }
        return 360 + angle;
    }

    private void updateAngle()
    {
        float deltaRotation = rotationSpeed * Time.deltaTime;
        Vector3 angle = this.transform.localEulerAngles;

        float[] newAngle = new float[3] { angle.x, angle.y, angle.z };


        if (targetAngle != newAngle[rotationAxis])
        {
            if (Math.Abs(targetAngle - curentAngle) > deltaRotation)
            {
                curentAngle += Math.Abs(targetAngle - curentAngle) / (targetAngle - curentAngle) * deltaRotation;
                newAngle[rotationAxis] += Math.Abs(targetAngle - curentAngle) / (targetAngle - curentAngle) * deltaRotation;
            }
            else
            {
                curentAngle = targetAngle;
                newAngle[rotationAxis] = angleConverter(targetAngle);
            }

            this.transform.localEulerAngles = new Vector3(newAngle[0], newAngle[1], newAngle[2]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateAngle();
    }
}
