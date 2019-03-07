/* using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;

using translate_spa.Scheduler.Scheduling;

namespace translate_spa.Scheduler
{
    public class PdfScheduler : IScheduledTask
    {
        public string Schedule => "0 5 * * *";

        readonly INodeServices _nodeServices;
        public PdfScheduler([FromServices] INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => new PdfMakerTask(nodeServices: _nodeServices).Execute(), cancellationToken);
        }
    }
} */