namespace sodium
{
    public abstract class ListenerBase : IListener
    {
        public abstract void Unlisten();

        /**
         * Combine listeners into one where a single unlisten() invocation will unlisten
         * both the inputs.
         */
        public IListener Append(IListener two)
        {
            return new DualListener(this, two);
        }
    }
}