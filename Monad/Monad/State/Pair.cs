using System;

namespace Monad.State
{
    /**
     * A result value in the state monad, which consists of a pair containing
     * the value of the computation and the contents of the state.
     *
     * @author Dave Herman
     */
    public class Pair : IResut
    {

        /** the value of the computation. */
        private Object value;

        /** the contents of the state. */
        private Object state;

        /**
         * Constructs a new result value.
         *
         * @param value the value of the computation.
         * @param state the contents of the state.
         */
        public Pair(Object value, Object state)
        {
            this.value = value;
            this.state = state;
        }

        public Object Value()
        {
            return value;
        }

        /**
         * Returns the contents of the state.
         *
         * @return the contents of the state.
         */
        public Object State()
        {
            return state;
        }

    }
}
