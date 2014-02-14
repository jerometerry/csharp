namespace sodium
{
    public class DelayTransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly EventSink<TA> _o;

        public DelayTransactionHandler(EventSink<TA> o)
        {
            _o = o;
        }

        public void Run(Transaction trans, TA a)
        {
            trans.Post(new Runnable(() =>
            {
                var trans2 = new Transaction();
                try
                {
                    _o.Send(trans2, a);
                }
                finally
                {
                    trans2.Close();
                }
            }));
        }
    }
}
