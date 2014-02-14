namespace sodium
{
    public class SnapshotTransactionHandler<TA, TB, TC> : ITransactionHandler<TA>
    {
        private readonly EventSink<TC> _o;
        private readonly ITwoParameterFunction<TA, TB, TC> _f;
        private readonly Behavior<TB> _b;

        public SnapshotTransactionHandler(EventSink<TC> o, ITwoParameterFunction<TA, TB, TC> f, Behavior<TB> b)
        {
            _o = o;
            _f = f;
            _b = b;
        }

        public void Run(Transaction trans, TA a)
        {
            _o.Send(trans, _f.Apply(a, _b.Sample()));
        }
    }
}
