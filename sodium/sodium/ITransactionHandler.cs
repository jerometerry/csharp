namespace sodium
{

    public interface ITransactionHandler<A>
    {
        void run(Transaction trans, A a);
    }

}