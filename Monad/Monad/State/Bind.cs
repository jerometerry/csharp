using System;

namespace Monad.State
{

    /**
     * The <i>bind</i> (<code>&gt;&gt;=</code>) monad operation.
     *
     * @author Dave Herman
     */
    public class Bind : Computation
    {

        /** the first sub-computation. */
        private readonly Computation c;

        /** the function to produce the second sub-computation. */
        private readonly IComputationFactory f;

        /**
         * Constructs a bind computation with the given sub-computation and
         * function to produce the next sub-computation.
         *
         * @param m the first sub-computation.
         * @param f the function to produce the second sub-computation.
         */
        public Bind(Computation c, IComputationFactory f)
        {
            this.c = c;
            this.f = f;
        }

        /**
         * Applies the bind computation to the given state.
         *
         * @param state the current contents of the state.
         * @return the result of applying the bind computation.
         */
        public override IResut Apply(Object state)
        {
            var a1 = (Pair)this.c.Apply(state);
            return f.Make(a1.Value()).Apply(a1.State());
        }

    }

}