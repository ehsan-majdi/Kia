using System;

namespace KiaGallery.Common
{
    /// <summary>
    /// کلاس کمکی برای کار کردن با گذرواژه
    /// </summary>
    public class PasswordTools
    {
        /// <summary>
        /// ساخت یک رشته کلیدی برای هش کردن گذرواژه
        /// </summary>
        /// <returns>رشته کلیدی برای هش کردن گذرواژه</returns>
        public static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        /// <summary>
        /// بررسی گذرواژه وارد شده
        /// </summary>
        /// <param name="Password">گذرواژه</param>
        /// <param name="HashPassword">گذرواژه کد شده</param>
        /// <param name="Salt">کلید ساخت گذرواژه</param>
        /// <returns>صحت برابر بودن گذرواژه و گذرواژه کد شده</returns>
        public static bool CheckPassword(string Password, string HashPassword, string Salt)
        {
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(Password, Salt);
            return hashPassword == HashPassword;
        }

        /// <summary>
        /// کد کردن رمز عبور
        /// </summary>
        /// <param name="password">گذرواژه</param>
        /// <returns>شامل یک زوج دوتایی که شماره اول آن کلید ساخته شدن گذرواژه و دیگری گذرواژه کد شده می باشد.</returns>
        public static Tuple<string, string> GetHashedPassword(string password)
        {
            string salt = GetRandomSalt();
            string hashPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return Tuple.Create(salt, hashPassword);
        }

    }
}
