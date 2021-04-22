using System;
/// <summary>
/// 
/// </summary>
public class Grid
{
    /// <summary>
    ///     dim is an int array that stores the number of Nodes stored in each axis of the Grid.
    /// </summary>
    /// <example>
    ///     dim[2] => [x,y]
    ///     dim[3] => [x,y,z]
    /// </example>
    private int[] dim;

    /// <summary>
    ///     nodes is a Node array that stores the number of nodes a grid.
    /// </summary>
    /// <remarks>
    ///     The index of a Node is related to the nodes position within the grid
    ///     index = x + (y  + z * dim[1]) * dim[0]
    /// </remarks>
    private Node[] nodes;
    
    /// <summary>
    ///     Range class is used to store the range of within a given axis that a given action will be bound to
    /// </summary>
    private class Range
    {
        public int start, end, step;
        
        /// <summary>
        ///     Constructor creates a range object
        /// </summary>
        /// <param name="dim">dim paramater is the max size in a given axis</param>
        /// <param name="range">range paramater is int array that defines range</param>
        public Range(int dim, int[] range)
        {
            switch (range.Length)
            {
                case 0:
                    throw new ArgumentException("range cannot be empty array");
                    break;
                case 1:
                    start = 0;
                    end = range[0];
                    step = 1;
                    break;
                case 2:
                    start = range[0];
                    end = range[1];
                    step = 1;
                    break;
                case 3:
                    start = range[0];
                    end = range[1];
                    step = range[2];
                    break;
                default:
                    throw new ArgumentException("range cannot have more than 3 values");
            }

            if (start < 0 )
            {
                throw new ArgumentException("Start must be greater than 0");
            }
            else if(start > dim)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (end < 0)
            {
                throw new ArgumentException("Start must be greater than 0");
            }
            else if (end > dim)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if(end > start)
            {
                throw new ArgumentOutOfRangeException("The end point needs to be greater than start");
            }

            if (step <= 0)
            {
                throw new ArgumentException("Step must be greater than 0");
            }
        }
    }

    /// <summary>
    ///     Constructor Creates a Grid object.
    /// </summary>
    /// <param name="nodes">The nodes paramater is a Node array that is used to initalize nodes instance</param>
    /// <param name="dim">The dim paramater is a int array that is used to initalize dim instance</param>
    public Grid(Node[] nodes, int[] dim)
    {
        setDim(dim);
        setNodes(nodes);
    }

    public Grid(float defaultVal, int[] dim)
    {
        setDim(dim);
        setNodes(new Node[0] { });
    }

    /// <summary>
    ///     setDim method sets the dim instance
    /// </summary>
    /// <param name="dim">dim paramater is an int array that will be used to set dim</param>
    public void setDim(int[] dim)
    {
        for(int i1 = 0; i1 < dim.Length; i1++)
        {
            if(dim[i1] < 0)
            {
                throw new ArgumentException($"Invalid dim value at index {i1}. dim[{i1}]({dim[i1]}) needs to be greater than 0");
            }
        }

        this.dim = (int[]) dim.Clone();
    }

    /// <summary>
    ///     setNodes method sets the nodes instance
    /// </summary>
    /// <param name="nodes">nodes paramater is an Node array that will be used to set nodes </param>
    public void setNodes(Node[] nodes)
    {
        if(nodes == null)
        {
            throw new ArgumentNullException();
        }

        int dimSize = dim[0] * dim[1] * dim[2];

        if (nodes.Length > dimSize)
        {
            throw new ArgumentOutOfRangeException($"nodes({nodes.Length}) has more elements than avaliable elements({dim[0] * dim[1] * dim[2]})");
        }
        else if(nodes.Length == dimSize)
        {
            this.nodes = (Node[]) nodes.Clone();
        }
        else
        {
            this.nodes = new Node[dimSize];

            int i1 = 0;
            
            for(; i1 < nodes.Length; i1++)
            {
                this.nodes[i1] = nodes[i1];
            }
            
            for(; i1 < dimSize; i1++)
            {
                this.nodes[i1] = new Node();
            }
        }
    }

