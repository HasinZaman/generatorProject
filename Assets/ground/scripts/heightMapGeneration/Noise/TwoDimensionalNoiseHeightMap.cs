using System;
using UnityEngine;

/// <summary>
///     TwoDimensionalNoiseHeightMap creates a height map that using 2d perlin noise 
/// </summary>
public class TwoDimensionalNoiseHeightMap : NoiseHeightMapGenerator
{

    /// <summary>
    ///     perlinNoiseVectors stores the perlin noise vectors used in generating noise
    /// </summary>
    float[][] perlinNoiseVectors;

    /// <summary>
    ///     perlinVectorDim stores the dim of perlinNoiseVectors
    /// </summary>
    uint[] perlinVectorDim;

    /// <summary>
    ///     shader is stores a computeShader used to calculate the noise
    /// </summary>
    ComputeShader shader;

    /// <summary>
    ///     NoiseHeightMapGenerator constructor intializes variables
    /// </summary>
    /// <param name="templateVector">templateVector paramater assigns the template vectors that will be used in calculating perlin noise</param>
    /// <param name="seed">seed paramater is used to intialize random</param>
    /// <param name="nodeSize">nodeSize is an array that stores the dimension of the final height map grid</param>
    /// <param name="perlinVectorDim">perlinVectorDim is an array that defines the size of perlinNoise vector</param>
    /// <param name="shader"></param>
    public TwoDimensionalNoiseHeightMap(float[][] templateVector, int seed, int[] nodeSize, uint[] perlinVectorDim, ComputeShader shader) : base( templateVector, seed, nodeSize)
    {
        if(perlinVectorDim.Length != 2)
        {
            throw new ArgumentException();
        }
        this.shader = shader;
        float[] vector;

        perlinNoiseVectors = new float[perlinVectorDim[0] * perlinVectorDim[1]][];

        this.perlinVectorDim = perlinVectorDim;

        for (int x = 0; x < perlinVectorDim[0]; x++)
        {
            for (int y = 0; y < perlinVectorDim[1]; y++)
            {
                vector = templateVector[random.Next(0, templateVector.Length)];
                perlinNoiseVectors[x + y * perlinVectorDim[0]]= vector;
            }
        }
    }

