namespace sodium
{
    public class ListenerInvoker<TA> : ISingleParameterFunction<Transaction, IListener>
    {
        private readonly Event<TA> _listener;
        private readonly Node _target;
        private readonly ITransactionHandler<TA> _action;

        public ListenerInvoker(Event<TA> listener, Node target, ITransactionHandler<TA> action)
        {
            _listener = listener;
            _target = target;
            _action = action;
        }

        public IListener Apply(Transaction trans)
        {
            return _listener.Listen(_target, trans, _action, false);
        }
    }
}
