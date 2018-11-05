using System.Threading;
using System.Threading.Tasks;

namespace translate_spa.Scheduler.Scheduling
{
	public interface IScheduledTask
	{
		string Schedule { get; }
		Task ExecuteAsync(CancellationToken cancellationToken);
	}
}