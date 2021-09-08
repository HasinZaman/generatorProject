/// <summary>
///     Iterator abstract class handles through a range of values
/// </summary>
public abstract class Iterator<T>
{
    /// <summary>
    ///     start value of Iterator
    /// </summary>
    protected T start;

    /// <summary>
    ///     end value of Iterator
    /// </summary>
    protected T end;

    /// <summary>
    ///     delta float step size required to iterate from start to end instance
    /// </summary>
    protected T delta;

    /// <summary>
    ///     current value of the iterator
    /// </summary>
    public T current;

    /// <summary>
    ///     Constructor sets up iterator
    /// </summary>
    /// <param name="start">start value of iterator</param>
    /// <param name="end">end value of iterator</param>
    public Iterator(T start, T end)
    {
        this.start = start;
        this.current = start;
        this.end = end;
    }

    /// <summary>
    ///     next method updates the current value of the iterator with the next valid value
    /// </summary>
    /// <returns>T of the updated value of current</returns>
    public abstract T next();

    /// <summary>
    ///     hasNext checks if the next value of the iterator exists
    /// </summary>
    /// <returns>bool if the next value exists</returns>
    public abstract bool hasNext();

    /// <summary>
    ///     getDelta returns class delta value
    /// </summary>
    /// <returns>T of delta value</returns>
    public T getDelta()
    {
        return this.delta;
    }

    /// <summary>
    /// toString method converts iterator into String
    /// </summary>
    /// <returns>String representation of iterator</returns>
    public override string ToString()
    {
        return $"start:{this.start}\tend:{this.end}\tdelta:{this.delta}\tcurrent:{this.current}";
    }
}
