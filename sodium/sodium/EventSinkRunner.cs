namespace sodium
{
    public class EventSinkRunner<TA> : IHandler<Transaction>
    {
        private readonly TA _a;
        private readonly EventSink<TA> _sink;

        public EventSinkRunner(EventSink<TA> sink, TA a)
        {
            _sink = sink;
            _a = a;
        }

        public void Run(Transaction trans)
        {
            _sink.Send(trans, _a);
        }
    }
}