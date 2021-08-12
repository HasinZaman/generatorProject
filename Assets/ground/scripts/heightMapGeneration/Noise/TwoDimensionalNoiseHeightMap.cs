﻿using System;
using UnityEngine;

/// <summary>
///     TwoDimensionalNoiseHeightMap creates a height map that using 2d perlin noise 
/// </summary>
public class TwoDimensionalNoiseHeightMap : NoiseHeightMapGenerator
{

    /// <summary>
    ///     Vector2DNode stores perlin vector and neighbor node
    /// </summary>
    protected class Vector2DNode : VectorNode
    {
        /// <summary>
        ///     neighbor node left relative to the current node
        /// </summary>
        public Vector2DNode left = null;

        /// <summary>
        ///     neighbor node right relative to the current node
        /// </summary>
        public Vector2DNode right = null;

        /// <summary>
        ///     neighbor node up relative to the current node
        /// </summary>
        public Vector2DNode up = null;

        /// <summary>
        ///     neighbor node down relative to the current node
        /// </summary>
        public Vector2DNode down = null;

        /// <summary>
        ///     Constructor creates Vector2DNode object 
        /// </summary>
        /// <param name="vector">intial value of vector value</param>
        public Vector2DNode(float[] vector)
        {
            this.dim = 2;
            this.set(vector);
        }
    }

    /// <summary>
    ///     shader is stores a computeShader used to calculate the noise
    /// </summary>
    ComputeShader shader;

    /// <summary>
    ///     root stores the starting point of Noise Grid
    /// </summary>
    protected Vector2DNode root = new Vector2DNode(null);

    /// <summary>
    ///     NoiseHeightMapGenerator constructor intializes variables
    /// </summary>
    /// <param name="templateVector">templateVector paramater assigns the template vectors that will be used in calculating perlin noise</param>
    /// <param name="seed">seed paramater is used to intialize random</param>
    /// <param name="nodeSize">nodeSize is an array that stores the dimension of the final height map grid</param>
    /// <param name="perlinVectorDim">perlinVectorDim is an array that defines the size of perlinNoise vector</param>
    /// <param name="shader"></param>
    public TwoDimensionalNoiseHeightMap(float[][] templateVector, int seed, int[] perlinVectorDim, ComputeShader shader) : base( templateVector, seed)
    {
        if(perlinVectorDim.Length != 2)
        {
            throw new ArgumentException();
        }

        this.shader = shader;

        generateVectors(new int[2] { 0, 0 }, perlinVectorDim, templateVector);
    }

