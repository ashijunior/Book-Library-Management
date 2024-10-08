using System.Security.Cryptography;

namespace BookManagement.Hasher
{
    public class PasswordHasher
    {
        private static readonly int SaltSize = 16;
        private static readonly int HashSize = 20;
        private static readonly int Iterations = 10000;

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt); // Using RandomNumberGenerator instead of RNGCryptoServiceProvider

            var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hash = key.GetBytes(HashSize);

            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            var base64Hash = Convert.ToBase64String(hashBytes);
            return base64Hash;
        }

        public static bool VerifyPassword(string password, string base64Hash)
        {
            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = key.GetBytes(HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }

            return true;
        }
    }
}

//Token Generator
public class SecretTokenGenerator
{
    public static string GenerateSecretToken(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        byte[] data = new byte[length];

        // Use RandomNumberGenerator static method instead of RNGCryptoServiceProvider
        RandomNumberGenerator.Fill(data);

        var token = new char[length];
        for (int i = 0; i < length; i++)
        {
            token[i] = chars[data[i] % chars.Length];
        }

        return new string(token);
    }

    public static void Main(string[] args)
    {
        string secretToken = GenerateSecretToken();
        Console.WriteLine("Generated Secret Token: " + secretToken);
    }
}
