namespace sodium
{
    using System;

    public class Lambda1<TA, TB> : ILambda1<TA, TB>
    {
        private readonly Func<TA, TB> _f;

        public Lambda1(Func<TA, TB> f)
        {
            _f = f;
        }

        public TB Apply(TA a)
        {
            return _f(a);
        }
    }
}
