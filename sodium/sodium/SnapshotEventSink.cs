namespace sodium
{
    using System;

    public class SnapshotEventSink<TA, TB, TC> : EventSink<TC>
    {
        private readonly Event<TA> _ev;
        private readonly ITwoParameterFunction<TA, TB, TC> _f;
        private readonly Behavior<TB> _b;

        public SnapshotEventSink(Event<TA> ev, ITwoParameterFunction<TA, TB, TC> f, Behavior<TB> b)
        {
            _ev = ev;
            _f = f;
            _b = b;
        }

        public override Object[] SampleNow()
        {
            var oi = _ev.SampleNow();
            if (oi != null)
            {
                var oo = new Object[oi.Length];
                for (int i = 0; i < oo.Length; i++)
                    oo[i] = _f.Apply((TA)oi[i], _b.Sample());
                return oo;
            }
            else
                return null;
        }
    }
}
