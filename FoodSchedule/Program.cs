using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kiagallery.FoodSchedule
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        /// <summary>
        /// کلاس کمکی برای ارسال اس ام اس 
        /// </summary>
        public class NikSmsWebServiceClient
        {
            private readonly PublicServiceV1 service;
            /// <summary>
            /// متد سازنده
            /// </summary>
            public NikSmsWebServiceClient()
            {
                service = new PublicServiceV1("09354047788", "123456789");
            }
            /// <summary>
            /// ارسال تکی پیام
            /// </summary>
            /// <param name="senderNumber">شماره ارسال کننده</param>
            /// <param name="number">شماره گیرنده</param>
            /// <param name="message">پیام</param>
            /// <param name="sendOn">تایین تاریخ ارسال</param>
            /// <param name="sendType">نوع ارسال</param>
            /// <param name="yourMessageId">ردیف پیام</param>
            /// <returns></returns>
            public async Task<ServiceV1SingleSmsResult> SendOne(
                string senderNumber,
                string number,
                string message,
                DateTime? sendOn = null,
                int? sendType = null,
                long? yourMessageId = null)
            {
                var result = await service.SendOne(
                    senderNumber,
                    number,
                    message,
                    sendOn,
                    sendType,
                    yourMessageId
                );

                switch (result.Status)
                {
                    case LibOperationResultStatus.Success:
                        return result.Data;
                    case LibOperationResultStatus.InvalidModel:
                        throw new Exception("Some required inputs are misssing...");
                    case LibOperationResultStatus.UnAuthorized:
                        throw new Exception("Some thing is wrong with your account (call support!)");
                    case LibOperationResultStatus.Failed:
                        throw new Exception("Ooops its our fault!");
                    default:
                        throw new Exception("You sould not be here... :D");
                }
            }
            public async Task<ReturnSmsResult> SendGroup(
               string senderNumber,
               List<string> numbers,
               string message,
               DateTime? sendOn = null,
               int? sendType = null,
               List<long> yourMessageIds = null)
            {
                var result = await service.SendGroup(
                    senderNumber,
                    numbers,
                    message,
                    sendOn,
                    sendType,
                    yourMessageIds
                );
                switch (result.Status)
                {
                    case LibOperationResultStatus.Success:
                        return result.Data;
                    case LibOperationResultStatus.InvalidModel:
                        throw new Exception("Some required inputs are misssing...");
                    case LibOperationResultStatus.UnAuthorized:
                        throw new Exception("Some thing is wrong with your account (call support!)");
                    case LibOperationResultStatus.Failed:
                        throw new Exception("Ooops its our fault!");
                    default:
                        throw new Exception("You sould not be here... :D");
                }
            }
            /// <summary>
            /// متد ارسال پیام برای استفاده در برنامه
            /// </summary>
            /// <param name="text">متن پیام</param>
            /// <param name="number">شماره گیرنده</param>
            /// <returns></returns>
            public static string SendSmsNik(string text, string number)
            {
                NikSmsWebServiceClient helper = new NikSmsWebServiceClient();

                var task = helper.SendOne("blacklist", number, text);
                task.Wait();
                var result = task.Result;
                var messageRes = SendSmsStatusHandling(result.Status);
                return messageRes;
            }
            /// <summary>
            /// متد ارسال پیام برای استفاده در برنامه
            /// </summary>
            /// <param name="text">متن پیام</param>
            /// <param name="number">شماره گیرنده</param>
            /// <returns></returns>
            public static string SendSmsNik(string text, List<string> numbers)
            {
                NikSmsWebServiceClient helper = new NikSmsWebServiceClient();

                var task = helper.SendGroup("blacklist", numbers, text);
                task.Wait();
                var result = task.Result;
                var messageRes = SendSmsStatusHandling(result.Status);
                return messageRes;
            }
            /// <summary>
            /// بازگرداندن نتیجه ارسال پیام
            /// </summary>
            /// <param name="smsReturn">نتیجه ارسال پیام</param>
            /// <returns></returns>
            private static string SendSmsStatusHandling(SmsReturn smsReturn)
            {
                switch (smsReturn)
                {
                    case SmsReturn.ArgumentIsNullOrIncorrect:
                        return "پارامترهای هایی که برای ارسال پیام خود به سیستم فرستاده اید، اشتباه است.";

                    case SmsReturn.Filtered:
                        return "پیام شما از نظر متنی مشکلی داشته که باعث فیلتر شدن پنل شما شده است.";

                    case SmsReturn.ForbiddenHours:
                        return "شما مجاز به ارسال در این ساعت نمی باشید";

                    case SmsReturn.InsufficientCredit:
                        return "موجودی یا اعتبار شما برای انجام عملیات کافی نیست.";

                    case SmsReturn.MessageBodyIsNullOrEmpty:
                        return "پیام ارسالی شما دارای متن نبوده است، متن پیام را باید حتما وارد نمایید.";

                    case SmsReturn.NoFilters:
                        return "پیام شما از نظر متنی مشکلی داشته که باعث فیلتر شدن پیام شما شده است.";

                    case SmsReturn.PanelIsBlocked:
                        return "پنل کاربری شما مسدود شده است و باید با پشتیبانی تماس بگیرید.";

                    case SmsReturn.PrivateNumberIsDisable:
                        return "شماره اختصاصی که برای ارسال پیام خود انتخاب کرده اید، غیر فعال شده است.";

                    case SmsReturn.PrivateNumberIsIncorrect:
                        return "شماره اختصاصی وارد شده اشتباه است و یا به شما تعلق ندارد.";

                    case SmsReturn.ReceptionNumberIsIncorrect:
                        return "شماره موبایل های ارسالی اشتباه است.";

                    case SmsReturn.SentTypeIsIncorrect:
                        return "نوع ارسالی که انتخاب کرده اید با محتوای ارسالی شما مطابقت نداشته و اشتباه است";

                    case SmsReturn.Successful:
                        return "پیام شما با موفقیت ارسال شده است";

                    case SmsReturn.SiteUpdating:
                        return "سایت در حال بروزرسانی می باشد لطفا دقایقی دیگر مجددا درخواست خود را ارسال نمایید";

                    case SmsReturn.UnknownError:
                        return
                            "خطای نامشخصی رخ داده است که پیش بینی نشده بوده و باید با پشتیبانی فنی تماس بگیرید. (احتمال رخ دادن این خطا نزدیک به صفر بوده ولی جهت اطمینان، در مستندات ارائه می شود) ";

                    case SmsReturn.Warning:
                        return "ارسال شما با موفقیت انجام شد ولی برای متن انتخابی شما هشداری به ثبت رسید";

                    default:
                        return "وضعیت تعریف نشده لطفا به پشتیبانی فنی نیک اس ام اس اطلاع دهید";

                }
            }
        }
    }
}
