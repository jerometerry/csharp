namespace sodium
{
    using System;

    public class Lambda3<TA, TB, TC, TD> : ILambda3<TA, TB, TC, TD>
    {
        private readonly Func<TA, TB, TC, TD> _f;

        public Lambda3(Func<TA, TB, TC, TD> f)
        {
            _f = f;
        }

        public TD Apply(TA a, TB b, TC c)
        {
            return _f(a, b, c);
        }
    }
}
