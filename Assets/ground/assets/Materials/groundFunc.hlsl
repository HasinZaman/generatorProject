#define PI 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679

// Noise Methods
float cosineInterpolate(float y1, float y2, float intermediaryPoint)
{
    float mu = (1 - cos(intermediaryPoint * PI)) / 2;

    return y1 * (1 - mu) + y2 * mu;
}

float dotProduct(float4 vectors, float3 dist)
{
    return vectors.x * dist.x + vectors.y * dist.y + vectors.z * dist.z;
}

void noiseSample_float(Texture3D vectors, float3 pos, float3 dim, out float sample)
{
    //gets the distance to nearest eight closest corners
    
    uint width, height, depth;
    
    Texture3D temp = vectors;
    
    float3 pointDist[8];
    
    float sampleTemp;
    
    vectors.GetDimensions(width, height, depth);
    
    width -= 1;
    height -= 1;
    depth -= 1;
    
    pos.x /= dim.x / width;
    pos.y /= dim.y / depth;
    pos.z /= dim.z / height;
    
    pointDist[0] = float3(pos.x % 1, pos.y % 1, pos.z % 1);
    pointDist[1] = float3(pos.x % 1 - 1, pos.y % 1, pos.z % 1);
    pointDist[2] = float3(pos.x % 1 - 1, pos.y % 1 - 1, pos.z % 1);
    pointDist[3] = float3(pos.x % 1, pos.y % 1 - 1, pos.z % 1);
    pointDist[4] = float3(pos.x % 1, pos.y % 1, pos.z % 1 - 1);
    pointDist[5] = float3(pos.x % 1 - 1, pos.y % 1, pos.z % 1 - 1);
    pointDist[6] = float3(pos.x % 1 - 1, pos.y % 1 - 1, pos.z % 1 - 1);
    pointDist[7] = float3(pos.x % 1, pos.y % 1 - 1, pos.z % 1 - 1);
    
    //gets the dot product for every corner
    float pointValue[8];
    
    pointValue[0] = dotProduct(temp.Load(uint4(floor(pos.x),    floor(pos.y),   floor(pos.z),   0)) * 2 - uint4(1, 1, 1, 0), pointDist[0]);
    pointValue[1] = dotProduct(temp.Load(uint4(ceil(pos.x),     floor(pos.y),   floor(pos.z),   0)) * 2 - uint4(1, 1, 1, 0), pointDist[1]);
    pointValue[2] = dotProduct(temp.Load(uint4(ceil(pos.x),     ceil(pos.y),    floor(pos.z),   0)) * 2 - uint4(1, 1, 1, 0), pointDist[2]);
    pointValue[3] = dotProduct(temp.Load(uint4(floor(pos.x),    ceil(pos.y),    floor(pos.z),   0)) * 2 - uint4(1, 1, 1, 0), pointDist[3]);
    pointValue[4] = dotProduct(temp.Load(uint4(floor(pos.x),    floor(pos.y),   ceil(pos.z),    0)) * 2 - uint4(1, 1, 1, 0), pointDist[4]);
    pointValue[5] = dotProduct(temp.Load(uint4(ceil(pos.x),     floor(pos.y),   ceil(pos.z),    0)) * 2 - uint4(1, 1, 1, 0), pointDist[5]);
    pointValue[6] = dotProduct(temp.Load(uint4(ceil(pos.x),     ceil(pos.y),    ceil(pos.z),    0)) * 2 - uint4(1, 1, 1, 0), pointDist[6]);
    pointValue[7] = dotProduct(temp.Load(uint4(floor(pos.x),    ceil(pos.y),    ceil(pos.z),    0)) * 2 - uint4(1, 1, 1, 0), pointDist[7]);
    
    // gets the interpolated value using the dot products
    float A = cosineInterpolate(
            pointValue[0],
            pointValue[1],
            pos.x % 1
        );

    float B = cosineInterpolate(
            pointValue[3],
            pointValue[2],
            pos.x % 1
        );

    float C = cosineInterpolate(
            A,
            B,
            pos.y % 1
        );

    float D = cosineInterpolate(
            pointValue[4],
            pointValue[5],
            pos.x % 1
        );

    float E = cosineInterpolate(
            pointValue[7],
            pointValue[6],
            pos.x % 1
        );

    float F = cosineInterpolate(
            D,
            E,
            pos.y % 1
        );

    sample = cosineInterpolate(
        C,
        F,
        pos.z % 1
    );
    
    //sample = sample * 0.2 + 0.15;
    
    //sample = float4(sampleTemp, sampleTemp, sampleTemp, sampleTemp);
}

void sort(float values[4], int key[4])
{
    bool sortCond = false;
    float valuesTemp;
    int keyTemp;
    int i2 = 0;
    
    while(!sortCond)
    {
        sortCond = true;
        for (int i1 = 0; i1 < 3 - i2; i1++)
        {
            if(values[i1] > values[i1 + 1])
            {
                valuesTemp = values[i1];
                keyTemp = key[i1];
                
                values[i1] = values[i1 + 1];
                key[i1] = key[i1 + 1];
                
                values[i1 + 1] = valuesTemp;
                key[i1 + 1] = keyTemp;
                
                sortCond = false;
            }
        }
        i2 += 1;
    }
}

