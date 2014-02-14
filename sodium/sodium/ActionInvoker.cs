namespace sodium
{
    public class ActionInvoker<TA> : ITransactionHandler<TA>
    {
        private readonly IHandler<TA> _action;

        public ActionInvoker(IHandler<TA> action)
        {
            _action = action;
        }

        public void Run(Transaction trans, TA a)
        {
            _action.Run(a);
        }
    }
}
