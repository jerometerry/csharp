namespace sodium
{
    using System;

    public class OnceEventSink<TA> : EventSink<TA>
    {
        private readonly Event<TA> _ev;
        private readonly IListener[] _la;

        public OnceEventSink(Event<TA> ev, IListener[] la)
        {
            _ev = ev;
            _la = la;
        }

        public override Object[] SampleNow()
        {
            var oi = _ev.SampleNow();
            var oo = oi;
            if (oo != null)
            {
                if (oo.Length > 1)
                    oo = new Object[] { oi[0] };
                if (_la[0] != null)
                {
                    _la[0].Unlisten();
                    _la[0] = null;
                }
            }
            return oo;
        }
    }
}
