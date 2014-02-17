namespace sodium
{
    class CoalesceInvoker<TEvent> : IFunction<Transaction, Event<TEvent>>
    {
        private readonly Event<TEvent> _event;
        private readonly IBinaryFunction<TEvent, TEvent, TEvent> _combiningFunction;

        public CoalesceInvoker(Event<TEvent> evt, IBinaryFunction<TEvent, TEvent, TEvent> combiningFunction)
        {
            _event = evt;
            _combiningFunction = combiningFunction;
        }

        public Event<TEvent> Apply(Transaction transaction)
        {
            return _event.Coalesce(transaction, _combiningFunction);
        }
    }
}