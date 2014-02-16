namespace sodium
{
    public class EventLoopTransactionHandler<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly EventLoop<TEvent> _loop;

        public EventLoopTransactionHandler(EventLoop<TEvent> loop)
        {
            _loop = loop;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            _loop.Send(transaction, evt);
        }
    }
}