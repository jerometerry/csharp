using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeContracts
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new MyStack<int>(10);
            s.Put(1);
            s.Put(2);
            s.Put(3);
            int top = s.Item();
        }
    }
}
