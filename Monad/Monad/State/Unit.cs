using System;

namespace Monad.State
{
    /**
     * The <i>unit</i> (<code>return</code>) monad operation.
     *
     * @author Dave Herman
     */
    public class Unit : Computation
    {

        /** the value of the computation. */
        private Object value;

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
         * Applies the unit computation to the given state.
         *
         * @param state the current contents of the state.
         * @return the result of applying the unit computation.
         */
        public override IResut Apply(Object state)
        {
            return new Pair(value, state);
        }

    }
}

