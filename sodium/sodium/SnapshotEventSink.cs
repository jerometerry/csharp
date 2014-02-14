namespace sodium
{
    using System;

    public class SnapshotEventSink<TEvent, TB, TC> : EventSink<TC>
    {
        private readonly Event<TEvent> _ev;
        private readonly ITwoParameterFunction<TEvent, TB, TC> _f;
        private readonly Behavior<TB> _b;

        public SnapshotEventSink(Event<TEvent> ev, ITwoParameterFunction<TEvent, TB, TC> f, Behavior<TB> b)
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
                    oo[i] = _f.Apply((TEvent)oi[i], _b.Sample());
                return oo;
            }
            else
                return null;
        }
    }
}
