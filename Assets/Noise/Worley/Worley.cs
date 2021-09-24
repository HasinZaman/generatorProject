using System;
/// <summary>
///     Worley abstract class creates foundation for higher level Dimension noise 
/// </summary>
public abstract class Worley<T> : Noise where T : CellNode
{

    /// <summary>
    ///     root stores the starting point of Noise Grid
    /// </summary>
    protected T root;

    /// <summary>
    ///     dim stores size of the perlin noise vectors
    /// </summary>
    protected int dim;

    /// <summary>
    ///     random is used to create grid using vectors
    /// </summary>
    protected System.Random random;

    private float worleyConst;

    /// <summary>
    ///     worley constructor sets up worley noise object
    /// </summary>
    /// <param name="dim"></param>
    public Worley(int dim)
    {
        this.dim = dim;

        worleyConst = 0;

        for(int i1 = 0; i1 < this.dim; i1++)
        {
            worleyConst += (float) Math.Pow(2, 2);
        }

        worleyConst = (float) Math.Sqrt(worleyConst);
    }


    /// <summary>
    ///     generateCells is an abstract method to randomly generate random points in cells
    /// </summary>
    /// <param name="start">int array that stores the starting position at which nodes will be genrated</param>
    /// <param name="end">int array that stores the ending position at which nodes will stop generating</param>
    public abstract void generateCells(int[] start, int[] end);
    /// <summary>
    ///     increment utility method increments pos array by one
    /// </summary>
    /// <param name="pos">position array that is incremented</param>
    /// <param name="i1">index in position array to be incremented</param>
    private void increment(int[] pos, int i1)
    {
        if (i1 >= pos.Length)
        {
            throw new ArgumentException();
        }

        if (pos[i1] < 1)
        {
            pos[i1] += 1;
        }
        else
        {
            pos[i1] = -1;
            if (i1 + 1 < pos.Length)
            {
                increment(pos, i1 + 1);
            }
        }
    }

    public float sample(float[] pos)
    {
        if (pos.Length != this.dim)
        {
            throw new ArgumentException();
        }

        int[] cellPos = new int[this.dim];
        int[] relativePos = new int[this.dim];

        for (int i1 = 0; i1 < this.dim; i1++)
        {
            cellPos[i1] = (int)Math.Floor(pos[i1]);
        }

        float min = float.MaxValue;
        float dist = 0;
        float[] pointDist;

        //Debug.Log($"START\nPos:{string.Join(",", cellPos)}\t");

        T node = getCell(cellPos);
        T tmp;

        for (int i1 = 0; i1 < this.dim; i1++)
        {
            relativePos[i1] = -1;
        }

        for (int i1 = 0; i1 < Math.Pow(3, this.dim); i1++)
        {
            tmp = this.getCell(relativePos, node);

            if (tmp != null)
            {
                dist = 0;

                pointDist = tmp.val();

                for (int i2 = 0; i2 < this.dim; i2++)
                {
                    dist += (float)Math.Pow(Math.Abs(pointDist[i2] + cellPos[i2] + relativePos[i2] - pos[i2]), 2);
                }

                dist = (float)Math.Sqrt(dist);

                if (dist < min)
                {
                    min = dist;
                }
                increment(relativePos, 0);
            }
        }

        return min / worleyConst;
    }

    // <summary>
    ///     getVector gets the perlin noise vector at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector</param>
    /// <returns>float array of noise vector</returns>
    protected T getCell(int[] pos)
    {
        return getCell(pos, (T) root.get(1, 1));
    }

    /// <summary>
    ///     getNode gets the perlin noise noise at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector relative to startNode</param>
    /// <param name="startNode">Vector2DNode instance is the start position</param>
    /// <returns>Vector2DNode of noise vector at pos</returns>
    protected T getCell(int[] pos, T start)
    {
        if (pos.Length != this.dim)
        {
            throw new ArgumentException();
        }

        if (start == null)
        {
            throw new ArgumentException();
        }

        T pointer = start;

        int delta;

        for (int i1 = 0; i1 < this.dim; i1++)
        {
            delta = Math.Min(1, Math.Max(-1, pos[i1]));
            //Debug.Log($"i1:{i1}|delta:{delta}|pos[{i1}]:{pos[i1]}");
            if (delta != 0)
            {
                for (int i2 = 0; i2 < Math.Abs(pos[i1]); i2++)
                {
                    if (pointer.get(i1, delta) == null)
                    {
                        return null;
                    }
                    else
                    {
                        pointer = (T)pointer.get(i1, delta);
                    }
                }
            }
        }

        return pointer;
    }

    /// <summary>
    ///     toString method returns a repsentation of perlin noise in a string format
    /// </summary>
    /// <returns>string repsentation of perlin noise</returns>
    public abstract string toString();
}
