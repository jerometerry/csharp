namespace sodium
{
    public interface ILambda2<in TA, in TB, out TC>
    {
        TC Apply(TA a, TB b);
    }
}