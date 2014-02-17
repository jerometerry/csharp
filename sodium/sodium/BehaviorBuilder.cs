namespace sodium
{
    class BehaviorBuilder<TEvent> : IFunction<Transaction, Behavior<TEvent>>
    {
        private readonly Event<TEvent> _event;
        private readonly TEvent _initValue;

        public BehaviorBuilder(Event<TEvent> evt, TEvent initValue)
        {
            _event = evt;
            _initValue = initValue;
        }

        public Behavior<TEvent> Apply(Transaction transaction)
        {
            var evt = _event.LastFiringOnly(transaction);
            return new Behavior<TEvent>(evt, _initValue);
        }
    }
}