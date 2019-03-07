using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using translate_spa.Models.Security;

namespace translate_spa.Controllers
{
    public class ReportController : Controller
    {
        ReportController()
        {
        }

        [HttpPost]
        public void Csp()
        {
            using(var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var cspReport = JsonConvert.DeserializeObject<CspReportRequest>(reader.ReadToEnd());
                Log.Error(cspReport.ToString());
            }
        }

        [HttpPost]
        public void hpkp()
        {
            using(var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                Log.Error(reader.ReadToEnd());
            }
        }
    }
}