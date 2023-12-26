using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Report;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers;

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
        var result = await _reportservice.GetAllReport();
        return Ok(new AppResponse<object>(result, "success"));
    }
    [HttpPost]
    public async Task<IActionResult> PostReport([FromBody] PostReportDto postReportDto)
    {
        var result =await _reportservice.PostReport(postReportDto);
        return Ok(new AppResponse<object>(result, "success"));
    }
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
// public async Task<IActionResult> DeleteReport([FromBody] ReportDto reportDto)
    public async Task<IActionResult> DeleteReport(int report_id)
    {
        try
        {
            // Call the delete function in your service
            bool deleteResult = await _reportservice.DeleteReport(report_id);

            if (deleteResult)
            {
                return NoContent(); // 204 No Content
            }
            else
            {
                return NotFound(); // 404 Not Found
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it accordingly
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    // [HttpDelete("{id}")]
    // public async Task<ActionResult> DeleteReport(int id)
    // {
    //     int reportId;
    // try
    // {
    //     var idStr = await Request.Form.ToFormCollectionAsync();
    //     var idValue = idStr.GetValues("id");
    //     if (idValue.Count == 0)
    //     {
    //         return BadRequest("Missing 'id' parameter");
    //     }
    //     var idStr = idValue[0];
    //     reportId = int.Parse(idStr);
    // }
    // catch (Exception ex)
    // {
    //     return BadRequest("Invalid ID format");
    // }

    // }
    
    
}