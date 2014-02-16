namespace sodium
{
    using System;
    using System.Collections.Generic;

    public class FilteredEventSink<TEvent> : EventSink<TEvent>
    {
        private readonly Event<TEvent> _event;
        private readonly IFunction<TEvent, Boolean> _predicate;

        public FilteredEventSink(Event<TEvent> evt, IFunction<TEvent, Boolean> predicate)
        {
            _event = evt;
            _predicate = predicate;
        }

        public override Object[] SampleNow()
        {
            var inputs = _event.SampleNow();
            if (inputs == null)
            {
                return null;
            }

            var outputs = new List<Object>();
            foreach(var i in inputs)
            {
                var evt = (TEvent)i;
                if (_predicate.Apply(evt))
                {
                    outputs.Add(i);
                }
            }

            if (outputs.Count == 0)
            {
                return null;
            }
            else
            {
                return outputs.ToArray();
            }
        }
    }
}
