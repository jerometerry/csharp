namespace sodium
{
    using System;

    class EventHelpers
    {
        public sealed class ListenerImplementation<TA> : Listener, IDisposable
        {
            /**
             * It's essential that we keep the listener alive while the caller holds
             * the Listener, so that the finalizer doesn't get triggered.
             */
            private readonly Event<TA> _evt;
            private readonly ITransactionHandler<TA> _action;
            private readonly Node _target;

            public ListenerImplementation(Event<TA> evt, ITransactionHandler<TA> action, Node target)
            {
                _evt = evt;
                _action = action;
                _target = target;
            }

            public override void Unlisten()
            {
                lock (Transaction.ListenersLock)
                {
                    _evt.Listeners.Remove(_action);
                    _evt.Node.unlinkTo(_target);
                }
            }

            public void Dispose()
            {
                Unlisten();
            }
        }

        public class TmpTransHandler1<TA> : ITransactionHandler<TA>
        {
            private readonly IHandler<TA> _action;

            public TmpTransHandler1(IHandler<TA> action)
            {
                _action = action;
            }

            public void Run(Transaction trans, TA a)
            {
                _action.Run(a);
            }
        }

        public class ListenerApplier<TA> : ILambda1<Transaction, Listener>
        {
            private readonly Event<TA> _listener;
            private readonly Node _target;
            private readonly ITransactionHandler<TA> _action;

            public ListenerApplier(Event<TA> listener, Node target, ITransactionHandler<TA> action)
            {
                _listener = listener;
                _target = target;
                _action = action;
            }

            public Listener Apply(Transaction trans)
            {
                return _listener.Listen(_target, trans, _action, false);
            }
        }

        public class TmpTransHandler7<TA, TB> : ITransactionHandler<TA>
        {
            private readonly EventSink<TB> _o;
            private readonly ILambda1<TA, TB> _f;

            public TmpTransHandler7(EventSink<TB> o, ILambda1<TA, TB> f)
            {
                _o = o;
                _f = f;
            }

            public void Run(Transaction trans, TA a)
            {
                _o.Send(trans, _f.Apply(a));
            }
        }

        public class TmpEventtSink7<TA, TB> : EventSink<TB>
        {
            private readonly Event<TA> _ev;
            private readonly ILambda1<TA, TB> _f;

            public TmpEventtSink7(Event<TA> ev, ILambda1<TA, TB> f)
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

        public class BehaviorBuilder<TA> : ILambda1<Transaction, Behavior<TA>>
        {
            private readonly Event<TA> _evt;
            private readonly TA _initValue;

            public BehaviorBuilder(Event<TA> evt, TA initValue)
            {
                _evt = evt;
                _initValue = initValue;
            }

            public Behavior<TA> Apply(Transaction trans)
            {
                return new Behavior<TA>(_evt.LastFiringOnly(trans), _initValue);
            }
        }

        public class SnapshotBehavior<TA, TB> : ILambda2<TA, TB, TB>
        {
            public TB Apply(TA a, TB b)
            {
                return b;
            }
        }

        public class TmpEventSink1<TA, TB, TC> : EventSink<TC>
        {
            private readonly Event<TA> _ev;
            private readonly ILambda2<TA, TB, TC> _f;
            private readonly Behavior<TB> _b;

            public TmpEventSink1(Event<TA> ev, ILambda2<TA, TB, TC> f, Behavior<TB> b)
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

        public class TmpTransHandler5<TA, TB, TC> : ITransactionHandler<TA>
        {
            private readonly EventSink<TC> _o;
            private readonly ILambda2<TA, TB, TC> _f;
            private readonly Behavior<TB> _b;

            public TmpTransHandler5(EventSink<TC> o, ILambda2<TA, TB, TC> f, Behavior<TB> b)
            {
                _o = o;
                _f = f;
                _b = b;
            }

            public void Run(Transaction trans, TA a)
            {
                _o.Send(trans, _f.Apply(a, _b.Sample()));
            }
        }

        public class TmpEventSink2<TA> : EventSink<TA>
        {
            private readonly Event<TA> _ea;
            private readonly Event<TA> _eb;

            public TmpEventSink2(Event<TA> ea, Event<TA> eb)
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

        public class TmpTransHandler2<TA> : ITransactionHandler<TA>
        {
            private readonly EventSink<TA> _o;

            public TmpTransHandler2(EventSink<TA> o)
            {
                _o = o;
            }

            public void Run(Transaction trans, TA a)
            {
                _o.Send(trans, a);
            }
        }

        public class TmpTransHandler3<TA> : ITransactionHandler<TA>
        {
            private readonly EventSink<TA> _o;

            public TmpTransHandler3(EventSink<TA> o)
            {
                _o = o;
            }

            public void Run(Transaction trans, TA a)
            {
                trans.Post(new Runnable(() =>
                {
                    var trans2 = new Transaction();
                    try
                    {
                        _o.Send(trans2, a);
                    }
                    finally
                    {
                        trans2.Close();
                    }
                }));
            }
        }

        public class Tmp2<TA> : ILambda1<Transaction, Event<TA>>
        {
            private readonly Event<TA> _evt;
            private readonly ILambda2<TA, TA, TA> _f;

            public Tmp2(Event<TA> evt, ILambda2<TA, TA, TA> f)
            {
                _evt = evt;
                _f = f;
            }

            public Event<TA> Apply(Transaction trans)
            {
                return _evt.Coalesce(trans, _f);
            }
        }

        public class TmpEventSink3<TA> : EventSink<TA>
        {
            private readonly Event<TA> _ev;
            private readonly ILambda2<TA, TA, TA> _f;

            public TmpEventSink3(Event<TA> ev, ILambda2<TA, TA, TA> f)
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

        public class Tmp4<TA> : ILambda2<TA, TA, TA>
        {
            public TA Apply(TA first, TA second)
            {
                return second;
            }
        }

        public class TmpEventSink5<TA> : EventSink<TA>
        {
            private readonly Event<TA> _ev;
            private readonly ILambda1<TA, Boolean> _f;

            public TmpEventSink5(Event<TA> ev, ILambda1<TA, Boolean> f)
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

        public class TmpTransHandler4<TA> : ITransactionHandler<TA>
        {
            private readonly ILambda1<TA, Boolean> _f;
            private readonly EventSink<TA> _o;

            public TmpTransHandler4(ILambda1<TA, Boolean> f, EventSink<TA> o)
            {
                _f = f;
                _o = o;
            }

            public void Run(Transaction trans, TA a)
            {
                if (_f.Apply(a)) _o.Send(trans, a);
            }
        }

        public class Tmp5<TA> : ILambda1<TA, Boolean>
        {
            public bool Apply(TA a)
            {
                return a != null;
            }
        }

        public class Tmp6<TA> : ILambda2<TA, Boolean, TA>
        {
            public TA Apply(TA a, bool pred)
            {
                return pred ? a : default(TA);
            }
        }

        public class Tmp7<TA, TB, TS> : ILambda1<Tuple2<TB, TS>, TB>
        {
            public TB Apply(Tuple2<TB, TS> bs)
            {
                return bs.X;
            }
        }

        public class Tmp8<TA, TB, TS> : ILambda1<Tuple2<TB, TS>, TS>
        {
            public TS Apply(Tuple2<TB, TS> bs)
            {
                return bs.Y;
            }
        }

        public class TmpEventSink4<TA> : EventSink<TA>
        {
            private readonly Event<TA> _ev;
            private readonly Listener[] _la;

            public TmpEventSink4(Event<TA> ev, Listener[] la)
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

        public class CoalesceHandler<TA> : ITransactionHandler<TA>
        {
            private readonly ILambda2<TA, TA, TA> _f;
            private readonly EventSink<TA> _o;
            public bool AccumValid = false;
            public TA Accum;

            public CoalesceHandler(ILambda2<TA, TA, TA> f, EventSink<TA> o)
            {
                _f = f;
                _o = o;
            }

            public void Run(Transaction trans1, TA a)
            {
                if (AccumValid)
                    Accum = _f.Apply(Accum, a);
                else
                {
                    var thiz = this;
                    trans1.Prioritized(_o.Node, new TransHandler<TA>(thiz, _o));
                    Accum = a;
                    AccumValid = true;
                }
            }
        }

        public class TransHandler<TA> : IHandler<Transaction>
        {
            private readonly CoalesceHandler<TA> _h;
            private readonly EventSink<TA> _o;

            public TransHandler(CoalesceHandler<TA> h, EventSink<TA> o)
            {
                _h = h;
                _o = o;
            }

            public void Run(Transaction trans)
            {
                _o.Send(trans, _h.Accum);
                _h.AccumValid = false;
                _h.Accum = default(TA);
            }
        }

        public class TmpTransHandler8<TA> : ITransactionHandler<TA>
        {
            private readonly EventSink<TA> _o;
            private readonly Listener[] _la;

            public TmpTransHandler8(EventSink<TA> o, Listener[] la)
            {
                _o = o;
                _la = la;
            }

            public void Run(Transaction trans, TA a)
            {
                _o.Send(trans, a);
                if (_la[0] != null)
                {
                    _la[0].Unlisten();
                    _la[0] = null;
                }
            }
        }
    }
}
