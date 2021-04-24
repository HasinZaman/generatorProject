using System;
using UnityEngine;
/// <summary>
///     TwoDimensionalNoiseHeightMap creates a height map that using 2d perlin noise 
/// </summary>
public class TwoDimensionalNoiseHeightMap : NoiseHeightMapGenerator
{
    /// <summary>
    ///     vectors is a 2d texture used to store vectors(that are used in calculating perlin noise) and provide as a means of transfering information to the gpu
    /// </summary>
    Texture2D perlinVector;

    /// <summary>
    ///     NoiseHeightMapGenerator constructor intializes variables
    /// </summary>
    /// <param name="templateVector">templateVector paramater assigns the template vectors that will be used in calculating perlin noise</param>
    /// <param name="seed">seed paramater is used to intialize random</param>
    /// <param name="nodeSize">nodeSize is an array that stores the dimension of the final height map grid</param>
    /// <param name="perlinVectorDim">perlinVectorDim is an array that defines the size of perlinNoise vector</param>
    public TwoDimensionalNoiseHeightMap(float[][] templateVector, int seed, int[] nodeSize, int[] perlinVectorDim) : base( templateVector, seed, nodeSize)
    {
        if(perlinVectorDim.Length != 2)
        {
            throw new ArgumentException();
        }
        
        Color tmpColor = new Color();
        float[] vector;

        perlinVector = new Texture2D(perlinVectorDim[0], perlinVectorDim[1]);

        for(int x = 0; x < perlinVectorDim[0]; x++)
        {
            for(int y = 0; y < perlinVectorDim[1]; y++)
            {
                vector = templateVector[random.Next(0, templateVector.Length)];

                tmpColor.r = vectorToPixel(vector[0]);
                tmpColor.g = vectorToPixel(vector[1]);

                perlinVector.SetPixel(x, y, tmpColor);
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
        if(x < 0 || x > perlinVector.width)
        {
            throw new ArgumentOutOfRangeException($"x value needs to be between {0} <= x < {perlinVector.width}");
        }

        if(y < 0 || y > perlinVector.height)
        {
            throw new ArgumentOutOfRangeException($"y value needs to be between {0} <= y < {perlinVector.height}");
        }

        if(vectorVal.Length != 2)
        {
            throw new ArgumentException("vectorVal cannot must have two values");
        }
        else if(vectorVal[0] == -1 && vectorVal[0] == 0 && vectorVal[0] == 1)
        {
            throw new ArgumentException("vectorVal[0] must between -1 <= vectorVal[0] <= 1");
        }
        else if (vectorVal[1] == -1 && vectorVal[1] == 0 && vectorVal[1] == 1)
        {
            throw new ArgumentException("vectorVal[1] must between -1 <= vectorVal[0] <= 1");
        }

        Color tmpColor = new Color(vectorToPixel(vectorVal[0]), vectorToPixel(vectorVal[1]), 0);

        perlinVector.SetPixel(x, y, tmpColor);
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public override Grid getHeightMap()
    {
        throw new NotImplementedException();
    }
}
