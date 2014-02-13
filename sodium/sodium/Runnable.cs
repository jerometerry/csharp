namespace sodium
{
    using System;

    public class Runnable : IRunnable
    {
        protected readonly Action action;

        public Runnable()
        {
            
        }

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
