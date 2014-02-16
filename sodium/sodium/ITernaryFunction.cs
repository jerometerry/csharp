namespace sodium
{
    public interface ITernaryFunction<in TP1, in TP2, in TP3, out TR>
    {
        TR Apply(TP1 p1, TP2 p2, TP3 p3);
    }
}