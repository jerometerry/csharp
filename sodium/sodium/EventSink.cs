namespace sodium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSink<TEvent> : Event<TEvent>
    {
        public void Send(TEvent evt)
        {
            var sink = this;
            var action = new Handler<Transaction>(t => sink.Send(t, evt));
            Transaction.Run(action);
        }

        public void Send(Transaction transaction, TEvent evt)
        {
            if (!Firings.Any())
                transaction.Last(new Runnable(() => Firings.Clear()));
            Firings.Add(evt);

            var listeners = new List<ITransactionHandler<TEvent>>(Actions);

            foreach (var action in listeners)
            {
                try
                {
                    action.Run(transaction, evt);
                }
                catch (Exception t)
                {
                    Console.WriteLine("{0}", t);
                }
            }
        }
    }
}