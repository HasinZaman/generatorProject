using System;
using UnityEngine;

public class Perlin2D : Perlin<Perlin2D.Vector2DNode>
{
    public class Vector2DNode : VectorNode
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
        public Vector2DNode(float[] vector)
        {
            this.dim = 2;

            this.set(vector);
        }

        /// <summary>
        ///     get method converts ints into a refrence to neighbor node
        /// </summary>
        /// <param name="axis">axis number</param>
        /// <param name="relativePos">int relative position of neighbor node on axis</param>
        /// <returns>VectorNode neighbor of current Node</returns>
        public override VectorNode get(int axis, int relativePos)
        {
            switch(axis)
            {
                case 0:
                    switch(relativePos)
                    {
                        case -1:
                            return this.left;
                        case 1:
                            return this.right;
                    }
                    break;
                case 1:
                    switch(relativePos)
                    {
                        case -1:
                            return this.down;
                        case 1:
                            return this.up;
                    }
                    break;
            }
            throw new ArgumentException();
        }
    }

    /// <summary>
    ///     Constructor sets up 2D perlin noise object
    /// </summary>
    /// <param name="templateVector"></param>
    /// <param name="seed"></param>
    /// <param name="perlinVectorDim"></param>
    public Perlin2D(float[][] templateVector, int seed, int[] perlinVectorDim) : base(2)
    {
        if (perlinVectorDim.Length != this.dim)
        {
            throw new ArgumentException();
        }

        root = new Vector2DNode(null);

        root.up = new Vector2DNode(null);

        this.templateVector = templateVector;

        random = new System.Random(seed);

        generateVectors(new int[2] { 0, 0 }, perlinVectorDim);
    }

    /// <summary>
    ///     createLine method is a private method that creates a column of connected vectorNodes
    /// </summary>
    /// <param name="length">length of column</param>
    /// <param name="direction">direction of in which nodes are connected</param>
    /// <param name="start">Starting node</param>
    /// <returns>Vector2DNode array of node column</returns>
    private Vector2DNode[] createLine(int length, int direction, Vector2DNode start)
    {
        Vector2DNode[] tmp = new Vector2DNode[length];

        Vector2DNode pointer = start;

        for (int i1 = 0; i1 < length; i1++)
        {
            if(pointer == null)
            {
                tmp[i1] = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
            }
            else
            {
                tmp[i1] = pointer;
                tmp[i1].set(templateVector[random.Next(0, templateVector.Length)]);
            }

            if(i1 != 0)
            {
                switch(direction)
                {
                    case -1:
                        tmp[i1].up = tmp[i1 - 1];
                        tmp[i1 - 1].down = tmp[i1];
                        break;
                    case 1:
                        tmp[i1].down = tmp[i1 - 1];
                        tmp[i1 - 1].up = tmp[i1];
                        break;
                }
            }
            pointer = tmp[i1];
            pointer = (Vector2DNode) pointer.get(0, direction);
        }

        return tmp;
    }

    /// <summary>
    ///     createFace method is a private method that creates a 2d slice/face of VectorNodes
    /// </summary>
    /// <param name="x">number of VectorNodes in the x axis</param>
    /// <param name="y">number of VectorNodes in the y axis</param>
    /// <param name="direction">int array of direction for each axis</param>
    /// <param name="start">starting node</param>
    /// <returns>2d Vector2DNode array of node rectangle</returns>
    private Vector2DNode[][] createFace(int x, int y, int[] direction, Vector2DNode start)
    {
        Vector2DNode[][] tmp = new Vector2DNode[x][];

        //generating columns
        if(start != null)
        {
            tmp[0] = createLine(y, direction[1], start);
        }
        else
        {
            tmp[0] = createLine(y, direction[1], null);
        }

        for (int x1 = 1; x1 < x; x1++)
        {
            tmp[x1] = createLine(y, direction[1], null);

            //connecting columns
            for (int y1 = 0; y1 < y; y1++)
            {
                switch (direction[0])
                {
                    case -1:
                        tmp[x1][y1].right = tmp[x1 - 1][y1];
                        tmp[x1 - 1][y1].left = tmp[x1][y1];
                        break;
                    case 1:
                        tmp[x1][y1].left = tmp[x1 - 1][y1];
                        tmp[x1 - 1][y1].right = tmp[x1][y1];
                        break;
                }
            }
        }
        return tmp;
    }

    /// <summary>
    ///     generateVectors is an abstract method to randomly generate perlin noise vector nodes
    /// </summary>
    /// <param name="start">int array that stores the starting position at which nodes will be genrated</param>
    /// <param name="end">int array that stores the ending position at which nodes will stop generating</param>
    /// <param name="template">array of perlin noise vectors</param>
    public override void generateVectors(int[] start, int[] end)
    {
        if (start.Length != this.dim && end.Length != this.dim)
        {
            throw new ArgumentException("start and end paramater must be length 2");
        }

        int[] delta = new int[this.dim];

        for(int i1 = 0; i1 < this.dim; i1++)
        {
            delta[i1] = Math.Max(-1, Math.Min(1, end[i1] - start[i1]));
        }

        Vector2DNode startNode = getVector(start);

        createFace(Math.Abs(end[0] - start[0]), Math.Abs(end[1] - start[1]), delta, startNode);
    }
    
    public override string toString()
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
