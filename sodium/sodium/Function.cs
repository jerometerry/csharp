namespace sodium
{
    using System;

    public class Function<TP, TR> : IFunction<TP, TR>
    {
        private readonly Func<TP, TR> _f;

        public Function(Func<TP, TR> f)
        {
            _f = f;
        }

        public TR Apply(TP a)
        {
            return _f(a);
        }
    }
}
