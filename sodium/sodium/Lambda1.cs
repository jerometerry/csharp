namespace sodium
{
    using System;

    public interface Lambda1<A, B>
    {
        B apply(A a);
    }

    public class Lambda1Impl<A,B> : Lambda1<A,B>
    {
        private readonly Func<A, B> _action;

        public Lambda1Impl(Func<A, B> action)
        {
            _action = action;
        }

        public B apply(A a)
        {
            return _action(a);
        }
    }
}



