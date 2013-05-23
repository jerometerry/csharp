using System.Diagnostics.Contracts;

namespace CodeContracts
{
    /// <summary>
    /// MyStackTests is used to test pushing / poping MyStack of type int,
    /// using Code Contracts
    /// </summary>
    public class MyStackTests
    {
        private MyStack<int> stack;

        public int Capacity { get; private set; }
        public int NumItems { get; private set; }

        public MyStackTests(int capacity, int numItems)
        {
            this.Capacity = capacity;
            this.NumItems = numItems;
            stack = new MyStack<int>(Capacity);
        }

        public void Test()
        {
            // Post-conditions
            Contract.Ensures(stack.Count == 0);
            Contract.Ensures(stack.Empty == true);
            Contract.Ensures(stack.Full == false);
            Contract.Ensures(stack.Capacity == Capacity);

            TestPush();

            TestPop();
        }

        private void TestPush()
        {
            for (int i = 0; i < NumItems; i++)
            {
                stack.Put(i);
                int top = stack.Item();
                Contract.Assert(top == i);
                Contract.Assert(stack.Count == i + 1);
            }
        }

        private void TestPop()
        {
            for (int i = NumItems - 1; i >= 0; i--)
            {
                int top = stack.Remove();
                Contract.Assert(i == top);
            }
        }

        /// <summary>
        /// Class invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(NumItems >= 0);
            Contract.Invariant(Capacity >= 0);
        }
    }
}