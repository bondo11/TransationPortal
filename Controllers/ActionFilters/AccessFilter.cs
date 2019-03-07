using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Serilog;
using translate_spa.Utilities;

namespace translate_spa.Controllers.ActionFilters
{
    public class AccessFilter : ActionFilterAttribute
    {
        public string _ipAddress { get; private set; }

        private string[] Get_allowedAddresses()
        {
            return Startup.Configuration.GetSection("Access:Allow").Get<string[]>();
        }

        public AccessFilter()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _ipAddress = new GetClientIp().Execute(context.HttpContext.Request);
            Log.Debug($"{GetSenderName(context)}::{FormatIp(_ipAddress)}");

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

        private string GetSenderName(FilterContext context)
        {
            var action = context.RouteData.Values["action"].ToString();
            var controller = context.RouteData.Values["controller"].ToString();
            return $"{controller}::{action}";
        }
    }
}