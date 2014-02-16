namespace sodium
{
    public sealed class Tuple2<TA, TB>
    {
        public TA X;
        public TB Y;

        public Tuple2(TA x, TB y)
        {
            X = x;
            Y = y;
        }
    }
}