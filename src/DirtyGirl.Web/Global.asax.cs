using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Data.RepositoryGroups;
using DirtyGirl.Services;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Helpers;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace DirtyGirl.Web
{
    internal class MembershipModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepositoryGroup>().To<RepositoryGroup>();                
        }        
    }

    internal class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IEventService>().To<EventService>();
            Bind<IUserService>().To<UserService>();
            Bind<IRegistrationService>().To<RegistrationService>();
            Bind<ITeamService>().To<TeamService>();
            Bind<ITransactionService>().To<TransactionService>();
            Bind<ICartService>().To<CartService>();
            Bind<IDiscountService>().To<DiscountService>();
            Bind<IEmailService>().To<EmailService>();
            Bind<IEventLeadService>().To<EventLeadService>();
            Bind<IReportingService>().To<ReportingService>();
        }
    }

    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            Kernel.Inject(Membership.Provider);
            Kernel.Inject(Roles.Provider);

        }       

        protected override IKernel CreateKernel()
        {
            var modules = new NinjectModule[] { new MembershipModule(), new ServiceModule() };
            var kernel = new StandardKernel(modules);            
            kernel.Load(Assembly.GetExecutingAssembly());

            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel); 

            return kernel;
        }
    }
}