    /// <summary>
    ///     Sets a range of Nodes within a given range
    /// </summary>
    /// <param name="val">val is a float that will be assigned to Nodes</param>
    /// <param name="xRange">xRange is a int array that stores define the range of Nodes affected along the x axis [start, end, stepSize]</param>
    /// <param name="yRange">yRange is a int array that stores define the range of Nodes affected along the x axis [start, end, stepSize]</param>
    /// <param name="zRange">zRange is a int array that stores define the range of Nodes affected along the x axis [start, end, stepSize]</param>
    public void setNodes(float val, int[] xRange, int[] yRange, int[] zRange)
    {
        Range[] range = new Range[3] {new Range(dim[0], xRange), new Range(dim[1], yRange), new Range(dim[2], zRange) };

        for(int x = range[0].start; x < range[0].end; x += range[0].step)
        {
            for (int y = range[1].start; y < range[1].end; y += range[1].step)
            {
                for (int z = range[2].start; z < range[2].end; z += range[2].step)
                {
                    nodes[coordToIndex(x, y, z)].setValue(val);
                }
            }
        }
    }

    /// <summary>
    ///     coordToIndex converts (x,y,z) into index in nodes
    /// </summary>
    /// <param name="x">x is an int pos along the x axis</param>
    /// <param name="y">y is an int pos along the y axis</param>
    /// <param name="z">z is an int pos along the z axis</param>
    /// <returns>method returns the index associated with a given coordinate</returns>
    private int coordToIndex(int x, int y, int z)
    {
        return x + (y + z * dim[1]) * dim[0];
    }

    /// <summary>
    ///     toArray returns an array of Node.val
    /// </summary>
    /// <returns>
    ///     float array that is filled with the value of a Node
    /// </returns>
    public float[] toArray()
    {
        return toArray(1, 1, 1);
    }

    /// <summary>
    ///     toArray returns an array of Node.val
    /// </summary>
    /// <param name="nStep">nStep is an int that allows the returned array to skip certain nodes</param>
    /// <returns>
    ///     float array that is filled with the value of a Node
    /// </returns>
    public float[] toArray(int nStep)
    {
        return toArray(nStep, nStep, nStep);
    }

    /// <summary>
    ///     toArray returns an array of Node.val
    /// </summary>
    /// <param name="xStep">xStep is an int that allows the returned array to skip certain nodes along the x axis</param>
    /// <param name="yStep">yStep is an int that allows the returned array to skip certain nodes along the y axis</param>
    /// <param name="zStep">zStep is an int that allows the returned array to skip certain nodes along the z axis</param>
    /// <returns>
    ///     float array that is filled with the value of a Node
    /// </returns>
    public float[] toArray(int xStep, int yStep, int zStep)
    {
        Range[] range = new Range[3];
        float[] val = new float[dim[0] * dim[1] * dim[2]];

        range[0] = new Range(dim[0], new int[] { 0, dim[0], xStep });
        range[1] = new Range(dim[1], new int[] { 0, dim[1], yStep });
        range[2] = new Range(dim[2], new int[] { 0, dim[2], zStep });

        for (int x = range[0].start; x < range[0].end; x += range[0].step)
        {
            for (int y = range[1].start; y < range[1].end; y += range[1].step)
            {
                for (int z = range[2].start; z < range[2].end; z += range[2].step)
                {
                    val[coordToIndex(x, y, z)] = nodes[coordToIndex(x, y, z)].getValue();
                }
            }
        }

        return val;
    }

    /// <summary>
    ///     getFace return a grid that contains the face that is parrel to the plane
    /// </summary>
    /// <param name="planeCode">planeCode is ushort that refers to a plane which nodes will be selected from</param>
    /// <param name="layer">layer is a int that is the layer from the grid that the face grid will be made out of</param>
    /// <returns>
    ///     Grid that stores a face from grid
    /// </returns>
    public Grid getFace(ushort planeCode, int layer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     getFace return a grid that contains a line that is perpendicular to the plane
    /// </summary>
    /// <param name="planeCode">planeCode is ushort that refers to a plane which nodes will be selected from</param>
    /// <param name="coord">coord is an array that stores the position on a plane in which a line would be extracted</param>
    /// <returns>
    ///     Grid that stores a line from grid
    /// </returns>
    public Grid getLine(ushort planeCode, int[] coord)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     getCorner returns a value from the corners of the grid
    /// </summary>
    /// <param name="cornerCode">cornerCode is that refers to a specific corner of the grid </param>
    /// <returns></returns>
    public Grid getCorner(ushort cornerCode)
    {
        throw new NotImplementedException();
    }
}
