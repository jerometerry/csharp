namespace sodium
{

    public class Listener {
        public Listener() {}

        public virtual void unlisten() {}

        ///
        /// Combine listeners into one where a single unlisten() invocation will unlisten
        /// both the inputs.
        ///
        public Listener append(Listener two) {
            Listener one = this;
            return new DualListener(one, two);
        }

        private class DualListener : Listener
        {
            private Listener one;
            private Listener two;

            public DualListener(Listener one, Listener two)
            {
                this.one = one;
                this.two = two;
            }

            public override  void unlisten()
            {
                this.one.unlisten();
                this.two.unlisten();
            }
        }
    }

}