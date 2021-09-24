using System;

/// <summary>
///     CellNode class is an abstract Node class for Worley Noise
/// </summary>
public abstract class CellNode
{
    /// <summary>
    ///     vector instances stores the positon of point contained in the cell. The position is measured from the lower left corner.
    /// </summary>
    protected float[] vector;

    /// <summary>
    ///     dim defines the number of dimension the point and cell have.
    /// </summary>
    protected int dim;

    /// <summary>
    ///     Constructor method sets up the foundation of every CellNode class
    /// </summary>
    /// <param name="dim">dim defines the number of dimensions the instance will have</param>
    /// <param name="val">val is the value given to the vector instance</param>
    public CellNode(int dim, float[] val)
    {
        this.dim = dim;

        this.set(val);
    }

    /// <summary>
    ///     set method handles the re-assignment of vector instance
    /// </summary>
    /// <param name="vector">vector is the new value of vector instance</param>
    public void set(float[] vector)
    {
        if (vector != null)
        {
            if (dim != vector.Length)
            {
                throw new ArgumentException($"vector lenght must have exactly {dim} elements");
            }
            this.vector = vector;
        }
    }

    /// <summary>
    ///     val method returns the value vector instance
    /// </summary>
    /// <returns>float array of vector instance</returns>
    public float[] val()
    {
        return this.vector;
    }

    /// <summary>
    ///     get method returns neighbor cell relative to the current cell
    /// </summary>
    /// <param name="axis">int that defines the axis the neighbor node resides on</param>
    /// <param name="relativePos">int that defines whether to get the neighbor node that is behind/infront of the cell in selected axis</param>
    /// <returns>neighbor CellNode object</returns>
    public abstract CellNode get(int axis, int relativePos);

    /// <summary>
    ///     toString method converts vector into a string
    /// </summary>
    /// <returns>String repersentation of object</returns>
    public string toString()
    {
        string temp = "(";

        temp += $"{this.vector[0]}";

        for (int i1 = 1; i1 < this.vector.Length; i1++)
        {
            temp += $",{this.vector[i1]}";
        }

        temp += ")";

        return temp;
    }
}
