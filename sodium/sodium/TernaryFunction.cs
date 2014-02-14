namespace sodium
{
    using System;

    public class TernaryFunction<TP1, TP2, TP3, TR> : ITernaryFunction<TP1, TP2, TP3, TR>
    {
        private readonly Func<TP1, TP2, TP3, TR> _f;

        public TernaryFunction(Func<TP1, TP2, TP3, TR> f)
        {
            _f = f;
        }

        public TR Apply(TP1 a, TP2 b, TP3 c)
        {
            return _f(a, b, c);
        }
    }
}
