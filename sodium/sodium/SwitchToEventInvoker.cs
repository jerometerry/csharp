namespace sodium
{
    class SwitchToEventInvoker<TBehavior> : IFunction<Transaction, Event<TBehavior>>
    {
        private readonly Behavior<Event<TBehavior>> _eventBehavior;

        public SwitchToEventInvoker(Behavior<Event<TBehavior>> eventBehavior)
        {
            _eventBehavior = eventBehavior;
        }

        public Event<TBehavior> Apply(Transaction transaction)
        {
            return Behavior<TBehavior>.SwitchE(transaction, _eventBehavior);
        }
    }
}