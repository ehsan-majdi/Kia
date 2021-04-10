using System;

namespace KiaGallery.Common
{
    /// <summary>
    /// کلاس کمکی برای اطلاعات پایه
    /// </summary>
    public class Core
    {
        /// <summary>
        /// ذخیره خطا در برنامه
        /// </summary>
        /// <param name="ex">خطا</param>
        public static void SaveException(Exception ex)
        {

        }

        /// <summary>
        /// ذخیره خطا و گرفتن شی پاسخ برای اجکس
        /// </summary>
        /// <param name="ex">خطا</param>
        /// <returns></returns>
        public static Response GetExceptionResponse(Exception ex)
        {
            SaveException(ex);
            return new Response()
            {
                status = 500,
                message = "خطایی رخ داد."
            };
        }
        /// <summary>
        /// جدا کننده سه رقمی برای اعداد
        /// </summary>
        /// <param name="value">عدد</param>
        /// <returns>رشته سه رقمی جدا شده</returns>
        public static string CardToSeparator(double value)
        {
            return value.ToString("N0", new System.Globalization.NumberFormatInfo()
            {
                NumberGroupSizes = new[] { 4 },
                NumberGroupSeparator = "-"
            });
        }
        /// <summary>
        /// جدا کننده سه رقمی برای اعداد
        /// </summary>
        /// <param name="value">عدد</param>
        /// <returns>رشته سه رقمی جدا شده</returns>
        public static string ToSeparator(long value)
        {
            return value.ToString("N0", new System.Globalization.NumberFormatInfo()
            {
                NumberGroupSizes = new[] { 3 },
                NumberGroupSeparator = ","
            });
        }
        /// <summary>
        /// جدا کننده سه رقمی برای اعداد
        /// </summary>
        /// <param name="value">عدد</param>
        /// <returns>رشته سه رقمی جدا شده</returns>
        public static string ToSeparator(double value)
        {
            return value.ToString("N0", new System.Globalization.NumberFormatInfo()
            {
                NumberGroupSizes = new[] { 3 },
                NumberGroupSeparator = ","
            });
        }

        /// <summary>
        /// جدا کننده سه رقمی برای اعداد
        /// </summary>
        /// <param name="value">عدد</param>
        /// <returns>رشته سه رقمی جدا شده</returns>
        public static string ToSeparator(int value)
        {
            return ToSeparator(long.Parse(value.ToString()));
        }


        public static double Roundup(double number, int digits)
        {
            int num = (int)number;
            if (num % 10000 > 0)
            {
                num = num - (num % 10000) + 10000;
            }
            return num;
        }

       
    }
}
