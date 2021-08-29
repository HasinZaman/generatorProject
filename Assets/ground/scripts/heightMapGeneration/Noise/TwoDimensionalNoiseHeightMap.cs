using System;
using UnityEngine;

/// <summary>
///     TwoDimensionalNoiseHeightMap creates a height map that using 2d perlin noise 
/// </summary>
public class TwoDimensionalNoiseHeightMap : NoiseHeightMapGenerator
{
    /// <summary>
    ///     GridParam stores custom paramater values for getHeightMap method
    /// </summary>
    public class GridParam
    {
        private float[] start = new float[2];
        private float[] end = new float[2];
        private int[] samples = new int[2];
        public int height;
        public float amplitude = 1;
        public float bias = 0;

        /// <summary>
        ///     setStart method sets start instance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setStart(float x, float y)
        {
            start[0] = x;
            start[1] = y;
        }

        /// <summary>
        ///     setEnd method set end instance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setEnd(float x, float y)
        {
            end[0] = x;
            end[1] = y;
        }

        /// <summary>
        ///     setSamples method set samples instance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setSamples(int x, int y)
        {
            samples[0] = x;
            samples[1] = y;
        }

        /// <summary>
        ///     getStart method returns start instance
        /// </summary>
        /// <returns>float[2] of start instance</returns>
        public float[] getStart()
        {
            return start;
        }

        /// <summary>
        ///     getEnd method returns end instance
        /// </summary>
        /// <returns>float[2] of end instance</returns>
        public float[] getEnd()
        {
            return end;
        }

        /// <summary>
        ///     getSamples method returns sample instance
        /// </summary>
        /// <returns>int[2] of sample instance</returns>
        public int[] getSamples()
        {
            return samples;
        }
    }

    private Perlin2D perlin2D;

    /// <summary>
    ///     NoiseHeightMapGenerator constructor intializes variables
    /// </summary>
    /// <param name="templateVector">templateVector paramater assigns the template vectors that will be used in calculating perlin noise</param>
    /// <param name="seed">seed paramater is used to intialize random</param>
    /// <param name="perlinVectorDim">perlinVectorDim is an array that defines the size of perlinNoise vector</param>
    /// <param name="shader"></param>
    public TwoDimensionalNoiseHeightMap(float[][] templateVector, int seed, int[] perlinVectorDim) : base()
    {
        if(perlinVectorDim.Length != 2)
        {
            throw new ArgumentException();
        }
        perlin2D = new Perlin2D(templateVector, seed, perlinVectorDim);
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public override Grid getHeightMap(object param)
    {
        if (typeof(GridParam) != param.GetType())
        {
            throw new ArgumentException();
        }

        Node[] nodes;

        float tmp;

        GridParam gridParam = (GridParam) param;

        float[] start = gridParam.getStart();
        float[] end = gridParam.getEnd();
        int[] dim = new int[3] { gridParam.getSamples()[0], gridParam.height, gridParam.getSamples()[1] };

        SampleIterator[] pos = new SampleIterator[2];
        
        for (int i1 = 0; i1 < 2; i1++)
        {
            pos[i1] = new SampleIterator(start[i1], end[i1], dim[2 * i1]);
        }

        nodes = new Node[dim[0] * dim[1] * dim[2]];

        for (int x = 0; x < dim[0]; x++)
        {
            for(int z = 0; z < dim[2]; z++)
            {
                Debug.Log(perlin2D.sample(new float[] { pos[0].current, pos[1].current }));
                tmp = perlin2D.sample(new float[] { pos[0].current, pos[1].current }) * (dim[1] - 1) * gridParam.amplitude + gridParam.bias;
                
                for (int y = 0; y < dim[1]; y++)
                {
                    nodes[x + (y + z * dim[1]) * dim[0]] = new Node();
                    if (tmp - y >= 1)
                    {
                        nodes[x + (y + z * dim[1]) * dim[0]].setValue(1);
                    }
                    else if(tmp - y <= 0)
                    {
                        nodes[x + (y + z * dim[1]) * dim[0]].setValue(0);
                    }
                    else
                    {
                        nodes[x + (y + z * dim[1]) * dim[0]].setValue(tmp%1);
                    }

                }
                pos[1].next();
            }
            pos[0].next();
            pos[1].restart();
        }
        return new Grid(nodes, dim);
    }

    public string toString()
    {
        return perlin2D.toString();
    }
}