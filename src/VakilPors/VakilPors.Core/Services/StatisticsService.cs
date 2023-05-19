using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Statistics;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IAppUnitOfWork appUnitOfWork;

    public StatisticsService(IAppUnitOfWork appUnitOfWork)
    {
        this.appUnitOfWork = appUnitOfWork;
    }
    public async Task AddVisit(string userGUID, string IPv4)
    {
        var visit = await appUnitOfWork.VisitorRepo.AsQueryableNoTracking().FirstOrDefaultAsync(v => v.UserGUID == userGUID);
        if (visit == null)
        {
            visit = new Visitor()
            {
                UserGUID = userGUID,
                IPv4 = IPv4
            };
        }
        await appUnitOfWork.VisitorRepo.AddAsync(visit);
        await appUnitOfWork.SaveChangesAsync();
    }

    public async Task<StatisticsDto> GetStatistics()
    {
        var lawyersCounts = await appUnitOfWork.UserRepo.AsQueryableNoTracking().CountAsync();
        var usersCounts = await appUnitOfWork.UserRepo.AsQueryableNoTracking().CountAsync() - lawyersCounts;
        var result = new StatisticsDto()
        {
            DailyVisits = await GetVisits(DateTime.Now.AddDays(-1)),
            MonthlyVisits = await GetVisits(DateTime.Now.AddMonths(-1)),
            YearlyVisits = await GetVisits(DateTime.Now.AddYears(-1)),
            UsersCount = usersCounts,
            LawyersCount = lawyersCounts,
            CasesCount = await appUnitOfWork.DocumentRepo.AsQueryableNoTracking().CountAsync(),
            MessagesCount = await appUnitOfWork.ChatMessageRepo.AsQueryableNoTracking().CountAsync(),
        };
        return result;
    }

    public async Task<int> GetVisits(DateTime from)
    {
        return await appUnitOfWork.VisitorRepo.AsQueryableNoTracking().Where(v => v.visitTime >= from).CountAsync();
    }
}
