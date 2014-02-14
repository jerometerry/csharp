namespace sodium
{
    public class CoalesceTransactionHandler<TA> : IHandler<Transaction>
    {
        private readonly CoalesceHandler<TA> _h;
        private readonly EventSink<TA> _o;

        public CoalesceTransactionHandler(CoalesceHandler<TA> h, EventSink<TA> o)
        {
            _h = h;
            _o = o;
        }

        public void Run(Transaction trans)
        {
            _o.Send(trans, _h.Accum);
            _h.AccumValid = false;
            _h.Accum = default(TA);
        }
    }
}
