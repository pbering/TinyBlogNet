using System;
using System.Web.Mvc;

namespace MvcBlog.Controllers
{
    public static class ControllerExtensions
    {
        public static string ServerUrl(this Controller controller)
        {
            if (controller.Request.Url != null)
            {
                return controller.Request.Url.GetLeftPart(UriPartial.Authority);
            }

            return string.Empty;
        }

        public static string ServerUrl(this Controller controller, string path)
        {
            return controller.ServerUrl() + path;
        }
    }
}