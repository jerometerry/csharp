namespace sodium
{
    public class BehaviorLifter3<TA,TB,TC,TD> : ISingleParameterFunction<TA, ISingleParameterFunction<TB, ISingleParameterFunction<TC, TD>>>
    {
        private readonly IThreeParameterFunction<TA, TB, TC, TD> _f;

        public BehaviorLifter3(IThreeParameterFunction<TA, TB, TC, TD> f)
        {
            _f = f;
        }

        public ISingleParameterFunction<TB, ISingleParameterFunction<TC, TD>> Apply(TA a)
        {
            return new Lifter1(_f, a);
        }

        private class Lifter1 : ISingleParameterFunction<TB, ISingleParameterFunction<TC, TD>>
        {
            private readonly IThreeParameterFunction<TA, TB, TC, TD> _f;
            private readonly TA _a;

            public Lifter1(IThreeParameterFunction<TA, TB, TC, TD> f, TA a)
            {
                _f = f;
                _a = a;
            }

            public ISingleParameterFunction<TC, TD> Apply(TB b)
            {
                return new Lifter2(_f, _a, b);
            }

            private class Lifter2 : ISingleParameterFunction<TC, TD>
            {
                private readonly IThreeParameterFunction<TA, TB, TC, TD> _f;
                private readonly TA _a;
                private readonly TB _b;

                public Lifter2(IThreeParameterFunction<TA, TB, TC, TD> f, TA a, TB b)
                {
                    _f = f;
                    _a = a;
                    _b = b;
                }

                public TD Apply(TC c)
                {
                    return _f.Apply(_a, _b, c);
                }
            }
        }
    }
}