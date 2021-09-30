using System;

/// <summary>
///     Node object stores a value of a Node in Grid
/// </summary>
public class Node : IComparable
{
    /// <summary>
    ///     val stores the float value of the Node
    /// </summary>
    private float val;

    /// <summary>
    ///     minVal and maxVal define the range in which val can reside within
    /// </summary>
    private const float minVal = 0, maxVal = 1;

    /// <summary>
    ///     Constructor that creates Node object with a specfic val
    /// </summary>
    /// <param name="val">A float value to set val instance</param>
    public Node(float val)
    {
        setValue(val);
    }

    /// <summary>
    ///     Constructor that creates Node object with a val of 0
    /// </summary>
    public Node()
    {
        this.val = 0;
    }

    /// <summary>
    ///     CompareTo method compares Node.val realtive to obj.val
    /// </summary>
    /// <param name="obj">obj is an object that will be used in comparison</param>
    /// <returns>
    ///     CompareTo returns an int repesenting the position of the Node realtive to obj.
    ///     -1  = Node val is less than obj
    ///     0   = Node val is same value as obj
    ///     1   = Node val is greater than obj
    /// </returns>
    public int CompareTo(object obj)
    {
        if(!(obj is Node))
        {
            throw new ArgumentException("n is not an instance or child of Node");
        }

        Node n = (Node)obj;

        float valTemp = n.getValue();

        if(this.val < valTemp)
        {
            return -1;
        }
        else if(this.val == valTemp)
        {
            return 0;
        }
        return 1;
    }

    /// <summary>
    ///     getValue method returns the value of the node
    /// </summary>
    /// <returns>
    ///     Returns the value of the Node
    /// </returns>
    public float getValue()
    {
        return this.val;
    }

    /// <summary>
    ///     setValue method changes the value of the Node
    /// </summary>
    /// <param name="val">val is the new value that will be set</param>
    public void setValue(float val)
    {
        if(val < Node.minVal || val > Node.maxVal)
        {
            throw new ArgumentException($"val is outside range of vaild values. Required Range: {Node.minVal} < val < {Node.maxVal}| val = {val}");
        }

        this.val = val;
    }

    /// <summary>
    ///     toString method repersentation object in the format of a string
    /// </summary>
    /// <returns>String repersentation of Object</returns>
    public string toString()
    {
        return $"({val.ToString()})";
    }
}
