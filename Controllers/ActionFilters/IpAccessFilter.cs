using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Serilog;
using translate_spa.Utilities;

namespace translate_spa.Controllers.ActionFilters
{
    public class IpAccessFilter : ActionFilterAttribute
    {
        public string _ipAddress { get; private set; }
        private static string[] _allowedAddresses = Get_allowedAddresses();
        private static string[] Get_allowedAddresses()
        {
            return Startup.Configuration.GetSection("Access:Allow").Get<string[]>();
        }

        public IpAccessFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _ipAddress = FormatIp(new GetClientIp().Execute(context.HttpContext.Request));
            Log.Debug($"{GetSenderName(context)}::{_ipAddress}");
            if (!IsIpAuthorized(_ipAddress))
            {
                Log.Error($"Accessing from unallowed network: '{_ipAddress}'");
                throw new UnauthorizedAccessException("Unauthorized. You are not allowed to access the resource you are trying to access.");
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Log.Warning("ClassFilter OnActionExecuted");
            base.OnActionExecuted(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            Log.Warning("ClassFilter OnResultExecuting");
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            Log.Warning("ClassFilter OnResultExecuted");
            base.OnResultExecuted(context);
        }

        string GetActionName(HttpContext context, string defaultName)
        {
            return context?.Request?.PathBase ?? defaultName;
        }

        string FormatIp(string ip)
        {
            if (ip == "::1")
            {
                return "127.0.0.1";
            }
            var addressArray = ip.Split(':').ToList();
            var ipSegments = addressArray;
            return string.Join(".", ipSegments);
        }

        private bool IsIpAuthorized(string ipAddress)
        {
            var ip = ipAddress.Split(':').FirstOrDefault();
            Log.Debug($"'{ip}' accessing");
            if (string.IsNullOrWhiteSpace(ip))
            {
                return true;
            }
            foreach (var address in _allowedAddresses)
            {
                if (address.Equals(ip))
                {
                    return true;
                }
            }
            Log.Debug($"'{ip}' is not a match with any of '{string.Join(", ", _allowedAddresses)}'");
            return false;
        }

        private string GetSenderName(FilterContext context)
        {
            var action = context.RouteData.Values["action"].ToString();
            var controller = context.RouteData.Values["controller"].ToString();
            return $"{controller}::{action}";
        }
    }
}