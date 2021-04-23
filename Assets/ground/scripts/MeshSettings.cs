using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MeshSettings is an object that stores the settings
/// </summary>
[CreateAssetMenu(menuName = "MarchingCubes/MeshSettings")]
public class MeshSettings : ScriptableObject
{
    public float threshold = 0;
    public float distPerNode = 1;
}
