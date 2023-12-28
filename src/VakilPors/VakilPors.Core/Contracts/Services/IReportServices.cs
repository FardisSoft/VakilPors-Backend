using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Domain.Dtos.Report;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;
namespace VakilPors.Core.Contracts.Services
{
    public interface IReportServices:IScopedDependency
    {
        Task<List<ReportDto>> GetAllReport();
        Task<bool> PostReport(PostReportDto reportDto);
        Task<bool> DeleteReport(int reportId);
        Task<Report> UpdateReportStatusAsync(int id, Status status);



    }

}