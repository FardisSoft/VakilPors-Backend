using VakilPors.Core.Domain.Dtos.Lawyer;

namespace VakilPors.Core.Domain.Dtos.Statistics;

public record StatisticsDto
{
    public int DailyVisits { get; set; }
    public IEnumerable<int> WeekVisits { get; set; }

    public int MonthlyVisits { get; set; }
    public int YearlyVisits { get; set; }
    public int UsersCount { get; set; }
    public int LawyersCount { get; set; }
    public int CasesCount { get; set; }
    public int MessagesCount { get; set; }
    public IEnumerable<LawyerCityCountDto> LawyerCityCount { get; set; }
    public IEnumerable<LawyerTitleCountDto> LawyerTitleCount { get; set; }
    public IEnumerable<int> TransactionMonthlyCounts { get; set; }
}
