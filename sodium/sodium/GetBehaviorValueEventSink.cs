namespace sodium
{
    using System;

    public class GetBehaviorValueEventSink<TA> : EventSink<TA>
    {
        private readonly Behavior<TA> _b;

        public GetBehaviorValueEventSink(Behavior<TA> b)
        {
            _b = b;
        }

        public override Object[] SampleNow()
        {
            return new Object[] { _b.Sample() };
        }
    }
}