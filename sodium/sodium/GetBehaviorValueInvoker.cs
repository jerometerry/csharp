namespace sodium
{
    public class GetBehaviorValueInvoker<TBehavior> : IFunction<Transaction, Event<TBehavior>>
    {
        private readonly Behavior<TBehavior> _b;

        public GetBehaviorValueInvoker(Behavior<TBehavior> b)
        {
            _b = b;
        }

        public Event<TBehavior> Apply(Transaction trans)
        {
            return _b.GetValue(trans);
        }
    }
}