namespace sodium
{
    public class SwitchToEventInvoker<TA> : ILambda1<Transaction, Event<TA>>
    {
        private readonly Behavior<Event<TA>> _bea;

        public SwitchToEventInvoker(Behavior<Event<TA>> bea)
        {
            _bea = bea;
        }

        public Event<TA> Apply(Transaction trans)
        {
            return Behavior<TA>.SwitchE(trans, _bea);
        }
    }
}