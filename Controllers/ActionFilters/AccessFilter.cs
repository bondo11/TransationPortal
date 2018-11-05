using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

using NLog;

using translate_spa.Utilities;

namespace translate_spa.Controllers.ActionFilters
{
    public class AccessFilter : ActionFilterAttribute
    {
        public readonly ILogger _log;
        public string _ipAddress { get; private set; }
        private static string[] _allowedAddresses = Get_allowedAddresses();
        private static string[] Get_allowedAddresses()
        {
            return Startup.Configuration.GetSection("Access:Allow").Get<string[]>();
        }

        public AccessFilter()
        {
            _log = new NLog.LogFactory().GetCurrentClassLogger();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _ipAddress = new GetClientIp().Execute(context.HttpContext.Request);
            _log.Debug($"{GetSenderName(context)}::{_ipAddress}");
            if (!IsIpAuthorized(_ipAddress))
            {
                _log.Error($"Accessing from unallowed network: '{_ipAddress}'");
                throw new UnauthorizedAccessException("Unauthorized. You are not allowed to access the resource you are trying to access.");
            }
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _log.Warn("ClassFilter OnActionExecuted");
            base.OnActionExecuted(context);
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            _log.Warn("ClassFilter OnResultExecuting");
            base.OnResultExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            _log.Warn("ClassFilter OnResultExecuted");
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
            _log.Debug($"'{ip}' accessing");
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
                _log.Debug($"'{ip}' is not a match with '{address}'");
            }
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