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
            EventLoop.Loop(updates);
            Val = behavior.Sample();
        }

        private EventLoop<TBehavior> EventLoop
        {
            get
            {
                return ((EventLoop<TBehavior>)Event);
            }
        }
    }
}