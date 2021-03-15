using System.Collections.Generic;
using TemplateReport.DTO;

namespace TemplateReport.Services
{
    public interface IMemoryReportStorage
    {
        void Add(Report report);
        IEnumerable<Report> Get();
    }
}