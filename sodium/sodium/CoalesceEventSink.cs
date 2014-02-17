namespace sodium
{
    using System;

    class CoalesceEventSink<TEvent> : EventSink<TEvent>
    {
        private readonly Event<TEvent> _event;
        private readonly IBinaryFunction<TEvent, TEvent, TEvent> _combiningFunction;

        public CoalesceEventSink(Event<TEvent> evt, IBinaryFunction<TEvent, TEvent, TEvent> combiningFunction)
        {
            _event = evt;
            _combiningFunction = combiningFunction;
        }

        public override Object[] SampleNow()
        {
            var oi = _event.SampleNow();
            if (oi != null)
            {
                var o = (TEvent)oi[0];
                for (var i = 1; i < oi.Length; i++)
                    o = _combiningFunction.Apply(o, (TEvent)oi[i]);
                return new Object[] { o };
            }
            else
            { 
                return null;
            }
        }
    }
}
