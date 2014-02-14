namespace sodium
{
    using System;

    public class Lambda2<TA, TB, TC> : ILambda2<TA, TB, TC>
    {
        private readonly Func<TA, TB, TC> _f;

        public Lambda2(Func<TA, TB, TC> f)
        {
            _f = f;
        }

        public TC Apply(TA a, TB b)
        {
            return _f(a, b);
        }
    }
}
