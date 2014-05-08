// Copyright (c) 2014, Insya Interaktif.
// All rights reserved.

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Insya.NetDash
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            
            Settings.Set("VERSION", Functions.CurrentVersion);

            //if(!Settings.KeyExists("LANGUAGE"))
            //{
            //    Settings.Set("LANGUAGE", "en_EN");
            //}

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}