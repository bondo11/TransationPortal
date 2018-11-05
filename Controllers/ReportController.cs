using System.IO;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using NLog;

using translate_spa.Models.Security;

namespace translate_spa.Controllers
{
    public class ReportController : Controller
    {
        public readonly ILogger _log;
        ReportController()
        {
            _log = new NLog.LogFactory().GetCurrentClassLogger();
        }

        [HttpPost]
        public void Csp()
        {
            using(var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var cspReport = JsonConvert.DeserializeObject<CspReportRequest>(reader.ReadToEnd());
                _log.Error(cspReport.ToString());
            }
        }

        [HttpPost]
        public void hpkp()
        {
            using(var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                _log.Error(reader.ReadToEnd());
            }
        }
    }
}