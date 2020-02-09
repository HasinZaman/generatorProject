using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noise
{
    public List<List<List<double[]>>> grid = new List<List<List<double[]>>>();

    private double[][] vectors;

    public int seed;
    private System.Random random;

    public noise(int seedRaw, double[][] vectersRaw, int width, int height, int depth)
    {
        seed = seedRaw;
        vectors = vectersRaw;
        random = new System.Random(seed);

        createGrid(width, height, depth);
    }
    public noise(int seedRaw, double[][] vectersRaw, int width, int height)
    {
        seed = seedRaw;
        vectors = vectersRaw;
        random = new System.Random(seed);

        createGrid(width, height, 1);

    }

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
    private double cosineInterpolate(double y1, double y2, double intermediaryPoint)
    {
        double mu = (1 - Math.Cos(intermediaryPoint * Math.PI)) / 2;

        return y1 * (1 - mu) + y2 * mu;
    }
    private bool sampleCoordCheck(double x, double y)
    {
        if (Math.Round(x) == x || Math.Round(y) == y)
        {
            return true;
        }

        if (x < -1)
        {
            return true;
        }

        if (y < -1)
        {
            return true;
        }

        if (x > grid[0][0].Count)
        {
            return true;
        }

        if (y > grid[0][0].Count)
        {
            return true;
        }

        return false;
    }
    private bool sampleCoordCheck(double x, double y, double z)
    {
        if (Math.Round(x) == x || Math.Round(y) == y || Math.Round(z) == z)
        {
            return true;
        }

        if (x < -1)
        {
            return true;
        }

        if (y < -1)
        {
            return true;
        }

        if (z < -1)
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


    public double sample(double x, double y)
    {
        if (sampleCoordCheck(x, y))
        {
            if (Math.Round(x) == x || Math.Round(y) == y)
            {
                return 0;
            }

            double newX = x, newY = y;

            if (x < -1)
            {
                newX = grid[0][0].Count + x % grid[0][0].Count;
            }
            else if (x > grid[0][0].Count)
            {
                newX = x % grid[0][0].Count;
            }

            if (y < -1)
            {
                newY = grid[0].Count + y % grid[0].Count;
            }
            else if (y > grid[0].Count)
            {
                newY = y % grid[0].Count;
            }

            if (newX != x || newY != y)
            {
                return sample(newX, newY);
            }
        }

        double[][] pointVectors = new double[4][]{
            null,
            null,
            null,
            null
            };

        double left = Math.Floor(x);
        double right = Math.Ceiling(x);
        double bottom = Math.Floor(y);
        double top = Math.Ceiling(y);

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

        pointVectors[0] = grid[0][Convert.ToInt32(left)][Convert.ToInt32(bottom)];
        pointVectors[1] = grid[0][Convert.ToInt32(right)][Convert.ToInt32(bottom)];
        pointVectors[2] = grid[0][Convert.ToInt32(right)][Convert.ToInt32(top)];
        pointVectors[3] = grid[0][Convert.ToInt32(left)][Convert.ToInt32(top)];

        double[][] pointDist = new double[][]
        {
            new double[]{x % 1, y % 1},
            new double[]{x % 1 - 1, y % 1},
            new double[]{x % 1 - 1, y % 1 - 1},
            new double[]{x % 1, y % 1 - 1}
        };

        double[] pointValue = vecterDistDotProduct(pointVectors, pointDist);

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

    public double sample(double x, double y, double z)
    {
        if (sampleCoordCheck(x, y, z))
        {
            if (Math.Round(x) == x || Math.Round(y) == y || Math.Round(z) == z)
            {
                return 0;
            }

            double newX = x, newY = y, newZ = z;

            if (x < -1)
            {
                newX = grid[0][0].Count + x % grid[0][0].Count;
            }
            else if (x > grid[0][0].Count)
            {
                newX = x % grid[0][0].Count;
            }

            if (y < -1)
            {
                newY = grid[0].Count + y % grid[0].Count;
            }
            else if (y > grid[0].Count)
            {
                newY = y % grid[0].Count;
            }

            if (z < -1)
            {
                newZ = grid.Count + z % grid.Count;
            }
            else if (z > grid.Count)
            {
                newZ = z % grid.Count;
            }

            if (newX != x || newY != y || newZ != z)
            {
                return sample(newX, newY, newZ);
            }
        }

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

        double left = Math.Floor(x);
        double right = Math.Ceiling(x);
        double bottom = Math.Floor(y);
        double top = Math.Ceiling(y);
        double back = Math.Floor(z);
        double front = Math.Ceiling(z);

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

        pointVectors[0] = grid[Convert.ToInt32(back)][Convert.ToInt32(left)][Convert.ToInt32(bottom)];
        pointVectors[1] = grid[Convert.ToInt32(back)][Convert.ToInt32(right)][Convert.ToInt32(bottom)];
        pointVectors[2] = grid[Convert.ToInt32(back)][Convert.ToInt32(right)][Convert.ToInt32(top)];
        pointVectors[3] = grid[Convert.ToInt32(back)][Convert.ToInt32(left)][Convert.ToInt32(top)];

        pointVectors[4] = grid[Convert.ToInt32(front)][Convert.ToInt32(left)][Convert.ToInt32(bottom)];
        pointVectors[5] = grid[Convert.ToInt32(front)][Convert.ToInt32(right)][Convert.ToInt32(bottom)];
        pointVectors[6] = grid[Convert.ToInt32(front)][Convert.ToInt32(right)][Convert.ToInt32(top)];
        pointVectors[7] = grid[Convert.ToInt32(front)][Convert.ToInt32(left)][Convert.ToInt32(top)];

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

        double[] pointValue = vecterDistDotProduct(pointVectors, pointDist);

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

    public double sample(double x, double y, double[][] pointVectors)
    {
        if (sampleCoordCheck(x, y))
        {
            if (Math.Round(x) == x || Math.Round(y) == y)
            {
                return 0;
            }

            double newX = x, newY = y;

            if (x < -1)
            {
                newX = grid[0][0].Count + x % grid[0][0].Count;
            }
            else if (x > grid[0][0].Count)
            {
                newX = x % grid[0][0].Count;
            }

            if (y < -1)
            {
                newY = grid[0].Count + y % grid[0].Count;
            }
            else if (y > grid[0].Count)
            {
                newY = y % grid[0].Count;
            }

            if (newX != x || newY != y)
            {
                return sample(newX, newY, pointVectors);
            }
        }

        double[][] pointDist = new double[][]
        {
            new double[]{x % 1, y % 1},
            new double[]{x % 1 - 1, y % 1},
            new double[]{x % 1 - 1, y % 1 - 1},
            new double[]{x % 1, y % 1 - 1}
        };

        double[] pointValue = vecterDistDotProduct(pointVectors, pointDist);

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

    public double sample(double x, double y, double z, double[][] pointVectors)
    {
        if (sampleCoordCheck(x, y, z))
        {
            if (Math.Round(x) == x || Math.Round(y) == y || Math.Round(z) == z)
            {
                return 0;
            }

            double newX = x, newY = y, newZ = z;

            if (x < -1)
            {
                newX = grid[0][0].Count + x % grid[0][0].Count;
            }
            else if (x > grid[0][0].Count)
            {
                newX = x % grid[0][0].Count;
            }

            if (y < -1)
            {
                newY = grid[0].Count + y % grid[0].Count;
            }
            else if (y > grid[0].Count)
            {
                newY = y % grid[0].Count;
            }

            if (z < -1)
            {
                newZ = grid.Count + z % grid.Count;
            }
            else if (z > grid.Count)
            {
                newZ = z % grid.Count;
            }

            if (newX != x || newY != y || newZ != z)
            {
                return sample(newX, newY, newZ, pointVectors);
            }
        }
        

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

        double[] pointValue = vecterDistDotProduct(pointVectors, pointDist);

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

    public double sampleEdge(List<List<List<double[]>>>[] suroundingGrid, double x, double y)
    {
        if (sampleCoordCheck(x, y))
        {
            if (Math.Round(x) == x || Math.Round(y) == y)
            {
                return 0;
            }

            double newX = x, newY = y;

            if (x < -1)
            {
                newX = grid[0][0].Count + x % grid[0][0].Count;
            }
            else if (x > grid[0][0].Count)
            {
                newX = x % grid[0][0].Count;
            }

            if (y < -1)
            {
                newY = grid[0].Count + y % grid[0].Count;
            }
            else if (y > grid[0].Count)
            {
                newY = y % grid[0].Count;
            }

            if (newX != x || newY != y)
            {
                return sampleEdge(suroundingGrid, newX, newY);
            }
        }

        double[][] pointVectors = new double[4][]{
            null,
            null,
            null,
            null
            };

        //idea
        /*
         List<List<List<List<double[]>>>>[] grids = new List<List<List<List<double[]>>>>[4]{
            new List<List<List<List<double[]>>>>{grid},
            new List<List<List<List<double[]>>>>{grid},
            new List<List<List<List<double[]>>>>{grid},
            new List<List<List<List<double[]>>>>{grid}
            }; 
         */

        List<List<List<double[]>>>[] grids = new List<List<List<double[]>>>[4]{
            grid,
            grid,
            grid,
            grid
            };

        double left = Math.Floor(x);
        double right = Math.Ceiling(x);
        double bottom = Math.Floor(y);
        double top = Math.Ceiling(y);

        if (Math.Floor(x) == -1)
        {
            grids[0] = suroundingGrid[7];
            grids[3] = suroundingGrid[7];
            left = grid[0][0].Count - 1;
        }
        else if (Math.Ceiling(x) == grid[0][0].Count)
        {
            grids[1] = suroundingGrid[3];
            grids[2] = suroundingGrid[3];
            right = 0;
        }

        if (Math.Floor(y) == -1)
        {
            if(grids[0] == suroundingGrid[7])
            {
                grids[0] = suroundingGrid[0];
            }
            else
            {
                grids[0] = suroundingGrid[1];
            }

            if(grids[1] == suroundingGrid[3])
            {
                grids[1] = suroundingGrid[2];
            }
            else
            {
                grids[1] = suroundingGrid[1];
            }
            bottom = grid[0].Count - 1;
        }
        else if (Math.Ceiling(y) == grid[0].Count)
        {
            if (grids[3] == suroundingGrid[7])
            {
                grids[3] = suroundingGrid[6];
            }
            else
            {
                grids[3] = suroundingGrid[5];
            }

            if (grids[2] == suroundingGrid[3])
            {
                grids[2] = suroundingGrid[4];
            }
            else
            {
                grids[2] = suroundingGrid[5];
            }
            top = 0;
        }

        pointVectors[0] = grids[0][0][Convert.ToInt32(left)][Convert.ToInt32(bottom)];
        pointVectors[1] = grids[1][0][Convert.ToInt32(right)][Convert.ToInt32(bottom)];
        pointVectors[2] = grids[2][0][Convert.ToInt32(right)][Convert.ToInt32(top)];
        pointVectors[3] = grids[3][0][Convert.ToInt32(left)][Convert.ToInt32(top)];
        

        return sample(x, y, pointVectors);
    }

    public double sampleEdge(List<List<List<int>>>[] suroundingGrid, double x, double y, double z)
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

        return sample(x, y, z, pointVectors);
    }

}


