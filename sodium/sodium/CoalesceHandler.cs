namespace sodium
{
    public class CoalesceHandler<TA> : ITransactionHandler<TA>
    {
        private readonly ITwoParameterFunction<TA, TA, TA> _f;
        private readonly EventSink<TA> _o;
        public bool AccumValid = false;
        public TA Accum;

        public CoalesceHandler(ITwoParameterFunction<TA, TA, TA> f, EventSink<TA> o)
        {
            _f = f;
            _o = o;
        }

        public void Run(Transaction trans1, TA a)
        {
            if (AccumValid)
                Accum = _f.Apply(Accum, a);
            else
            {
                trans1.Prioritized(_o.Node, new CoalesceTransactionHandler<TA>(this, _o));
                Accum = a;
                AccumValid = true;
            }
        }
    }
}
