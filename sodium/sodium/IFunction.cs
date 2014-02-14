namespace sodium
{
    public interface IFunction<in TP, out TR>
    {
        TR Apply(TP a);
    }
}