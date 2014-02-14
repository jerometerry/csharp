namespace sodium
{
    using System;

    public class Handler<TA> : IHandler<TA>
    {
        readonly Action<TA> _f;

        public Handler(Action<TA> f)
        {
            _f = f;
        }

        public void Run(TA a)
        {
            _f(a);
        }
    }
}
