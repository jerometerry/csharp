using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sodium
{
    public interface Runnable
    {
        void run();
    }

    public class RunnableImpl : Runnable
    {
        private Action action;
        public RunnableImpl(Action action)
        {
            this.action = action;
        }

        public void run()
        {
            action();
        }
    }
}
