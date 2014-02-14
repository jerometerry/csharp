namespace sodium
{
    public class BehaviorLifter2<TA, TB,TC> : IFunction<TA, IFunction<TB, TC>>
    {
        private readonly IBinaryFunction<TA, TB, TC> _f;

        public BehaviorLifter2(IBinaryFunction<TA, TB, TC> f)
        {
            _f = f;
        }

        public IFunction<TB, TC> Apply(TA a)
        {
            return new Lifter(_f, a);
        }

        private class Lifter : IFunction<TB, TC>
        {
            private readonly TA _a;
            private readonly IBinaryFunction<TA, TB, TC> _f;

            public Lifter(IBinaryFunction<TA, TB, TC> f, TA a)
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