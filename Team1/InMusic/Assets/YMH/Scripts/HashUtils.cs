using System.Security.Cryptography;
using System.Text;

public static class HashUtils
{
    public static string GetSha256(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            foreach (var b in hash)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
}