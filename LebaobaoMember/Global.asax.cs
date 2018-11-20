using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LebaobaoMember
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs eventArgs)
        {
            //if (Request.Url.ToString().StartsWith("http://") && !Request.Url.ToString().Contains("localhost") && !Request.Url.ToString().Contains("mgr"))
            //{
            //    Response.Redirect(Request.Url.ToString().Replace("http://", "https://"));
            //}

        }
    }
}
