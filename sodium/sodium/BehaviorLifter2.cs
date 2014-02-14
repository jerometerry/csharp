namespace sodium
{
    public class BehaviorLifter2<TA, TB,TC> : ISingleParameterFunction<TA, ISingleParameterFunction<TB, TC>>
    {
        private readonly ITwoParameterFunction<TA, TB, TC> _f;

        public BehaviorLifter2(ITwoParameterFunction<TA, TB, TC> f)
        {
            _f = f;
        }

        public ISingleParameterFunction<TB, TC> Apply(TA a)
        {
            return new Lifter(_f, a);
        }

        private class Lifter : ISingleParameterFunction<TB, TC>
        {
            private readonly TA _a;
            private readonly ITwoParameterFunction<TA, TB, TC> _f;

            public Lifter(ITwoParameterFunction<TA, TB, TC> f, TA a)
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