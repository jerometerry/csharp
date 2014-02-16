namespace sodium
{
    public interface IBinaryFunction<in TP1, in TP2, out TR>
    {
        TR Apply(TP1 p1, TP2 p2);
    }
}