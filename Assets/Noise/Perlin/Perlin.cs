using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Perlin abstract class creates foundation for higher level Dimension noise 
/// </summary>
public abstract class Perlin<T> : Noise where T : VectorNode
{
    /// <summary>
    ///     SampleIterator iterates through a range of values
    /// </summary>
    protected class SampleIterator : Iterator<float>
    {
        /// <summary>
        ///     Constructor sets up iterator
        /// </summary>
        /// <param name="start">start value of iterator</param>
        /// <param name="end">end value of iterator</param>
        /// <param name="samples">Number of samples that will be iterated through between start and end</param>
        public SampleIterator(float start, float end, int samples) : base(start, end)
        {
            if (samples < 3)
            {
                throw new ArgumentException("samples need to be greater than 2");
            }

            this.delta = (end - current) / (float)(samples - 1);
            this.current -= this.delta;
        }

        /// <summary>
        ///     next method updates the current value of the iterator with the next valid value
        /// </summary>
        /// <returns>float of the updated value of current</returns>
        public override float next()
        {
            if (this.current + this.delta > this.end + 0.00001)
            {
                throw new InvalidOperationException("Iterator has reached end");
            }
            this.current += this.delta;

            return this.current;
        }

        /// <summary>
        ///     hasNext checks if the next value of the iterator exists
        /// </summary>
        /// <returns>bool if the next value exists</returns>
        public override bool hasNext()
        {
            return !(this.current + this.delta > this.end + 0.00001);
        }

        /// <summary>
        ///     restart method sets current value back to start
        /// </summary>
        public void restart()
        {
            this.current = this.start;
            this.current -= this.delta;
        }
    }

    /// <summary>
    ///     GeneratorIterator class traverses in a back and forth manner between a range of values inorder to create a linked grids using Vector2DNode
    /// </summary>
    protected class GeneratorIterator : Iterator<int>
    {
        /// <summary>
        ///     constructor sets up Iterator class
        /// </summary>
        /// <param name="start">Starting position of iterator</param>
        /// <param name="end">Ending position of iterator</param>
        public GeneratorIterator(int start, int end) : base(start - 1, end + 1)
        {
            if (end == start)
            {
                this.delta = 0;
            }
            else if (end - start > 0)
            {
                this.delta = 1;
            }
            else
            {
                this.delta = -1;
            }

            this.restart();
        }

        /// <summary>
        ///     hasNext method checks if another value exists
        /// </summary>
        /// <returns>boolean if the next number exists</returns>
        public override bool hasNext()
        {
            int tmp = this.delta + this.current;

            return this.start < tmp && tmp < this.end && this.delta != 0;
        }

