using System;

namespace Monad.State
{

    /**
     * The <i>lookup</i> monad operation.
     *
     * @author Dave Herman
     */
    public class Lookup : Computation
    {

        /**
         * Applies the lookup computation to the given state.
         *
         * @param state the current contents of the state.
         * @return the result of applying the lookup computation.
         */
        public override IResut Apply(Object state)
        {
            return new Pair(state, state);
        }

    }

}