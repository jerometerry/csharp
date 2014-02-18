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

        public EventSink<TBehavior> Sink
        {
            get
            {
                return (EventSink<TBehavior>)Event;
            }
        }
    }
}