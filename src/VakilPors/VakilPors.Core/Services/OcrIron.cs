using IronOcr;
using VakilPors.Core.Contracts.Services;

namespace VakilPors.Core.Services;

public class OcrIron:IOcrService
{
    public async Task<string> PerformOcrAsync(Stream imageFile, string fileName)
    {
        var ocr = new IronTesseract();
        ocr.Language = OcrLanguage.Persian;
        using var input = new OcrInput(imageFile);
        var result = ocr.Read(input);
        var allText = result.Text;
        return allText;
    }
}