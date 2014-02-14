namespace sodium
{
    public class BehaviorListenInvoker<TA> : IHandler<Transaction>
    {
        private readonly Behavior<TA> _b;
        private readonly Event<TA> _evt;

        public BehaviorListenInvoker(Behavior<TA> b, Event<TA> evt)
        {
            _b = b;
            _evt = evt;
        }

        public void Run(Transaction trans1)
        {
            _b.Cleanup = _evt.Listen(Node.NULL, trans1, new BehaviorTransactionHandler<TA>(_b), false);
        }
    }
}