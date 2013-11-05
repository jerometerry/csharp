using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(SESG.Mobile.Service.Startup))]
namespace SESG.Mobile.Service
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HubConfiguration config = new HubConfiguration();
            config.EnableJSONP = true;
            app.MapSignalR(config);
        }
    }
}