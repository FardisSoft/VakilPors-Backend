using System.Security.Cryptography;

namespace VakilPors.Shared.Utilities;

public static class RandomEngine
{
    public static string GenerateString(int length)
    {
        var rng = RandomNumberGenerator.Create();
        byte[] randomNumberBytes = new byte[length];
        rng.GetBytes(randomNumberBytes);
        return Convert.ToBase64String(randomNumberBytes)[..length];
    }
}

