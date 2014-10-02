using System;

namespace Monad
{
    /**
     * An abstract computation in a monad. Computations are represented as abstract
     * datatypes, which form unevaluated trees. The {@link #apply} method evaluates
     * an entire computation tree, returning the final result of the computation.
     * There are two forms to the <code>apply</code> method, since some monads take
     * a parameter and some do not. (The equivalent in functional programming is
     * computations represented as data vs. functions).
     *
     * @author Dave Herman
    */
    public abstract class Computation
    {
        /**
         * The zero-parameter evaluation method. By default, this method calls
         * the one-parameter evaluation method with a <code>null</code> argument.
         *
         * @return the result of evaluating the computation.
         */
        public virtual IResut Apply()
        {
            return this.Apply(null);
        }

        /**
         * The one-parameter evaluation method. By default, this method calls
         * the zero-parameter evaluation method.
         *
         * @return the result of evaluating the computation.
         */

        public virtual IResut Apply(Object param)
        {
            return this.Apply();
        }
    }
}