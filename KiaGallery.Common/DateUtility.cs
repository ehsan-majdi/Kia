using System;
using System.Globalization;

namespace KiaGallery.Common
{
    /// <summary>
    /// کلاس کمکی برای کار کردن با تاریخ
    /// </summary>
    public class DateUtility
    {
        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ و ساعت شمسی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>تاریخ و ساعت کنار با فرمت استاندارد</returns>
        public static string GetPersianDateTime(DateTime dateTime)
        {
            try
            {
                PersianCalendar Calendar = new PersianCalendar();
                string Year = Calendar.GetYear(dateTime).ToString();
                string Month = Calendar.GetMonth(dateTime) >= 10 ? Calendar.GetMonth(dateTime).ToString() : "0" + Calendar.GetMonth(dateTime);
                string Day = Calendar.GetDayOfMonth(dateTime) >= 10 ? Calendar.GetDayOfMonth(dateTime).ToString() : "0" + Calendar.GetDayOfMonth(dateTime);

                string TimeOfTheDay = dateTime.TimeOfDay.ToString();
                string hour = dateTime.Hour >= 10 ? dateTime.Hour.ToString() : "0" + dateTime.Hour;
                string minute = dateTime.Minute >= 10 ? dateTime.Minute.ToString() : "0" + dateTime.Minute;

                return Year + "/" + Month + "/" + Day + " " + hour + ":" + minute;
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ و ساعت شمسی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>تاریخ و ساعت کنار با فرمت استاندارد</returns>
        public static string GetPersianDateTime(DateTime? dateTime)
        {
            if (dateTime != null)
                return GetPersianDateTime(dateTime.GetValueOrDefault());
            else
                return null;
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ شمسی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>تاریخ با فرمت استاندارد و سال 4 رقمی</returns>
        public static string GetPersianDate(DateTime dateTime)
        {
            try
            {
                PersianCalendar Calendar = new PersianCalendar();
                string Year = Calendar.GetYear(dateTime).ToString();
                string Month = Calendar.GetMonth(dateTime) >= 10 ? Calendar.GetMonth(dateTime).ToString() : "0" + Calendar.GetMonth(dateTime);
                string Day = Calendar.GetDayOfMonth(dateTime) >= 10 ? Calendar.GetDayOfMonth(dateTime).ToString() : "0" + Calendar.GetDayOfMonth(dateTime);

                return Year + "/" + Month + "/" + Day;
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به ماه تاریخ شمسی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>تاریخ با فرمت استاندارد و سال 4 رقمی</returns>
        public static string GetPersianMonthName(DateTime dateTime)
        {
            try
            {
                PersianCalendar Calendar = new PersianCalendar();
                string Year = Calendar.GetYear(dateTime).ToString();
                string Month = "";

                switch (Calendar.GetMonth(dateTime))
                {
                    case 1:
                        Month = "فروردین";
                        break;
                    case 2:
                        Month = "اردیبهشت";
                        break;
                    case 3:
                        Month = "خرداد";
                        break;
                    case 4:
                        Month = "تیر";
                        break;
                    case 5:
                        Month = "مرداد";
                        break;
                    case 6:
                        Month = "شهریور";
                        break;
                    case 7:
                        Month = "مهر";
                        break;
                    case 8:
                        Month = "آبان";
                        break;
                    case 9:
                        Month = "آذر";
                        break;
                    case 10:
                        Month = "دی";
                        break;
                    case 11:
                        Month = "بهمن";
                        break;
                    case 12:
                        Month = "اسفند";
                        break;
                }

                return Year + " " + Month;
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ شمسی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>تاریخ با فرمت استاندارد و سال 4 رقمی</returns>
        public static string GetPersianDateNoDivider(DateTime dateTime)
        {
            try
            {
                PersianCalendar Calendar = new PersianCalendar();
                string Year = Calendar.GetYear(dateTime).ToString();
                string Month = Calendar.GetMonth(dateTime) >= 10 ? Calendar.GetMonth(dateTime).ToString() : "0" + Calendar.GetMonth(dateTime);
                string Day = Calendar.GetDayOfMonth(dateTime) >= 10 ? Calendar.GetDayOfMonth(dateTime).ToString() : "0" + Calendar.GetDayOfMonth(dateTime);

                return Day + " ".PadLeft(3) + Month + " ".PadLeft(3) + Year + " ".PadLeft(3);
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ کوتاه شمسی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>تاریخ با فرمت استاندارد و سال 2 رقمی</returns>
        public static string GetShortPersianDate(DateTime dateTime)
        {
            try
            {
                PersianCalendar Calendar = new PersianCalendar();
                string Year = Calendar.GetYear(dateTime).ToString().Substring(2);
                string Month = Calendar.GetMonth(dateTime) >= 10 ? Calendar.GetMonth(dateTime).ToString() : "0" + Calendar.GetMonth(dateTime);
                string Day = Calendar.GetDayOfMonth(dateTime) >= 10 ? Calendar.GetDayOfMonth(dateTime).ToString() : "0" + Calendar.GetDayOfMonth(dateTime);

                return Year + "/" + Month + "/" + Day;
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ کوتاه شمسی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>تاریخ با فرمت استاندارد و سال 2 رقمی</returns>
        public static string GetPersianDate(DateTime? dateTime)
        {
            if (dateTime != null)
                return GetPersianDate(dateTime.GetValueOrDefault());
            else
                return null;
        }

        /// <summary>
        /// تبدیل تاریخ شمسی به میلادی
        /// </summary>
        /// <param name="dateTime">تاریخ شمسی</param>
        /// <param name="hour">ساعت</param>
        /// <param name="minute">دقیقه</param>
        /// <param name="second">روز</param>
        /// <returns></returns>
        public static DateTime? GetDateTime(string dateTime, int hour = 0, int minute = 0, int second = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(dateTime)) return null;

                string[] parts = dateTime.Split('/', '-');
                if (parts.Length != 3)
                {
                    return null;
                }
                PersianCalendar Calendar = new PersianCalendar();
                return Calendar.ToDateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), hour, minute, second, 0);
            }
            catch (Exception ex)
            {
                Core.SaveException(ex);
                return null;
            }
        }

        /// <summary>
        /// گرفتن سال شمسی از تاریخ میلادی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>سال شمسی</returns>
        public static string GetPersianYear(DateTime dateTime)
        {
            PersianCalendar Calendar = new PersianCalendar();
            return Calendar.GetYear(dateTime).ToString();
        }

        /// <summary>
        /// گرفتن سال شمسی از تاریخ میلادی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>سال شمسی</returns>
        public static string GetPersianMonth(DateTime dateTime)
        {
            PersianCalendar Calendar = new PersianCalendar();
            return Calendar.GetMonth(dateTime).ToString("D2");
        }

        /// <summary>
        /// گرفتن سال شمسی از تاریخ میلادی
        /// </summary>
        /// <param name="dateTime">تاریخ میلادی</param>
        /// <returns>سال شمسی</returns>
        public static string GetPersianDay(DateTime dateTime)
        {
            PersianCalendar Calendar = new PersianCalendar();
            return Calendar.GetDayOfMonth(dateTime).ToString("D2");
        }

        /// <summary>
        /// تعداد دقیقه های سپرسی شده از ابتدای تاریخ فرستاده شده
        /// </summary>
        /// <param name="datetime">تاریخ</param>
        /// <returns>تعداد دقیقه های سپری شده</returns>
        public static int GetNumberOfMinutesDateTime(DateTime datetime)
        {
            var minute = DateTime.Now.Minute;
            var hour = DateTime.Now.Hour;
            return hour * 60 + minute;
        }

        /// <summary>
        /// گرفتن تعداد روزهای یک ماه بر اساس ماه و سال
        /// </summary>
        /// <param name="year">سال</param>
        /// <param name="month">ماه</param>
        /// <returns>تعداد روز های ماه وارد شده</returns>
        public static int GetMonthDayCount(int year, int month)
        {
            if (month <= 6) // شش ماه اول سال 31 روز است
                return 31;

            if (month < 12) // شش ماه دوم سال 30 روز است
                return 30;

            var helper = new PersianCalendar();
            if (helper.IsLeapYear(year)) // سال کبیسه 30 روزه است
                return 30;
            else
                return 29;
        }

        /// <summary>
        /// تبدیل تاریخ میلادی به تاریخ شمسی
        /// </summary>
        /// <param name="dateTime">ورودی تاریخ میلادی مورد نظر</param>
        /// <returns>تاریخ با فرمت استاندارد و سال 4 رقمی</returns>
        public static string GetPersianShortDateString(DateTime dateTime)
        {
            PersianCalendar Calendar = new PersianCalendar();
            string Day = Calendar.GetDayOfMonth(dateTime) >= 10 ? Calendar.GetDayOfMonth(dateTime).ToString() : "0" + Calendar.GetDayOfMonth(dateTime);

            return PersionDayOfWeek(dateTime) + " " + Day + " " + PersianGetMonthName(Calendar.GetMonth(dateTime));
        }
        /// <summary>
        /// دریافت عنوان فارسی ماه
        /// </summary>
        /// <param name="month">عدد ماه مورد نظر</param>
        /// <returns>عنوان ماه</returns>
        private static string PersianGetMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "فروردین";
                case 2:
                    return "اردیبهشت";
                case 3:
                    return "خرداد";
                case 4:
                    return "تیر";
                case 5:
                    return "مرداد";
                case 6:
                    return "شهریور";
                case 7:
                    return "مهر";
                case 8:
                    return "آبان";
                case 9:
                    return "آذر";
                case 10:
                    return "دی";
                case 11:
                    return "بهمن";
                case 12:
                    return "اسفند";
                default:
                    throw new Exception();
            }
        }
        private static string[] dayOfWeek = new string[] { "شنبه", "یک شنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه" };

        /// <summary>
        /// دریافت عنوان روز هفته از تاریخ
        /// </summary>
        /// <param name="date">تاریخ مورد نظر</param>
        /// <returns>عنوان روز مورد نظر</returns>
        private static string PersionDayOfWeek(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    return dayOfWeek[0];
                case DayOfWeek.Sunday:
                    return dayOfWeek[1];
                case DayOfWeek.Monday:
                    return dayOfWeek[2];
                case DayOfWeek.Tuesday:
                    return dayOfWeek[3];
                case DayOfWeek.Wednesday:
                    return dayOfWeek[4];
                case DayOfWeek.Thursday:
                    return dayOfWeek[5];
                case DayOfWeek.Friday:
                    return dayOfWeek[6];
                default:
                    throw new Exception();
            }
        }

    }
}
