using VakilPors.Core.Domain.Dtos.Ocr;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IOcrServices:IScopedDependency
{
    Task<OcrDto> GetNationalCode(byte[] imageFile, string fileName);

}