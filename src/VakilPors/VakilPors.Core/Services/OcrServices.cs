using VakilPors.Core.Contracts.Services;

namespace VakilPors.Core.Services;

class OcrServices : IOcrServices
{
    private readonly HttpClient _httpClient;
    public OcrServices(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }
    public Task<string> GetMelliCode(Stream imageFile, string fileName)
    {
        throw new NotImplementedException();
    }
}