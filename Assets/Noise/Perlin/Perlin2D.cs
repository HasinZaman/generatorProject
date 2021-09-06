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
    ///     getVector gets the perlin noise vector at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector</param>
    /// <returns>float array of noise vector</returns>
    protected override Vector2DNode getVector(int[] pos)
    {
        return getVector(pos, root.up);
    }

    /// <summary>
    ///     getNode gets the perlin noise noise at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector relative to startNode</param>
    /// <param name="startNode">Vector2DNode instance is the start position</param>
    /// <returns>Vector2DNode of noise vector at pos</returns>
    protected override Vector2DNode getVector(int[] pos, Vector2DNode start)
    {
        Vector2DNode pointer = start;

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

    private Vector2DNode generateXaxis(Vector2DNode pointer)
    {
        if (pointer.right == null)
        {
            pointer.right = new Vector2DNode(templateVector[random.Next(0, templateVector.Length)]);
            pointer.right.left = pointer;
        }
        else
        {
            pointer.right.set(templateVector[random.Next(0, templateVector.Length)]);
        }

        return pointer;
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

        //primary pointer
        Vector2DNode pointer1 = null;

        //secondary pointer that points to the previous y layer
        Vector2DNode pointer2 = null;

        //setting point to the starting position
        GeneratorIterator[] iterator = new GeneratorIterator[2];

        pointer1 = this.getVector(start);

        if (pointer1.down != null)
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
                    if (pointer1.right == null)
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
