namespace sodium
{
    using System;

    public class FilterEventSink<TA> : EventSink<TA>
    {
        private readonly Event<TA> _ev;
        private readonly ISingleParameterFunction<TA, Boolean> _f;

        public FilterEventSink(Event<TA> ev, ISingleParameterFunction<TA, Boolean> f)
        {
            _ev = ev;
            _f = f;
        }

        public override Object[] SampleNow()
        {
            var oi = _ev.SampleNow();
            if (oi != null)
            {
                var oo = new Object[oi.Length];
                var j = 0;
                for (var i = 0; i < oi.Length; i++)
                    if (_f.Apply((TA)oi[i]))
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
