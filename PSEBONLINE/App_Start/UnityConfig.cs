using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using PSEBONLINE.Models;
using PSEBONLINE.Repository;
using PSEBONLINE.AbstractLayer;

namespace PSEBONLINE
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers
            //container.RegisterType<ISchoolRepository,SchoolDB>();
            container.RegisterType<ISchoolRepository, SchoolDB>();
            container.RegisterType<ISelfDeclarationRepository, SelfDeclarationRepository>();
            container.RegisterType<IAgencyRepository, AgencyRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}