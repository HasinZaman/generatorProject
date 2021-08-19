using System;

/// <summary>
///     NoiseHeightMapGenerator is an abstract class to create a height map using perlin noise
/// </summary>
public abstract class NoiseHeightMapGenerator : HeightMapGenerator
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
                temp += $"{this.vector[i1]}";
            }

            temp += ")";

            return temp;
        }
    }

    /// <summary>
    ///     Iterator abstract class handles through a range of values
    /// </summary>
    protected abstract class Iterator<T>
    {
        /// <summary>
        ///     start value of Iterator
        /// </summary>
        protected T start;

        /// <summary>
        ///     end value of Iterator
        /// </summary>
        protected T end;

        /// <summary>
        ///     delta float step size required to iterate from start to end instance
        /// </summary>
        protected T delta;

        /// <summary>
        ///     current value of the iterator
        /// </summary>
        public T current;

        /// <summary>
        ///     Constructor sets up iterator
        /// </summary>
        /// <param name="start">start value of iterator</param>
        /// <param name="end">end value of iterator</param>
        /// <param name="samples">Number of samples that will be iterated through between start and end</param>
        public SampleIterator(float start, float end, int samples)
        {
            this.start = start;
            this.current = start;
            this.end = end;

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
    protected Random random;

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
        this.random = new Random(seed);

        this.templateVector = templateVector;
    }

    /// <summary>
    ///     vectorToPixel converts a int from templateVector into a texture value
    /// </summary>
    /// <param name="f">int that is coverted into a texture value</param>
    /// <returns>
    ///     a float that can repersents f for a texture
    /// </returns>
    protected float vectorToPixel(float f)
    {
        return (f + 1) / 2;
    }

    /// <summary>
    ///     dotProduct returns the dotProduct of two vectors
    /// </summary>
    /// <param name="v1">v1 is a vector of n length</param>
    /// <param name="v2">v2 is a vector of n length</param>
    /// <returns>
    ///     float of the dotProduct of v1 and v2
    /// </returns>
    protected float dotProduct(float[] v1, float[] v2)
    {
        if (v1.Length != v2.Length)
        {
            throw new ArgumentException($"vector 1 and vector need to be the same size. v1.length={v1.Length} v2.length={v2.Length}");
        }
        float temp = 0;
        for (int i1 = 0; i1 < v1.Length; i1++)
        {
            temp += v1[i1] * v2[i1];
        }
        return temp;
    }

    /// <summary>
    ///     cosineInterpolate interpolates between y1 and y2
    /// </summary>
    /// <param name="y1">y1 is the first value</param>
    /// <param name="y2">y2 is the second value</param>
    /// <param name="intermediaryPoint">intermediaryPoint is the x position which an interpolated value will be extracted at</param>
    /// <returns>float of the interpolated value between y1 and y2</returns>
    protected float cosineInterpolate(float y1, float y2, float intermediaryPoint)
    {
        float mu = (float)(1 - Math.Cos(intermediaryPoint * Math.PI)) / 2;

        return y1 * (1 - mu) + y2 * mu;
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public abstract Grid getHeightMap();

    /// <summary>
    ///     generateVectors is an abstract method to randomly generate perlin noise vector nodes
    /// </summary>
    /// <param name="start">int array that stores the starting position at which nodes will be genrated</param>
    /// <param name="end">int array that stores the ending position at which nodes will stop generating</param>
    /// <param name="template">array of perlin noise vectors</param>
    public abstract void generateVectors(int[] start, int[] end, float[][] template);

    /// <summary>
    ///     getVector gets the perlin noise vector at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector</param>
    /// <returns>float array of noise vector</returns>
    public abstract float[] getVector(int[] pos);
}