        /// <summary>
        ///     next method updates iterator
        /// </summary>
        /// <returns>int of current value</returns>
        public override int next()
        {
            if (this.hasNext())
            {
                this.current += this.delta;
                return this.current;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///     restart method sets current value back to start
        /// </summary>
        public void restart()
        {
            if (this.delta == 1)
            {
                this.current = this.start;
            }
            else
            {
                this.current = this.end;
            }
            this.current += this.delta;
        }

        /// <summary>
        ///     reverse method swaps the start and end value
        /// </summary>
        public void reverse()
        {
            this.delta *= -1;
        }

        /// <summary>
        /// toString method converts iterator into String
        /// </summary>
        /// <returns>String representation of iterator</returns>
        public override string ToString()
        {
            return $"start:{this.start + delta}\tend:{this.end - delta}\tdelta:{this.delta}\tcurrent:{this.current}";
        }
    }

    /// <summary>
    ///     root stores the starting point of Noise Grid
    /// </summary>
    T root;

    /// <summary>
    ///     dim stores size of the perlin noise vectors
    /// </summary>
    protected int dim;

    /// <summary>
    ///     random is used to create grid using vectors
    /// </summary>
    protected System.Random random;

    private bool lastPosCheck(int[] pos)
    {
        for(int i1 = 0; i1 < pos.Length; i1++)
        {
            if(pos[i1] != 0)
            {
                return false;
            }
        }
        return true;
    }

    private void increment(int[] pos, int i1)
    {
        if(i1 > pos.Length)
        {
            throw new Exception();
        }

        if(pos[i1] == 0)
        {
            pos[i1] = 1;
        }
        else
        {
            pos[i1] = 0;
            increment(pos, i1 + 1);
        }
    }

    private int calcPos(int[] pos)
    {
        int tmp = 0;

        for (int i1 = this.dim - 1; i1 >= 0; i1--)
        {
            tmp = 2 * (pos[i1] + tmp);
        }

        return tmp;
    }

    /// <summary>
    ///     abstract sample method gets noise value at a given position
    /// </summary>
    /// <param name="pos">array of float repsenting a given position</param>
    /// <returns>Sample value at a given position</returns>
    public float sample(float[] samplePos)
    {
        if (this.dim != samplePos.Length)
        {
            throw new ArgumentException();
        }

        bool endCond = false;

        float sampleConst = 2.084991f;

        float[][] vectors = new float[1 << this.dim][];
        float[][] dist = new float[1 << this.dim][];
        float[] vertexVal = new float[1 << this.dim];

        int[] pos = new int[this.dim];

        float[] interplateTmp1;

        for (int i1 = 0; i1 < this.dim; i1++)
        {
            pos[i1] = (int)samplePos[i1];
        }

        VectorNode pointer = getVector(pos);

        pos = new int[this.dim];

        int posTmp;

        while (lastPosCheck(pos) || endCond)
        {
            //converting vector position into array index
            posTmp = calcPos(pos);

            //get vector at position
            vectors[posTmp] = getVector(pos, pointer).val();

            //get distance from node
            dist[posTmp] = new float[this.dim];
            
            for(int i1 = 0; i1 < this.dim; i1++)
            {
                dist[posTmp][i1] = samplePos[i1] % 1 - pos[i1];
            }

            //geting vertVal
            vertexVal[posTmp] = dotProduct(vectors[posTmp], dist[posTmp]);


            if(lastPosCheck(pos) && !endCond)
            {
                endCond = true;
            }
            else if(endCond)
            {
                break;
            }
            else
            {
                increment(pos, 0);
            }
        }

        int dimTmp = this.dim - 1;

        interplateTmp1 = new float[1 << dimTmp];

        pos = new int[this.dim];

        int i = 0;

        ArraySegment<int> splice;
        int[] spliceArray;
        int[][] tmpArray = new int[][] { new int[] { 0 }, new int[] { 1 } };
        
        while (dimTmp / 2 != 1)
        {
            for(int i1 = 0; i1 < pos.Length; i1++)
            {
                pos[i + i1] = 0;
            }
            endCond = false;



            while (lastPosCheck(pos) || endCond)
            {
                splice = new ArraySegment<int>(pos, i, pos.Length);
                spliceArray = splice.ToArray();
                vertexVal[calcPos(spliceArray)] =
                    cosineInterpolate
                    (
                        vertexVal[calcPos(tmpArray[0].Concat(spliceArray).ToArray())],
                        vertexVal[calcPos(tmpArray[1].Concat(spliceArray).ToArray())],
                        pos[i] % 1
                    );

                if (lastPosCheck(pos) && !endCond)
                {
                    endCond = true;
                }
                else if (endCond)
                {
                    break;
                }
                else
                {
                    increment(pos, i);
                }
            }
        }
        return (cosineInterpolate(vertexVal[0], vertexVal[1], samplePos[samplePos.Length - 1] % 1) + sampleConst / 2f) / sampleConst;
    }

    /// <summary>
    ///     dotProduct returns the dotProduct of two vectors
    /// </summary>
    /// <param name="v1">v1 is a vector of n length</param>
    /// <param name="v2">v2 is a vector of n length</param>
    /// <returns>
    ///     float of the dotProduct of v1 and v2
    /// </returns>
    protected float dotProduct(float[] v1, float[] v2)
    {
        if (v1.Length != v2.Length)
        {
            throw new ArgumentException($"vector 1 and vector need to be the same size. v1.length={v1.Length} v2.length={v2.Length}");
        }
        float temp = 0;
        for (int i1 = 0; i1 < v1.Length; i1++)
        {
            temp += v1[i1] * v2[i1];
        }
        return temp;
    }

    /// <summary>
    ///     cosineInterpolate interpolates between y1 and y2
    /// </summary>
    /// <param name="y1">y1 is the first value</param>
    /// <param name="y2">y2 is the second value</param>
    /// <param name="intermediaryPoint">intermediaryPoint is the x position which an interpolated value will be extracted at</param>
    /// <returns>float of the interpolated value between y1 and y2</returns>
    protected float cosineInterpolate(float y1, float y2, float intermediaryPoint)
    {
        float mu = (float)(1 - Math.Cos(intermediaryPoint * Math.PI)) / 2;

        return y1 * (1 - mu) + y2 * mu;
    }

    /// <summary>
    ///     generateVectors is an abstract method to randomly generate perlin noise vector nodes
    /// </summary>
    /// <param name="start">int array that stores the starting position at which nodes will be genrated</param>
    /// <param name="end">int array that stores the ending position at which nodes will stop generating</param>
    /// <param name="template">array of perlin noise vectors</param>
    public abstract void generateVectors(int[] start, int[] end);

    // <summary>
    ///     getVector gets the perlin noise vector at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector</param>
    /// <returns>float array of noise vector</returns>
    protected abstract VectorNode getVector(int[] pos);

    /// <summary>
    ///     getNode gets the perlin noise noise at a given position
    /// </summary>
    /// <param name="pos">int array of the position of the perlin noise vector relative to startNode</param>
    /// <param name="startNode">Vector2DNode instance is the start position</param>
    /// <returns>Vector2DNode of noise vector at pos</returns>
    protected abstract VectorNode getVector(int[] pos, VectorNode start);

    /// <summary>
    ///     toString method returns a repsentation of perlin noise in a string format
    /// </summary>
    /// <returns>string repsentation of perlin noise</returns>
    public abstract string toString();
}
