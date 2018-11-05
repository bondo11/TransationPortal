using System;

namespace translate_spa.Scheduler.Cron
{
	[Serializable]
	public enum CrontabFieldKind
	{
		Minute,
		Hour,
		Day,
		Month,
		DayOfWeek
	}
}