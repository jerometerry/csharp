namespace sodium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventLoop<TA> : Event<TA>
    {
        private Event<TA> _eaOut;

        // TO DO: Copy & paste from EventSink. Can we improve this?
        public void Send(Transaction trans, TA a)
        {
            if (!Firings.Any())
                trans.Last(new Runnable(() => Firings.Clear())
                {
                });
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

        public void Loop(Event<TA> eaOut)
        {
            if (_eaOut != null)
                throw new ApplicationException("EventLoop looped more than once");
            _eaOut = eaOut;
            var me = this;
            var action = new EventLoopTransactionHandler<TA>(me);
            AddCleanup(eaOut.Listen(Node, action));
        }
    }
}