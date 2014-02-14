namespace sodium
{
    using System;

    class BehaviorHelpers
    {
        public class TmpTransHandler1<TA> : IHandler<Transaction>
        {
            private readonly Behavior<TA> _b;
            private readonly Event<TA> _evt;

            public TmpTransHandler1(Behavior<TA> b, Event<TA> evt)
            {
                _b = b;
                _evt = evt;
            }

            public void Run(Transaction trans1)
            {
                _b.Cleanup = _evt.Listen(Node.NULL, trans1, new TransHandler2<TA>(_b), false);
            }
        }

        public sealed class TransHandler2<TA> : ITransactionHandler<TA>
        {
            private readonly Behavior<TA> _b;

            public TransHandler2(Behavior<TA> b)
            {
                this._b = b;
            }

            public void Run(Transaction trans, TA a)
            {
                if (_b.ValueUpdate == null)
                {
                    trans.Last(new Runnable(() =>
                    {
                        _b.Value = _b.ValueUpdate;
                        _b.ValueUpdate = default(TA);
                    }));
                    _b.ValueUpdate = a;
                }
            }
        }

        public class Tmp1<TA> : ILambda1<Transaction, Event<TA>>
        {
            private readonly Behavior<TA> _b;

            public Tmp1(Behavior<TA> b)
            {
                _b = b;
            }

            public Event<TA> Apply(Transaction trans)
            {
                return _b.GetValue(trans);
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

        public class TmpEventSink1<TA> : EventSink<TA>
        {
            private readonly Behavior<TA> _b;

            public TmpEventSink1(Behavior<TA> b)
            {
                _b = b;
            }

            public override Object[] SampleNow()
            {
                return new Object[] { _b.Sample() };
            }
        }

        public class Tmp2<TA, TB, TC> : ILambda1<TA, ILambda1<TB, TC>>
        {
            private readonly ILambda2<TA, TB, TC> _f;

            public Tmp2(ILambda2<TA, TB, TC> f)
            {
                _f = f;
            }

            public ILambda1<TB, TC> Apply(TA a)
            {
                return new Tmp3<TA, TB, TC>(_f, a);
            }
        }

        public class Tmp3<TA, TB, TC> : ILambda1<TB, TC>
        {
            private readonly ILambda2<TA, TB, TC> _f;
            private readonly TA _a;

            public Tmp3(ILambda2<TA, TB, TC> f, TA a)
            {
                _f = f;
                _a = a;
            }

            public TC Apply(TB b)
            {
                return _f.Apply(_a, b);
            }
        }

        public class Tmp4<TA, TB, TC, TD> : ILambda1<TA, ILambda1<TB, ILambda1<TC, TD>>>
        {
            private readonly ILambda3<TA, TB, TC, TD> _f;

            public Tmp4(ILambda3<TA, TB, TC, TD> f)
            {
                _f = f;
            }

            public ILambda1<TB, ILambda1<TC, TD>> Apply(TA a)
            {
                return new Tmp5<TA, TB, TC, TD>(a, _f);
            }
        }

        public class Tmp5<TA, TB, TC, TD> : ILambda1<TB, ILambda1<TC, TD>>
        {
            private readonly TA _a;
            private readonly ILambda3<TA, TB, TC, TD> _f;

            public Tmp5(TA a, ILambda3<TA, TB, TC, TD> f)
            {
                _a = a;
                _f = f;
            }

            public ILambda1<TC, TD> Apply(TB b)
            {
                return new Tmp6<TA, TB, TC, TD>(_a, b, _f);
            }
        }

        public class Tmp6<TA, TB, TC, TD> : ILambda1<TC, TD>
        {
            private readonly TA _a;
            private readonly TB _b;
            private readonly ILambda3<TA, TB, TC, TD> _f;

            public Tmp6(TA a, TB b, ILambda3<TA, TB, TC, TD> f)
            {
                _a = a;
                _b = b;
                _f = f;
            }

            public TD Apply(TC c)
            {
                return _f.Apply(_a, _b, c);
            }
        }

        public class Tmp9<TA, TB> : ITransactionHandler<ILambda1<TA, TB>>
        {
            private readonly IHandler<Transaction> _h;

            public Tmp9(IHandler<Transaction> h)
            {
                _h = h;
            }

            public void Run(Transaction trans, ILambda1<TA, TB> a)
            {
                _h.Run(trans);
            }
        }

        public class Tmp10<TA, TB> : ITransactionHandler<TA>
        {
            private readonly IHandler<Transaction> _h;

            public Tmp10(IHandler<Transaction> h)
            {
                _h = h;
            }

            public void Run(Transaction trans, TA a)
            {
                _h.Run(trans);
            }
        }

        public class Tmp7<TA, TB> : IHandler<Transaction>
        {
            public bool Fired = false;
            private readonly EventSink<TB> _o;
            private readonly Behavior<ILambda1<TA, TB>> _bf;
            private readonly Behavior<TA> _ba;

            public Tmp7(EventSink<TB> o, Behavior<ILambda1<TA, TB>> bf, Behavior<TA> ba)
            {
                _o = o;
                _bf = bf;
                _ba = ba;
            }

            public void Run(Transaction trans)
            {
                if (Fired)
                    return;

                Fired = true;
                trans.Prioritized(_o.Node, new Tmp8<TA, TB>(_o, _bf, _ba, this));
            }
        }

        public class Tmp8<TA, TB> : IHandler<Transaction>
        {
            private readonly EventSink<TB> _o;
            private readonly Behavior<ILambda1<TA, TB>> _bf;
            private readonly Behavior<TA> _ba;
            private readonly Tmp7<TA, TB> _tmp7;

            public Tmp8(EventSink<TB> o, Behavior<ILambda1<TA, TB>> bf, Behavior<TA> ba, Tmp7<TA, TB> tmp7)
            {
                _o = o;
                _bf = bf;
                _ba = ba;
                _tmp7 = tmp7;
            }

            public void Run(Transaction trans)
            {
                _o.Send(trans, _bf.NewValue().Apply(_ba.NewValue()));
                _tmp7.Fired = false;
            }
        }

        public class Tmp13<TA> : ILambda1<Transaction, Event<TA>>
        {
            private readonly Behavior<Event<TA>> _bea;

            public Tmp13(Behavior<Event<TA>> bea)
            {
                _bea = bea;
            }

            public Event<TA> Apply(Transaction trans)
            {
                return Behavior<TA>.SwitchE(trans, _bea);
            }
        }

        public class Tmp11<TA> : ITransactionHandler<Behavior<TA>>, IDisposable
        {
            private Listener _currentListener;
            private readonly EventSink<TA> _o;
            private bool _disposed;

            public Tmp11(EventSink<TA> o)
            {
                _o = o;
            }

            public void Run(Transaction trans, Behavior<TA> a)
            {
                // Note: If any switch takes place during a transaction, then the
                // value().listen will always cause a sample to be fetched from the
                // one we just switched to. The caller will be fetching our output
                // using value().listen, and value() throws away all firings except
                // for the last one. Therefore, anything from the old input behaviour
                // that might have happened during this transaction will be suppressed.
                if (_currentListener != null)
                    _currentListener.Unlisten();
                _currentListener = a.GetValue(trans).Listen(_o.Node, trans, new Tmp12<TA>(_o), false);
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
                        if (_currentListener != null)
                            _currentListener.Unlisten();
                    }

                    _disposed = true;
                }
            }
        }

        public class Tmp12<TA> : ITransactionHandler<TA>
        {
            private readonly EventSink<TA> _o;

            public Tmp12(EventSink<TA> o)
            {
                _o = o;
            }

            public void Run(Transaction trans, TA a)
            {
                _o.Send(trans, a);
            }
        }

        public class Tmp14<TA> : ITransactionHandler<TA>
        {
            private readonly EventSink<TA> _o;

            public Tmp14(EventSink<TA> o)
            {
                _o = o;
            }

            public void Run(Transaction trans, TA a)
            {
                _o.Send(trans, a);
            }
        }

        public class Tmp15<TA> : ITransactionHandler<Event<TA>>, IDisposable
        {
            private readonly EventSink<TA> _o;
            private Listener _currentListener;
            private readonly ITransactionHandler<TA> _h2;
            private bool _disposed;

            public Tmp15(EventSink<TA> o, Behavior<Event<TA>> bea, Transaction trans, ITransactionHandler<TA> h2)
            {
                _currentListener = bea.Sample().Listen(o.Node, trans, h2, false);
                _h2 = h2;
                _o = o;
            }

            public void Run(Transaction trans, Event<TA> a)
            {
                trans.Last(new Runnable(() =>
                {
                    if (_currentListener != null)
                        _currentListener.Unlisten();
                    _currentListener = a.Listen(_o.Node, trans, _h2, true);
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
                        if (_currentListener != null)
                            _currentListener.Unlisten();
                    }

                    _disposed = true;
                }
            }
        }

        public class Tmp16<TA> : ILambda2<TA, TA, TA>
        {
            public TA Apply(TA a, TA b)
            {
                return b;
            }
        }

        public class Tmp17<TA, TB, TS> : ILambda1<Tuple2<TB, TS>, TS>
        {
            public TS Apply(Tuple2<TB, TS> x)
            {
                return x.Y;
            }
        }

        public class Tmp18<TA, TB, TS> : ILambda1<Tuple2<TB, TS>, TB>
        {
            public TB Apply(Tuple2<TB, TS> x)
            {
                return x.X;
            }
        }
    }
}
