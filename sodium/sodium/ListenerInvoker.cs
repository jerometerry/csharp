namespace sodium
{
    class ListenerInvoker<TEvent> : IFunction<Transaction, IListener>
    {
        private readonly Event<TEvent> _listener;
        private readonly Node _target;
        private readonly ITransactionHandler<TEvent> _action;

        public ListenerInvoker(Event<TEvent> listener, Node target, ITransactionHandler<TEvent> action)
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
