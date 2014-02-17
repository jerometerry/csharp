namespace sodium
{
    class DualListener : ListenerBase
    {
        private readonly IListener _listener1;
        private readonly IListener _listener2;

        public DualListener(IListener listener1, IListener listener2)
        {
            _listener1 = listener1;
            _listener2 = listener2;
        }

        public override void Unlisten()
        {
            _listener1.Unlisten();
            _listener2.Unlisten();
        }
    }
}
