using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TemplateReport.DTO;
using TemplateReport.Services;

namespace TemplateReport.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IMemoryReportStorage memoryReportStorage;

        public ReportController(IMemoryReportStorage memoryReportStorage)
        {
            this.memoryReportStorage = memoryReportStorage;
        }

        [HttpGet]
        public IEnumerable<Report> Get()
        {
            return memoryReportStorage.Get();
        }
    }
}
