namespace sodium
{
    using System;

    class MappedEventSink<TEvent, TNewEvent> : EventSink<TNewEvent>
    {
        private readonly Event<TEvent> _event;
        private readonly IFunction<TEvent, TNewEvent> _mapFunction;

        public MappedEventSink(Event<TEvent> evt, IFunction<TEvent, TNewEvent> mapFunction)
        {
            _event = evt;
            _mapFunction = mapFunction;
        }

        public override Object[] SampleNow()
        {
            var oi = _event.SampleNow();
            if (oi == null)
                return null;
            
            var results = new Object[oi.Length];
            for (var i = 0; i < results.Length; i++)
                results[i] = _mapFunction.Apply((TEvent)oi[i]);
            return results;
            
        }
    }
}