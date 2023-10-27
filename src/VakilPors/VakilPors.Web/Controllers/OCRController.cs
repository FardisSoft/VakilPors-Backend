using System.Net;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Services;
using VakilPors.Shared.Response;
using VakilPors.Web.Controllers;

namespace VakilPors.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class OCRController:ControllerBase
{
    private readonly IOcrService _ocrApiClient;

    public OCRController()
    {
        _ocrApiClient = new OcrIron();
    }

    [HttpPost("perform-ocr")]
    public async Task<ActionResult<AppResponse>> PerformOcr(IFormFile imageFile)
    {
        try
        {
            var ocrResult = await _ocrApiClient.PerformOcrAsync(imageFile.OpenReadStream(),imageFile.FileName);
            return new AppResponse<string>(ocrResult,"ocr done!");
        }
        catch (Exception ex)
        {
            return new AppResponse(HttpStatusCode.BadRequest,ex.Message);
        }
    }
    
}