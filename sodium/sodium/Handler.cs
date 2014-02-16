namespace sodium
{
    using System;

    public class Handler<T> : IHandler<T>
    {
        readonly Action<T> _function;

        public Handler(Action<T> function)
        {
            _function = function;
        }

        public void Run(T p)
        {
            _function(p);
        }
    }
}
