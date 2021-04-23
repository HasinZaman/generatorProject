using UnityEngine;
using System;
/// <summary>
///     MeshData stores vertices and triangles of a mesh
/// </summary>
public readonly struct MeshData
{
    /// <summary>
    ///     verticies stores an array of vector3 that contains the verticies of a mesh
    /// </summary>
    public readonly Vector3[] vertices;
    
    /// <summary>
    ///     triangles stores an array of ints that contains the triangles of a mesh
    /// </summary>
    public readonly int[] triangles;

    /// <summary>
    ///     Constructor intialzies variables
    /// </summary>
    /// <param name="vertices">verticies paramater is used to intialize vertices instance</param>
    /// <param name="triangles">triangles paramater is used to intialize triangle instance</param>
    public MeshData(Vector3[] vertices, int[] triangles)
    {
        if(triangles.Length % 3 != 0)
        {
            throw new ArgumentException($"There needs to be three points per triangle. There is {triangles.Length % 3} extra points");
        }
        this.vertices = vertices;
        this.triangles = triangles;
    }
}
