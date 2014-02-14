namespace sodium
{
    public class ApplyBehaviorEventSinkSender<TA, TB> : IHandler<Transaction>
    {
        private readonly EventSink<TB> _o;
        private readonly Behavior<IFunction<TA, TB>> _bf;
        private readonly Behavior<TA> _ba;
        private readonly BehaviorPrioritizedInvoker<TA, TB> _behaviorPrioritizedInvoker;

        public ApplyBehaviorEventSinkSender(EventSink<TB> o, Behavior<IFunction<TA, TB>> bf, Behavior<TA> ba, BehaviorPrioritizedInvoker<TA, TB> behaviorPrioritizedInvoker)
        {
            _o = o;
            _bf = bf;
            _ba = ba;
            _behaviorPrioritizedInvoker = behaviorPrioritizedInvoker;
        }

        public void Run(Transaction trans)
        {
            _o.Send(trans, _bf.NewValue().Apply(_ba.NewValue()));
            _behaviorPrioritizedInvoker.Fired = false;
        }
    }
}