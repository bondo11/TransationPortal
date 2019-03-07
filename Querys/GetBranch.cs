using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Serilog;
using translate_spa.Models;
using translate_spa.Models.Interfaces;
using translate_spa.Repositories;

namespace translate_spa.Querys
{
    public class GetBranch
    {
        readonly HttpRequest _request;
        public bool HasBranch { get; private set; }
        public string Value { get; private set; }

        public GetBranch(HttpRequest request)
        {
            _request = request;

            this.HasBranch = _request.Headers.TryGetValue("X-esignatur-branch", out var branchValue);
            this.Value = this.HasBranch ? branchValue.ToString() : string.Empty;
            if (this.HasBranch)
            {
                Log.Debug($"Has branch of value: '{this.Value}'");
            }
        }

    }

}