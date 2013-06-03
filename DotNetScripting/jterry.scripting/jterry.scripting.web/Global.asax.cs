using System;
using jterry.scripting.api;
using jterry.scripting.host;

namespace jterry.scripting.web
{
    public class Global : System.Web.HttpApplication
    {
        ScriptHost _scriptHost;

        public ScriptHost ScriptHost
        {
            get 
            {
                if (_scriptHost == null)
                {
                    _scriptHost = new ScriptHost();
                    IFactory factory = new Factory();
                    _scriptHost.RegisterVariable("factory", factory);
                }
                return _scriptHost; 
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
        }
    }
}
