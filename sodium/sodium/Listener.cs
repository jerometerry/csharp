namespace sodium
{
    using System;

    public sealed class Listener<TEvent> : ListenerBase, IDisposable
    {
        /**
         * It's essential that we keep the listener alive while the caller holds
         * the Listener, so that the finalizer doesn't get triggered.
         */
        private readonly Event<TEvent> _event;
        private readonly ITransactionHandler<TEvent> _action;
        private readonly Node _target;

        public Listener(Event<TEvent> evt, ITransactionHandler<TEvent> action, Node target)
        {
            _event = evt;
            _action = action;
            _target = target;
        }

        public override void Unlisten()
        {
            lock (Transaction.ListenersLock)
            {
                _event.Listeners.Remove(_action);
                _event.Node.UnlinkTo(_target);
            }
        }

        public void Dispose()
        {
            Unlisten();
        }
    }
}
