using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IOcrServices:IScopedDependency
{
    Task<string> GetMelliCode(Stream imageFile, string fileName);

}