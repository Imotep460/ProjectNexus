using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class EncryptionController : MonoBehaviour
{
    public TextMeshProUGUI resultOfDecryption;  //
    public TextMeshProUGUI encryptedString;

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
    public void HashPassword(TMP_InputField stringToEncrypt)
    {
        var cryptoProvider = new RNGCryptoServiceProvider();
        byte[] salt = new byte[SaltByteSize];
        cryptoProvider.GetBytes(salt);

        var hash = GetPbkdf2Bytes(stringToEncrypt.text, salt, Pbkdf2Iterations, HashByteSize);
        encryptedString.text = Pbkdf2Iterations + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Check if a input string matches an encrypted string.
    /// </summary>
    /// <param name="userInput">The password the User inputs.</param>
    /// <param name="correctHash">The encrypted password.</param>
    /// <returns></returns>
    public void ValidatePassword(TMP_InputField userInput)
    {
        char[] delimiter = { ':' };
        var split = encryptedString.text.Split(delimiter);
        var iterations = Int32.Parse(split[IterationIndex]);
        var salt = Convert.FromBase64String(split[SaltIndex]);
        var hash = Convert.FromBase64String(split[Pbkdf2Index]);

        var testHash = GetPbkdf2Bytes(userInput.text, salt, iterations, hash.Length);
        bool isMatch = SlowEquals(hash, testHash);

        if (isMatch == true)
        {
            resultOfDecryption.text = "Password MATCHED!";
        }
        else
        {
            resultOfDecryption.text = "Password DID NOT MATCH!";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool SlowEquals(byte[] a, byte[] b)
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
    private byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt);
        pbkdf2.IterationCount = iterations;
        return pbkdf2.GetBytes(outputBytes);
    }
}