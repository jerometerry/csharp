namespace sodium
{
    using System;

    public class FilterTransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly ILambda1<TA, Boolean> _f;
        private readonly EventSink<TA> _o;

        public FilterTransactionHandler(ILambda1<TA, Boolean> f, EventSink<TA> o)
        {
            _f = f;
            _o = o;
        }

        public void Run(Transaction trans, TA a)
        {
            if (_f.Apply(a)) _o.Send(trans, a);
        }
    }
}
