using EzyFlightBot.App_Start;
using System.Web.Http;

namespace EzyFlightBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            SimpleInjectorWebApiInitializer.Initialize();
        }
    }
}