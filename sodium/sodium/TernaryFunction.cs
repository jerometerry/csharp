namespace sodium
{
    using System;

    public class TernaryFunction<TP1, TP2, TP3, TR> : ITernaryFunction<TP1, TP2, TP3, TR>
    {
        private readonly Func<TP1, TP2, TP3, TR> _function;

        public TernaryFunction(Func<TP1, TP2, TP3, TR> function)
        {
            _function = function;
        }

        public TR Apply(TP1 p1, TP2 p2, TP3 p3)
        {
            return _function(p1, p2, p3);
        }
    }
}
