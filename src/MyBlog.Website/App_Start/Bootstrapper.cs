using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using TinyBlogNet;
using TinyBlogNet.IO;

namespace MyBlog.Website.App_Start
{
    public static class Bootstrapper
    {
        private static readonly ConcurrentDictionary<Type, object> _services;

        static Bootstrapper()
        {
            var dataFolder = ConfigurationManager.AppSettings["MyBlog:DataFolderRoot"];
            var postsPhysicalPath = HostingEnvironment.MapPath(Path.Combine(dataFolder, "Posts"));
            var textsPhysicalPath = HostingEnvironment.MapPath(dataFolder);

            _services = new ConcurrentDictionary<Type, object>();
            _services.TryAdd(typeof(PostRepository), new PostRepository(new FileSystem(postsPhysicalPath), new Cache()));
            _services.TryAdd(typeof(TextRepository), new TextRepository(new FileSystem(textsPhysicalPath), new Cache()));
        }

        public static void Boot()
        {
            ControllerBuilder.Current.SetControllerFactory(new CustomControllerFactory(_services));
        }
    }

    public class CustomControllerFactory : DefaultControllerFactory
    {
        private readonly ConcurrentDictionary<Type, object> _services;

        public CustomControllerFactory(ConcurrentDictionary<Type, object> services)
        {
            _services = services;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            var parameters = new List<object>();
            var constructor = controllerType.GetConstructors().FirstOrDefault(info => info.GetParameters().Length > 0);

            if (constructor != null)
            {
                foreach (var parameterInfo in constructor.GetParameters())
                {
                    object parameterInstance;

                    if (!_services.TryGetValue(parameterInfo.ParameterType, out parameterInstance))
                    {
                        throw new Exception(string.Format("Service of type '{0}' was not found", parameterInfo.ParameterType));
                    }

                    parameters.Add(parameterInstance);
                }
            }

            return (IController)Activator.CreateInstance(controllerType, parameters.ToArray());
        }
    }
}