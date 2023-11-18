using System.Net;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Shared.Response;
using VakilPors.Web.Controllers;

namespace VakilPors.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class OcrController : MyControllerBase
{
    private readonly IOcrServices _ocrServices;

    public OcrController(IOcrServices ocrServices)
    {
        _ocrServices = ocrServices;
    }
    [HttpPost("perform-ocr")]
    public async Task<ActionResult<AppResponse>> PerformOcr(IFormFile imageFile)
    {
        try
        {
            var ocrResult = await _ocrServices.GetMelliCode(imageFile.OpenReadStream(), imageFile.FileName);
            return new AppResponse<string>(ocrResult, "ocr done!");
        }
        catch (Exception ex)
        {
            return new AppResponse(HttpStatusCode.BadRequest, ex.Message);
        }
    }
}