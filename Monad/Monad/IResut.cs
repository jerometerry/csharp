using System;

namespace Monad
{
    /**
     * The final result of running a computation.
     *
     * @author Dave Herman
     */
    public interface IResut
    {
        /**
         * Returns the value of the result.
         *
         * @return the value of the result.
         */
        Object Value();
    }
}