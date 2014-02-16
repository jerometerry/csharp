namespace sodium
{
    public class GetBehaviorValueInvoker<TBehavior> : IFunction<Transaction, Event<TBehavior>>
    {
        private readonly Behavior<TBehavior> _behavior;

        public GetBehaviorValueInvoker(Behavior<TBehavior> behavior)
        {
            _behavior = behavior;
        }

        public Event<TBehavior> Apply(Transaction transaction)
        {
            return _behavior.GetValue(transaction);
        }
    }
}