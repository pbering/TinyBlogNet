using System;
using System.Web;

namespace TinyBlogNet
{
    public static class HttpRequestExtensions
    {
        public static string GetServerUrl(this HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentException("Can not get server url without an request", "request");
            }

            var host = GetServerHost(request);
            var scheme = request.Url != null ? request.Url.Scheme : "http";
            var serverUrl = string.Format("{0}://{1}", scheme, host);

            return serverUrl;
        }

        public static string GetServerHost(this HttpRequestBase request)
        {
            if (request == null)
            {
                throw new ArgumentException("Can not get server url without an request", "request");
            }

            return request.Headers["HOST"];
        }
    }
}