﻿using System;

/// </summary>
public class Perlin3D : Perlin<Perlin3D.Vector3DNode>
{
    public class Vector3DNode : VectorNode
    {
        /// <summary>
        ///     neighbor node left relative to the current node
        /// </summary>
        public Vector3DNode left = null;

        /// <summary>
        ///     neighbor node right relative to the current node
        /// </summary>
        public Vector3DNode right = null;

        /// <summary>
        ///     neighbor node up relative to the current node
        /// </summary>
        public Vector3DNode up = null;

        /// <summary>
        ///     neighbor node down relative to the current node
        /// </summary>
        public Vector3DNode down = null;

        /// <summary>
        ///     neighbor node above relative to the current node
        /// </summary>
        public Vector3DNode above = null;

        /// <summary>
        ///     neighbor node below relative to the current node
        /// </summary>
        public Vector3DNode below = null;

        public Vector3DNode(float[] vector)
        {
            this.dim = 3;

            this.set(vector);
        }

        public override VectorNode get(int axis, int relativePos)
        {
            switch (axis)
            {
                case 0:
                    switch (relativePos)
                    {
                        case -1:
                            return this.left;
                        case 1:
                            return this.right;
                    }
                    break;
                case 1:
                    switch (relativePos)
                    {
                        case -1:
                            return this.down;
                        case 1:
                            return this.up;
                    }
                    break;
                case 2:
                    switch(relativePos)
                    {
                        case -1:
                            return this.below;
                        case 1:
                            return this.above;
                    }
                    break;
            }
            throw new ArgumentException();
        }
    }
 
    public Perlin3D(float[][] templateVector, int seed, int[] perlinVectorDim) : base(3)
    {
        if (perlinVectorDim.Length != this.dim)
        {
            throw new ArgumentException();
        }

        root = new Vector3DNode(null);

        root.up = new Vector3DNode(null);

        this.templateVector = templateVector;

        random = new System.Random(seed);

        generateVectors(new int[3] { 0, 0, 0 }, perlinVectorDim);
    }

    /// <summary>
    ///     createLine method is a private method that creates a column of connected vectorNodes
    /// </summary>
    /// <param name="length">length of column</param>
    /// <param name="direction">direction of in which nodes are connected</param>
    /// <param name="start">Starting node</param>
    /// <returns>Vector3DNode array of node column</returns>
    private Vector3DNode[] createLine(int length, int direction, Vector3DNode start)
    {
        Vector3DNode[] tmp = new Vector3DNode[length];

        Vector3DNode pointer = start;

        for (int i1 = 0; i1 < length; i1++)
        {
            if (pointer == null)
            {
                tmp[i1] = new Vector3DNode(templateVector[random.Next(0, templateVector.Length)]);
            }
            else
            {
                tmp[i1] = pointer;
                tmp[i1].set(templateVector[random.Next(0, templateVector.Length)]);
            }

            if (i1 != 0)
            {
                switch (direction)
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
            pointer = (Vector3DNode) pointer.get(0, direction);
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
    /// <returns>2d Vector3DNode array of node rectangle</returns>
    private Vector3DNode[][] createFace(int x, int y, int[] direction, Vector3DNode start)
    {
        Vector3DNode[][] tmp = new Vector3DNode[x][];

        //generating columns
        if (start != null)
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
    ///     createForm method is a private method that uses 2d slices to make a 3d prisim
    /// </summary>
    /// <param name="x">number of VectorNodes in the x axis</param>
    /// <param name="y">number of VectorNodes in the y axis</param>
    /// <param name="z">number of VectorNodes in the z axis</param>
    /// <param name="direction">int array of direction for each axis</param>
    /// <param name="start">starting node</param>
    /// <returns>2d Vector3DNode array of node rectanglar prisim</returns>
    private Vector3DNode[][][] createForm(int x, int y, int z, int[] direction, Vector3DNode start)
    {
        Vector3DNode[][][] tmp = new Vector3DNode[z][][];

        tmp[0] = createFace(x, y, new int[] { direction[0], direction[1] }, start);
        for(int z1 = 1; z1 < z; z1++)
        {
            tmp[z1] = createFace(x, y, new int[] { direction[0], direction[1] }, null);

            for(int y1 = 0; y1 < y; y1++)
            {
                for(int x1 = 0; x1 < x; x1++)
                {
                    switch (direction[2])
                    {
                        case -1:
                            tmp[z1][x1][y1].above = tmp[z1 - 1][x1][y1];
                            tmp[z1 - 1][x1][y1].below = tmp[z1][x1][y1];
                            break;
                        case 1:
                            tmp[z1][x1][y1].below = tmp[z1 - 1][x1][y1];
                            tmp[z1 - 1][x1][y1].above = tmp[z1][x1][y1];
                            break;
                    }
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
    public override void generateVectors(int[] start, int[] end)
    {
        if (start.Length != this.dim && end.Length != this.dim)
        {
            throw new ArgumentException("start and end paramater must be length 2");
        }

        int[] delta = new int[this.dim];

        for (int i1 = 0; i1 < this.dim; i1++)
        {
            delta[i1] = Math.Max(-1, Math.Min(1, end[i1] - start[i1]));
        }

        Vector3DNode startNode = getVector(start);

        createForm(Math.Abs(end[0] - start[0]), Math.Abs(end[1] - start[1]), Math.Abs(end[2] - start[2]), delta, startNode);
    }

    /// <summary>
    ///     sliceToString method returns the string form of a 2d slice at a given z level
    /// </summary>
    /// <param name="start">starting z level node</param>
    /// <returns>String form 2d slice at given z level</returns>
    private string sliceToString(Vector3DNode start)
    {
        string temp = "";

        Vector3DNode pointer = start;

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

    /// <summary>
    ///     toString method represents class 3d vector grid in the form of a string
    /// </summary>
    /// <returns>string representation of class</returns>
    public override string toString()
    {
        Vector3DNode pointer1 = root.up;

        string tmp = "";

        int i1 = 0;

        while (pointer1.above != null)
        {
            pointer1 = pointer1.above;
            i1++;
        }

        while (pointer1 != null)
        {
            tmp += $"Layer:{i1}\n{sliceToString(pointer1)}\n";
            
            i1--;
            pointer1 = pointer1.below;
        }

        return tmp;
    }
}
