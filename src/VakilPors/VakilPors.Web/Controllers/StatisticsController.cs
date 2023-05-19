using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Contracts.UnitOfWork;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Statistics;
using VakilPors.Core.Domain.Entities;

namespace VakilPors.Web.Controllers;

[Authorize(Roles = RoleNames.Admin)]
[ApiController]
[Route("[controller]/[action]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService statisticsService;
    private readonly ILogger<StatisticsController> logger;

    public StatisticsController(IStatisticsService statisticsService, ILogger<StatisticsController> logger)
    {
        this.statisticsService = statisticsService;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<string> AddVisit()
    {
        string ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(); //this is the ip of the client.
        string guid = Guid.NewGuid().ToString();
        await statisticsService.AddVisit(guid, ip);
        logger.LogInformation($"user with ip:{ip} and guid:{guid} visited website!");
        return guid;
    }
    [HttpGet]
    public async Task<StatisticsDto> GetStatistics()
    {
        var statistics = await statisticsService.GetStatistics(); //this is the statistics.
        return statistics;
    }
}
