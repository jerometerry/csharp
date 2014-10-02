using System;

namespace Monad
{
    /**
     * A monad, represented as an abstract factory of computations. Computations
     * are represented as ADT trees that can be run via the {@link Comp#apply}
     * method.
     *
     * @author Dave Herman
     */
    public abstract class Monad
    {
        /**
         * Produces a <i>unit</i> (<code>return</code>) computation with the
         * given value.
         *
         * @param o the value of the unit computation.
         * @return a unit computation.
         */
        public abstract Computation Unit(Object o);

        /**
         * Produces a <i>bind</i> (<code>&gt;&gt;=</code>) computation with the
         * given initial computation and function to produce the subsequent
         * computation.
         *
         * @param c the initial computation.
         * @param f the function to produce the subsequent computation.
         * @return a bind computation.
         */
        public abstract Computation Bind(Computation c, IComputationFactory f);

        /**
         * Produces a <i>sequence</i> (<code>&gt;&gt;</code>) computation, which
         * runs two sub-computations in sequence.
         *
         * @param c1 the first sub-computation.
         * @param c2 the second sub-computation.
         * @return a sequence computation.
         */
        public Computation Sequence(Computation c1, Computation c2)
        {
            return this.Bind(c1, new ComputationFactory(c2));
        }

        /**
         * Runs the given computation and returns its final answer.
         *
         * @param c the computation to run.
         * @return the final answer.
         */
        public IResut Run(Computation c)
        {
            return c.Apply();
        }

        /**
         * Runs the given computation with the given initial argument and returns
         * its final answer.
         *
         * @param c the computation to run.
         * @param initArg the initial argument to the computation.
         */
        public IResut Run(Computation c, Object initArg)
        {
            return c.Apply(initArg);
        }
    }
}