using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
