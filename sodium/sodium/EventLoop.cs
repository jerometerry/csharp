namespace sodium
{
    using System;

    public class EventLoop<TEvent> : Event<TEvent>
    {
        private Event<TEvent> _event;

        public void Loop(Event<TEvent> evt)
        {
            if (_event != null)
            { 
                throw new ApplicationException("EventLoop looped more than once");
            }
            _event = evt;
            var loop = this;
            var action = new TransactionHandler<TEvent>(loop.Send);
            RegisterListener(evt.Listen(Node, action));
        }
    }
}