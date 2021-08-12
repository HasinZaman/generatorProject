using System;
using UnityEngine;

/// <summary>
///     TwoDimensionalNoiseHeightMap creates a height map that using 2d perlin noise 
/// </summary>
public class TwoDimensionalNoiseHeightMap : NoiseHeightMapGenerator
{

    /// <summary>
    ///     Vector2DNode stores perlin vector and neighbor node
    /// </summary>
    protected class Vector2DNode : VectorNode
    {
        /// <summary>
        ///     neighbor node left relative to the current node
        /// </summary>
        public Vector2DNode left = null;

        /// <summary>
        ///     neighbor node right relative to the current node
        /// </summary>
        public Vector2DNode right = null;

        /// <summary>
        ///     neighbor node up relative to the current node
        /// </summary>
        public Vector2DNode up = null;

        /// <summary>
        ///     neighbor node down relative to the current node
        /// </summary>
        public Vector2DNode down = null;

        /// <summary>
        ///     Constructor creates Vector2DNode object 
        /// </summary>
        /// <param name="vector">intial value of vector value</param>
        public Vector2DNode(float[] vector)
        {
            this.dim = 2;
            this.set(vector);
        }
    }

    /// <summary>
    ///     shader is stores a computeShader used to calculate the noise
    /// </summary>
    ComputeShader shader;

    /// <summary>
    ///     root stores the starting point of Noise Grid
    /// </summary>
    protected Vector2DNode root = new Vector2DNode(null);

    /// <summary>
    ///     NoiseHeightMapGenerator constructor intializes variables
    /// </summary>
    /// <param name="templateVector">templateVector paramater assigns the template vectors that will be used in calculating perlin noise</param>
    /// <param name="seed">seed paramater is used to intialize random</param>
    /// <param name="nodeSize">nodeSize is an array that stores the dimension of the final height map grid</param>
    /// <param name="perlinVectorDim">perlinVectorDim is an array that defines the size of perlinNoise vector</param>
    /// <param name="shader"></param>
    public TwoDimensionalNoiseHeightMap(float[][] templateVector, int seed, int[] perlinVectorDim, ComputeShader shader) : base( templateVector, seed)
    {
        if(perlinVectorDim.Length != 2)
        {
            throw new ArgumentException();
        }

        this.shader = shader;

        generateVectors(new int[2] { 0, 0 }, perlinVectorDim, templateVector);
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public override Grid getHeightMap()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// </summary>
    {
        {
        }



        

        {
            {
            }

            {
            }
        }

        {

            {
            }
        }

        {
            {
                {
                }
            }
            {
            }

            {
                    {
                    }
                    {
                    }
                    {
                    }
                    {
                    }
            }
        }
        {
        }

        {
            {
            }
            {
            }
        }

        {
            {
            }
        }

    }

    /// <summary>
    /// </summary>
    {
    }

    {



        {
            {
            }
        }
    }
