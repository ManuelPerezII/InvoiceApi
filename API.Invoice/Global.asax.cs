using SimpleInjector.Integration.Wcf;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace API.Invoice
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            
            var container = App_Start.DependencyInjectionConfig.CreateContainer();
            container.Verify(SimpleInjector.VerificationOption.VerifyAndDiagnose);
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            SimpleInjectorServiceHostFactory.SetContainer(container);

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);            
        }
    }
}