    /// <summary>
    ///     getHeightMap returns the final height map grid
    /// </summary>
    /// <returns>
    ///     Grid that repesents the heightMap created with perlin noise
    /// </returns>
    public override Grid getHeightMap()
    {
        float[] samplePos = new float[2];

        int[] nodeSizeTmp = new int[2] { nodeSize[0], nodeSize[1] };

        float sampleTemp;

        SampleIterator xPos, yPos;
        float start, end;

        start = 0;
        end = (float) perlinVectorDim[0] - 1;

        /*

        if(neighbors[NoiseNeighborCornerCode.edge1] != null)
        {
            start = -0.5f;
            nodeSizeTmp[0] += 1;
        }
        if(neighbors[NoiseNeighborCornerCode.edge2] != null)
        {
            end += 0.5f;
            nodeSizeTmp[0] += 1;
        }
        */
        xPos = new SampleIterator(start, end, nodeSizeTmp[0]) ;
        /*

        start = 0;
        end = (float) perlinVectorDim[1] - 1;

        if (neighbors[NoiseNeighborCornerCode.edge0] != null)
        {
            start -= 0.5f;
            nodeSizeTmp[1] += 1;
        }
        if(neighbors[NoiseNeighborCornerCode.edge3] != null)
        {
            end += 0.5f;
            nodeSizeTmp[1] += 1;
        }*/

        yPos = new SampleIterator(start, end, nodeSizeTmp[1]);

        Node[] nodes = new Node[nodeSizeTmp[0] * nodeSize[1] * nodeSizeTmp[1]];
        int x = 0;
        int z = 0;

        int i1 = 0;



        //Debug.Log($"X : {xPos.toString()}");
        //Debug.Log($"Y : {yPos.toString()}");

        while (xPos.hasNext())
        {
            samplePos[0] = xPos.next();
            z = 0;
            while (yPos.hasNext())
            {
                samplePos[1] = yPos.next();
                //Debug.Log($"{x},{z} = {samplePos[0]},{samplePos[1]}");
                sampleTemp = sample(samplePos[0], samplePos[1]);
                sampleTemp *= (float)nodeSize[1];
                for (int y = 0; y < nodeSize[1]; y++)
                {
                    nodes[x + (y + z * nodeSize[1]) * nodeSizeTmp[0]] = new Node(Mathf.Clamp(sampleTemp - y, 0, 1));
                    i1++;
                }
                z++;
            }
            yPos.restart();
            x++;
        }
        
        grid = new Grid(new int[] { nodeSizeTmp[0], nodeSize[1], nodeSizeTmp[1] });
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
        int[] perlinNoisePos = new int[2];
        if(x < -1 || x > perlinVectorDim[0])
        {
            throw new OverflowException($"x paramater greater than maximum size{perlinVectorDim[0] - 1}");
        }
        else if(x < 0)
        {
            perlinNoisePos[0] = -1;
            //Debug.Log("X LESS THAN 0");
        }
        else if(x > perlinVectorDim[0] - 1)
        {
            perlinNoisePos[0] = 1;
           // Debug.Log($"X Greater MAX {perlinVectorDim[0] - 1}");
        }

        if (y < -1 || y > perlinVectorDim[1])
        {
            throw new OverflowException($"y paramater greater than maximum size{perlinVectorDim[1] - 1}");
        }
        else if(y < 0)
        {
            perlinNoisePos[1] = -1;
        }
        else if( y > perlinVectorDim[1] - 1)
        {
            perlinNoisePos[1] = 1;
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

        float[][] pointDist = getPointDist(pos);
        float[][] pointVector = new float[4][];

        uint[] perlinVectorDimTemp = new uint[2] { perlinVectorDim[0] - 1, perlinVectorDim[1] - 1 };

        int[][] posRounded = new int[2][] {
            new int[] { (int)Math.Floor(pos[0]), (int)Math.Ceiling(pos[0]) },
            new int[] { (int)Math.Floor(pos[1]), (int)Math.Ceiling(pos[1]) }
        };

        for (int i1 = 0; i1 < pos.Length; i1++)
        {
            if(pos[i1] % 1 == 0)
            {
                return 0;
            }
        }

        //gets the dot product for every corner
        for (int x1 = 0; x1 < 2; x1++)
        {
            for (int y1 = 0; y1 < 2; y1++)
            {
                pointVector[binaryToPositionIndex(x1, y1)] = perlinNoiseVectors[posRounded[0][x1] + posRounded[1][y1] * perlinVectorDimTemp[0]];
            }
        }

        return sample(pos, pointDist, pointVector);
    }

    private float sample(float[] pos, float[][] pointDist, float[][] pointVector)
    {
        if(pos == null)
        {
            throw new ArgumentNullException("pos cannot be null");
        }
        else if(pos.Length != 2)
        {
            throw new ArgumentException("Pos must contain two values");
        }

        if (pointDist == null)
        {
            throw new ArgumentNullException("pointDist cannot be null");
        }
        else if(pointDist.Length != 4)
        {
            throw new ArgumentException("pointDist must contain 4 distances");
        }
        else
        {
            for(int i1 = 0; i1 < 4; i1++)
            {
                if(pointDist[i1] == null)
                {
                    throw new ArgumentException($"pointDist contains a null value at {i1}");
                }
            }
        }

        if (pointVector == null)
        {
            throw new ArgumentNullException("pointVector cannot be null");
        }
        else if(pointVector.Length != 4)
        {
            throw new ArgumentNullException("pointVector must contain 4 distances");
        }
        else
        {
            for (int i1 = 0; i1 < 4; i1++)
            {
                if (pointVector[i1] == null)
                {
                    throw new ArgumentException($"pointVector contains a null value at {i1}");
                }
            }
        }

        float[] pointValue = new float[4];

        uint[] perlinVectorDimTemp = new uint[2] { perlinVectorDim[0] - 1, perlinVectorDim[1] - 1 };

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

    private float sampleEdge(float[] pos, int neighborCode)
    {
        float[][] pointVector = new float[4][];

        int[][] posRounded = new int[2][] {
            new int[] { (int)Math.Floor(pos[0]), (int)Math.Ceiling(pos[0]) },
            new int[] { (int)Math.Floor(pos[1]), (int)Math.Ceiling(pos[1]) }
        };
        try
        {
            switch (neighborCode)
            {
                case NoiseNeighborCornerCode.corner0:
                    //Debug.Log("Corner0");
                    pointVector[binaryToPositionIndex(0, 0)] = neighbors[NoiseNeighborCornerCode.corner0].perlinNoiseVectors[(perlinVectorDim[0] - 1) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(1, 0)] = neighbors[NoiseNeighborCornerCode.edge0].perlinNoiseVectors[(0) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(0, 1)] = neighbors[NoiseNeighborCornerCode.edge1].perlinNoiseVectors[(perlinVectorDim[0] - 1) + (0) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(1, 1)] = perlinNoiseVectors[(0) + (0) * perlinVectorDim[0]];
                    //Debug.Log("Corner 0");
                    break;
                case NoiseNeighborCornerCode.corner1:
                    //Debug.Log("Corner1");
                    pointVector[binaryToPositionIndex(0, 0)] = neighbors[NoiseNeighborCornerCode.edge0].perlinNoiseVectors[(perlinVectorDim[0] - 1) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(1, 0)] = neighbors[NoiseNeighborCornerCode.corner1].perlinNoiseVectors[(0) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(0, 1)] = perlinNoiseVectors[(perlinVectorDim[0] - 1) + (0) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(1, 1)] = neighbors[NoiseNeighborCornerCode.edge2].perlinNoiseVectors[(0) + (0) * perlinVectorDim[0]];
                    break;
                case NoiseNeighborCornerCode.corner2:
                    //Debug.Log("Corner2");
                    pointVector[binaryToPositionIndex(0, 0)] = neighbors[NoiseNeighborCornerCode.edge1].perlinNoiseVectors[(perlinVectorDim[0] - 1) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(1, 0)] = perlinNoiseVectors[(0) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(0, 1)] = neighbors[NoiseNeighborCornerCode.corner2].perlinNoiseVectors[(perlinVectorDim[0] - 1) + (0) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(1, 1)] = neighbors[NoiseNeighborCornerCode.edge3].perlinNoiseVectors[(0) + (0) * perlinVectorDim[0]];
                    break;
                case NoiseNeighborCornerCode.corner3:
                    //Debug.Log("Corner3");
                    pointVector[binaryToPositionIndex(0, 0)] = perlinNoiseVectors[(perlinVectorDim[0] - 1) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(1, 0)] = neighbors[NoiseNeighborCornerCode.edge2].perlinNoiseVectors[(0) + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    pointVector[binaryToPositionIndex(0, 1)] = neighbors[NoiseNeighborCornerCode.edge3].perlinNoiseVectors[(perlinVectorDim[0] - 1) + (0) * perlinVectorDim[0]];
                    neighbors[NoiseNeighborCornerCode.corner3].gridDebug();
                    pointVector[binaryToPositionIndex(1, 1)] = neighbors[NoiseNeighborCornerCode.corner3].perlinNoiseVectors[(0) + (0) * perlinVectorDim[0]];
                    break;

                case NoiseNeighborCornerCode.edge0:
                    // Debug.Log("EDGE0");
                    for (int x = 0; x < 2; x++)
                    {
                        pointVector[binaryToPositionIndex(x, 0)] = neighbors[NoiseNeighborCornerCode.edge0].perlinNoiseVectors[posRounded[0][x] + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                        pointVector[binaryToPositionIndex(x, 1)] = perlinNoiseVectors[posRounded[0][x] + (0) * perlinVectorDim[0]];
                    }
                    break;
                case NoiseNeighborCornerCode.edge1:
                    //Debug.Log("EDGE1");
                    for (int y = 0; y < 2; y++)
                    {
                        pointVector[binaryToPositionIndex(0, y)] = neighbors[NoiseNeighborCornerCode.edge0].perlinNoiseVectors[(perlinVectorDim[0] - 1) + posRounded[1][y] * perlinVectorDim[0]];
                        pointVector[binaryToPositionIndex(1, y)] = perlinNoiseVectors[(0) + posRounded[1][y] * perlinVectorDim[0]];
                    }
                    break;
                case NoiseNeighborCornerCode.edge2:
                    //Debug.Log("EDGE2");
                    for (int y = 0; y < 2; y++)
                    {
                        pointVector[binaryToPositionIndex(0, y)] = neighbors[NoiseNeighborCornerCode.edge0].perlinNoiseVectors[(perlinVectorDim[0] - 1) + posRounded[1][y] * perlinVectorDim[0]];
                        pointVector[binaryToPositionIndex(1, y)] = perlinNoiseVectors[(0) + posRounded[1][y] * perlinVectorDim[0]];
                    }
                    break;
                case NoiseNeighborCornerCode.edge3:
                    //Debug.Log("EDGE3");
                    for (int x = 0; x < 2; x++)
                    {
                        pointVector[binaryToPositionIndex(x, 0)] = neighbors[NoiseNeighborCornerCode.edge0].perlinNoiseVectors[posRounded[0][x] + (0) * perlinVectorDim[0]];
                        pointVector[binaryToPositionIndex(x, 1)] = perlinNoiseVectors[posRounded[0][x] + (perlinVectorDim[1] - 1) * perlinVectorDim[0]];
                    }
                    break;
                default:
                    throw new ArgumentException($"Invalid NoiseNeighborCornerCode {neighborCode}");
            }
        }
        catch(NullReferenceException e)
        {
            return 0;
        }

        
        string tmp = "";
        float[] tmpVector;
        
        for(int x = 0; x <  2; x++)
        {
            for(int y = 0; y < 2; y++)
            {
                tmpVector = pointVector[binaryToPositionIndex(x, y)];
                if(tmpVector == null)
                {
                    Debug.Log($"Pos:{pos[0]},{pos[1]}");
                }
                tmp += $"{tmpVector[0]},{tmpVector[1]}\t";
            }
            tmp +="\n";
        }
        Debug.Log(tmp);
        Debug.Log($"OG POS:{pos[0]},{pos[1]}");
        for (int i1 = 0; i1 < pos.Length; i1++)
        {
            if(pos[i1] < 0)
            {
                pos[i1] = 1 + pos[i1];
            }
        }
        Debug.Log($"SAMPLE:{sample(pos, getPointDist(pos), pointVector)}");
        return sample(pos, getPointDist(pos), pointVector);
    }

    private float[][] getPointDist(float[] pos)
    {
        float[][] pointDist = new float[4][];

        for (int x1 = 0; x1 < 2; x1++)
        {
            for (int y1 = 0; y1 < 2; y1++)
            {
                pointDist[binaryToPositionIndex(x1, y1)] = new float[2] { (pos[0] % 1) - x1, (pos[1] % 1) - y1 };
            }
        }

        return pointDist;
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

    private uint coordToPerlinPositionIndex(int x, int y)
    {
        return (uint)(x + y * perlinVectorDim[1]);
    }

    private uint coordToPerlinPositionIndex(uint x, uint y)
    {
        return (uint) (x + y * perlinVectorDim[1]);
    }

    public void gridDebug()
    {
        string tmp = "";

        for(int y = (int) perlinVectorDim[1] - 1; y >= 0; y--)
        {
            for(int x = 0; x < perlinVectorDim[0]; x++)
            {
                tmp += $"({perlinNoiseVectors[coordToPerlinPositionIndex(x,y)][0]},{perlinNoiseVectors[coordToPerlinPositionIndex(x, y)][1]})\t";
            }
            tmp += "\n";
        }
        Debug.Log(tmp);
    }
}
