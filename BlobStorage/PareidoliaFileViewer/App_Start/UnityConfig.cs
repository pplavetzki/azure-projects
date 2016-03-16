using Microsoft.Practices.Unity;
using PareidoliaFileViewer.Services.Implementation;
using PareidoliaFileViewer.Services.Interfaces;
using StackExchange.Redis;
using System.Configuration;
using System.Web.Http;
using Unity.WebApi;

namespace PareidoliaFileViewer
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            string redisConnection = ConfigurationManager.ConnectionStrings["RedisConnection"].ConnectionString;

            ConnectionMultiplexer connectionMulp = ConnectionMultiplexer.Connect(redisConnection);
            container.RegisterInstance<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<ISASTokenProvider, SASTokenProvider>();
            container.RegisterType<IRedisProvider, RedisProvider>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}