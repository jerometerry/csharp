namespace sodium
{
    public interface ISingleParameterFunction<in TP, out TR>
    {
        TR Apply(TP a);
    }
}