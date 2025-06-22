using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace pos_system.pos.BLL.Utilities
{
    public static class HashHelper
    {
        public static string ComputeSqlCompatibleHash(string password)
        {
            // Use ISO-8859-1 encoding to match SQL's VARCHAR
            byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(password);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes)
                    .Replace("-", string.Empty)
                    .ToLower(); // Match SQL's lowercase hex
            }
        }
    }
}
