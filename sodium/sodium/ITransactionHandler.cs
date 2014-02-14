namespace sodium
{
    public interface ITransactionHandler<TA>
    {
        void Run(Transaction trans, TA a);
    }
}