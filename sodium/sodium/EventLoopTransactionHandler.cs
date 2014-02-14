namespace sodium
{
    public class EventLoopTransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly EventLoop<TA> _me;

        public EventLoopTransactionHandler(EventLoop<TA> me)
        {
            _me = me;
        }

        public void Run(Transaction trans, TA a)
        {
            _me.Send(trans, a);
        }
    }
}