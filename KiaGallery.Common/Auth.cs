using System;
using System.Security.Cryptography;
using System.Text;

namespace KiaGallery.Common
{
    public class Auth
    {
        private const string Key = "KiaTech@Esmaiel";
        private const string Algorithm = "HmacSHA256";
        private const string Salt = "rz8LuOtFBXphj9WQfvFh";

        /// <summary>
        /// ساخت توکن برای اعتبار سنجی کاربر
        /// </summary>
        /// <param name="userId">ردیف کاربر</param>
        /// <param name="ip">آی پی ایجاد کننده توکن</param>
        /// <returns>توکن ایجاد شده</returns>
        public static string GenerateToken(int userId, string ip)
        {
            var ticks = DateTime.Now.Ticks;
            string hash = string.Join(":", new string[] { userId.ToString(), ip, Key, ticks.ToString() });
            string hashLeft = "";
            string hashRight = "";
            using (var hmac = HMAC.Create(Algorithm))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(hash));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hashLeft = Convert.ToBase64String(hmac.Hash);
                hashRight = string.Join(":", new string[] { userId.ToString(), ticks.ToString(), Key });
            }

            string TempValue = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
            return TempValue + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
        }

        private static string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, Salt });
            using (HMAC hmac = HMAC.Create(Algorithm))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(Salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hmac.Hash);
            }
        }

        /// <summary>
        /// بررسی توکن برای اعتبار سنجی کاربر اعتبار سنجی شده و بازگرداندن ردیف کاربر مورد نظر
        /// </summary>
        /// <param name="token">توکن</param>
        /// <returns>ردیف کاربر که توکن برای آن صادر شده است</returns>
        public static int? CheckToken(string token)
        {
            token = token.Remove(0, 6);
            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                string key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                // Split the parts.
                string[] parts = key.Split(new char[] { ':' });
                if (parts.Length == 4)
                {
                    // Get the hash message, username, and timestamp.
                    string hash = parts[0];
                    int userId = int.Parse(parts[1]);
                    long ticks = long.Parse(parts[2]);
                    string userkey = parts[3];

                    DateTime timeStamp = new DateTime(ticks);
                    // Ensure the timestamp is valid.

                    return userId;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
