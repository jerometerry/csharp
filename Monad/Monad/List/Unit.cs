using System;

namespace Monad.List
{
    /**
     * The <i>unit</i> (<code>return</code>) monad operation.
     *
     * @author Dave Herman
     */
    public class Unit : Computation
    {
        /** the value of the computation. */
        private readonly Object value;

        /**
         * Constructs a unit computation with the given value.
         *
         * @param value the value of the unit computation.
         */
        public Unit(Object value)
        {
            this.value = value;
        }

        /**
         * Applies the unit computation.
         *
         * @return the result of applying the unit computation.
         */
        public override IResut Apply()
        {
            return new List(value);
        }
    }
}