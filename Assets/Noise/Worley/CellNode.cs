using System;

public abstract class CellNode
{
    protected float[] vector;
    protected int dim;

    public CellNode(int dim, float[] val)
    {
        this.dim = dim;

        this.set(val);
    }

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

    public float[] val()
    {
        return this.vector;
    }

    public abstract CellNode get(int axis, int relativePos);

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
