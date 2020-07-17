using System;
using System.Threading.Tasks;
using API.Invoice.Interfaces;
using API.Invoice.Providers;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Web.Http;
using System.Web.Mvc;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.Wcf;
using SimpleInjector.Integration.WebApi;

[assembly: OwinStartup(typeof(API.Invoice.App_Start.Startup))]

namespace API.Invoice.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = App_Start.DependencyInjectionConfig.CreateContainer();
            container.Verify(SimpleInjector.VerificationOption.VerifyAndDiagnose);
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            SimpleInjectorServiceHostFactory.SetContainer(container);

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            
            HttpConfiguration config = new HttpConfiguration();
           
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
           

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            app.UseWebApi(config);
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IInvoicesProvider, InvoicesProvider>();
            services.AddAutoMapper(typeof(Startup));            
            
            
            //services.AddControllers();
        }
    }
}
