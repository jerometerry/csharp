namespace sodium
{
    public sealed class BehaviorLoop<TBehavior> : Behavior<TBehavior>
    {
        public BehaviorLoop()
            : base(new EventLoop<TBehavior>(), default(TBehavior))
        {
        }

        public void Loop(Behavior<TBehavior> behavior)
        {
            var updates = behavior.Updates();
            var eventLoop = ((EventLoop<TBehavior>)Event);
            eventLoop.Loop(updates);
            Value = behavior.Sample();
        }
    }
}