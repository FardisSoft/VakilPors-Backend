﻿
using VakilPors.Shared.Settings;
using System.Security.Cryptography;
using System.Text;
using Beshno.Shared.Settings;

namespace VakilPors.Utilities;

public static class Encryption
{
    private static readonly byte[] key = Encoding.UTF8.GetBytes(EncryptionSettings.StringEncryptionKey);
    private static readonly byte[] iv = EncryptionSettings.StringEncryptionIv;

    public static string EncryptString(string plainText)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var encrypted = msEncrypt.ToArray();
        return Convert.ToBase64String(encrypted);
    }

    public static string DecryptString(string cipherText)
    {
        var cipherBytes = Convert.FromBase64String(cipherText);

        using var aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherBytes);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}

