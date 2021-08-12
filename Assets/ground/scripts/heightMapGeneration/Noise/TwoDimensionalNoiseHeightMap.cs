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
    ///     generateVectors is an abstract method to randomly generate perlin noise vector nodes
    /// </summary>
    /// <param name="start">int array that stores the starting position at which nodes will be genrated</param>
    /// <param name="end">int array that stores the ending position at which nodes will stop generating</param>
    /// <param name="template">array of perlin noise vectors</param>
    public override void generateVectors(int[] start, int[] end, float[][] template)
    {
        if(start.Length != 2 && end.Length != 2)
        {
            throw new ArgumentException("start and end paramater must be length 2");
        }


        Vector2DNode pointer = root.up;

        //setting point to the starting position
        int[] direction = new int[2];
        
        for (int i1 = 0; i1 < 2; i1++)
        {
            direction[i1] = start[i1] / Math.Abs(start[i1]);
        }

        for (int x = 0; x < start[0]; x += direction[0])
        {
            if (pointer == null)
            {
                throw new ArgumentException();
            }

            if (direction[0] == 1)
            {
                pointer = pointer.up;
            }
            else
            {
                pointer = pointer.down;
            }
        }

        for (int y = 0; y < start[1]; y += direction[1])
        {
            if (pointer == null)
            {
                throw new ArgumentException();
            }

            if (direction[1] == 1)
            {
                pointer = pointer.right;
            }
            else
            {
                pointer = pointer.left;
            }
        }

        //generating
        for (int y = start[1]; y < end[1]; y += direction[1])
        {
            if(direction[1] == 1)
            {
                if(pointer.up == null)
                {
                    Debug.Log("Create: Up");
                    pointer.up = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                    pointer.up.down = pointer;
                }
                else
                {
                    pointer.up.set(templateVector[random.Next(0, templateVector.Length)]);
                }
                
                pointer = pointer.up;
            }
            else
            {
                if (pointer.down == null)
                {
                    pointer.down = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                    pointer.down.up = pointer;
                }
                else
                {
                    pointer.down.set(templateVector[random.Next(0, templateVector.Length)]);
                }
                
                pointer = pointer.down;
            }

            for (int x = start[0] + direction[0]; x < end[0]; x += direction[0])
            {
                if(direction[0] == 1)
                {
                    if(pointer.right == null)
                    {
                        Debug.Log("Create: Right");
                        pointer.right = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                        pointer.right.left = pointer;
                    }
                    else
                    {
                        pointer.right.set(templateVector[random.Next(0, templateVector.Length)]);
                    }

                    pointer = pointer.right;
                }
                else
                {
                    if (pointer.left == null)
                    {
                        pointer.left = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                        pointer.left.right = pointer;
                    }
                    else
                    {
                        pointer.left.set(templateVector[random.Next(0, templateVector.Length)]);
                    }

                    pointer = pointer.left;
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
