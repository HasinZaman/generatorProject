using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door : MonoBehaviour
{
    public buildingPoint[][][] buildingMap;

    public Vector3 samplesPerCell;

    public Vector3[] vertices;
    public int[] triangles;

    public Vector2 rotation;

    public marchingCube marching;

    public Mesh mesh;
    public MeshFilter mf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void prepMesh()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
