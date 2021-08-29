using System;
using UnityEngine;

public class Perlin2D : Perlin
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
    }

    /// <summary>
    ///     dim stores the size of dimension
    /// </summary>
    int dim = 2;

    /// <summary>
    ///     root stores the starting point of Noise Grid
    /// </summary>
    Vector2DNode root = new Vector2DNode(null);

    /// <summary>
    ///     templateVector is an arry of vectors that used in calculating perlin noise
    /// </summary>
    float[][] templateVector;

    /// <summary>
    ///     Constructor sets up 2D perlin noise object
    /// </summary>
    /// <param name="templateVector"></param>
    /// <param name="seed"></param>
    /// <param name="perlinVectorDim"></param>
    public Perlin2D(float[][] templateVector, int seed, int[] perlinVectorDim)
    {
        if (perlinVectorDim.Length != 2)
        {
            throw new ArgumentException();
        }

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
    protected Vector2DNode getVector(int[] pos)
    {
        return getVector(pos, root.up);
    }

    /// <summary>
    ///     getNode gets the perlin noise noise at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector relative to startNode</param>
    /// <param name="startNode">Vector2DNode instance is the start position</param>
    /// <returns>Vector2DNode of noise vector at pos</returns>
    protected Vector2DNode getVector(int[] pos, Vector2DNode start)
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

    /// <summary>
    ///     generateVectors is an abstract method to randomly generate perlin noise vector nodes
    /// </summary>
    /// <param name="start">int array that stores the starting position at which nodes will be genrated</param>
    /// <param name="end">int array that stores the ending position at which nodes will stop generating</param>
    /// <param name="template">array of perlin noise vectors</param>
    public override void generateVectors(int[] start, int[] end)
    {
        if (start.Length != 2 && end.Length != 2)
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public override float sample(float[] pos)
    {
        if (dim != pos.Length)
        {
            throw new ArgumentException();
        }
        float sampleConst = 2.084991f;

        float[][] vectors = new float[4][];
        float[][] dist = new float[4][];
        float[] vertexVal = new float[4];

        float x = pos[0];
        float y = pos[1];

        Vector2DNode pointer = getVector(new int[] { (int)Math.Floor(x), (int)Math.Floor(y) });

        for (int x1 = 0; x1 < 2; x1++)
        {
            for (int y1 = 0; y1 < 2; y1++)
            {
                vectors[x1 + y1 * 2] = getVector(new int[] { x1, y1 }, pointer).val();
                dist[x1 + y1 * 2] = new float[] { x % 1 - x1, y % 1 - y1 };

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
        float line1 = cosineInterpolate(vertexVal[0 + 0 * 2], vertexVal[1 + 0 * 2], x % 1);

        return (cosineInterpolate(line1, line0, y % 1) + sampleConst / 2f) / sampleConst;
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
