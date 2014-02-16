namespace sodium
{
    public class EventLoopSender<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly EventLoop<TEvent> _loop;

        public EventLoopSender(EventLoop<TEvent> loop)
        {
            _loop = loop;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            _loop.Send(transaction, evt);
        }
    }
}