using Microsoft.Practices.Unity;
using PareidoliaFileViewer.Services.Implementation;
using PareidoliaFileViewer.Services.Interfaces;
using System.Web.Http;
using Unity.WebApi;

namespace PareidoliaFileViewer
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ISASTokenProvider, SASTokenProvider>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}