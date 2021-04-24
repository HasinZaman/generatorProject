using System;

/// <summary>
///     NoiseHeightMapGenerator is an abstract class to create a height map using perlin noise
/// </summary>
public abstract class NoiseHeightMapGenerator : HeightMapGenerator
{
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
    public NoiseHeightMapGenerator(float[][] templateVector, int seed, int[] nodeSize)
    {
        if(nodeSize.Length != 3)
        {
            throw new ArgumentException("nodeSize must contain only three elements [x size, y size, z size]");
        }

        this.random = new Random(seed);
        this.grid = new Grid(new int[3] { nodeSize[0], nodeSize[1], nodeSize[2] });

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
        return (f + 1) / 3;
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public abstract Grid getHeightMap();
}