    /// <summary>
    ///     setVector sets a vector at a certian position
    /// </summary>
    /// <param name="x">x component of the location that will be modified</param>
    /// <param name="y">y component of the location that will be modified</param>
    /// <param name="vectorVal">vectorVal is a new vector that will be modified</param>
    public void setVector(int x, int y, float[] vectorVal)
    {
        if(x < 0 || x > perlinVectorDim[0])
        {
            throw new ArgumentOutOfRangeException($"x value needs to be between {0} <= x < {perlinVectorDim[0]}");
        }

        if(y < 0 || y > perlinVectorDim[1])
        {
            throw new ArgumentOutOfRangeException($"y value needs to be between {0} <= y < {perlinVectorDim[1]}");
        }

        if(vectorVal.Length != 2)
        {
            throw new ArgumentException("vectorVal cannot must have two values");
        }

        for(int i1 = 0; i1 < 2; i1++)
        {
            if(vectorVal[i1] < -1 || vectorVal[i1] > 1)
            {
                throw new ArgumentException($"vectorVal[{i1}] must between -1 <= vectorVal[{i1}] <= 1");
            }
        }

        Color tmpColor = new Color(vectorToPixel(vectorVal[0]), vectorToPixel(vectorVal[1]), 0);

        perlinNoiseVectors[x * y] = vectorVal;
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public override Grid getHeightMap()
    {
        int kernelHandle = shader.FindKernel("noiseCalc");

        int[] dim = this.grid.getDim();

        float[] samplePosDetla = new float[2] {(float) (perlinVectorDim[0] - 1) / (float)dim[0], (float)(perlinVectorDim[1] - 1) / (float)dim[2]};

        float[] samplePos = new float[2] { samplePosDetla[0] * 0.5f, samplePosDetla[1] * 0.5f };

        Node[] nodes = new Node[dim[0] * dim[1] * dim[2]];
        float sampleTemp;

        for (int x = 0; x < dim[0]; x++)
        {
            samplePos[0] = samplePosDetla[0] * (0.5f + (float)x);
            for (int z = 0; z < dim[2]; z++)
            {
                samplePos[1] = samplePosDetla[1] * (0.5f + (float)z);
                
                sampleTemp = sample(samplePos);

                sampleTemp *= (float)dim[1];
                
                for(int y = 0; y < dim[1]; y++)
                {
                    nodes[x + (y + z * dim[1]) * dim[0]] = new Node(Mathf.Clamp(sampleTemp - y, 0, 1));
                }
            }
        }

        grid.setNodes(nodes);
        return grid;
    }

    /// <summary>
    ///     Sample gets a noise value at x and y position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>a perlin noise value is returned at a given coordinate position</returns>
    public float sample(float x, float y)
    {
        if(x > perlinVectorDim[0] - 1)
        {
            throw new OverflowException($"x paramater greater than maximum size{perlinVectorDim[0] - 1}");
        }

        if(y > perlinVectorDim[1] - 1)
        {
            throw new OverflowException($"y paramater greater than maximum size{perlinVectorDim[1] - 1}");
        }
        return sample(new float[] { x, y });
    }

    /// <summary>
    ///     Sample gets a noise value at x and y position
    /// </summary>
    /// <param name="pos">float array containing x and y position</param>
    /// <returns>a perlin noise value is returned at a given coordinate position</returns>
    private float sample(float[] pos)
    {
        int[] sampleDim = grid.getDim();

        float[][] pointDist = new float[4][];
        float[][] pointVector = new float[4][];
        float[] pointValue = new float[4];

        uint[] perlinVectorDimTemp = new uint[2] { perlinVectorDim[0] - 1, perlinVectorDim[1] - 1 };

        for (int x1 = 0; x1 < 2; x1++)
        {
            for (int y1 = 0; y1 < 2; y1++)
            {
                pointDist[binaryToPositionIndex(x1, y1)] = new float[2] { (pos[0] % 1) - x1, (pos[1] % 1) - y1 };
            }
        }

        int[][] posRounded = new int[2][] {
            new int[] { (int)Math.Floor(pos[0]), (int)Math.Ceiling(pos[0]) },
            new int[] { (int)Math.Floor(pos[1]), (int)Math.Ceiling(pos[1]) }
        };

        //gets the dot product for every corner
        for (int x1 = 0; x1 < 2; x1++)
        {
            for (int y1 = 0; y1 < 2; y1++)
            {
                pointVector[binaryToPositionIndex(x1, y1)] = perlinNoiseVectors[posRounded[0][x1] + posRounded[1][y1] * perlinVectorDimTemp[0]];
            }
        }

        for (int x1 = 0; x1 < 2; x1++)
        {
            for (int y1 = 0; y1 < 2; y1++)
            {
                pointValue[binaryToPositionIndex(x1, y1)] = dotProduct(pointVector[binaryToPositionIndex(x1, y1)], pointDist[binaryToPositionIndex(x1, y1)]);

            }
        }
        // gets the interpolated value using the dot products

        // p(0,1) --- Line 1 --- P(1,1)
        //              |
        //            line 2
        //              |
        // p(0,0) --- Line 0 --- P(1,0)
        float line0Val = cosineInterpolate(
                pointValue[binaryToPositionIndex(0, 0)],
                pointValue[binaryToPositionIndex(1, 0)],
                pos[0] % 1
            );

        float line1Val = cosineInterpolate(
                pointValue[binaryToPositionIndex(0, 1)],
                pointValue[binaryToPositionIndex(1, 1)],
                pos[0] % 1
            );

        float line2Val = cosineInterpolate(
                line0Val,
                line1Val,
                pos[1] % 1
            );
        return (line2Val + 2) / 4;
    }

    /// <summary>
    /// binaryToPositionIndex converts a coord into a index for a one dimensitional 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>an int repsenting the index of a coord</returns>
    private int binaryToPositionIndex(int x, int y)
    {
        return x + y * 2;
    }
}
