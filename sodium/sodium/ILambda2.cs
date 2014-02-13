namespace sodium
{

    public interface ILambda2<A, B, C>
    {
        C apply(A a, B b);
    }

}