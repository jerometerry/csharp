namespace sodium
{
    public class MapTransactionHandler<TA, TB> : ITransactionHandler<TA>
    {
        private readonly EventSink<TB> _o;
        private readonly ILambda1<TA, TB> _f;

        public MapTransactionHandler(EventSink<TB> o, ILambda1<TA, TB> f)
        {
            _o = o;
            _f = f;
        }

        public void Run(Transaction trans, TA a)
        {
            _o.Send(trans, _f.Apply(a));
        }
    }
}