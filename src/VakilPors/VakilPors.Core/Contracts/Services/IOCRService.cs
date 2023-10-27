
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services;

public interface IOcrService : IScopedDependency
{
    Task<string> PerformOcrAsync(Stream imageFile,string fileName);
}