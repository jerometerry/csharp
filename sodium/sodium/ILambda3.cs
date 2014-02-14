namespace sodium
{
    public interface ILambda3<in TA, in TB, in TC, out TD>
    {
        TD Apply(TA a, TB b, TC c);
    }
}