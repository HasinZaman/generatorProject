using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     VectorNode stores Perlin Noise Vector used in Perlin Noise classes
/// </summary>
public abstract class VectorNode
{
    /// <summary>
    ///     vector stores the perlin noise vector
    /// </summary>
    protected float[] vector;

    /// <summary>
    ///     dim stores size of the perlin noise vectors
    /// </summary>
    protected int dim;

    /// <summary>
    ///     Set method assigns vector value
    /// </summary>
    /// <param name="vector">New vector value</param>
    public void set(float[] vector)
    {
        if(vector != null)
        {
            if (dim != vector.Length)
            {
                throw new ArgumentException($"vector lenght must have exactly {dim} elements");
            }
            this.vector = vector;
        }
    }

    /// <summary>
    ///     get method returns vector value
    /// </summary>
    /// <returns>A float array of vector</returns>
    public float[] val()
    {
        return this.vector;
    }

    public abstract VectorNode get(int axis, int relativePos);

    /// <summary>
    ///     toString method converts object into a string repersentation
    /// </summary>
    /// <returns>string format of VectorNode object</returns>
    public string toString()
    {
        string temp = "(";

        temp += $"{this.vector[0]}";

        for (int i1 = 1; i1 < this.vector.Length; i1++)
        {
            temp += $",{this.vector[i1]}";
        }

        temp += ")";

        return temp;
    }
}
