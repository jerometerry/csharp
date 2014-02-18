namespace sodium
{
    public sealed class BehaviorSink<TBehavior> : Behavior<TBehavior>
    {
        public BehaviorSink(TBehavior initValue)
            : base(new EventSink<TBehavior>(), initValue)
        {
        }

        public void Send(TBehavior behavior)
        {
            Sink.Send(behavior);
        }

        private EventSink<TBehavior> Sink
        {
            get
            {
                return (EventSink<TBehavior>)Event;
            }
        }
    }
}