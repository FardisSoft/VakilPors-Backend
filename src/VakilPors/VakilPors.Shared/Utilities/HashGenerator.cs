
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using VakilPors.Shared.Settings;

namespace VakilPors.Shared.Utilities;

public static class HashGenerator
{ 
    public static string Hash(string key, string salt)
    {
        byte[] keyBytes = Encoding.Unicode.GetBytes(key);
        byte[] saltBytes = Encoding.Unicode.GetBytes(salt);
        byte[] array = new byte[keyBytes.Length + saltBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, array, 0, saltBytes.Length);
        Buffer.BlockCopy(keyBytes, 0, array, saltBytes.Length, keyBytes.Length);
        return Convert.ToBase64String(HashAlgorithm.Create("SHA256")!.ComputeHash(array));
    }

    public static string Hash(string key)
    {
        return Hash(key, HashSettings.DefaultSalt);
    }
}
