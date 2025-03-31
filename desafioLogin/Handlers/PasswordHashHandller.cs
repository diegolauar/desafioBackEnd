using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

public class PasswordHashHandler
{
    private static readonly int _iterationCount = 100000;
    private static readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();

    public static string HashPassword(string password)
    {
        int saltSize = 16; // 128 bits
        var salt = new byte[saltSize];
        _randomNumberGenerator.GetBytes(salt);

        var subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA512, _iterationCount, 32); // 256 bits

        var outputBytes = new byte[1 + salt.Length + subkey.Length];
        outputBytes[0] = 0x01; // Versão do hash
        Buffer.BlockCopy(salt, 0, outputBytes, 1, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + saltSize, subkey.Length);

        return Convert.ToBase64String(outputBytes);
    }

    public static bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var decodedBytes = Convert.FromBase64String(hashedPassword);

        if (decodedBytes[0] != 0x01) // Verifica versão
            return false;

        var salt = new byte[16]; // 128 bits
        Buffer.BlockCopy(decodedBytes, 1, salt, 0, salt.Length);

        var subkey = new byte[32]; // 256 bits
        Buffer.BlockCopy(decodedBytes, 1 + salt.Length, subkey, 0, subkey.Length);

        var providedSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, KeyDerivationPrf.HMACSHA512, _iterationCount, 32);

        return CryptographicOperations.FixedTimeEquals(subkey, providedSubkey);
    }
}
