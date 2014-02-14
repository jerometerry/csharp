namespace sodium
{
    public class BehaviorLifter2<TA, TB,TC> : ILambda1<TA, ILambda1<TB, TC>>
    {
        private readonly ILambda2<TA, TB, TC> _f;

        public BehaviorLifter2(ILambda2<TA, TB, TC> f)
        {
            _f = f;
        }

        public ILambda1<TB, TC> Apply(TA a)
        {
            return new T2<TB, TC>(_f, a);
        }

        private class T2<TB, TC> : ILambda1<TB, TC>
        {
            private readonly TA _a;
            private readonly ILambda2<TA, TB, TC> _f;

            public T2(ILambda2<TA, TB, TC> f, TA a)
            {
                _f = f;
                _a = a;
            }

            public TC Apply(TB b)
            {
                return _f.Apply(_a, b);
            }
        }
    }
}