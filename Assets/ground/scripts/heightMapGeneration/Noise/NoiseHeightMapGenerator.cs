using System;
using UnityEngine;

/// <summary>
///     NoiseHeightMapGenerator is an abstract class to create a height map using perlin noise
/// </summary>
public class NoiseHeightMapGenerator : HeightMapGenerator<Grid>
{
   
    /// <summary>
    ///     SampleIterator iterates through a range of values
    /// </summary>
    protected class SampleIterator : Iterator<float>
    {
        /// <summary>
        ///     Constructor sets up iterator
        /// </summary>
        /// <param name="start">start value of iterator</param>
        /// <param name="end">end value of iterator</param>
        /// <param name="samples">Number of samples that will be iterated through between start and end</param>
        public SampleIterator(float start, float end, int samples) : base(start, end)
        {
            if (samples < 3)
            {
                throw new ArgumentException("samples need to be greater than 2");
            }

            this.delta = (end - current) / (float)(samples - 1);
            this.current -= this.delta;
        }

        /// <summary>
        ///     next method updates the current value of the iterator with the next valid value
        /// </summary>
        /// <returns>float of the updated value of current</returns>
        public override float next()
        {
            if (this.current + this.delta > this.end + 0.00001)
            {
                throw new InvalidOperationException("Iterator has reached end");
            }
            this.current += this.delta;

            return this.current;
        }

        /// <summary>
        ///     hasNext checks if the next value of the iterator exists
        /// </summary>
        /// <returns>bool if the next value exists</returns>
        public override bool hasNext()
        {
            return !(this.current + this.delta > this.end + 0.00001);
        }

        /// <summary>
        ///     restart method sets current value back to start
        /// </summary>
        public void restart()
        {
            this.current = this.start;
            this.current -= this.delta;
        }
    }

    /// <summary>
    ///     NoiseParam stores values of used in getHeightMap method
    /// </summary>
    public class NoiseParam
    {
        /// <summary>
        ///     float array stores the starting sample points
        /// </summary>
        public float[] start;

        /// <summary>
        ///     float array stores the end sample points
        /// </summary>
        public float[] end;

        /// <summary>
        ///     int array that stores the number of samples stored between start and end
        /// </summary>
        public int[] dim;

        /// <summary>
        ///     int stores the height of grid map
        /// </summary>
        public int height;

        /// <summary>
        ///     Constructor of NoiseParam
        /// </summary>
        public NoiseParam()
        {
        }
    }

    /// <summary>
    ///     noise stores an object that implements Noise and is used to sample at given coordinates
    /// </summary>
    Noise noise;

    /// <summary>
    ///     NoiseHeightMapGenerator constructor intializes variables
    /// </summary>
    /// <param name="noise">Noise object used to sample</param>
    public NoiseHeightMapGenerator(Noise noise)
    {
        this.noise = noise;
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <param name="param"></param>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public Grid getHeightMap(object param)
    {
        if (typeof(NoiseParam) != param.GetType())
        {
            throw new ArgumentException();
        }

        Node[] nodes;

        NoiseParam noiseParam = (NoiseParam) param;

        float[] start = noiseParam.start;
        float[] end = noiseParam.end;
        int[] dim = noiseParam.dim;

        SampleIterator[] pos = new SampleIterator[3];

        float tmp;

        for (int i1 = 0; i1 < 3; i1++)
        {
            pos[i1] = new SampleIterator(start[i1], end[i1], dim[i1]);
        }

        nodes = new Node[dim[0] * dim[1] * dim[2]];

        for (int x = 0; x < dim[0]; x++)
        {
            pos[0].next();
            for (int z = 0; z < dim[2]; z++)
            {
                pos[2].next();

                for (int y = 0; y < dim[1]; y++)
                {
                    pos[1].next();
                    tmp = noise.sample(new float[] { pos[0].current, pos[1].current, pos[2].current, x, y, z });

                    
                    nodes[x + (y + z * dim[1]) * dim[0]] = new Node();

                    nodes[x + (y + z * dim[1]) * dim[0]].setValue(Math.Max(0, Math.Min(1, tmp)));

                }
                pos[1].restart();
            }
            pos[2].restart();
        }
        return new Grid(nodes, dim);
    }
}
