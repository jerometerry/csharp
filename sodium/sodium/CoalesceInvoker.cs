namespace sodium
{
    public class CoalesceInvoker<TA> : ILambda1<Transaction, Event<TA>>
    {
        private readonly Event<TA> _evt;
        private readonly ILambda2<TA, TA, TA> _f;

        public CoalesceInvoker(Event<TA> evt, ILambda2<TA, TA, TA> f)
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