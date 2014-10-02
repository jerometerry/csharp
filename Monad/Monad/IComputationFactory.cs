using System;

namespace Monad
{
    /**
     * A computation-producing function, used as the second parameter to the
     * {@link Monad#bind} factory method.
     *
     * @author Dave Herman
     */
    public interface IComputationFactory
    {

        /**
         * Given the result of the previous computation, produces the next
         * computation.
         *
         * @param o the result of the previous computation.
         * @return the next computation.
         */
        Computation Make(Object o);

    }
}