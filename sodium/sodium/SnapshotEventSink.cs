namespace sodium
{
    using System;

    public class SnapshotEventSink<TEvent, TBehavior, TSnapshot> : EventSink<TSnapshot>
    {
        private readonly Event<TEvent> _event;
        private readonly IBinaryFunction<TEvent, TBehavior, TSnapshot> _snapshotFunction;
        private readonly Behavior<TBehavior> _behavior;

        public SnapshotEventSink(
            Event<TEvent> ev, 
            IBinaryFunction<TEvent, TBehavior, TSnapshot> snapshotFunction, 
            Behavior<TBehavior> behavior)
        {
            _event = ev;
            _snapshotFunction = snapshotFunction;
            _behavior = behavior;
        }

        public override Object[] SampleNow()
        {
            var oi = _event.SampleNow();
            if (oi != null)
            {
                var oo = new Object[oi.Length];
                for (int i = 0; i < oo.Length; i++)
                    oo[i] = _snapshotFunction.Apply((TEvent)oi[i], _behavior.Sample());
                return oo;
            }
            else
                return null;
        }
    }
}
