namespace sodium
{
    using System;
    using System.Collections.Generic;

    class EventHelpers
    {
        public sealed class ListenerImplementation<A> : Listener
        {
            /**
             * It's essential that we keep the listener alive while the caller holds
             * the Listener, so that the finalizer doesn't get triggered.
             */
            private readonly Event<A> evt;
            private readonly ITransactionHandler<A> action;
            private readonly Node target;

            public ListenerImplementation(Event<A> evt, ITransactionHandler<A> action, Node target)
            {
                this.evt = evt;
                this.action = action;
                this.target = target;
            }

            public void unlisten()
            {
                lock (Transaction.listenersLock)
                {
                    evt.listeners.Remove(action);
                    evt.node.unlinkTo(target);
                }
            }

            protected void finalize()
            {
                unlisten();
            }
        }
        public class TmpTransHandler1<A> : ITransactionHandler<A>
        {
            private IHandler<A> action;

            public TmpTransHandler1(IHandler<A> action)
            {
                this.action = action;
            }

            public void run(Transaction trans, A a)
            {
                action.run(a);
            }
        }
        public class ListenerApplier<A> : ILambda1<Transaction, Listener>
        {
            private Event<A> listener;
            private Node target;
            private ITransactionHandler<A> action;

            public ListenerApplier(Event<A> listener, Node target, ITransactionHandler<A> action)
            {
                this.listener = listener;
                this.target = target;
                this.action = action;
            }

            public Listener apply(Transaction trans)
            {
                return listener.listen(target, trans, action, false);
            }
        }
        public class TmpTransHandler7<A, B> : ITransactionHandler<A>
        {
            private EventSink<B> o;
            private ILambda1<A, B> f;

            public TmpTransHandler7(EventSink<B> o, ILambda1<A, B> f)
            {
                this.o = o;
                this.f = f;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, f.apply(a));
            }
        }
        public class TmpEventtSink7<A, B> : EventSink<B>
        {
            private Event<A> ev;
            private ILambda1<A, B> f;

            public TmpEventtSink7(Event<A> ev, ILambda1<A, B> f)
            {
                this.ev = ev;
                this.f = f;
            }

            public override Object[] sampleNow()
            {
                Object[] oi = ev.sampleNow();
                if (oi != null)
                {
                    Object[] oo = new Object[oi.Length];
                    for (int i = 0; i < oo.Length; i++)
                        oo[i] = f.apply((A)oi[i]);
                    return oo;
                }
                else
                    return null;
            }
        }
        public class BehaviorBuilder<A> : ILambda1<Transaction, Behavior<A>>
        {
            private Event<A> evt;
            private A initValue;

            public BehaviorBuilder(Event<A> evt, A initValue)
            {
                this.evt = evt;
                this.initValue = initValue;
            }

            public Behavior<A> apply(Transaction trans)
            {
                return new Behavior<A>(evt.lastFiringOnly(trans), initValue);
            }
        }
        public class SnapshotBehavior<A, B> : ILambda2<A, B, B>
        {
            public B apply(A a, B b)
            {
                return b;
            }
        }
        public class TmpEventSink1<A, B, C> : EventSink<C>
        {
            private Event<A> ev;
            private ILambda2<A, B, C> f;
            private Behavior<B> b;

            public TmpEventSink1(Event<A> ev, ILambda2<A, B, C> f, Behavior<B> b)
            {
                this.ev = ev;
                this.f = f;
                this.b = b;
            }

            public override Object[] sampleNow()
            {
                Object[] oi = ev.sampleNow();
                if (oi != null)
                {
                    Object[] oo = new Object[oi.Length];
                    for (int i = 0; i < oo.Length; i++)
                        oo[i] = f.apply((A)oi[i], b.sample());
                    return oo;
                }
                else
                    return null;
            }
        }
        public class TmpTransHandler5<A, B, C> : ITransactionHandler<A>
        {
            private EventSink<C> o;
            private ILambda2<A, B, C> f;
            private Behavior<B> b;

