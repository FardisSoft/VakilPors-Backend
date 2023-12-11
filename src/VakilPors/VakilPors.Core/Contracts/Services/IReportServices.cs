using VakilPors.Core.Domain.Dtos.Report;
using VakilPors.Core.Domain.Entities;
using VakilPors.Shared.Services;
namespace VakilPors.Core.Contracts.Services
{
    public interface IReportServices:IScopedDependency
    {
        Task<List<ReportDto>> GetAllLawyers();


    }

}