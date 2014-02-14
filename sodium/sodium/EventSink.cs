namespace sodium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSink<A> : Event<A>
    {
        public void Send(A a)
        {
            Transaction.Run(new EventSinkRunner<A>(this, a));
        }

        private class EventSinkRunner<A> : IHandler<Transaction>
        {
            private readonly A _a;
            private readonly EventSink<A> _sink;

            public EventSinkRunner(EventSink<A> sink, A a)
            {
                _sink = sink;
                _a = a;
            }

            public void Run(Transaction trans)
            {
                _sink.Send(trans, _a);
            }
        }

        public void Send(Transaction trans, A a)
        {
            if (!Firings.Any())
                trans.Last(new Runnable(() => Firings.Clear()));
            Firings.Add(a);

            var listeners = new List<ITransactionHandler<A>>(this.Listeners);

            foreach (var action in listeners)
            {
                try
                {
                    action.Run(trans, a);
                }
                catch (Exception t)
                {
                    Console.WriteLine("{0}", t);
                }
            }
        }
    }
}