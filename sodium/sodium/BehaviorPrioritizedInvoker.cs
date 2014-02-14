namespace sodium
{
    public class BehaviorPrioritizedInvoker<TA, TB> : IHandler<Transaction>
    {
        public bool Fired = false;
        private readonly EventSink<TB> _o;
        private readonly Behavior<IFunction<TA, TB>> _bf;
        private readonly Behavior<TA> _ba;

        public BehaviorPrioritizedInvoker(EventSink<TB> o, Behavior<IFunction<TA, TB>> bf, Behavior<TA> ba)
        {
            _o = o;
            _bf = bf;
            _ba = ba;
        }

        public void Run(Transaction trans)
        {
            if (Fired)
                return;

            Fired = true;
            trans.Prioritized(_o.Node, new ApplyBehaviorEventSinkSender<TA, TB>(_o, _bf, _ba, this));
        }
    }
}