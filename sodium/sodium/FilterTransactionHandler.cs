namespace sodium
{
    using System;

    public class FilterTransactionHandler<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly IFunction<TEvent, Boolean> _predicate;
        private readonly EventSink<TEvent> _sink;

        public FilterTransactionHandler(IFunction<TEvent, Boolean> predicate, EventSink<TEvent> sink)
        {
            _predicate = predicate;
            _sink = sink;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            if (_predicate.Apply(evt)) _sink.Send(transaction, evt);
        }
    }
}
