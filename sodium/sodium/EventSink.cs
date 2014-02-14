namespace sodium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSink<TA> : Event<TA>
    {
        public void Send(TA a)
        {
            Transaction.Run(new EventSinkRunner<TA>(this, a));
        }

        public void Send(Transaction trans, TA a)
        {
            if (!Firings.Any())
                trans.Last(new Runnable(() => Firings.Clear()));
            Firings.Add(a);

            var listeners = new List<ITransactionHandler<TA>>(this.Listeners);

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