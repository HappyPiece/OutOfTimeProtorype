using System.Security.Cryptography;
using System.Text;

namespace OutOfTimePrototype.Utilities;

public static class HashingHelper
{
    public static string ComputeSha256Hash(string rawString)
    {
        var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawString));
        var builder = new StringBuilder(bytes.Length);

        foreach (var b in bytes) builder.Append(b.ToString("X"));

        return builder.ToString();
    }
}