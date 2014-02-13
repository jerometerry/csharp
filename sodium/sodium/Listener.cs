namespace sodium {

    public class Listener {
        public Listener() {}

        public virtual void unlisten() {}

        /**
         * Combine listeners into one where a single unlisten() invocation will unlisten
         * both the inputs.
         */
        public Listener append(Listener two) {
            return new DualListener(this, two);
        }

        private class DualListener : Listener
        {
            private Listener listener1;
            private Listener listener2;

            public DualListener(Listener listener1, Listener listener2)
            {
                this.listener1 = listener1;
                this.listener2 = listener2;
            }

            public override  void unlisten()
            {
                listener1.unlisten();
                listener2.unlisten();
            }
        }
    }

}