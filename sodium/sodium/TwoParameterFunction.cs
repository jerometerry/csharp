namespace sodium
{
    using System;

    public class TwoParameterFunction<TP1, TP2, TR> : ITwoParameterFunction<TP1, TP2, TR>
    {
        private readonly Func<TP1, TP2, TR> _f;

        public TwoParameterFunction(Func<TP1, TP2, TR> f)
        {
            _f = f;
        }

        public TR Apply(TP1 a, TP2 b)
        {
            return _f(a, b);
        }
    }
}
