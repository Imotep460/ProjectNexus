using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

public class EncryptionController : MonoBehaviour
{
    public const int SaltByteSize = 24;
    public const int HashByteSize = 20; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
    public const int Pbkdf2Iterations = 1000;
    public const int IterationIndex = 0;
    public const int SaltIndex = 1;
    public const int Pbkdf2Index = 2;

    /// <summary>
    /// Use an implementation of a PBKDF2 (Password-Based-Key-Derivation-Function-2) algorithme to encrypt a string.
    /// </summary>
    /// <param name="stringToEncrypt">The string you want to encrypt.</param>
    /// <returns>Returns an encrypted sting.</returns>
    public static string HashPassword(string stringToEncrypt)
    {
        var cryptoProvider = new RNGCryptoServiceProvider();
        byte[] salt = new byte[SaltByteSize];
        cryptoProvider.GetBytes(salt);

        var hash = GetPbkdf2Bytes(stringToEncrypt, salt, Pbkdf2Iterations, HashByteSize);
        return Pbkdf2Iterations + ":" +
               Convert.ToBase64String(salt) + ":" +
               Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Check if a input string matches an encrypted string.
    /// </summary>
    /// <param name="userInput">The password the User inputs.</param>
    /// <param name="correctHash">The encrypted password.</param>
    /// <returns></returns>
    public static bool ValidatePassword(string userInput, string correctHash)
    {
        char[] delimiter = { ':' };
        var split = correctHash.Split(delimiter);
        var iterations = Int32.Parse(split[IterationIndex]);
        var salt = Convert.FromBase64String(split[SaltIndex]);
        var hash = Convert.FromBase64String(split[Pbkdf2Index]);

        var testHash = GetPbkdf2Bytes(userInput, salt, iterations, hash.Length);
        return SlowEquals(hash, testHash);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static bool SlowEquals(byte[] a, byte[] b)
    {
        var diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }
        return diff == 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="password"></param>
    /// <param name="salt"></param>
    /// <param name="iterations"></param>
    /// <param name="outputBytes"></param>
    /// <returns></returns>
    private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
        pbkdf2.IterationCount = iterations;
        return pbkdf2.GetBytes(outputBytes);
    }
}