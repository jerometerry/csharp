namespace sodium
{
    public class DelayTransactionHandler<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly EventSink<TEvent> _sink;

        public DelayTransactionHandler(EventSink<TEvent> sink)
        {
            _sink = sink;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            transaction.Post(new Runnable(() =>
            {
                var trans2 = new Transaction();
                try
                {
                    _sink.Send(trans2, evt);
                }
                finally
                {
                    trans2.Close();
                }
            }));
        }
    }
}
