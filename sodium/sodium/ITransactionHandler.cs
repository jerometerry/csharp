namespace sodium
{
    public interface ITransactionHandler<in T>
    {
        void Run(Transaction transaction, T p);
    }
}