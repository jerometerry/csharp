using System.Diagnostics.Contracts;

namespace CodeContracts
{
    /// <summary>
    /// A simple (generic) array based implementation of a stack to test out code contracts
    /// </summary>
    /// <typeparam name="T">Type type of objects the stack will hold</typeparam>
    /// <remarks>
    /// Adapted from "Object Oriented Software Construction" by Bertrand Myer.
    /// </remarks>
    public class MyStack<T>
    {
        /// <summary>
        /// The maximum capacity of the stack
        /// </summary>
        public int Capacity { get; private set; }
        
        /// <summary>
        /// The number of items in the stack
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The items in the stack
        /// </summary>
        private T[] representation;

        /// <summary>
        /// Constructs a new MyStack object
        /// </summary>
        /// <param name="capacity">The maximum number of items the stack can hold</param>
        public MyStack(int capacity)
        {
            Contract.Requires(capacity >= 0);
            this.Capacity = capacity;
            representation = new T[capacity];
        }

        /// <summary>
        /// The index of the top of the stack
        /// </summary>
        private int Top
        {
            get
            {
                Contract.Requires(Empty == false);
                return Count - 1;
            }
        }

        /// <summary>
        /// Gets whether the stack has zero elements
        /// </summary>
        public bool Empty
        {
            get
            {
                return Count == 0;
            }
        }

        /// <summary>
        /// Gets whether the stack has the maximum number of items
        /// </summary>
        public bool Full
        {
            get
            {
                return Count == Capacity;
            }
        }
        
        /// <summary>
        /// Pushes an item onto the stack
        /// </summary>
        /// <param name="item">The item to add to the stack</param>
        public void Put(T item)
        {
            Contract.Requires(Full == false);
            Contract.Ensures(Empty == false);
            representation[Count] = item;
            Count += 1;
        }

        /// <summary>
        /// Pops the top of the stack
        /// </summary>
        /// <returns>The item removed from the top of the stack</returns>
        public T Remove()
        {
            Contract.Requires(Empty == false);
            T item = representation[Top];
            Count -= 1;
            return item;
        }

        /// <summary>
        /// Gets the item at the top of the stack
        /// </summary>
        /// <returns></returns>
        public T Item()
        {
            Contract.Requires(Empty == false);
            return representation[Top];
        }

        /// <summary>
        /// Class invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(representation != null);
            Contract.Invariant(Count >= 0);
            Contract.Invariant(Count <= Capacity);
        }
    }
}
