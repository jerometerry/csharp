namespace sodium
{
    public class GetBehaviorValueInvoker<TA> : ISingleParameterFunction<Transaction, Event<TA>>
    {
        private readonly Behavior<TA> _b;

        public GetBehaviorValueInvoker(Behavior<TA> b)
        {
            _b = b;
        }

        public Event<TA> Apply(Transaction trans)
        {
            return _b.GetValue(trans);
        }
    }
}