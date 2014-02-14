namespace sodium
{
    public interface ITwoParameterFunction<in TP1, in TP2, out TR>
    {
        TR Apply(TP1 a, TP2 b);
    }
}