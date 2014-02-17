namespace sodium
{
    using System;

    class MergeEventSink<TEvent> : EventSink<TEvent>
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
            var output1 = _event1.SampleNow();
            var output2 = _event2.SampleNow();
            if (output1 != null && output2 != null)
            {
                var outputs = new Object[output1.Length + output2.Length];
                int i = 0;
                foreach (var t in output1)
                    outputs[i++] = t;
                foreach (var t in output2)
                    outputs[i++] = t;
                return outputs;
            }
            else
            { 
                if (output1 != null)
                    return output1;
                else
                    return output2;
                }
        }
    }
}
