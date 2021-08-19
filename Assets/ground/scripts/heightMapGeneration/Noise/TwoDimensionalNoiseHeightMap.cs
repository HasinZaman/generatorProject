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
            if(vector != null)
            {
                this.set(vector);
            }
        }
    }

    protected class GeneratorIterator : Iterator<int>
    {
        /// <summary>
        ///     constructor sets up Iterator class
        /// </summary>
        /// <param name="start">Starting position of iterator</param>
        /// <param name="end">Ending position of iterator</param>
        public GeneratorIterator(int start, int end) : base(start - 1, end + 1)
        {
            if (end == start)
            {
                this.delta = 0;
            }
            else if (end - start > 0)
            {
                this.delta = 1;
            }
            else
            {
                this.delta = -1;
            }

            this.restart();
        }

        /// <summary>
        ///     hasNext method checks if another value exists
        /// </summary>
        /// <returns>boolean if the next number exists</returns>
        public override bool hasNext()
        {
            int tmp = this.delta + this.current;

            return this.start < tmp && tmp < this.end;
        }

        /// <summary>
        ///     next method updates iterator
        /// </summary>
        /// <returns>int of current value</returns>
        public override int next()
        {
            if(this.hasNext())
            {
                this.current += this.delta;
                return this.current;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        
        /// <summary>
        ///     restart method sets current value back to start
        /// </summary>
        public void restart()
        {
            if(this.delta == 1)
            {
                this.current = this.start;
            }
            else
            {
                this.current = this.end;
            }
            this.current += this.delta;
        }

        /// <summary>
        ///     reverse method swaps the start and end value
        /// </summary>
        public void reverse()
        {
            this.delta *= -1;
        }

        /// <summary>
        /// toString method converts iterator into String
        /// </summary>
        /// <returns>String representation of iterator</returns>
        public override string ToString()
        {
            return $"start:{this.start + delta}\tend:{this.end - delta}\tdelta:{this.delta}\tcurrent:{this.current}";
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

        root.up = new Vector2DNode(null);

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

        //primary pointer
        Vector2DNode pointer1 = null;

        //secondary pointer that points to the previous y layer
        Vector2DNode pointer2 = null;

        //setting point to the starting position
        GeneratorIterator[] iterator = new GeneratorIterator[2];

        pointer1 = this.getNode(start);

        if(pointer1.down != null)
        {
            pointer2 = pointer1.down;
        }

        iterator[0] = new GeneratorIterator(start[0], end[0] - 1);
        iterator[1] = new GeneratorIterator(start[1], end[1]);


        pointer1.set(templateVector[random.Next(0, templateVector.Length)]);

        //generating
        while (iterator[1].hasNext())
        {
            
            while (iterator[0].hasNext())
            {
                if (iterator[0].getDelta() == 1)
                {
                    if(pointer1.right == null)
                    {
                        pointer1.right = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                        pointer1.right.left = pointer1;
                    }
                    else
                    {
                        pointer1.right.set(templateVector[random.Next(0, templateVector.Length)]);
                    }

                    pointer1 = pointer1.right;

                    if (pointer2 != null)
                    {
                        pointer2 = pointer2.right;

                        if (iterator[1].getDelta() == 1)
                        {
                            pointer2.up = pointer1;
                            pointer1.down = pointer2;
                        }
                        else
                        {
                            pointer2.down = pointer1;
                            pointer1.up = pointer2;
                        }
                    }
                }
                else
                {
                    if (pointer1.left == null)
                    {
                        pointer1.left = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                        pointer1.left.right = pointer1;
                    }
                    else
                    {
                        pointer1.left.set(templateVector[random.Next(0, templateVector.Length)]);
                    }

                    pointer1 = pointer1.left;

                    if (pointer2 != null)
                    {
                        pointer2 = pointer2.left;
                        
                        if (iterator[1].getDelta() == 1)
                        {
                            pointer2.up = pointer1;
                            pointer1.down = pointer2;
                        }
                        else
                        {
                            pointer1.up = pointer2;
                            pointer2.down = pointer1;
                        }
                    }
                }
                iterator[0].next();
            }
            iterator[0].reverse();

            //remeber start of current layer
            pointer2 = pointer1;


            //create next layer
            if (iterator[1].getDelta() == 1)
            {
                if (pointer1.up == null)
                {
                    pointer1.up = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                }
                else
                {
                    pointer1.up.set(templateVector[random.Next(0, templateVector.Length)]);
                }

                pointer1 = pointer1.up;
                pointer1.down = pointer2;
                pointer2.up = pointer1;
            }
            else
            {
                if (pointer1.down == null)
                {
                    pointer1.down = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
                }
                else
                {
                    pointer1.down.set(templateVector[random.Next(0, templateVector.Length)]);
                }

                pointer1 = pointer1.down;
                pointer1.up = pointer2;
                pointer2.down = pointer1;
            }
            iterator[1].next();
        }
    }

    /// <summary>
    ///     getNode gets the perlin noise noise at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector</param>
    /// <returns>Vector2DNode of noise vector at pos</returns>
    protected Vector2DNode getNode(int[] pos)
    {
        Vector2DNode pointer = root.up;

        GeneratorIterator[] iterator = new GeneratorIterator[2];

        for(int i1 = 0; i1 < 2; i1++)
        {
            iterator[i1] = new GeneratorIterator(0, pos[i1]);
        }

        while (iterator[1].hasNext())
        {
            iterator[1].next();

            if (pointer == null)
            {
                throw new ArgumentException();
            }

            switch(iterator[1].getDelta())
            {
                case 1:
                    pointer = pointer.up;
                    break;
                case -1:
                    pointer = pointer.down;
                    break;
            }
        }

        while (iterator[0].hasNext())
        {
            if (pointer == null)
            {
                throw new ArgumentException();
            }

            switch (iterator[1].getDelta())
            {
                case 1:
                    pointer = pointer.right;
                    break;
                case -1:
                    pointer = pointer.left;
                    break;
            }
            iterator[0].next();
        }

        return pointer;
    }

    /// <summary>
    ///     getNode gets the perlin noise noise at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector relative to startNode</param>
    /// <param name="startNode">Vector2DNode instance is the start position</param>
    /// <returns>Vector2DNode of noise vector at pos</returns>
    protected Vector2DNode getNode(int[] pos, Vector2DNode startNode)
    {
        Vector2DNode pointer = startNode;

        GeneratorIterator[] iterator = new GeneratorIterator[2];

        for (int i1 = 0; i1 < 2; i1++)
        {
            iterator[i1] = new GeneratorIterator(0, pos[i1]);
        }

        while (iterator[1].hasNext())
        {
            iterator[1].next();

            if (pointer == null)
            {
                throw new ArgumentException();
            }

            switch (iterator[1].getDelta())
            {
                case 1:
                    pointer = pointer.up;
                    break;
                case -1:
                    pointer = pointer.down;
                    break;
            }
        }

        while (iterator[0].hasNext())
        {
            if (pointer == null)
            {
                throw new ArgumentException();
            }

            switch (iterator[1].getDelta())
            {
                case 1:
                    pointer = pointer.right;
                    break;
                case -1:
                    pointer = pointer.left;
                    break;
            }
            iterator[0].next();
        }

        return pointer;
    }

    /// <summary>
    ///     getVector gets the perlin noise vector at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector</param>
    /// <returns>float array of noise vector</returns>
    public override float[] getVector(int[] pos)
    {
        return getNode(pos).val();
    }

    public string toString()
    {
        string temp = "";

        Vector2DNode pointer = root;

        int i1 = 0;

        //get top most value
        while (pointer.up != null)
        {
            i1++;
            pointer = pointer.up;
        }

        //get left most value

        while (pointer.left != null)
        {
            pointer = pointer.left;
        }

        while (pointer.down != null)
        {
            while (pointer.right != null)
            {
                temp += $"{pointer.toString()}\t";
                pointer = pointer.right;
            }
            temp += $"{pointer.toString()}\t";

            while (pointer.left != null)
            {
                pointer = pointer.left;
            }
            pointer = pointer.down;
            temp += "\n";
        }

        while (pointer.right != null)
        {
            temp += $"{pointer.toString()}\t";
            pointer = pointer.right;
        }
        temp += $"{pointer.toString()}";

        return temp;
    }
}