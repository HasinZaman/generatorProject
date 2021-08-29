﻿using System;
using UnityEngine;

/// <summary>
///     NoiseHeightMapGenerator is an abstract class to create a height map using perlin noise
/// </summary>
public abstract class NoiseHeightMapGenerator : HeightMapGenerator<Grid>
{
    /// <summary>
    ///     VectorNode stores noise vector
    /// </summary>
    protected class VectorNode
    {
        /// <summary>
        ///     vector stores the perlin noise vector
        /// </summary>
        protected float[] vector;

        /// <summary>
        ///     dim stores size of the perlin noise vectors
        /// </summary>
        protected int dim;

        public VectorNode()
        {

        }

        /// <summary>
        ///     Set method assigns vector value
        /// </summary>
        /// <param name="vector">New vector value</param>
        public void set(float[] vector)
        {
            if (dim != vector.Length)
            {
                throw new ArgumentException($"vector lenght must have exactly {dim} elements");
            }
            this.vector = vector;
        }

        /// <summary>
        ///     get method returns vector value
        /// </summary>
        /// <returns>A float array of vector</returns>
        public float[] val()
        {
            return this.vector;
        }

        /// <summary>
        ///     toString method converts object into a string repersentation
        /// </summary>
        /// <returns>string format of VectorNode object</returns>
        public string toString()
        {
            string temp = "(";

            temp += $"{this.vector[0]}";

            for (int i1 = 1; i1 < this.vector.Length; i1++)
            {
                temp += $",{this.vector[i1]}";
            }

            temp += ")";

            return temp;
        }
    }

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
    ///     random is used to create grid using vectors
    /// </summary>
    protected System.Random random;

    /// <summary>
    ///     grid is the output height map
    /// </summary>
    protected Grid grid;

    /// <summary>
    ///     templateVector is an arry of vectors that used in calculating perlin noise
    /// </summary>
    protected float[][] templateVector;

    /// <summary>
    ///     NoiseHeightMapGenerator constructor intializes variables
    /// </summary>
    /// <param name="templateVector">templateVector paramater assigns the template vectors that will be used in calculating perlin noise</param>
    /// <param name="seed">seed paramater is used to intialize random</param>
    /// <param name="nodeSize">nodeSize is an array that stores the dimension of the final height map grid</param>
    public NoiseHeightMapGenerator(float[][] templateVector, int seed)
    {
        this.random = new System.Random(seed);

        this.templateVector = templateVector;
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public abstract Grid getHeightMap(object param);
}
