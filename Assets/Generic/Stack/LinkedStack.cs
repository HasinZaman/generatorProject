using System;
/// <summary>
///     Stack implementation with linked elements
/// </summary>
/// <typeparam name="E"></typeparam>
public class LinkedStack<E> : Stack<E>
{
    /// <summary>
    ///     Element class used to link elements
    /// </summary>
    private class Element
    {
        public E val;
        public Element next;

        /// <summary>
        ///     Element Constructor
        /// </summary>
        /// <param name="val">value of element</param>
        /// <param name="next">pointer to the next elment in stack</param>
        public Element(E val, Element next)
        {
            this.val = val;
            this.next = next;
        }
        
    }

    private int i = 0;
    private Element root;

    /// <summary>
    ///     isEmpty returns bool if stack is empty
    /// </summary>
    /// <returns>bool of stack is empty</returns>
    public bool isEmpty()
    {
        return i == 0;
    }

    /// <summary>
    ///     pop method removes and returns top most element
    /// </summary>
    /// <returns>Top most elment</returns>
    public virtual E pop()
    {
        if(i == 0)
        {
            throw new InvalidOperationException();
        }

        Element tmp = root;
        root = root.next;
        tmp.next = null;
        i--;
        return tmp.val;
    }

    /// <summary>
    ///     Adds element into the stack
    /// </summary>
    /// <param name="elem"></param>
    public virtual void push(E elem)
    {
        Element tmp = new Element(elem, root);

        root = tmp;
        i++;
    }


    /// <summary>
    ///     method returns the size of the stack
    /// </summary>
    /// <returns>int size of stack</returns>
    public int size()
    {
        return i;
    }

    /// <summary>
    ///     top method returns the top most element
    /// </summary>
    /// <returns>Top most Element</returns>
    public E top()
    {
        if(root == null)
        {
            throw new InvalidOperationException();
        }

        return root.val;
    }
}
