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
            var sink = (EventSink<TBehavior>)Event;
            sink.Send(behavior);
        }
    }
}