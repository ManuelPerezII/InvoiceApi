using API.Invoice.DB;
using API.Invoice.Profile;
using AutoMapper;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Lifestyles;
using System.Data.Entity;
using System.Web;
using System.Web.Http;

namespace API.Invoice.App_Start
{
    public static class DependencyInjectionConfig
    {
        private static Container _container;

        public static Container CreateContainer()
        {
            _container = new Container();
            _container.Options.AllowOverridingRegistrations = true;

            _container.Options.DefaultScopedLifestyle = Lifestyle.CreateHybrid(() => HttpContext.Current == null,
                new ThreadScopedLifestyle(), new WebRequestLifestyle());
            
            Providers.DependencyInjectionStrapper.AddRegistrationsToContainer(_container);
            //var config = AutomapperConfig.RegisterMappings();

            //_container.RegisterInstance<MapperConfiguration>(config);
            //_container.Register<IMapper>(() => config.CreateMapper(_container.GetInstance));

            // Web API controllers
            _container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            _container.Verify();

            return _container;
        }
        
        public static T Get<T>() where T : class
        {
            return _container.GetInstance<T>();
        }
    }
}