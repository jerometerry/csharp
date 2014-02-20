namespace sodium
{
    using System;

    public interface Handler<A> {
        void run(A a);
    }

    public class HandlerImpl<A> : Handler<A>
    {
        private Action<A> _action;

        public HandlerImpl(Action<A> action)
        {
            _action = action;
        }

        public void run(A a)
        {
            _action(a);
        }
    }
}

