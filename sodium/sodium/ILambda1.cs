namespace sodium
{
    public interface ILambda1<in TA, out TB>
    {
        TB Apply(TA a);
    }
}