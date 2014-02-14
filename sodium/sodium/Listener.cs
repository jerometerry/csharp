namespace sodium
{
    using System;

    public sealed class Listener<TA> : ListenerBase, IDisposable
    {
        /**
         * It's essential that we keep the listener alive while the caller holds
         * the Listener, so that the finalizer doesn't get triggered.
         */
        private readonly Event<TA> _evt;
        private readonly ITransactionHandler<TA> _action;
        private readonly Node _target;

        public Listener(Event<TA> evt, ITransactionHandler<TA> action, Node target)
        {
            _evt = evt;
            _action = action;
            _target = target;
        }

        public override void Unlisten()
        {
            lock (Transaction.ListenersLock)
            {
                _evt.Listeners.Remove(_action);
                _evt.Node.UnlinkTo(_target);
            }
        }

        public void Dispose()
        {
            Unlisten();
        }
    }
}
