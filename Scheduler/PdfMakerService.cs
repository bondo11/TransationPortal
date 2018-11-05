/* using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;

using translate_spa.Models;
using translate_spa.MongoDB.DbBuilder;
using translate_spa.Repositories;
using translate_spa.Tasks;

namespace translate_spa.Scheduler
{
    public class PdfMakerService : HostedService
    {
        readonly INodeServices _nodeServices;
        public PdfMakerService([FromServices] INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var userRepository = new BaseRepository<User>(new BaseDbBuilder());

            var users = await userRepository.GetAll();

            foreach (var user in users)
            {
                _logger.LogDebug($"Generating PDF for user: {user.Name}");
                await new MakePdf(nodeServices: _nodeServices, user: user).Execute();
            }
        }
    }
} */