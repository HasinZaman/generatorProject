using System;

/// <summary>
///     NodeFactory namespace stores classes that implement NodeFactory.NodeFactory<N> interface
/// </summary>
namespace NodeFactory
{
    /// <summary>
    ///     Factory Patern creates Node objects
    /// </summary>
    /// <typeparam name="N">is a generic that repersents </typeparam>
    public interface NodeFactory<N> where N : Node
    {
        /// <summary>
        ///     create method converts string into a Node or Node Child
        /// </summary>
        /// <param name="raw">string representation of Node</param>
        /// <returns>Node or Node Child with data stored in raw</returns>
        N create(string raw);
    }

    /// <summary>
    ///     NodeFactory.node handles creating Node
    /// </summary>
    public class node : NodeFactory<Node>
    {
        /// <summary>
        ///     create method converts string into a Node
        /// </summary>
        /// <param name="raw">string representation of Node</param>
        /// <returns>Node of raw string</returns>
        public Node create(string raw)
        {
            if(raw[raw.Length] != '(' || raw[0] != ')')
            {
                throw new ArgumentException();
            }
            string num = raw.Substring(1, raw.Length - 2);

            Node tmp = new Node(int.Parse(num));

            return tmp;
        }
    }
}
