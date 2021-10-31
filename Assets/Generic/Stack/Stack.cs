/// <summary>
///     Stack data structure 
/// </summary>
/// <typeparam name="E">Element that makes the Stack</typeparam>
public interface Stack<E>
{
    /// <summary>
    ///     Adds element into the stack
    /// </summary>
    /// <param name="elem"></param>
    void push(E elem);

    /// <summary>
    ///     pop method removes and returns top most element
    /// </summary>
    /// <returns>Top most elment</returns>
    E pop();

    /// <summary>
    ///     top method returns the top most element
    /// </summary>
    /// <returns>Top most Element</returns>
    E top();

    /// <summary>
    ///     method returns the size of the stack
    /// </summary>
    /// <returns>int size of stack</returns>
    int size();

    /// <summary>
    ///     isEmpty returns bool if stack is empty
    /// </summary>
    /// <returns>bool of stack is empty</returns>
    bool isEmpty();
}
