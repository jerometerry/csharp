namespace sodium
{
    public class BehaviorLifter3<TA,TB,TC,TD> : ILambda1<TA, ILambda1<TB, ILambda1<TC, TD>>>
    {
        private readonly ILambda3<TA, TB, TC, TD> _f;

        public BehaviorLifter3(ILambda3<TA, TB, TC, TD> f)
        {
            _f = f;
        }

        public ILambda1<TB, ILambda1<TC, TD>> Apply(TA a)
        {
            return new T4<TB, TC, TD>(_f, a);
        }

        private class T4<TB, TC, TD> : ILambda1<TB, ILambda1<TC, TD>>
        {
            private readonly ILambda3<TA, TB, TC, TD> _f;
            private readonly TA _a;

            public T4(ILambda3<TA, TB, TC, TD> f, TA a)
            {
                _f = f;
                _a = a;
            }

            public ILambda1<TC, TD> Apply(TB b)
            {
                return new T5<TB, TC, TD>(_f, _a, b);
            }

            private class T5<TB, TC, TD> : ILambda1<TC, TD>
            {
                private readonly ILambda3<TA, TB, TC, TD> _f;
                private readonly TA _a;
                private readonly TB _b;

                public T5(ILambda3<TA, TB, TC, TD> f, TA a, TB b)
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