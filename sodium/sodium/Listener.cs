namespace sodium
{
    public class Listener
    {
        public virtual void Unlisten() { }

        /**
         * Combine listeners into one where a single unlisten() invocation will unlisten
         * both the inputs.
         */
        public Listener Append(Listener two)
        {
            return new DualListener(this, two);
        }

        private class DualListener : Listener
        {
            private readonly Listener _listener1;
            private readonly Listener _listener2;

            public DualListener(Listener listener1, Listener listener2)
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
}