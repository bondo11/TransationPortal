using System.Threading.Tasks;
using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.Repositories;
using translate_spa.Utilities;

namespace translate_spa.Actions
{
	public class PrepareForInsert
	{
		readonly Translation _translation;

		public PrepareForInsert(Translation translation)
		{
			_translation = translation;
		}
		
		public void Execute()
		{
			if (string.IsNullOrWhiteSpace(_translation.Id))
			{
				_translation.SetNewId();
			}
			if (_translation.Environment == null || _translation.Environment == 0)
			{
				new SetEnvironmentFromKey(_translation).Execute();
			}
		}
	}
}