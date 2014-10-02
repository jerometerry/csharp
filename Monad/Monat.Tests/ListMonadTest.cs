using System;
using System.Collections;
using FluentAssertions;
using Monad;
using Monad.List;
using NUnit.Framework;

namespace Monat.Tests
{
    using System.Collections.Generic;

    /**
     * Example of using the list monad.
     *
     * @author Dave Herman
     */
    [TestFixture]
    public class ListMonadTest
    {
        private class Example
        {
            private readonly ListMonad<long> m = new ListMonad<long>();

            /**
             * Produces a computation that will calculate the list of integers
             * between <code>n</code> and <code>upper</code>, inclusive.
             *
             * @param n the lower bound.
             * @param upper the upper bound.
             * @return the computation that will calculate the list of integers
             *         between the two bounds.
             */

            private Computation IntsBetween(long n, long upper)
            {
                return n > upper ? this.m.Fail() : this.m.Disjoin(this.m.Unit(n), this.IntsBetween(n + 1, upper));
            }

            /**
             * Determines whether <code>n</code> is even.
             *
             * @param n the number to check.
             * @return <code>true</code> if <code>n</code> is even;
             *         <code>false</code> if not.
             */

            private static bool IsEven(long n)
            {
                return n % 2L == 0L;
            }

            /**
             * Calculates the list of natural numbers up to and potentially
             * including an upper limit by running a computation through the
             * list monad.
             *
             * @param upper the upper bound, inclusive (if it is even).
             * @return the list of natural numbers up to an potentially including
             *         <code>upper</code>.
             */
            public ArrayList EvensTo(long upper)
            {
                var c = m.Bind(this.IntsBetween(0, upper), new C(m));
                return (ArrayList)m.Run(c).Value();
            }

            private class C : IComputationFactory
            {
                private readonly ListMonad<long> m;

                public C(ListMonad<long> m)
                {
                    this.m = m;
                }

                public Computation Make(Object o)
                {
                    return this.Make((long)o);
                }

                private Computation Make(long n)
                {
                    return IsEven(n) ? this.m.Unit(n) : this.m.Fail();
                }
            }
        }

        [TestCase(10)]
        public void Test(long n)
        {
            var e = new Example();
            var res = e.EvensTo(n);
            res.Should().NotBeNullOrEmpty();
        }
    }
}
