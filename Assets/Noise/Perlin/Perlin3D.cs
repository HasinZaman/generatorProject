using System;

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

        root.up = new Vector3DNode(null);

        this.templateVector = templateVector;

        random = new System.Random(seed);

        generateVectors(new int[3] { 0, 0, 0 }, perlinVectorDim);
    }

    public override void generateVectors(int[] start, int[] end)
    {
        throw new System.NotImplementedException();
    }

    public override string toString()
    {
        throw new System.NotImplementedException();
    }

    protected override Vector3DNode getVector(int[] pos)
    {
        throw new System.NotImplementedException();
    }

    protected override Vector3DNode getVector(int[] pos, Vector3DNode start)
    {
        throw new System.NotImplementedException();
    }
}
