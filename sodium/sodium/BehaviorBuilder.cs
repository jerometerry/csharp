namespace sodium
{
    public class BehaviorBuilder<TEvent> : IFunction<Transaction, Behavior<TEvent>>
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
            return new Behavior<TEvent>(_event.LastFiringOnly(transaction), _initValue);
        }
    }
}