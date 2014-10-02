using System;

namespace Monad.List
{
    /**
     * The list monad, which runs non-deterministic computations, generating a
     * list of all valid results.
     *
     * @author Dave Herman
     */
    public class ListMonad<T> : Monad
    {
        public override Computation Unit(Object o)
        {
            return new Unit(o);
        }

        public override Computation Bind(Computation c, IComputationFactory f)
        {
            return new Bind<T>(c, f);
        }

        /**
         * Produces a <i>fail</i> computation, which generates an empty list of
         * results.
         *
         * @return a failure computation.
         */
        public Computation Fail()
        {
            return new Fail<T>();
        }

        /**
         * Produces a <i>disjoin</i> computation, which appends the lists of
         * results from both sub-computations.
         *
         * @param c1 the first sub-computation.
         * @param c2 the second sub-computation.
         * @return a disjunction computation.
         */
        public Computation Disjoin(Computation c1, Computation c2)
        {
            return new Disjoin<T>(c1, c2);
        }

    }
}