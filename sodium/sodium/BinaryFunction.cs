namespace sodium
{
    using System;

    public class BinaryFunction<TP1, TP2, TR> : IBinaryFunction<TP1, TP2, TR>
    {
        private readonly Func<TP1, TP2, TR> _f;

        public BinaryFunction(Func<TP1, TP2, TR> f)
        {
            _f = f;
        }

        public TR Apply(TP1 a, TP2 b)
        {
            return _f(a, b);
        }
    }
}
