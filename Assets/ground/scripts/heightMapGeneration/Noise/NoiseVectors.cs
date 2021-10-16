public static class NoiseVectors
{
    public static float[][] TwoDimensionSet1 =
        new float[][] {
            //new float[2] { 0, 1},
            new float[2] { 1, 1 },
            //new float[2] { 1, 0 },
            new float[2] { 1, -1 },
            //new float[2] { 0, -1 },
            new float[2] { -1, -1 },
            //new float[2] { -1, 0 },
            new float[2] { -1, 1 }
        };

    public static float[][] ThreeDimensionSet1 = new float[][] {
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

    public static float[][] ThreeDimensionSet2 = new float[][] {
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
        new float[3] { 1, -1, -1},

        new float[3] { 1,  1,  0},
        new float[3] {-1, -1,  0},
        new float[3] { 0, -1, 1},
        new float[3] { 0, -1, -1}
    };
}
