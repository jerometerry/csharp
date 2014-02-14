namespace sodium
{
    using System;

    public class MergeEventSink<TA> : EventSink<TA>
    {
        private readonly Event<TA> _ea;
        private readonly Event<TA> _eb;

        public MergeEventSink(Event<TA> ea, Event<TA> eb)
        {
            _ea = ea;
            _eb = eb;
        }

        public override Object[] SampleNow()
        {
            var oa = _ea.SampleNow();
            var ob = _eb.SampleNow();
            if (oa != null && ob != null)
            {
                var oo = new Object[oa.Length + ob.Length];
                int i = 0;
                foreach (var t in oa)
                    oo[i++] = t;
                foreach (var t in ob)
                    oo[i++] = t;
                return oo;
            }
            else
                if (oa != null)
                    return oa;
                else
                    return ob;
        }
    }
}
