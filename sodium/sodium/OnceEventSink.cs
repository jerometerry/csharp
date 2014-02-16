namespace sodium
{
    using System;

    public class OnceEventSink<TEvent> : EventSink<TEvent>
    {
        private readonly Event<TEvent> _event;
        private readonly IListener[] _listeners;

        public OnceEventSink(Event<TEvent> ev, IListener[] listeners)
        {
            _event = ev;
            _listeners = listeners;
        }

        public override Object[] SampleNow()
        {
            var oi = _event.SampleNow();
            var oo = oi;
            if (oo != null)
            {
                if (oo.Length > 1)
                    oo = new Object[] { oi[0] };
                if (_listeners[0] != null)
                {
                    _listeners[0].Unlisten();
                    _listeners[0] = null;
                }
            }
            return oo;
        }
    }
}
