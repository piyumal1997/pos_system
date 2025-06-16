using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

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
                    .Replace("-", "")
                    .ToLower(); // Match SQL's lowercase hex
            }
        }
    }
}
