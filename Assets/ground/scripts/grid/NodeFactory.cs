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
        /// <param name="str">string representation of Node</param>
        /// <returns>Node of raw string</returns>
        public Node create(string str)
        {
            if(str[0] != '(' || str[str.Length - 1] != ')')
            {
                throw new ArgumentException($"'{str}' is invalid format");
            }

            string valStr = str.Substring(1, str.Length - 2);

            float val;

            if (!float.TryParse(valStr, out val))
            {
                throw new ArgumentException($"'{str}' could not be converted into node value");
            }

            Node tmp;

            if (val > 1)
            {
                tmp = new Node(1);
            }
            else if(val < 0)
            {
                tmp = new Node(0);
            }
            else
            {
                tmp = new Node(val);
            }

            return tmp;
        }
    }
}
