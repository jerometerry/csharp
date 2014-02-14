namespace sodium
{
    using System;

    public class MapEventSink<TA, TB> : EventSink<TB>
    {
        private readonly Event<TA> _ev;
        private readonly ILambda1<TA, TB> _f;

        public MapEventSink(Event<TA> ev, ILambda1<TA, TB> f)
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
                for (int i = 0; i < oo.Length; i++)
                    oo[i] = _f.Apply((TA)oi[i]);
                return oo;
            }
            else
                return null;
        }
    }
}