namespace sodium
{
    using System;

    public class SingleParameterFunction<TP, TR> : ISingleParameterFunction<TP, TR>
    {
        private readonly Func<TP, TR> _f;

        public SingleParameterFunction(Func<TP, TR> f)
        {
            _f = f;
        }

        public TR Apply(TP a)
        {
            return _f(a);
        }
    }
}
