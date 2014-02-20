using System;
using NUnit.Framework;

namespace sodium
{
    [TestFixture]
    public class PriorityQueuesProgram
    {
        [Test]
        public void TestPriorityQueue()
        {
            Console.WriteLine("\nBegin Priority Queue demo");

            Console.WriteLine("\nCreating priority queue of Employee items\n");
            PriorityQueue<Employee> pq = new PriorityQueue<Employee>();

            Employee e1 = new Employee("Aiden", 1.0);
            Employee e2 = new Employee("Baker", 2.0);
            Employee e3 = new Employee("Chung", 3.0);
            Employee e4 = new Employee("Dunne", 4.0);
            Employee e5 = new Employee("Eason", 5.0);
            Employee e6 = new Employee("Flynn", 6.0);

            Console.WriteLine("Adding " + e5.ToString() + " to priority queue");
            pq.Enqueue(e5);
            Console.WriteLine("Adding " + e3.ToString() + " to priority queue");
            pq.Enqueue(e3);
            Console.WriteLine("Adding " + e6.ToString() + " to priority queue");
            pq.Enqueue(e6);
            Console.WriteLine("Adding " + e4.ToString() + " to priority queue");
            pq.Enqueue(e4);
            Console.WriteLine("Adding " + e1.ToString() + " to priority queue");
            pq.Enqueue(e1);
            Console.WriteLine("Adding " + e2.ToString() + " to priority queue");
            pq.Enqueue(e2);

            Console.WriteLine("\nPriory queue is: ");
            Console.WriteLine(pq.ToString());
            Console.WriteLine("\n");

            Console.WriteLine("Removing an employee from priority queue");
            Employee e = pq.Dequeue();
            Console.WriteLine("Removed employee is " + e.ToString());
            Console.WriteLine("\nPriory queue is now: ");
            Console.WriteLine(pq.ToString());
            Console.WriteLine("\n");

            Console.WriteLine("Removing a second employee from queue");
            e = pq.Dequeue();
            Console.WriteLine("\nPriory queue is now: ");
            Console.WriteLine(pq.ToString());
            Console.WriteLine("\n");

            Console.WriteLine("Testing the priority queue");
            TestPriorityQueue(50000);


            Console.WriteLine("\nEnd Priority Queue demo");
            Console.ReadLine();
        } // Main()

        static void TestPriorityQueue(int numOperations)
        {
            Random rand = new Random(0);
            PriorityQueue<Employee> pq = new PriorityQueue<Employee>();
            for (int op = 0; op < numOperations; ++op)
            {
                int opType = rand.Next(0, 2);

                if (opType == 0) // enqueue
                {
                    string lastName = op + "man";
                    double priority = (100.0 - 1.0) * rand.NextDouble() + 1.0;
                    pq.Enqueue(new Employee(lastName, priority));
                    if (pq.IsConsistent() == false)
                    {
                        Console.WriteLine("Test fails after enqueue operation # " + op);
                    }
                }
                else // dequeue
                {
                    if (pq.Count() > 0)
                    {
                        Employee e = pq.Dequeue();
                        if (pq.IsConsistent() == false)
                        {
                            Console.WriteLine("Test fails after dequeue operation # " + op);
                        }
                    }
                }
            } // for
            Console.WriteLine("\nAll tests passed");
        } // TestPriorityQueue

    } // class PriorityQueuesProgram

    // ===================================================================

    public class Employee : IComparable<Employee>
    {
        public string lastName;
        public double priority; // smaller values are higher priority

        public Employee(string lastName, double priority)
        {
            this.lastName = lastName;
            this.priority = priority;
        }

        public override string ToString()
        {
            return "(" + lastName + ", " + priority.ToString("F1") + ")";
        }

        public int CompareTo(Employee other)
        {
            if (this.priority < other.priority) return -1;
            else if (this.priority > other.priority) return 1;
            else return 0;
        }
    } // Employee

    // ===================================================================

    // PriorityQueue

} // ns
