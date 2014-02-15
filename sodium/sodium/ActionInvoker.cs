namespace sodium
{
    public class ActionInvoker<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly IHandler<TEvent> _action;

        public ActionInvoker(IHandler<TEvent> action)
        {
            _action = action;
        }

        public void Run(Transaction trans, TEvent a)
        {
            _action.Run(a);
        }
    }
}
