namespace sodium
{
    public sealed class Tuple2<TA, TB>
    {
        public TA V1;
        public TB V2;

        public Tuple2(TA v1, TB v2)
        {
            V1 = v1;
            V2 = v2;
        }
    }
}