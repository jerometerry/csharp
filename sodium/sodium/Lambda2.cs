namespace sodium {
    using System;

    public interface Lambda2<A,B,C> {
        C apply(A a, B b);
    }

    public class Lambda2Impl<A,B,C> : Lambda2<A,B,C>
    {
        public Func<A, B, C> _f;

        public Lambda2Impl(Func<A, B, C> f)
        {
            _f = f;
        }

        public C apply(A a, B b)
        {
            return _f(a, b);
        }
    }
}
