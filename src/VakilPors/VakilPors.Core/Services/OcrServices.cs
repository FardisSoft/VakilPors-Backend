using System.Net;
using System.Net.Http.Json;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos.Ocr;
using VakilPors.Core.Exceptions;

namespace VakilPors.Core.Services;
public class OcrServices : IOcrServices
{
    private readonly HttpClient _httpClient;
    private const string DjangoApiUrl = "https://fardissoft.pythonanywhere.com/ocr/Image/";

    public OcrServices(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<OcrDto> GetNationalCode(byte[] imageFile, string fileName)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new ByteArrayContent(imageFile), "image", fileName);

        var response = await _httpClient.PostAsync(DjangoApiUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new BadImageFormatException("Image is can not be processed");
            }

            throw new InternalServerException("API call failed");
        }
        
        return await response.Content.ReadFromJsonAsync<OcrDto>();
    }
}