namespace Monad.List
{
    /**
     * The <i>disjoin</i> monad operation.
     *
     * @author Dave Herman
     */
    public class Disjoin<T> : Computation
    {
        /** the first sub-computation. */
        private readonly Computation c1;

        /** the second sub-computation. */
        private readonly Computation c2;

        /**
         * Constructs a disjunction computation with the given two sub-computations.
         *
         * @param m1 the first sub-computation.
         * @param m2 the second sub-computation.
         */
        public Disjoin(Computation c1, Computation c2)
        {
            this.c1 = c1;
            this.c2 = c2;
        }

        /**
         * Applies the disjunction computation.
         *
         * @return the result of applying the bind computation.
         */
        public override IResut Apply()
        {
            return List.Append((List)this.c1.Apply(), (List)this.c2.Apply());
        }
    }
}