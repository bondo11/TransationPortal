using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
			if(Startup.Configuration.GetSection("NotificationSettings:DisaleNotificationService").Get<bool>())
			{
				return Task.Run(() => _log.Debug("Notification disabled"));
			}

            _log.Debug("########## Startng MissingTranslations Service ############");
            return Task.Run(() => new MissingTranslationsRunner().ExecuteAsync(cancellationToken), cancellationToken);
        }
    }
}