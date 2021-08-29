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
    /// <param name="templateVector">templateVector paramater assigns the template vectors that will be used in calculating perlin noise</param>
    /// <param name="seed">seed paramater is used to intialize random</param>
    /// <param name="nodeSize">nodeSize is an array that stores the dimension of the final height map grid</param>
    public NoiseHeightMapGenerator()
    {
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public abstract Grid getHeightMap(object param);
}
