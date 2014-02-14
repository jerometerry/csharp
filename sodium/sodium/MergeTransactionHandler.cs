
namespace sodium
{
    public class MergeTransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly EventSink<TA> _o;

        public MergeTransactionHandler(EventSink<TA> o)
        {
            _o = o;
        }

        public void Run(Transaction trans, TA a)
        {
            _o.Send(trans, a);
        }
    }
}
