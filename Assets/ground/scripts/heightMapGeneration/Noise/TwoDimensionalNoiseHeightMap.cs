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

    /// <summary>
    ///     GeneratorIterator class traverses in a back and forth manner between a range of values inorder to create a linked grids using Vector2DNode
    /// </summary>
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

            return this.start < tmp && tmp < this.end && this.delta != 0;
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
    ///     GridParam stores custom paramater values for getHeightMap method
    /// </summary>
    public class GridParam
    {
        private float[] start = new float[2];
        private float[] end = new float[2];
        private int[] samples = new int[2];
        public int height;

        /// <summary>
        ///     setStart method sets start instance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setStart(float x, float y)
        {
            start[0] = x;
            start[1] = y;
        }

        /// <summary>
        ///     setEnd method set end instance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setEnd(float x, float y)
        {
            end[0] = x;
            end[1] = y;
        }

        /// <summary>
        ///     setSamples method set samples instance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setSamples(int x, int y)
        {
            samples[0] = x;
            samples[1] = y;
        }

        /// <summary>
        ///     getStart method returns start instance
        /// </summary>
        /// <returns>float[2] of start instance</returns>
        public float[] getStart()
        {
            return start;
        }

        /// <summary>
        ///     getEnd method returns end instance
        /// </summary>
        /// <returns>float[2] of end instance</returns>
        public float[] getEnd()
        {
            return end;
        }

        /// <summary>
        ///     getSamples method returns sample instance
        /// </summary>
        /// <returns>int[2] of sample instance</returns>
        public int[] getSamples()
        {
            return samples;
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
    public override Grid getHeightMap(object param)
    {
        if (typeof(GridParam) != param.GetType())
        {
            throw new ArgumentException();
        }

        Node[] nodes;

        GridParam gridParam = (GridParam) param;

        float[] start = gridParam.getStart();
        float[] end = gridParam.getEnd();
        int[] dim = new int[3] { gridParam.getSamples()[0], gridParam.height, gridParam.getSamples()[1] };

        Grid grid = new Grid(dim);

        SampleIterator[] pos = new SampleIterator[2];
        
        for (int i1 = 0; i1 < 2; i1++)
        {
            pos[i1] = new SampleIterator(start[i1], end[i1], dim[2 * i1]);
        }


        nodes = new Node[dim[0] * dim[1] * dim[2]];

        float tmp;

        for(int y = 0; y < dim[1]; y++)
        {
            for(int z = 0; z < dim[2]; z++)
            {
                tmp = this.sample(pos[0].current, pos[1].current) * (float) (dim[1] - 1);

                for (int x = 0; x < dim[0]; x++)
                {
                    nodes[x + (y + z * dim[1]) * dim[0]] = new Node();
                    if(tmp - y > 1)
                    {
                        nodes[x + (y + z * dim[1]) * dim[0]].setValue(1);
                    }
                    else
                    {
                        nodes[x + (y + z * dim[1]) * dim[0]].setValue(Math.Max(tmp % 1, 0));
                    }
                }
                pos[1].next();
            }
            pos[0].next();
            pos[1].restart();
        }

        grid.setNodes(nodes);

        return grid;
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
        return getNode(pos, root.up);
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

            switch (iterator[0].getDelta())
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

    public float sample(float x, float y)
    {
        float[][] vectors = new float[4][];
        float[][] dist = new float[4][];
        float[] vertexVal = new float[4];

        Vector2DNode pointer = getNode(new int[] { (int)Math.Floor(x), (int)Math.Floor(y) });

        for (int x1 = 0; x1 < 2; x1++)
        {
            for (int y1 = 0; y1 < 2; y1++)
            {
                vectors[x1 + y1 * 2] = getNode(new int[] { x1, y1 }, pointer).val();
                dist[x1 + y1 * 2] = new float[] { x % 1 - x1, y % 1 - y1};

                vertexVal[x1 + y1 * 2] = dotProduct(vectors[x1 + y1 * 2], dist[x1 + y1 * 2]);
            }
        }

        /*
         * 01--Line0--11
         *       |
         *     Line2 (return)
         *       |
         * 00--Line1--10
         */

        float line0 = cosineInterpolate(vertexVal[0 + 1 * 2], vertexVal[1 + 1 * 2], x % 1);
        float line1 = cosineInterpolate(vertexVal[0 + 1 * 2], vertexVal[1 + 0 * 2], x % 1);

        return cosineInterpolate(line0, line1, y % 1);
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