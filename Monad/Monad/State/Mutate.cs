using System;

namespace Monad.State
{

    /**
     * The <i>mutate</i> monad operation.
     *
     * @author Dave Herman
     */
    public class Mutate : Computation
    {

        /** the new contents of the state. */
        private Object newState;

        /**
         * Constructs a new <i>mutate</i> computation with the given value to
         * mutate the state.
         *
         * @param newState the new contents of the state.
         */
        public Mutate(Object newState)
        {
            this.newState = newState;
        }

        /**
         * Applies the mutate computation to the given state.
         *
         * @param state the contents of the state before applying the computation.
         * @return the result of applying the mutate computation.
         */
        public override IResut Apply(Object state)
        {
            return new Pair(null, newState);
        }

    }

}