namespace sodium
{
    public interface IHandler<A>
    {
        void run(A a);
    }
}