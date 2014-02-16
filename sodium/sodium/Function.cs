namespace sodium
{
    using System;

    public class Function<TP, TR> : IFunction<TP, TR>
    {
        private readonly Func<TP, TR> _function;

        public Function(Func<TP, TR> function)
        {
            _function = function;
        }

        public TR Apply(TP p)
        {
            return _function(p);
        }
    }
}
