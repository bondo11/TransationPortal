using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using NLog;

namespace translate_spa.Controllers.ActionFilters
{
    [AccessFilter]
    public class BaseController : Controller, IExceptionFilter
    {
        public readonly ILogger _log;

        public BaseController()
        {
            _log = new NLog.LogFactory().GetCurrentClassLogger();
        }

        public void OnException(ExceptionContext context)
        {
            _log.Error(context.Exception.Message);
            _log.Error(context.Exception.StackTrace);
            context.HttpContext.Response.Headers.Add("X-Error-Message", context.Exception.Message);
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}