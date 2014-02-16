namespace sodium
{
    using System;

    public class MergeEventSink<TEvent> : EventSink<TEvent>
    {
        private readonly Event<TEvent> _event1;
        private readonly Event<TEvent> _event2;

        public MergeEventSink(Event<TEvent> evt1, Event<TEvent> evt2)
        {
            _event1 = evt1;
            _event2 = evt2;
        }

        public override Object[] SampleNow()
        {
            var o1 = _event1.SampleNow();
            var o2 = _event2.SampleNow();
            if (o1 != null && o2 != null)
            {
                var oo = new Object[o1.Length + o2.Length];
                int i = 0;
                foreach (var t in o1)
                    oo[i++] = t;
                foreach (var t in o2)
                    oo[i++] = t;
                return oo;
            }
            else
                if (o1 != null)
                    return o1;
                else
                    return o2;
        }
    }
}
