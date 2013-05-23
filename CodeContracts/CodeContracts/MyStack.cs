using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace CodeContracts
{
    public class MyStack<T>
    {
        private int capacity;
        private int count;
        private T[] representation;

        public MyStack(int capacity)
        {
            this.capacity = capacity;
            representation = new T[capacity];
        }

        private int Top
        {
            get
            {
                Contract.Requires(Empty == false);
                return count - 1;
            }
        }

        public bool Empty
        {
            get
            {
                return count == 0;
            }
        }

        public bool Full
        {
            get
            {
                return count == capacity;
            }
        }
        
        public void Put(T item)
        {
            Contract.Requires(Full == false);
            Contract.Ensures(Empty == false);
            representation[count] = item;
            count += 1;
        }

        public T Remove()
        {
            Contract.Requires(Empty == false);
            T item = representation[Top];
            count -= 1;
            return item;
        }

        public T Item()
        {
            Contract.Requires(Empty == false);
            return representation[Top];
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(representation != null);
            Contract.Invariant(count >= 0);
            Contract.Invariant(capacity >= 0);
            Contract.Invariant(count <= capacity);
        }
    }
}
