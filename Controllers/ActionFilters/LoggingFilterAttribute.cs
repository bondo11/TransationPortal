using Microsoft.AspNetCore.Mvc.Filters;

using NLog;

namespace translate_spa.Controllers.ActionFilters
{
    public class LoggingFilterAttribute : ActionFilterAttribute
    {
        private readonly ILogger _log;

        public LoggingFilterAttribute(LogFactory loggerFactory)
        {
            _log = loggerFactory.GetCurrentClassLogger();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _log.Warn("ClassFilter OnActionExecuting");
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
    }
}