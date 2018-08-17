using System.Linq;

using Microsoft.AspNetCore.Http;

namespace translate_spa.Utilities
{
    public class GetClientIp
    {
        public string Execute(HttpRequest request)
        {
            var headers = request.Headers;
            if (headers == null)
            {
                return request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            var keys = request.Headers.Keys.Where(x => "x-forwarded-for".Equals(x.ToLower()));
            if (keys.Any())
            {
                var key = keys.First();
                return headers[key];
            }

            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}