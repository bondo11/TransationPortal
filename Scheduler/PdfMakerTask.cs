/* using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.NodeServices;

using NLog;

using translate_spa.Models;
using translate_spa.MongoDB.DbBuilder;
using translate_spa.Repositories;
using translate_spa.Tasks;

namespace translate_spa.Scheduler
{
    public class PdfMakerTask
    {
        readonly IEnumerable<Translation> _translations;
        readonly ILogger _logger;
        readonly INodeServices _nodeServices;
        public PdfMakerTask(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
            var mongoRepository = new MongoRepository<Translation>(new BaseDbBuilder());
            _translations = mongoRepository.All();
            _logger = new NLog.LogFactory().CreateNullLogger();
        }

        public async Task Execute()
        {
            foreach (var item in _translations)
            {
                _logger.LogDebug($"Generating PDF for user: {item.Key}");
                await new MakePdf(nodeServices: _nodeServices, user: user).Execute();
            }
        }
    }
} */