//     XY
// 0 = 00
// 1 = 01
// 2 = 10
// 3 = 11
//
// 1 - 3
// |   |
// 0 - 2
//
void biomeDecoder_float(Texture2D biomeBorders, float3 pos, out float4 corner0, out float4 corner1, out float4 corner2, out float4 corner3)
{
    corner0 = biomeBorders.Load(uint3(  floor(pos.x),   floor(pos.z),   0));
    corner1 = biomeBorders.Load(uint3(  ceil(pos.x),    floor(pos.z),    0));
    corner2 = biomeBorders.Load(uint3(  floor(pos.x),   ceil(pos.z),   0));
    corner3 = biomeBorders.Load(uint3(  ceil(pos.x),    ceil(pos.z),    0));
}

void biomeMeshVal_float(float4 corner0, float4 corner1, float4 corner2, float4 corner3, float3 pos, out float4 sample)
{
    //cosineInterpolate
    // P2 ----(x2)---- p3
    //          |
    //      (sample)
    //          |
    // p0 ----(x1)---- p1
    
    float4 x1 = float4
    (
        cosineInterpolate(corner0.r, corner1.r, pos.x),
        cosineInterpolate(corner0.g, corner1.g, pos.x),
        cosineInterpolate(corner0.b, corner1.b, pos.x),
        cosineInterpolate(corner0.a, corner1.a, pos.x)
    );
    
    float4 x2 = float4
    (
        cosineInterpolate(corner2.r, corner3.r, pos.x),
        cosineInterpolate(corner2.g, corner3.g, pos.x),
        cosineInterpolate(corner2.b, corner3.b, pos.x),
        cosineInterpolate(corner2.a, corner3.a, pos.x)
    );
    
    sample = float4
    (
        cosineInterpolate(x1.r, x2.r, pos.z),
        cosineInterpolate(x1.g, x2.g, pos.z),
        cosineInterpolate(x1.b, x2.b, pos.z),
        cosineInterpolate(x1.a, x2.a, pos.z)
    );
}

void cornerVal_float(Texture2D biomeBorders, float3 pos, out float4 sample)
{
    sample = biomeBorders.Load(uint3(floor(pos.x), floor(pos.z), 0));
}


//biomeTextureExtraction
void biomeBlendingAtPoint1_float(float4 biomeBlendFactor, out float4 sample)
{
    float max = biomeBlendFactor.r + biomeBlendFactor.g + biomeBlendFactor.b;
    
    sample.r = biomeBlendFactor.r / max;
    sample.g = biomeBlendFactor.g / max;
    sample.b = biomeBlendFactor.b / max;
    sample.a = 0;

}

void biomeColourPicker_float(float4 biomeBlendFactor, float4 colour0, float4 colour1, float4 colour2, float4 colour3, out float4 colour0Out, out float4 colour1Out, out float4 colour2Out, out float4 colour3Out)
{
    float4 colourOut[4] =
    {
        colour0,
        colour1,
        colour2,
        colour3
    };
    
    float biomeBlendFactorValue[4] =
    {
        biomeBlendFactor.r,
        biomeBlendFactor.g,
        biomeBlendFactor.b,
        biomeBlendFactor.a
    };
    int biomeBlendFactorIndex[4] =
    {
        0,
        1,
        2,
        3
    };
    
    bool sortCond = false;
    float valuesTemp;
    int keyTemp;
    int i2 = 0;
    
    while (!sortCond)
    {
        sortCond = true;
        for (int i1 = 0; i1 < 3 - i2; i1++)
        {
            if (biomeBlendFactorValue[i1] > biomeBlendFactorValue[i1 + 1])
            {
                valuesTemp = biomeBlendFactorValue[i1];
                keyTemp = biomeBlendFactorIndex[i1];
                
                biomeBlendFactorValue[i1] = biomeBlendFactorValue[i1 + 1];
                biomeBlendFactorIndex[i1] = biomeBlendFactorIndex[i1 + 1];
                
                biomeBlendFactorValue[i1 + 1] = valuesTemp;
                biomeBlendFactorIndex[i1 + 1] = keyTemp;
                
                sortCond = false;
            }
        }
        i2 += 1;
    }
    
    //sort(biomeBlendFactorValue, biomeBlendFactorIndex);
    
    colour0Out = colourOut[biomeBlendFactorIndex[0]];
    colour1Out = colourOut[biomeBlendFactorIndex[1]];
    colour2Out = colourOut[biomeBlendFactorIndex[2]];
    colour3Out = colourOut[biomeBlendFactorIndex[3]];

}

//biomeTextureExtraction
void biomeBlendingAtPoint_float(Texture2D biomeBorders, float3 pos, out float4 sample)
{
    float4 pointValue = biomeBorders.Load(uint3(floor(pos.x), floor(pos.z), 0));
    
    float max = pointValue.r + pointValue.g + pointValue.b;
    
    sample.r = pointValue.r / max;
    sample.g = pointValue.g / max;
    sample.b = pointValue.b / max;
    sample.a = 0;
}
