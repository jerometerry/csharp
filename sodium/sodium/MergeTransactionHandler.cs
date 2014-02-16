
namespace sodium
{
    public class MergeTransactionHandler<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly EventSink<TEvent> _sink;

        public MergeTransactionHandler(EventSink<TEvent> sink)
        {
            _sink = sink;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            _sink.Send(transaction, evt);
        }
    }
}
