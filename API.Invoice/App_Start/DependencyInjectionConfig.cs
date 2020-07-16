using API.Invoice.DB;
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

            //_container.Register<DbContext>(() => {
            //    return new ZubairEntities();
            //});
            //_container.Register<IDbContext>(() => app.GetRequiredRequestService<DbContext>(), Lifestyle.Scoped);
            //Repository.DependencyInjectionStrapper.AddRegistrationsToContainer(_container);

            Providers.DependencyInjectionStrapper.AddRegistrationsToContainer(_container);

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