            public TmpTransHandler5(EventSink<C> o, ILambda2<A, B, C> f, Behavior<B> b)
            {
                this.o = o;
                this.f = f;
                this.b = b;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, f.apply(a, b.sample()));
            }
        }
        public class TmpEventSink2<A> : EventSink<A>
        {
            private Event<A> ea;
            private Event<A> eb;

            public TmpEventSink2(Event<A> ea, Event<A> eb)
            {
                this.ea = ea;
                this.eb = eb;
            }

            public override Object[] sampleNow()
            {
                Object[] oa = ea.sampleNow();
                Object[] ob = eb.sampleNow();
                if (oa != null && ob != null)
                {
                    Object[] oo = new Object[oa.Length + ob.Length];
                    int j = 0;
                    for (int i = 0; i < oa.Length; i++) oo[j++] = oa[i];
                    for (int i = 0; i < ob.Length; i++) oo[j++] = ob[i];
                    return oo;
                }
                else
                    if (oa != null)
                        return oa;
                    else
                        return ob;
            }
        }
        public class TmpTransHandler2<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;

            public TmpTransHandler2(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
            }
        }
        public class TmpTransHandler3<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;

            public TmpTransHandler3(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                trans.post(new Runnable(() =>
                {
                    Transaction trans2 = new Transaction();
                    try
                    {
                        o.send(trans2, a);
                    }
                    finally
                    {
                        trans2.close();
                    }
                }));
            }
        }
        public class Tmp2<A> : ILambda1<Transaction, Event<A>>
        {
            private Event<A> evt;
            private ILambda2<A, A, A> f;

            public Tmp2(Event<A> evt, ILambda2<A, A, A> f)
            {
                this.evt = evt;
                this.f = f;
            }

            public Event<A> apply(Transaction trans)
            {
                return evt.coalesce(trans, f);
            }
        }
        public class TmpEventSink3<A> : EventSink<A>
        {
            private Event<A> ev;
            private ILambda2<A, A, A> f;

            public TmpEventSink3(Event<A> ev, ILambda2<A, A, A> f)
            {
                this.ev = ev;
                this.f = f;
            }

            public override Object[] sampleNow()
            {
                Object[] oi = ev.sampleNow();
                if (oi != null)
                {
                    A o = (A)oi[0];
                    for (int i = 1; i < oi.Length; i++)
                        o = f.apply(o, (A)oi[i]);
                    return new Object[] { o };
                }
                else
                    return null;
            }
        }
        public class Tmp4<A> : ILambda2<A, A, A>
        {
            public A apply(A first, A second)
            {
                return second;
            }
        }
        public class TmpEventSink5<A> : EventSink<A>
        {
            private Event<A> ev;
            private ILambda1<A, Boolean> f;

            public TmpEventSink5(Event<A> ev, ILambda1<A, Boolean> f)
            {
                this.ev = ev;
                this.f = f;
            }

            public override Object[] sampleNow()
            {
                Object[] oi = ev.sampleNow();
                if (oi != null)
                {
                    Object[] oo = new Object[oi.Length];
                    int j = 0;
                    for (int i = 0; i < oi.Length; i++)
                        if (f.apply((A)oi[i]))
                            oo[j++] = oi[i];
                    if (j == 0)
                        oo = null;
                    else
                        if (j < oo.Length)
                        {
                            Object[] oo2 = new Object[j];
                            for (int i = 0; i < j; i++)
                                oo2[i] = oo[i];
                            oo = oo2;
                        }
                    return oo;
                }
                else
                    return null;
            }
        }
        public class TmpTransHandler4<A> : ITransactionHandler<A>
        {
            private ILambda1<A, Boolean> f;
            private EventSink<A> o;

            public TmpTransHandler4(ILambda1<A, Boolean> f, EventSink<A> o)
            {
                this.f = f;
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                if (f.apply(a)) o.send(trans, a);
            }
        }
        public class Tmp5<A> : ILambda1<A, Boolean>
        {
            public bool apply(A a)
            {
                return a != null;
            }
        }
        public class Tmp6<A> : ILambda2<A, Boolean, A>
        {
            public A apply(A a, bool pred)
            {
                return pred ? a : default(A);
            }
        }
        public class Tmp7<A, B, S> : ILambda1<Tuple2<B, S>, B>
        {
            public B apply(Tuple2<B, S> bs)
            {
                return bs.a;
            }
        }
        public class Tmp8<A, B, S> : ILambda1<Tuple2<B, S>, S>
        {
            public S apply(Tuple2<B, S> bs)
            {
                return bs.b;
            }
        }
        public class TmpEventSink4<A> : EventSink<A>
        {
            private Event<A> ev;
            private Listener[] la;

            public TmpEventSink4(Event<A> ev, Listener[] la)
            {
                this.ev = ev;
                this.la = la;
            }

            public override Object[] sampleNow()
            {
                Object[] oi = ev.sampleNow();
                Object[] oo = oi;
                if (oo != null)
                {
                    if (oo.Length > 1)
                        oo = new Object[] { oi[0] };
                    if (la[0] != null)
                    {
                        la[0].unlisten();
                        la[0] = null;
                    }
                }
                return oo;
            }
        }
        public class CoalesceHandler<A> : ITransactionHandler<A>
        {
            private ILambda2<A, A, A> f;
            private EventSink<A> o;
            public bool accumValid = false;
            public A accum;

            public CoalesceHandler(ILambda2<A, A, A> f, EventSink<A> o)
            {
                this.f = f;
                this.o = o;
            }

            public void run(Transaction trans1, A a)
            {
                if (accumValid)
                    accum = f.apply(accum, a);
                else
                {
                    CoalesceHandler<A> thiz = this;
                    trans1.prioritized(o.node, new TransHandler<A>(thiz, o));
                    accum = a;
                    accumValid = true;
                }
            }


        }
        public class TransHandler<A> : IHandler<Transaction>
        {
            private CoalesceHandler<A> h;
            private EventSink<A> o;

            public TransHandler(CoalesceHandler<A> h, EventSink<A> o)
            {
                this.h = h;
                this.o = o;
            }

            public void run(Transaction trans)
            {
                o.send(trans, h.accum);
                h.accumValid = false;
                h.accum = default(A);
            }
        }
        public class TmpTransHandler8<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;
            private Listener[] la;

            public TmpTransHandler8(EventSink<A> o, Listener[] la)
            {
                this.o = o;
                this.la = la;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
                if (la[0] != null)
                {
                    la[0].unlisten();
                    la[0] = null;
                }
            }
        }
    }
}
