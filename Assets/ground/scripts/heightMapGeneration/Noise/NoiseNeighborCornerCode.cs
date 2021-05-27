using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseNeighborCornerCode
{
    /*
     * 5 - 6 - 7
     * |       |
     * 3       4
     * |       |
     * 0 - 1 - 2
     */

    public const int corner0 = 0;
    public const int corner1 = 2;
    public const int corner2 = 5;
    public const int corner3 = 7;

    public const int edge0 = 1;
    public const int edge1 = 3;
    public const int edge2 = 4;
    public const int edge3 = 6;

    static int center = -1;

    public static int getNoiseCode(int[] relativePos)
    {
        switch(relativePos[0])
        {
            case -1:
                return getNoiseCode(relativePos[1], new int[] { corner0, edge1, corner2});
            case 0:
                return getNoiseCode(relativePos[1], new int[] {edge0, center, edge3 });
            case 1:
                return getNoiseCode(relativePos[1], new int[] { corner2, edge2, corner3 });
            default:
                throw new Exception();
        }
    }
    private static int getNoiseCode(int relativePos, int[] options)
    {
        switch(relativePos)
        {
            case -1:
                return options[0];
            case 0:
                return options[1];
            case 1:
                return options[2];
            default:
                throw new Exception();
        }
    }

}
