namespace sodium
{
    public interface IHandler<in T>
    {
        void Run(T p);
    }
}