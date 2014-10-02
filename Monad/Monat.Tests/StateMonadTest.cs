using System;
using System.Collections;
using FluentAssertions;
using Monad;
using Monad.State;
using NUnit.Framework;

namespace Monat.Tests
{
    [TestFixture]
    public class StateMonadTest
    {
        /**
         * Example of using the state monad.
         *
         * @author Dave Herman
         */
        private static class Example
        {
            /**
             * Simple example of running a string through a calculation involving
             * destructive update modelled by a state monad.
             *
             * @param initial the starting string.
             * @return the result of running the string through the computation.
             */
            public static String ToUpperCase(String initial)
            {
                var m = new StateMonad();

                // do a <- lookup
                //    mutate(a.toUpperCase)
                //    lookup

                Computation c =
                    m.Bind(m.Lookup(),
                           new C(m));
                return (String)m.Run(c, initial).Value();
            }

            private class C : IComputationFactory
            {
                private StateMonad m;

                public C(StateMonad m)
                {
                    this.m = m;
                }

                public Computation Make(object o)
                {
                    return Make((string)o);
                }

                private Computation Make(string a)
                {
                    return m.Sequence(m.Mutate(a.ToUpper()), m.Lookup());
                }
            }
        }

        [TestCase("ThIs Is a TeSt StRiNg")]
        public void Test(string s)
        {
            var res = Example.ToUpperCase(s);
            res.Should().NotBeNullOrEmpty();
            Assert.True(res.Equals(s.ToUpper()));
        }
    }
}
