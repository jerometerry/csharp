namespace sodium
{
    using System;

    class GetBehaviorValueEventSink<TBehavior> : EventSink<TBehavior>
    {
        private readonly Behavior<TBehavior> _behavior;

        public GetBehaviorValueEventSink(Behavior<TBehavior> behavior)
        {
            _behavior = behavior;
        }

        public override Object[] SampleNow()
        {
            return new Object[] 
            { 
                _behavior.Sample() 
            };
        }
    }
}