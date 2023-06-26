using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace PortableHttpServer
{
    internal static class UrlUtils
    {
        public static string? CreateUnescapedRouteUrl(this IUrlHelper self, string routeName, object values)
        {
            var url = self.RouteUrl(routeName, values);

            if (url == null)
                return null;

            var queryIndex = url.IndexOf('?');

            if (queryIndex == -1)
            {
                url = HttpUtility.UrlDecode(url);
            }
            else
            {
                url = HttpUtility.UrlDecode(url[..queryIndex]) +
                    url[(queryIndex + 1)..];
            }

            return url;
        }
    }
}
