namespace sodium
{
    using System;

    public class ThreeParameterFunction<TP1, TP2, TP3, TR> : IThreeParameterFunction<TP1, TP2, TP3, TR>
    {
        private readonly Func<TP1, TP2, TP3, TR> _f;

        public ThreeParameterFunction(Func<TP1, TP2, TP3, TR> f)
        {
            _f = f;
        }

        public TR Apply(TP1 a, TP2 b, TP3 c)
        {
            return _f(a, b, c);
        }
    }
}
