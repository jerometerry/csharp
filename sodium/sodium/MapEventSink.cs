using System;

namespace sodium
{
    public class MapEventSink<A,B> : EventSink<B>
    {
        private Event<A> _ev;
        private Lambda1<A, B> _f;

        public MapEventSink(Event<A> ev, Lambda1<A,B> f)
        {
            _ev = ev;
            _f = f;
        }

        protected internal override Object[] sampleNow()
        {
            Object[] oi = _ev.sampleNow();
            if (oi != null)
            {
                Object[] oo = new Object[oi.Length];
                for (int i = 0; i < oo.Length; i++)
                    oo[i] = _f.apply((A)oi[i]);
                return oo;
            }
            else
                return null;
        }
    }
}
