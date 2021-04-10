using System;
using System.Linq;

namespace KiaGallery.Common
{
    /// <summary>
    /// کلاس کمکی برای داده های متنی
    /// </summary>
    public class StringUtility
    {

        private static Random random = new Random();
        /// <summary>
        /// ساخت یک رشته تصادفی متنی برای توکن ها
        /// </summary>
        /// <param name="length">طول رشته</param>
        /// <returns>رشته ساخته شده</returns>
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// ساخت یک رشته تصادفی عددی
        /// </summary>
        /// <returns>عدد تصادفی 6 رقمی</returns>
        public static string RandomNumber()
        {
            return random.Next(100000, 999999).ToString();
        }
        
    }
}
