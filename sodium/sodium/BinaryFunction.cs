namespace sodium
{
    using System;

    public class BinaryFunction<TP1, TP2, TR> : IBinaryFunction<TP1, TP2, TR>
    {
        private readonly Func<TP1, TP2, TR> _function;

        public BinaryFunction(Func<TP1, TP2, TR> function)
        {
            _function = function;
        }

        public TR Apply(TP1 p1, TP2 p2)
        {
            return _function(p1, p2);
        }
    }
}
