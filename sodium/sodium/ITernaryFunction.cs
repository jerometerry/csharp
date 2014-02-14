namespace sodium
{
    public interface ITernaryFunction<in TP1, in TP2, in TP3, out TR>
    {
        TR Apply(TP1 a, TP2 b, TP3 c);
    }
}