namespace sodium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventLoop<TEvent> : Event<TEvent>
    {
        private Event<TEvent> _eaOut;

        // TO DO: Copy & paste from EventSink. Can we improve this?
        public void Send(Transaction transaction, TEvent a)
        {
            if (!Firings.Any())
                transaction.Last(new Runnable(() => Firings.Clear())
                {
                });
            Firings.Add(a);

            var listeners = new List<ITransactionHandler<TEvent>>(this.Listeners);
            foreach (var action in listeners)
            {
                try
                {
                    action.Run(transaction, a);
                }
                catch (Exception t)
                {
                    Console.WriteLine("{0}", t);
                }
            }
        }

        public void Loop(Event<TEvent> eaOut)
        {
            if (_eaOut != null)
                throw new ApplicationException("EventLoop looped more than once");
            _eaOut = eaOut;
            var me = this;
            var action = new EventLoopTransactionHandler<TEvent>(me);
            AddCleanup(eaOut.Listen(Node, action));
        }
    }
}