namespace sodium
{
    public class OnceTransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly EventSink<TA> _o;
        private readonly IListener[] _la;

        public OnceTransactionHandler(EventSink<TA> o, IListener[] la)
        {
            _o = o;
            _la = la;
        }

        public void Run(Transaction trans, TA a)
        {
            _o.Send(trans, a);
            if (_la[0] != null)
            {
                _la[0].Unlisten();
                _la[0] = null;
            }
        }
    }
}
