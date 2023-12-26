using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Ocr;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OcrController : MyControllerBase
{
    private readonly IOcrServices _ocrServices;
    private readonly ILogger<OcrController> _logger;

    public OcrController(IOcrServices ocrServices, ILogger<OcrController> logger)
    {
        _ocrServices = ocrServices;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AppResponse>> PerformOcr(IFormFile imageFile)
    {
        _logger.LogInformation($"calling ocr api on file:{imageFile.FileName}");
        using var memoryStream = new MemoryStream();
        await imageFile.CopyToAsync(memoryStream);
        var ocrResult = await _ocrServices.GetNationalCode(memoryStream.ToArray(), imageFile.FileName);
        return new AppResponse<OcrDto>(ocrResult, "ocr done!");
    }
}