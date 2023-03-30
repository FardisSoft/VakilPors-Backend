using System.Security.Cryptography;

namespace VakilPors.Shared.Utilities;

public static class RandomEngine
{
    public static string GenerateString(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] randomNumberBytes = new byte[length];
        rng.GetBytes(randomNumberBytes);
        return Convert.ToBase64String(randomNumberBytes)[..length];
    }
    public static int Next(int minValue, int maxExclusiveValue)
    {
        if (minValue >= maxExclusiveValue)
            throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");

        long diff = (long)maxExclusiveValue - minValue;
        long upperBound = uint.MaxValue / diff * diff;

        uint ui;
        do
        {
            ui = GetRandomUInt();
        } while (ui >= upperBound);
        return (int)(minValue + (ui % diff));
    }

    private static uint GetRandomUInt()
    {
        var randomBytes = GenerateRandomBytes(sizeof(uint));
        return BitConverter.ToUInt32(randomBytes, 0);
    }

    private static byte[] GenerateRandomBytes(int bytesNumber)
    {
        using var csp = RandomNumberGenerator.Create();
        byte[] buffer = new byte[bytesNumber];
        csp.GetBytes(buffer);
        return buffer;
    }
}

