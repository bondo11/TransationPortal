using System.Threading;
using System.Threading.Tasks;

using NLog;

using translate_spa.Scheduler.Scheduling;

namespace translate_spa.Scheduler
{
    public class MissingTranslationsSchedule : IScheduledTask
    {
        public string Schedule => Startup.Configuration.GetSection("NotificationSettings")["CronSchedule"];
        // "30 9 * * 1";

        public ILogger _log = new LogFactory().CreateNullLogger();

        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _log.Debug("########## Startng MissingTranslations Service ############");
            return Task.Run(() => new MissingTranslationsRunner().ExecuteAsync(cancellationToken), cancellationToken);
        }
    }
}