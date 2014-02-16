namespace sodium
{
    using System;

    public class FilterEventSink<TEvent> : EventSink<TEvent>
    {
        private readonly Event<TEvent> _event;
        private readonly IFunction<TEvent, Boolean> _predicate;

        public FilterEventSink(Event<TEvent> evt, IFunction<TEvent, Boolean> predicate)
        {
            _event = evt;
            _predicate = predicate;
        }

        public override Object[] SampleNow()
        {
            var oi = _event.SampleNow();
            if (oi != null)
            {
                var oo = new Object[oi.Length];
                var j = 0;
                for (var i = 0; i < oi.Length; i++)
                    if (_predicate.Apply((TEvent)oi[i]))
                        oo[j++] = oi[i];
                if (j == 0)
                    oo = null;
                else
                    if (j < oo.Length)
                    {
                        var oo2 = new Object[j];
                        for (var i = 0; i < j; i++)
                            oo2[i] = oo[i];
                        oo = oo2;
                    }
                return oo;
            }
            else
                return null;
        }
    }
}
