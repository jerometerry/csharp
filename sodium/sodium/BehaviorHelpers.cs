namespace sodium
{
    using System;

    class BehaviorHelpers
    {
        public class TmpTransHandler1<A> : IHandler<Transaction>
        {
            private Behavior<A> b;
            private Event<A> evt;

            public TmpTransHandler1(Behavior<A> b, Event<A> evt)
            {
                this.b = b;
                this.evt = evt;
            }

            public void run(Transaction trans1)
            {
                b.cleanup = evt.listen(Node.NULL, trans1, new TransHandler2<A>(this.b), false);
            }
        }
        public sealed class TransHandler2<A> : ITransactionHandler<A>
        {
            private Behavior<A> b;

            public TransHandler2(Behavior<A> b)
            {
                this.b = b;
            }

            public void run(Transaction trans, A a)
            {
                if (b.valueUpdate == null)
                {
                    trans.last(new Runnable(() =>
                    {
                        b._value = b.valueUpdate;
                        b.valueUpdate = default(A);
                    }));
                    b.valueUpdate = a;
                }
            }
        }

        public class Tmp1<A> : ILambda1<Transaction, Event<A>>
        {
            private Behavior<A> b;

            public Tmp1(Behavior<A> b)
            {
                this.b = b;
            }

            public Event<A> apply(Transaction trans)
            {
                return b.value(trans);
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

        public class TmpEventSink1<A> : EventSink<A>
        {
            private Behavior<A> b;

            public TmpEventSink1(Behavior<A> b)
            {
                this.b = b;
            }

            public override Object[] sampleNow()
            {
                return new Object[] { this.b.sample() };
            }
        }

        public class Tmp2<A, B, C> : ILambda1<A, ILambda1<B, C>>
        {
            private ILambda2<A, B, C> f;

            public Tmp2(ILambda2<A, B, C> f)
            {
                this.f = f;
            }

            public ILambda1<B, C> apply(A a)
            {
                return new Tmp3<A, B, C>(f, a);
            }
        }

        public class Tmp3<A, B, C> : ILambda1<B, C>
        {
            private ILambda2<A, B, C> f;
            private A a;

            public Tmp3(ILambda2<A, B, C> f, A a)
            {
                this.f = f;
                this.a = a;
            }

            public C apply(B b)
            {
                return f.apply(a, b);
            }
        }

        public class Tmp4<A, B, C, D> : ILambda1<A, ILambda1<B, ILambda1<C, D>>>
        {
            private ILambda3<A, B, C, D> f;

            public Tmp4(ILambda3<A, B, C, D> f)
            {
                this.f = f;
            }

            public ILambda1<B, ILambda1<C, D>> apply(A a)
            {
                return new Tmp5<A, B, C, D>(a, f);
            }
        }

        public class Tmp5<A, B, C, D> : ILambda1<B, ILambda1<C, D>>
        {
            private A a;
            private ILambda3<A, B, C, D> f;

            public Tmp5(A a, ILambda3<A, B, C, D> f)
            {
                this.a = a;
                this.f = f;
            }

            public ILambda1<C, D> apply(B b)
            {
                return new Tmp6<A, B, C, D>(a, b, f);
            }
        }

        public class Tmp6<A, B, C, D> : ILambda1<C, D>
        {
            private A a;
            private B b;
            private ILambda3<A, B, C, D> f;

            public Tmp6(A a, B b, ILambda3<A, B, C, D> f)
            {
                this.a = a;
                this.b = b;
                this.f = f;
            }

            public D apply(C c)
            {
                return f.apply(a, b, c);
            }
        }

        public class Tmp9<A, B> : ITransactionHandler<ILambda1<A, B>>
        {
            private IHandler<Transaction> h;

            public Tmp9(IHandler<Transaction> h)
            {
                this.h = h;
            }

            public void run(Transaction trans, ILambda1<A, B> a)
            {
                h.run(trans);
            }
        }

        public class Tmp10<A, B> : ITransactionHandler<A>
        {
            private IHandler<Transaction> h;

            public Tmp10(IHandler<Transaction> h)
            {
                this.h = h;
            }

            public void run(Transaction trans, A a)
            {
                h.run(trans);
            }
        }

        public class Tmp7<A, B> : IHandler<Transaction>
        {
            public bool fired = false;
            private EventSink<B> o;
            private Behavior<ILambda1<A, B>> bf;
            private Behavior<A> ba;

            public Tmp7(EventSink<B> o, Behavior<ILambda1<A, B>> bf, Behavior<A> ba)
            {
                this.o = o;
                this.bf = bf;
                this.ba = ba;
            }

            public void run(Transaction trans)
            {
                if (fired)
                    return;

                fired = true;
                trans.prioritized(o.node, new Tmp8<A, B>(o, bf, ba, this));
            }
        }

        public class Tmp8<A, B> : IHandler<Transaction>
        {
            private EventSink<B> o;
            private Behavior<ILambda1<A, B>> bf;
            private Behavior<A> ba;
            private Tmp7<A, B> tmp7;

            public Tmp8(EventSink<B> o, Behavior<ILambda1<A, B>> bf, Behavior<A> ba, Tmp7<A, B> tmp7)
            {
                this.o = o;
                this.bf = bf;
                this.ba = ba;
                this.tmp7 = tmp7;
            }

            public void run(Transaction trans)
            {
                o.send(trans, bf.newValue().apply(ba.newValue()));
                tmp7.fired = false;
            }
        }

        public class Tmp13<A> : ILambda1<Transaction, Event<A>>
        {
            private Behavior<Event<A>> bea;

            public Tmp13(Behavior<Event<A>> bea)
            {
                this.bea = bea;
            }

            public Event<A> apply(Transaction trans)
            {
                return Behavior<A>.switchE(trans, bea);
            }
        }

        public class Tmp11<A> : ITransactionHandler<Behavior<A>>, IDisposable
        {
            private Listener currentListener;
            private EventSink<A> o;
            private bool _disposed;

            public Tmp11(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, Behavior<A> a)
            {
                // Note: If any switch takes place during a transaction, then the
                // value().listen will always cause a sample to be fetched from the
                // one we just switched to. The caller will be fetching our output
                // using value().listen, and value() throws away all firings except
                // for the last one. Therefore, anything from the old input behaviour
                // that might have happened during this transaction will be suppressed.
                if (currentListener != null)
                    currentListener.unlisten();
                currentListener = a.value(trans).listen(o.node, trans, new Tmp12<A>(o), false);
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disponsing)
            {
                if (!_disposed)
                {
                    if (disponsing)
                    {
                        if (currentListener != null)
                            currentListener.unlisten();
                    }

                    _disposed = true;
                }
            }
        }

        public class Tmp12<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;

            public Tmp12(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
            }
        }

        public class Tmp14<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;

            public Tmp14(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
            }
        }

        public class Tmp15<A> : ITransactionHandler<Event<A>>, IDisposable
        {
            private EventSink<A> o;
            private Listener currentListener;
            private ITransactionHandler<A> h2;
            private bool _disposed;

            public Tmp15(EventSink<A> o, Behavior<Event<A>> bea, Transaction trans, ITransactionHandler<A> h2)
            {
                this.currentListener = bea.sample().listen(o.node, trans, h2, false);
                this.h2 = h2;
            }

            public void run(Transaction trans, Event<A> a)
            {
                trans.last(new Runnable(() =>
                {
                    if (currentListener != null)
                        currentListener.unlisten();
                    currentListener = a.listen(o.node, trans, h2, true);
                }));
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        if (currentListener != null)
                            currentListener.unlisten();
                    }

                    _disposed = true;
                }
            }
        }

        public class Tmp16<A> : ILambda2<A, A, A>
        {
            public A apply(A a, A b)
            {
                return b;
            }
        }

        public class Tmp17<A, B, S> : ILambda1<Tuple2<B, S>, S>
        {
            public S apply(Tuple2<B, S> x)
            {
                return x.b;
            }
        }

        public class Tmp18<A, B, S> : ILambda1<Tuple2<B, S>, B>
        {
            public B apply(Tuple2<B, S> x)
            {
                return x.a;
            }
        }
    }
}
