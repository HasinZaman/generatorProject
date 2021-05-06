using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MarchingCubes/ComputeShaderList")]
public class ComputeShaderList : ScriptableObject
{
    public ComputeShader MarchingCube;
    public ComputeShader Noise;
}
