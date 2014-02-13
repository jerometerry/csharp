namespace sodium
{

    public interface ILambda3<A, B, C, D>
    {
        D apply(A a, B b, C c);
    }

}