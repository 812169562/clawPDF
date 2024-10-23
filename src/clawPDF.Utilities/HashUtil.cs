using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace clawSoft.clawPDF.Utilities
{
    public class HashUtil
    {
        /// <summary>
        ///     Gets the SHA1 hash.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>hashed text</returns>
        public static string GetSha1Hash(string text)
        {
            var SHA1 = new SHA1CryptoServiceProvider();

            string result = null;

            var arrayData = Encoding.ASCII.GetBytes(text);
            var arrayResult = SHA1.ComputeHash(arrayData);

            for (var i = 0; i < arrayResult.Length; i++)
            {
                var temp = Convert.ToString(arrayResult[i], 16);
                if (temp.Length == 1)
                    temp = "0" + temp;
                result += temp;
            }

            return result;
        }

        /// <summary>
        /// Gets the SHA256 hash.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetSha256Hash(string filePath)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            byte[] hash = null;
            if (File.Exists(filePath))
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    hash = sha256.ComputeHash(stream);
                }
            }
            else
            {
                var arrayData = Encoding.ASCII.GetBytes(filePath);
                hash = sha256.ComputeHash(arrayData);
            }
            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }
    }
}