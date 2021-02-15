#define PI 3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679

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