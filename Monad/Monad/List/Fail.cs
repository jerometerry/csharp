namespace Monad.List
{
    /**
     * The <i>fail</i> monad operation.
     *
     * @author Dave Herman
     */
    public class Fail<T> : Computation
    {
        /**
         * Applies the failure computation.
         *
         * @return the result of applying the failure computation.
         */
        public override IResut Apply()
        {
            return List.Empty;
        }
    }
}
