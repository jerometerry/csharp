
namespace sodium
{
    using System;

    public class CoalesceEventSink<TA> : EventSink<TA>
    {
        private readonly Event<TA> _ev;
        private readonly ILambda2<TA, TA, TA> _f;

        public CoalesceEventSink(Event<TA> ev, ILambda2<TA, TA, TA> f)
        {
            _ev = ev;
            _f = f;
        }

        public override Object[] SampleNow()
        {
            var oi = _ev.SampleNow();
            if (oi != null)
            {
                var o = (TA)oi[0];
                for (var i = 1; i < oi.Length; i++)
                    o = _f.Apply(o, (TA)oi[i]);
                return new Object[] { o };
            }
            else
                return null;
        }
    }
}
