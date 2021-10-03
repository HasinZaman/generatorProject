/// <summary>
///     NodeReader co
/// </summary>
/// <typeparam name="T">is a generic that repersents </typeparam>
public interface NodeFactory<T> where T : Node
{
    /// <summary>
    ///     create method converts data into a Node or Node Child
    /// </summary>
    /// <param name="raw">string representation of Node</param>
    /// <returns>Node or Node Child with data stored in raw</returns>
    T create(string raw);
}
