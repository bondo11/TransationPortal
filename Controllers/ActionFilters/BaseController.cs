using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using translate_spa.Querys;
using translate_spa.Utilities;

namespace translate_spa.Controllers.ActionFilters
{
    [AccessFilter]
    public class BaseController : Controller, IExceptionFilter, IActionFilter
    {
        public string _ipAddress { get; private set; }
        public GetBranch _Branch { get; private set; }

        public BaseController()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _ipAddress = new GetClientIp().Execute(context.HttpContext.Request);
            _Branch = new GetBranch(Request);
            base.OnActionExecuting(context);
        }

        public void OnException(ExceptionContext context)
        {
            Log.Error(context.Exception.Message);
            Log.Error(context.Exception.StackTrace);
            context.HttpContext.Response.Headers.Add("X-Error-Message", context.Exception.Message);
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}