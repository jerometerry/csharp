namespace sodium
{
    public class BehaviorBuilder<TA> : IFunction<Transaction, Behavior<TA>>
    {
        private readonly Event<TA> _evt;
        private readonly TA _initValue;

        public BehaviorBuilder(Event<TA> evt, TA initValue)
        {
            _evt = evt;
            _initValue = initValue;
        }

        public Behavior<TA> Apply(Transaction trans)
        {
            return new Behavior<TA>(_evt.LastFiringOnly(trans), _initValue);
        }
    }
}