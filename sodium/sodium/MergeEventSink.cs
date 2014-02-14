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
                int j = 0;
                for (var i = 0; i < oa.Length; i++) oo[j++] = oa[i];
                for (var i = 0; i < ob.Length; i++) oo[j++] = ob[i];
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
