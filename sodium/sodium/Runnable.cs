namespace sodium
{
    using System;

    public class Runnable : IRunnable
    {
        private readonly Action action;

        public Runnable(Action action)
        {
            this.action = action;
        }

        public virtual void run()
        {
            action();
        }
    }
}
