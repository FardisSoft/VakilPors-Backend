using VakilPors.Core.Domain.Dtos.Statistics;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IStatisticsService : IScopedDependency
{
    public Task AddVisit(string userGUID, string IPv4);
    public Task<int> GetVisits(DateTime from);
    public Task<StatisticsDto> GetStatistics();
}
