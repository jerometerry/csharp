using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace jterry.nunit.multithreaded
{
    /// <summary>
    /// http://www.peterprovost.org/blog/2004/11/03/Using-CrossThreadTestRunner/
    /// </summary>
    class CrossThreadTestRunner
    {
        private Exception lastException;
        private readonly Thread thread;
        private readonly ThreadStart start;

        public CrossThreadTestRunner(ThreadStart start)
        {
            this.start = start;
            this.thread = new Thread(Run);
            this.thread.SetApartmentState(ApartmentState.STA);
        }

        private void Run()
        {
            try
            {
                start.Invoke();
            }
            catch (Exception e)
            {
                lastException = e;
            }
        }

        public void Start()
        {
            lastException = null;
            thread.Start();
        }

        public void Join()
        {
            thread.Join();

            if (lastException != null)
            {
                ThrowExceptionPreservingStack(lastException);
            }
        }

        [ReflectionPermission(SecurityAction.Demand)]
        private static void ThrowExceptionPreservingStack(Exception exception)
        {
            var remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            if (remoteStackTraceString != null)
            {
                remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
            }
            throw exception;
        }
    }
}
