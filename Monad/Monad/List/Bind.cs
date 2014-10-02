namespace Monad.List
{
    /**
     * The <i>bind</i> (<code>&gt;&gt;=</code>) monad operation.
     *
     * @author Dave Herman
     */
    public class Bind<T> : Computation
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
         * Applies the bind computation.
         *
         * @return the result of applying the bind computation.
         */
        public override IResut Apply()
        {
            return ((List)c.Apply()).MapAppend(f);
        }
    }
}