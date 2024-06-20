using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace clawPDF.Lincense
{
    /// <summary>
    /// 加密操作
    /// 说明：
    /// 1. AES加密整理自支付宝SDK
    /// </summary>
    public static class Encrypt
    {

        #region Md5加密

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By16(string value)
        {
            return Md5By16(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回16位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string Md5By16(string value, Encoding encoding)
        {
            return Md5(value, encoding, 4, 8);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        private static string Md5(string value, Encoding encoding, int? startIndex, int? length)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            var md5 = new MD5CryptoServiceProvider();
            string result;
            try
            {
                var hash = md5.ComputeHash(encoding.GetBytes(value));
                result = startIndex == null ? BitConverter.ToString(hash) : BitConverter.ToString(hash, startIndex ?? 0, length ?? 0);
            }
            finally
            {
                md5.Clear();
            }
            return result.Replace("-", "");
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        public static string Md5By32(this string value)
        {
            return Md5By32(value, Encoding.UTF8);
        }

        /// <summary>
        /// Md5加密，返回32位结果
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="encoding">字符编码</param>
        public static string Md5By32(string value, Encoding encoding)
        {
            return Md5(value, encoding, null, null);
        }

        #endregion

        #region DES加密

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="validityDate">有效期</param>
        public static string DesEncryptMD5(string validityDate)
        {
            if (string.IsNullOrEmpty(validityDate))
                validityDate = DateTime.Now.AddDays(10).AddYears(1).ToString("yyyy-MM-dd");
            return DesEncrypt($"{Compter().Md5By32()},{validityDate}", DesKey);
        }
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="validityDate">有效期</param>
        public static string DesEncryptMD5(string value, string validityDate)
        {
            if (string.IsNullOrEmpty(validityDate))
                validityDate = DateTime.Now.AddDays(10).AddYears(1).ToString("yyyy-MM-dd");
            return DesEncrypt($"{value},{validityDate}", DesKey);
        }


        public static string Compter()
        {
            string reslut = "";
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                foreach (var item in searcher.Get())
                {
                    reslut += $"Manufacturer: {item["Manufacturer"]}";
                    reslut += $"Product: {item["Product"]}";
                    reslut += $"SerialNumber: {item["SerialNumber"]}";
                }
                if (string.IsNullOrEmpty(reslut))
                {
                    return "djiodjwe90i192j3i12j3io123jjpw";
                }
                return reslut;
            }
            catch (Exception)
            {
                return reslut;
            }
        }
        /// <summary>
        /// DES密钥,24位字符串
        /// </summary>
        public static string DesKey = "#s^un2ye21fcv%|f0XpR,+vc";

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string DesEncrypt(object value)
        {
            return DesEncrypt(value, DesKey);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DesEncrypt(object value, string key)
        {
            return DesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DesEncrypt(object value, string key, Encoding encoding)
        {
            string text = value == null ? "" : value.ToString().Trim();
            if (ValidateDes(text, key) == false)
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateEncryptor())
            {
                return GetEncryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 验证Des加密参数
        /// </summary>
        private static bool ValidateDes(string text, string key)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(key))
                return false;
            return key.Length == 24;
        }

        /// <summary>
        /// 创建Des加密服务提供程序
        /// </summary>
        private static TripleDESCryptoServiceProvider CreateDesProvider(string key)
        {
            return new TripleDESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(key), Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        }

        /// <summary>
        /// 获取加密结果
        /// </summary>
        private static string GetEncryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = encoding.GetBytes(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Convert.ToBase64String(result);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string DesDecrypt(object value)
        {
            return DesDecrypt(value, DesKey);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        public static string DesDecrypt(object value, string key)
        {
            return DesDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥,24位</param>
        /// <param name="encoding">编码</param>
        public static string DesDecrypt(object value, string key, Encoding encoding)
        {
            string text = value == null ? "" : value.ToString().Trim();
            if (!ValidateDes(text, key))
                return string.Empty;
            using (var transform = CreateDesProvider(key).CreateDecryptor())
            {
                return GetDecryptResult(text, encoding, transform);
            }
        }

        /// <summary>
        /// 获取解密结果
        /// </summary>
        private static string GetDecryptResult(string value, Encoding encoding, ICryptoTransform transform)
        {
            var bytes = System.Convert.FromBase64String(value);
            var result = transform.TransformFinalBlock(bytes, 0, bytes.Length);
            return encoding.GetString(result);
        }

        #endregion

        #region AES加密

        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] _iv;
        /// <summary>
        /// 128位0向量
        /// </summary>
        private static byte[] Iv
        {
            get
            {
                if (_iv == null)
                {
                    var size = 16;
                    _iv = new byte[size];
                    for (int i = 0; i < size; i++)
                        _iv[i] = 0;
                }
                return _iv;
            }
        }

        /// <summary>
        /// AES密钥
        /// </summary>
        public const string AesKey = "QqP2AF3utIarcBqdhYTZpVGbiNQ9M5AL";

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">key</param>
        public static string ToAesEncrypt(this string value, string key = AesKey)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            return AesEncrypt(value, key);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">待解密的值</param>
        /// <param name="key">key</param>
        public static string ToAesDecrypt(this string value, string key = AesKey)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            return AesDecrypt(value, key);
        }

        ///// <summary>
        ///// AES加密
        ///// </summary>
        ///// <param name="value">待加密的值</param>
        //public static string ToAesEncrypt(this string value)
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        return "";
        //    }
        //    return AesEncrypt(value, AesKey);
        //}

        ///// <summary>
        ///// AES加密
        ///// </summary>
        ///// <param name="value">待加密的值</param>
        //public static string ToAesDecrypt(this string value)
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        return "";
        //    }
        //    return AesDecrypt(value, AesKey);
        //}

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        public static string AesEncrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            return AesEncrypt(value, AesKey);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        public static string AesEncrypt(string value, string key)
        {
            return AesEncrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">待加密的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string AesEncrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key);
            using (var transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetEncryptResult(value, encoding, transform);
            }
        }

        /// <summary>
        /// 创建RijndaelManaged
        /// </summary>
        private static RijndaelManaged CreateRijndaelManaged(string key)
        {
            return new RijndaelManaged
            {
                Key = System.Convert.FromBase64String(key),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = Iv
            };
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        public static string AesDecrypt(string value)
        {
            return AesDecrypt(value, AesKey);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        public static string AesDecrypt(string value, string key)
        {
            return AesDecrypt(value, key, Encoding.UTF8);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">加密后的值</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码</param>
        public static string AesDecrypt(string value, string key, Encoding encoding)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(key))
                return string.Empty;
            var rijndaelManaged = CreateRijndaelManaged(key);
            using (var transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV))
            {
                return GetDecryptResult(value, encoding, transform);
            }
        }

        #endregion
    }
}
