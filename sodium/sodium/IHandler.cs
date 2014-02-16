namespace sodium
{
    public interface IHandler<in TA>
    {
        void Run(TA p);
    }
}