using System;

/// <summary>
///     Worley2D class generates 2d Worley noise
/// </summary>
public class Worley2D : Worley<Worley2D.Worely2DCell>
{
    /// <summary>
    ///     Worely2DCell stores cell information in 2d
    /// </summary>
    public class Worely2DCell : CellNode
    {
        public Worely2DCell left;
        public Worely2DCell right;
        public Worely2DCell up;
        public Worely2DCell down;

        /// <summary>
        ///     Constructor set up Worely2DCell class
        /// </summary>
        /// <param name="val">float array of intial vector value</param>
        public Worely2DCell(float[] val) : base(2, val)
        {

        }

        /// <summary>
        ///     get method returns refrence to neighbor node
        /// </summary>
        /// <param name="axis">axis in which the neighbor refrence be retrived from</param>
        /// <param name="relativePos">position of neighbor node relative to current node</param>
        /// <returns>A refrence to neighbor node</returns>
        public override CellNode get(int axis, int relativePos)
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
            }
            throw new ArgumentException();
        }
    }

    /// <summary>
    ///     Worley2D constructor sets up Worley2D Noise generator class
    /// </summary>
    /// <param name="seed">int seed sets up noise random object</param>
    /// <param name="cellDim">cellDim is int array that defines the number of cells in each axis</param>
    public Worley2D(int seed, int[] cellDim) : base(2)
    {
        root = new Worely2DCell(null);

        root.up = new Worely2DCell(null);

        random = new Random(seed);

        generateCells(new int[2] { 0, 0 }, cellDim);
    }

    /// <summary>
    ///     createLine method is a private method that creates a column of connected vectorNodes
    /// </summary>
    /// <param name="length">length of column</param>
    /// <param name="direction">direction of in which nodes are connected</param>
    /// <param name="start">Starting node</param>
    /// <returns>Vector2DNode array of node column</returns>
    private Worely2DCell[] createLine(int length, int direction, Worely2DCell start)
    {
        Worely2DCell[] tmp = new Worely2DCell[length];

        Worely2DCell pointer = start;

        for (int i1 = 0; i1 < length; i1++)
        {
            if (pointer == null)
            {
                tmp[i1] = new Worely2DCell(new float[2] { (float) random.NextDouble(), (float) random.NextDouble() });
            }
            else
            {
                tmp[i1] = pointer;
                tmp[i1].set(new float[2] { (float) random.NextDouble(), (float) random.NextDouble() });
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
            pointer = (Worely2DCell) pointer.get(0, direction);
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
    private Worely2DCell[][] createFace(int x, int y, int[] direction, Worely2DCell start)
    {
        Worely2DCell[][] tmp = new Worely2DCell[x][];

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
    ///     generateVectors is an abstract method to randomly generate perlin noise vector nodes
    /// </summary>
    /// <param name="start">int array that stores the starting position at which nodes will be genrated</param>
    /// <param name="end">int array that stores the ending position at which nodes will stop generating</param>
    public override void generateCells(int[] start, int[] end)
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

        Worely2DCell startNode = getCell(start);

        createFace(Math.Abs(end[0] - start[0]), Math.Abs(end[1] - start[1]), delta, startNode);
    }

    /// <summary>
    ///     toString returns a string repersentation of object
    /// </summary>
    /// <returns>String repersentation of object</returns>
    public override string toString()
    {
        string temp = "";

        Worely2DCell pointer = root;

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
