namespace sodium
{
    public class CoalesceInvoker<TA> : ISingleParameterFunction<Transaction, Event<TA>>
    {
        private readonly Event<TA> _evt;
        private readonly ITwoParameterFunction<TA, TA, TA> _f;

        public CoalesceInvoker(Event<TA> evt, ITwoParameterFunction<TA, TA, TA> f)
        {
            _evt = evt;
            _f = f;
        }

        public Event<TA> Apply(Transaction trans)
        {
            return _evt.Coalesce(trans, _f);
        }
    }
}