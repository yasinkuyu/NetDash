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

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}