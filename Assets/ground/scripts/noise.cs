using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noise
{
    //declaring object variables

    public static double[][] SET1 = new double[][] {
        new double[2] { 0, 1},
        new double[2] { 1, 1 },
        new double[2] { 1, 0 },
        new double[2] { 1, -1 },
        new double[2] { 0, -1 },
        new double[2] { -1, -1 },
        new double[2] { -1, 0 },
        new double[2] { -1, 1 }
    };
    public static float[][] SET2 = new float[][] {
        new float[3] { 1,  1,  0},
        new float[3] {-1,  1,  0},
        new float[3] { 1, -1,  0},
        new float[3] {-1, -1,  0},
        new float[3] { 1,  0,  1},
        new float[3] {-1,  0,  1},
        new float[3] { 1,  0, -1},
        new float[3] {-1,  0, -1},
        new float[3] { 0,  1,  1},
        new float[3] { 0, -1,  1},
        new float[3] { 0,  1, -1},
        new float[3] { 1, -1, -1}
    };


    public static double[][] SET3 = new double[][] {
        new double[3] { 1,  1,  0},
        new double[3] {-1,  1,  0},
        new double[3] { 1, -1,  0},
        new double[3] {-1, -1,  0},
        new double[3] { 1,  0,  1},
        new double[3] {-1,  0,  1},
        new double[3] { 1,  0, -1},
        new double[3] {-1,  0, -1},
        new double[3] { 0,  1,  1},
        new double[3] { 0, -1,  1},
        new double[3] { 0,  1, -1},
        new double[3] { 1, -1, -1},

        new double[3] { 1,  1,  0},
        new double[3] {-1, -1,  0},
        new double[3] { 0, -1, 1},
        new double[3] { 0, -1, -1}
    };


    //grid variable holds the vectors used inorder to generate perlin noise
    public List<List<List<double[]>>> grid = new List<List<List<double[]>>>();

    //vectors variable holds a pool of vectors that will be inserted into grid
    private double[][] vectors;
    
    public int seed;
    private System.Random random;

    //sets up the noise algorthim for 2d perlin noise
    public noise(int seedRaw, double[][] vectersRaw, int width, int height)
    {
        seed = seedRaw;
        vectors = vectersRaw;
        random = new System.Random(seed);

        createGrid(width, height, 1);
    }

    //sets up the noise algorthim for 3d perlin noise
    public noise(int seedRaw, double[][] vectersRaw, int width, int height, int depth)
    {
        seed = seedRaw;
        vectors = vectersRaw;
        random = new System.Random(seed);

        createGrid(width, height, depth);
    }

    //create the grid by using the pool of vectors in vectors array
    private void createGrid(int width, int height, int depth)
    {
        for (int z = 0; z < depth; z++)
        {
            grid.Add(new List<List<double[]>>());

            for (int y = 0; y < height; y++)
            {
                grid[z].Add(new List<double[]>());

                for (int x = 0; x < width; x++)
                {
                    grid[z][y].Add(vectors[random.Next(vectors.Length)]);
                }
            }
        }
    }

    //calculates the dot products of the 4 corners in a cell
    private double[] vecterDistDotProduct(double[][] vectors, double[][] dist)
    {
        double[] dotProducts = new double[vectors.Length];
        double temp;

        for (int i1 = 0; i1 < vectors.Length; i1++)
        {
            temp = 0;

            for (int i2 = 0; i2 < vectors[i1].Length; i2++)
            {
                temp += vectors[i1][i2] * dist[i1][i2];
            }

            dotProducts[i1] = temp;
        }
        return dotProducts;
    }

    //returns the itermediate value using cosine interpolation
    private double cosineInterpolate(double y1, double y2, double intermediaryPoint)
    {
        double mu = (1 - Math.Cos(intermediaryPoint * Math.PI)) / 2;

        return y1 * (1 - mu) + y2 * mu;
    }

    //checks if the sample coordinate are valid - 2d version 
    private bool sampleCoordCheck(double x, double y)
    {
        if (Math.Round(x) == x || Math.Round(y) == y)
        {
            return true;
        }

        if (x < 0)
        {
            return true;
        }

        if (y < 0)
        {
            return true;
        }

        if (x > grid[0][0].Count - 1)
        {
            return true;
        }

        if (y > grid[0].Count - 1)
        {
            return true;
        }

        return false;
    }
    //checks if the sample coordinate are valid - 3d version 
    private bool sampleCoordCheck(double x, double y, double z)
    {
        if (Math.Round(x) == x || Math.Round(y) == y || Math.Round(z) == z)
        {
            return true;
        }

        if (x < 0)
        {
            return true;
        }

        if (y < 0)
        {
            return true;
        }

        if (z < 0)
        {
            return true;
        }

        if (x > grid[0][0].Count)
        {
            return true;
        }

        if (y > grid[0].Count)
        {
            return true;
        }

        if (z > grid.Count)
        {
            return true;
        }

        return false;
    }

    //gets the sample value - 2d version
    private double sample(double x, double y, double[][] pointVectors)
    {
        //checks if the sample coordinate is valid
        if (sampleCoordCheck(x, y))
        {
            //if x or y is a whole return 0
            
            if (Math.Round(x) == x || Math.Round(y) == y)
            {
                return 0;
                /*
                 *if x or y equals to zero,
                 *the perlin noise algorthim would return 0 after doing all the caluculation
                 * by return 0 now, the algorthim can take a short cut without doing any of the calculations required
                 */
            }

            double newX = x, newY = y;

            //checks if coordinate is are too small or too small. then fixes the coordinates
            if (x < 0)
            {
                newX = grid[0][0].Count + x % grid[0][0].Count;
            }
            else if (x > grid[0][0].Count - 1)
            {
                newX = x % grid[0][0].Count - 1;
            }

            if (y < 0)
            {
                newY = grid[0].Count + y % grid[0].Count;
            }
            else if (y > grid[0].Count - 1)
            {
                newY = y % grid[0].Count - 1;
            }

            //if any fixes are required then restart the sample function with the fixed coordinates
            if (newX != x || newY != y)
            {
                return sample(newX, newY, pointVectors);
            }
        }

        //gets the distance to the four closest corners
        double[][] pointDist = new double[][]
        {
            new double[]{x % 1, y % 1},
            new double[]{x % 1 - 1, y % 1},
            new double[]{x % 1 - 1, y % 1 - 1},
            new double[]{x % 1, y % 1 - 1}
        };

        //gets the dot product of all the corners
        double[] pointValue = vecterDistDotProduct(pointVectors, pointDist);

        //interoplate the sample values
        double A = cosineInterpolate(
            pointValue[0],
            pointValue[1],
            x % 1
        );

        double B = cosineInterpolate(
            pointValue[3],
            pointValue[2],
            x % 1
        );

        double C = cosineInterpolate(
            A,
            B,
            y % 1
        );

        return C;
    }
    
    //gets the sample value - 3d version
    private double sample(double x, double y, double z, double[][] pointVectors)
    {
        //checks if the sample coordinate is valid
        if (sampleCoordCheck(x, y, z))
        {
            //if x, y or z is a whole return 0
            if (Math.Round(x) == x || Math.Round(y) == y || Math.Round(z) == z)
            {
                /*
                 *if x, y or z equals to zero,
                 *the perlin noise algorthim would return 0 after doing all the caluculation
                 * by return 0 now, the algorthim can take a short cut without doing any of the calculations required
                 */
                return 0;
            }

            double newX = x, newY = y, newZ = z;

            //checks if coordinate is are too small or too small. then fixes the coordinates
            if (x < 0)
            {
                newX = grid[0][0].Count + x % grid[0][0].Count;
            }
            else if (x > grid[0][0].Count)
            {
                newX = x % grid[0][0].Count;
            }

            if (y < 0)
            {
                newY = grid[0].Count + y % grid[0].Count;
            }
            else if (y > grid[0].Count)
            {
                newY = y % grid[0].Count;
            }

            if (z < 0)
            {
                newZ = grid.Count + z % grid.Count;
            }
            else if (z > grid.Count)
            {
                newZ = z % grid.Count;
            }

            //if any fixes are required then restart the sample function with the fixed coordinates
            if (newX != x || newY != y || newZ != z)
            {
                return sample(newX, newY, newZ, pointVectors);
            }
        }


        //gets the distance to nearest eight closest corners
        double[][] pointDist = new double[][]
        {
            new double[]{ x % 1, y % 1, z % 1},
            new double[]{ x % 1 - 1, y % 1, z % 1},
            new double[]{ x % 1 -1, y % 1 - 1, z % 1},
            new double[]{ x % 1, y % 1 - 1, z % 1},

            new double[]{ x % 1, y % 1, z % 1 - 1},
            new double[]{ x % 1 - 1, y % 1, z % 1 - 1},
            new double[]{ x % 1 - 1, y % 1 - 1, z % 1 - 1},
            new double[]{ x % 1, y % 1 - 1, z % 1 - 1}
        };

        //gets the dot product for every corner
        double[] pointValue = vecterDistDotProduct(pointVectors, pointDist);

        // gets the interpolated value using the dot products
        double A = cosineInterpolate(
            pointValue[0],
            pointValue[1],
            x % 1
        );

        double B = cosineInterpolate(
            pointValue[3],
            pointValue[2],
            x % 1
        );

        double C = cosineInterpolate(
            A,
            B,
            y % 1
        );

        double D = cosineInterpolate(
            pointValue[4],
            pointValue[5],
            x % 1
        );

        double E = cosineInterpolate(
            pointValue[7],
            pointValue[6],
            x % 1
        );

        double F = cosineInterpolate(
            D,
            E,
            y % 1
        );

        double G = cosineInterpolate(
            C,
            F,
            z % 1
        );

        return G;
    }

    //gets the sample - 2d version
    public double sample(double x, double y)
    {
        //declares the point vectors
        double[][] pointVectors = new double[4][]{
            null,
            null,
            null,
            null
            };

        //set up the default values of the orientation variables
        //the orientation variables are used to find the vectors that will be used to calcuate the sample
        double left = Math.Floor(x);
        double right = Math.Ceiling(x);
        double bottom = Math.Floor(y);
        double top = Math.Ceiling(y);

        //in the case of an invalid oreintation value, the grid is assumed to be repeating in the x and y axis and the invaild oreintation value is replaced.
        if (left == -1)
        {
            left = grid[0][0].Count - 1;
        }
        else if (right == grid[0][0].Count)
        {
            right = 0;
        }

        if (bottom == -1)
        {
            bottom = grid[0].Count - 1;
        }
        else if (top == grid[0].Count)
        {
            top = 0;
        }

        //sets up point vectors using the orrientation variables
        pointVectors[0] = grid[0][Convert.ToInt32(bottom)][Convert.ToInt32(left)];
        pointVectors[1] = grid[0][Convert.ToInt32(bottom)][Convert.ToInt32(right)];
        pointVectors[2] = grid[0][Convert.ToInt32(top)][Convert.ToInt32(right)];
        pointVectors[3] = grid[0][Convert.ToInt32(top)][Convert.ToInt32(left)];

        //uses the private sample function to get the sample
        return sample(x, y, pointVectors);
    }
    public double sample(double x, double y, double z)
    {
        //declares the point vectors
        double[][] pointVectors = new double[8][]{
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
            };

        //set up the default values of the orientation variables
        //the orientation variables are used to find the vectors that will be used to calcuate the sample
        double left = Math.Floor(x);
        double right = Math.Ceiling(x);
        double bottom = Math.Floor(y);
        double top = Math.Ceiling(y);
        double back = Math.Floor(z);
        double front = Math.Ceiling(z);
        
        //in the case of an invalid oreintation value, the grid is assumed to be repeating in the x and y axis and the invaild oreintation value is replaced.
        if (Math.Floor(x) == -1)
        {
            left = grid[0][0].Count - 1;
        }
        else if (Math.Ceiling(x) == grid[0][0].Count)
        {
            right = 0;
        }

        if (Math.Floor(y) == -1)
        {
            bottom = grid[0].Count - 1;
        }
        else if (Math.Ceiling(y) == grid[0].Count)
        {
            top = 0;
        }

        if (Math.Floor(z) == -1)
        {
            back = grid.Count - 1;
        }
        else if (Math.Ceiling(z) == grid.Count)
        {
            front = 0;
        }

        //sets up point vectors using the orrientation variables
        pointVectors[0] = grid[Convert.ToInt32(back)][Convert.ToInt32(bottom)][Convert.ToInt32(left)];
        pointVectors[1] = grid[Convert.ToInt32(back)][Convert.ToInt32(bottom)][Convert.ToInt32(right)];
        pointVectors[2] = grid[Convert.ToInt32(back)][Convert.ToInt32(top)][Convert.ToInt32(right)];
        pointVectors[3] = grid[Convert.ToInt32(back)][Convert.ToInt32(top)][Convert.ToInt32(left)];

        pointVectors[4] = grid[Convert.ToInt32(front)][Convert.ToInt32(bottom)][Convert.ToInt32(left)];
        pointVectors[5] = grid[Convert.ToInt32(front)][Convert.ToInt32(bottom)][Convert.ToInt32(right)];
        pointVectors[6] = grid[Convert.ToInt32(front)][Convert.ToInt32(top)][Convert.ToInt32(right)];
        pointVectors[7] = grid[Convert.ToInt32(front)][Convert.ToInt32(top)][Convert.ToInt32(left)];

        //uses the private sample function to get the sample
        return sample(x, y, z, pointVectors);
    }

    //find the index of a corner vector - used for find the samples that are inbetween chunks
    private int cornerPointFinder(int counter, int axis, int maxVal)
    {
        if(axis == 0)
        {
            if(counter == 0 || counter == 3)
            {
                return maxVal - 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (counter == 0 || counter == 1)
            {
                return maxVal - 1;
            }
            else
            {
                return 0;
            }

        }
    }
    //find the index of a edge vector - used for find the samples that are inbetween chunks
    private int edgePointFinder1(double val, int counter, int axis)
    {
        if (axis == 0)
        {
            if (counter == 0 || counter == 3)
            {
                return Convert.ToInt32(Math.Floor(val));
            }
            else
            {
                return Convert.ToInt32(Math.Ceiling(val));
            }
        }
        else
        {
            if (counter == 0 || counter == 1)
            {
                return Convert.ToInt32(Math.Floor(val));
            }
            else
            {
                return Convert.ToInt32(Math.Ceiling(val));
            }
        }
    }
    //find the index of a edge vector - used for find the samples that are inbetween chunks
    private int edgePointFinder2(int maxVal, int counter, int axis)
    {
        if (axis == 0)
        {
            if (counter == 0 || counter == 3)
            {
                return maxVal - 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (counter == 0 || counter == 1)
            {
                return maxVal - 1;
            }
            else
            {
                return 0;
            }
        }
    }

    // comment function
    //gets the sample that is between two chunks
    public double sample(double x, double y, List<List<List<double[]>>>[] suroundingGrid)
    {
        double[][] pointVectors = new double[4][]{
            null,
            null,
            null,
            null
            };

        List<List<List<double[]>>>[] grids = new List<List<List<double[]>>>[4];
        
        //region holds the index of chunks that going to be used
        double[] region = new double[2] {0, 0};
        double zone = 0;
        
        //finds the region depending on the coordinates
        if (x < 0)
        {
            region[0] -= 1;
        }
        else if (x > grid[0].Count - 1)
        {
            region[0] += 1;
        }

        if (y < 0)
        {
            region[1] -= 1;
        }
        else if (y > grid[0][0].Count - 1)
        {
            region[1] += 1;
        }
        zone = (1 + region[1]) * 3 + (1 + region[0]);

        if (zone >= 4)
        {
            zone -= 1;
        }

        if(region[1] == 0 && region[0] == 0)
        {
            return sample(x, y);
        }
        
        double[] xVal;
        double[] yVal;

        //find xVal and yVal depending on region
        if (region[0] < 0)
        {
            xVal = new double[2]
            {
                0,1
            };
        }
        else if (region[0] > 0)
        {
            xVal = new double[2]
            {
                1,0
            };
        }
        else
        {
            xVal = new double[2]
            {
                0,0
            };
        }

        if (region[1] < 0)
        {
            yVal = new double[2]
            {
                0,1
            };
        }
        else if (region[1] > 0)
        {
            yVal = new double[2]
            {
                1,0
            };
        }
        else
        {
            yVal = new double[2]
            {
                0,0
            };
        }

        double temp;

        int counter = 0;
        int[] pointVectorCord;
        double[] point = new double[2] { x, y };

        foreach (double y1 in yVal)
        {
            foreach (double x1 in xVal)
            {
                pointVectorCord = new int[2];

                temp = (1 + region[1]) * 3 + (1 + region[0]) - y1 * 3 * region[1] - x1 * region[0];
                
                if (temp == 4)
                {
                    grids[counter] = grid;
                }
                else if (temp > 4)
                {
                    temp -= 1;
                    grids[counter] = suroundingGrid[Convert.ToInt32(temp)];
                }
                else
                {
                    grids[counter] = suroundingGrid[Convert.ToInt32(temp)];
                }

                if(grids[counter] == null)
                {
                    return 0;
                }
                

                for(int i1 = 0; i1 < 2; i1++)
                {
                    if(Math.Abs(region[0]) != Math.Abs(region[1]))
                    {
                        if (region[i1] == 0)
                        {
                            pointVectorCord[i1] = edgePointFinder1(point[i1], counter, i1);
                        }
                        else
                        {
                            if (i1 == 0)
                            {
                                pointVectorCord[i1] = edgePointFinder2(grids[counter][0].Count, counter, i1);
                            }
                            else
                            {
                                pointVectorCord[i1] = edgePointFinder2(grids[counter][0][0].Count, counter, i1);
                            }
                        }
                    }
                    else
                    {
                        if (i1 == 0)
                        {
                            pointVectorCord[i1] = cornerPointFinder(counter, i1, grids[counter][0].Count);
                        }
                        else
                        {
                            pointVectorCord[i1] = cornerPointFinder(counter, i1, grids[counter][0][0].Count);
                        }
                    }
                    
                }
                pointVectors[counter] = grids[counter][0][pointVectorCord[0]][pointVectorCord[1]];

                counter += 1;
            }
            Array.Reverse(xVal);
            
        }

        return sample(x, y, pointVectors);
    }

    //unfinished
    public double sample(double x, double y, double z, List<List<List<int>>>[] suroundingGrid)
    {
        double[][] pointVectors = new double[8][]{
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
            };

        List<List<List<double[]>>>[] grids = new List<List<List<double[]>>>[8]{
            grid,
            grid,
            grid,
            grid,
            grid,
            grid,
            grid,
            grid
            };

        return sample(x, y, z, pointVectors);
    }
}


