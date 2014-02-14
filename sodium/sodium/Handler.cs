namespace sodium
{
    using System;

    public class Handler<A> : IHandler<A>
    {
        Action<A> f;

        public Handler(Action<A> f)
        {
            this.f = f;
        }

        public void run(A a)
        {
            this.f(a);
        }
    }
}
