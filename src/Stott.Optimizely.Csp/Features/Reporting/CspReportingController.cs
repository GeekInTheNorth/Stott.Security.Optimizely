using System;

using Microsoft.AspNetCore.Mvc;

namespace Stott.Optimizely.Csp.Features.Reporting
{
    public class CspReportingController : Controller
    {
        [HttpPost]
        [Route("[controller]/[action]")]
        public IActionResult Report([FromBody]ReportModel cspReport)
        {
            try
            {
                return Ok();
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}
