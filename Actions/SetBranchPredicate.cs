using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using NLog;

using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.MongoDB;
using translate_spa.Querys;
using translate_spa.Repositories;

namespace translate_spa.Actions
{
	public class SetBranchPedicate
	{
		Expression<Func<Translation, bool>> _predicate;
		readonly ILogger _log;

		public SetBranchPedicate(Expression<Func<Translation, bool>> predicate, ILogger log)
		{
			_predicate = predicate;
			_log = log;
		}

		public Expression<Func<Translation, bool>> Execute(GetBranch branch)
		{
			var branchValue = branch.HasBranch ? branch.Value : string.Empty;
			_log.Debug($"Setting branch predicate to value: {branchValue}");

			_predicate = _predicate.And(x => x.Branch == branchValue);
			return _predicate;
		}
	}

}