
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Shared.Response;

namespace VakilPors.Web.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ReportController : MyControllerBase
{
    private readonly IReportServices _reportservice;

    public ReportController(IReportServices reportservice)
    {
        this._reportservice = reportservice;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // _logger.LogInformation($"get all lawyers");
        var result = await _reportservice.GetAllLawyers();
        return Ok(new AppResponse<object>(result, "success"));
    }
}