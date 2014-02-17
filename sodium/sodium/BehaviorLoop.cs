namespace sodium
{
    public sealed class BehaviorLoop<TBehavior> : Behavior<TBehavior>
    {
        public BehaviorLoop()
            : base(new EventLoop<TBehavior>(), Maybe<TBehavior>.Null)
        {
        }

        public void Loop(Behavior<TBehavior> behavior)
        {
            var updates = behavior.Updates();
            var eventLoop = ((EventLoop<TBehavior>)Event);
            eventLoop.Loop(updates);
            Val = behavior.Sample();
        }
    }
}