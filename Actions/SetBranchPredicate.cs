using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog;
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

		public SetBranchPedicate(Expression<Func<Translation, bool>> predicate)
		{
			_predicate = predicate;
		}

		public Expression<Func<Translation, bool>> Execute(GetBranch branch)
		{
			var branchValue = branch.HasBranch ? branch.Value : string.Empty;
			Log.Debug($"Setting branch predicate to value: {branchValue}");

			_predicate = _predicate.And(x => x.Branch == branchValue);
			return _predicate;
		}
	}

}