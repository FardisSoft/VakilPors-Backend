using Microsoft.EntityFrameworkCore;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Statistics;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Core.Services;

public class StatisticsService : IStatisticsService
{
    private readonly IAppUnitOfWork appUnitOfWork;
    private readonly ILawyerServices _lawyerServices;
    private readonly IWalletServices _walletServices;

    public StatisticsService(IAppUnitOfWork appUnitOfWork, ILawyerServices lawyerServices,
        IWalletServices walletServices)
    {
        this.appUnitOfWork = appUnitOfWork;
        _lawyerServices = lawyerServices;
        _walletServices = walletServices;
    }

    public async Task AddVisit(string userGUID, string IPv4)
    {
        var visit = await appUnitOfWork.VisitorRepo.AsQueryableNoTracking()
            .FirstOrDefaultAsync(v => v.UserGUID == userGUID);
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
        var lawyersCounts = await appUnitOfWork.LawyerRepo.AsQueryableNoTracking().CountAsync();
        var usersCounts = await appUnitOfWork.UserRepo.AsQueryableNoTracking().CountAsync() - lawyersCounts;
        var result = new StatisticsDto()
        {
            DailyVisits = await GetVisits(DateTime.Now.AddDays(-1)),
            WeekVisits = await GetWeekVisits(),
            MonthlyVisits = await GetVisits(DateTime.Now.AddMonths(-1)),
            YearlyVisits = await GetVisits(DateTime.Now.AddYears(-1)),
            UsersCount = usersCounts,
            LawyersCount = lawyersCounts,
            CasesCount = await appUnitOfWork.DocumentRepo.AsQueryableNoTracking().CountAsync(),
            MessagesCount = await appUnitOfWork.ChatMessageRepo.AsQueryableNoTracking().CountAsync(),
            LawyerCityCount = await _lawyerServices.GetLawyerCityCounts(),
            LawyerTitleCount = await _lawyerServices.GetLawyerTitleCounts(),
            TransactionMonthlyCounts = await _walletServices.GetMonthlyTransactions()
        };
        return result;
    }

    private async Task<List<int>> GetWeekVisits()
    {
        var dailyVisits = new List<int>();
        for (int i = 0; i < 7; i++)
        {
            dailyVisits.Add(await GetVisits(DateTime.Now.AddDays(-i - 1), DateTime.Now.AddDays(-i)));
        }

        return dailyVisits;
    }

    public async Task<int> GetVisits(DateTime from)
    {
        return await appUnitOfWork.VisitorRepo.AsQueryableNoTracking().Where(v => v.VisitTime >= from).CountAsync();
    }

    public async Task<int> GetVisits(DateTime from, DateTime to)
    {
        return await appUnitOfWork.VisitorRepo.AsQueryableNoTracking()
            .Where(v => v.VisitTime >= from && v.VisitTime <= to).CountAsync();
    }
}