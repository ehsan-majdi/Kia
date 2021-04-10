using System;

namespace KiaGallery.Model
{
    /// <summary>
    /// کلاس کمکی برای عناوین داده های شمارشی استفاده شده در برنامه
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// دریافت عنوان نوع پیام
        /// </summary>
        /// <param name="smsCategoryType">نوع پرسش</param>
        /// <returns>عنوان نوع پرسش</returns>
        public static string GetTitle(SmsCategoryType smsCategoryType)
        {
            switch (smsCategoryType)
            {
                case SmsCategoryType.MultiChoice:
                    return "چندانتخابی به همراه توضیحات مرتبط";
                case SmsCategoryType.SingleChoice:
                    return "تک انتخابی بدون توضیحات مرتبط";
                case SmsCategoryType.Descriptive:
                    return "تشریحی";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریافت عنوان نوع پرسش
        /// </summary>
        /// <param name="crmQuestionType">نوع پرسش</param>
        /// <returns>عنوان نوع پرسش</returns>
        public static string GetTitle(CrmQuestionType crmQuestionType)
        {
            switch (crmQuestionType)
            {
                case CrmQuestionType.YesNo:
                    return "بله/خیر";
                case CrmQuestionType.MultiChoice:
                    return "چند انتخابی";
                case CrmQuestionType.SingleChoice:
                    return "تک انتخابی";
                case CrmQuestionType.Descriptive:
                    return "تشریحی";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریافت عنوان نوع چرم
        /// </summary>
        /// <param name="leatherType">نوع چرم</param>
        /// <returns>عنوان نوع چرم</returns>
        public static string GetTitle(LeatherType leatherType)
        {
            switch (leatherType)
            {
                case LeatherType.Piped:
                    return "لوله ای";
                case LeatherType.Sewing:
                    return "دوخت";
                case LeatherType.Texture:
                    return "بافت";
                case LeatherType.Rail:
                    return "ریلی";
                case LeatherType.Masculine:
                    return "مردانه";
                case LeatherType.Rope:
                    return "رشته ای";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// مدرک تحصیلی
        /// </summary>
        /// <param name="leatherType">نوع چرم</param>
        /// <returns>عنوان مدرک تحصیلی</returns>
        public static string GetTitle(Education education)
        {
            switch (education)
            {
                case Education.Pre_highSchool:
                    return "سیکل";
                case Education.Diploma:
                    return "دیپلم";
                case Education.AssociatedDegree:
                    return "کاردانی";
                case Education.Bachelor:
                    return "کارشناسی";
                case Education.Master:
                    return "کارشناسی ارشد";
                case Education.PhD:
                    return "دکترا";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریافت عنوان نوع سنگ
        /// </summary>
        /// <param name="stoneType">نوع سنگ</param>
        /// <returns>عنوان نوع سنگ</returns>
        public static string GetTitle(StoneType stoneType)
        {
            switch (stoneType)
            {
                case StoneType.Transparent:
                    return "براق";
                case StoneType.Sedimentary:
                    return "رسوبی";
                case StoneType.Pearl:
                    return "مروارید";
                case StoneType.Atomic:
                    return "اتمی";
                case StoneType.LeatherBraceletStone:
                    return "سنگ دستبند چرمی";
                case StoneType.BraceletStone:
                    return "دستبند سنگی";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریافت هنوان نوع محصول
        /// </summary>
        /// <param name="productType">نوع محصول</param>
        /// <returns>هنوان نوع محصول</returns>
        public static string GetTitle(ProductType productType)
        {
            switch (productType)
            {
                case ProductType.GoldBracelet:
                    return "دستبند زنجير طلا";
                case ProductType.Necklace:
                    return "گردنبند";
                case ProductType.Earring:
                    return "گوشواره";
                case ProductType.Ring:
                    return "انگشتر";
                case ProductType.WatchPendent:
                    return "آویز ساعت";
                case ProductType.Brooch:
                    return "سنجاق بچگانه";
                case ProductType.Set:
                    return "ست";
                case ProductType.Anklet:
                    return "پابند";
                case ProductType.Pendant:
                    return "آویز";
                case ProductType.Bangle:
                    return "النگو";
                case ProductType.OuterWerk:
                    return "خرج کار";
                case ProductType.LeatherBracelet:
                    return "دستبند چرمی";
                case ProductType.RailBracelet:
                    return "دستبند ریلی";
                case ProductType.StoneBracelet:
                    return "دستبند سنگی";
                case ProductType.Plaque:
                    return "پلاک لیزر";
                case ProductType.WatchPendent2:
                    return "زنجیرساعت2";
                case ProductType.CottonBracelet:
                    return "دستبند نخی";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریافت عنوان نوع سفارش
        /// </summary>
        /// <param name="orderType">نوع سفارش</param>
        /// <returns>عنوان نوع سفارش</returns>
        public static string GetTitle(OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.None:
                    return "عادی";
                case OrderType.Personalization:
                    return "سفارشی";
                case OrderType.Customize:
                    return "سفارش داخلی";
                case OrderType.Repair:
                    return "تعمیری";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// نوع محصول سفارشی
        /// </summary>
        /// <param name="orderTypeForm">نوع سفارش</param>
        /// <returns>عنوان نوع سفارش</returns>
        public static string GetTitle(OrderTypeForm orderTypeForm)
        {
            switch (orderTypeForm)
            {
                case OrderTypeForm.DefineProduct:
                    return "سفارش ساخت برای محصولات تعریف شده";
                case OrderTypeForm.UnDefineProduct:
                    return "سفارش ساخت برای محصولات تعریف نشده";
                case OrderTypeForm.DesignProduct:
                    return "سفارش ساخت برای محصولات";
                case OrderTypeForm.RepairProduct:
                    return "تعمیری";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریافت عنوان نوع طلا
        /// </summary>
        /// <param name="goldType">نوع طلا</param>
        /// <returns>عنوان نوع سفارش</returns>
        public static string GetTitle(GoldType? goldType)
        {
            if (goldType == null)
                return "";

            switch (goldType)
            {
                case GoldType.Both:
                    return "سلیقه ای";
                case GoldType.Matte:
                    return "مات";
                case GoldType.Shiny:
                    return "براق";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریافت عنوان وضعیت کالای داخل سفارش
        /// </summary>
        /// <param name="orderDetailStatus">وضعیت کالای داخل سفارش</param>
        /// <returns>عنوان وضعیت کالای داخل سفارش</returns>
        public static string GetTitle(OrderDetailStatus orderDetailStatus)
        {
            switch (orderDetailStatus)
            {
                case OrderDetailStatus.Registered:
                    return "ثبت شده";
                case OrderDetailStatus.InWorkshop:
                    return "ارسال شده به کارگاه";
                case OrderDetailStatus.UnderConstruction:
                    return "در حال ساخت";
                case OrderDetailStatus.OutOfConstruction:
                    return "اتمام ساخت";
                case OrderDetailStatus.InPreparation:
                    return "در حال آماده سازی";
                case OrderDetailStatus.ReadyForDelivery:
                    return "آماده تحویل";
                case OrderDetailStatus.Sent:
                    return "ارسال شده";
                case OrderDetailStatus.Shortage:
                    return "کسری";
                case OrderDetailStatus.ShortageOrder:
                    return "سفارش مجدد";
                case OrderDetailStatus.Cancel:
                    return "لغو شده";
                case OrderDetailStatus.InWorkshop2:
                    return "ارسال شده به کارگاه دوم/مونتاژ";
                case OrderDetailStatus.UnderConstruction2:
                    return "در حال ساخت در کارگاه دوم/مونتاژ";
                case OrderDetailStatus.OutOfConstruction2:
                    return "اتمام ساخت در کارگاه دوم/مونتاژ";
                case OrderDetailStatus.Ordered:
                    return "سفارش داده شده";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریافت عنوان نوع شکل سنگ
        /// </summary>
        /// <param name="stoneShape">شکل سنگ</param>
        /// <returns>عنوان نوع شکل سنگ</returns>
        public static string GetTitle(StoneShape stoneShape)
        {
            switch (stoneShape)
            {
                case StoneShape.Circle:
                    return "دایره";
                case StoneShape.Marquise:
                    return "مارکیز";
                case StoneShape.Tear:
                    return "اشک";
                case StoneShape.Triangle:
                    return "مثلث";
                case StoneShape.Square:
                    return "مربع";
                case StoneShape.Rectangle:
                    return "مستطیل";
                case StoneShape.Oval:
                    return "بیضی";
                case StoneShape.Other:
                    return "سایر";
                default:
                    throw new Exception("StoneShape type not found");
            }
        }

        /// <summary>
        /// دریافت عنوان وضعیت گیفت
        /// </summary>
        /// <param name="giftStatus">وضعیت گیفت</param>
        /// <returns>عنوان وضعیت گیفت</returns>
        public static string GetTitle(GiftStatus giftStatus)
        {
            switch (giftStatus)
            {
                case GiftStatus.Registered:
                    return "ثبت شده";
                case GiftStatus.PrintingHouse:
                    return "ارسال شده به چاپخانه";
                case GiftStatus.DeliveryFromPrintingHouse:
                    return "از چاپخانه تحویل گرفته شد";
                case GiftStatus.SoldToTheBranch:
                    return "فروخته شده به شعبه/شرکت";
                case GiftStatus.SoldToTheCustomer:
                    return "فروخته شده به مشتری";
                case GiftStatus.Used:
                    return "استفاده شده";
                case GiftStatus.Cancel:
                    return "لغو شده";
                case GiftStatus.Burn:
                    return "سوزانده شده";
                default:
                    throw new Exception("GiftStatus type not found");
            }
        }

        /// <summary>
        /// دریافت عنوان وضعیت گیفت
        /// </summary>
        /// <param name="giftStatus">وضعیت گیفت</param>
        /// <returns>عنوان وضعیت گیفت</returns>
        public static string GetTitle(OuterWerkType? outerWerkType)
        {
            switch (outerWerkType)
            {
                case OuterWerkType.TwoLittleRings:
                    return "دو حلقه چه کوچک (زنجیر ساعت)";
                case OuterWerkType.TwoBigRings:
                    return "دو حلقه چه بزرگ (دستبند)";
                case OuterWerkType.TwoSidesRing:
                    return "دو طرف حلقه چه (دستبند)";
                case OuterWerkType.Size4:
                    return "سایز 4";
                case OuterWerkType.Size6:
                    return "سایز 6";
                case OuterWerkType.Size8:
                    return "سایز 8";
                case OuterWerkType.Tiny:
                    return "کوچک";
                case OuterWerkType.Medium:
                    return "متوسط";
                case OuterWerkType.Big:
                    return "بزرگ";
                case null:
                    return "";
                default:
                    throw new Exception("GiftStatus type not found");
            }
        }

        /// <summary>
        /// دریافت عنوان وضعیت گزارش روزانه
        /// </summary>
        /// <param name="calendarStatus">وضعیت روز در تقویم</param>
        /// <returns>عنوان وضعیت روز در تقویم</returns>
        public static string GetTitle(CalendarStatus? calendarStatus)
        {
            switch (calendarStatus)
            {
                case CalendarStatus.Draft:
                    return "پیش نویس";
                case CalendarStatus.Submit:
                    return "ثبت نهایی شده";
                case CalendarStatus.None:
                case null:
                    return "بدون وضعیت";
                default:
                    throw new Exception("TypePayments type not found");
            }
        }

        /// <summary>
        /// دریافت عنوان نوع پرداخت
        /// </summary>
        /// <param name="typePayments">نوع پرداخت</param>
        /// <returns>عنوان نوع پرداخت</returns>
        public static string GetTitle(TypePayments? typePayments)
        {
            switch (typePayments)
            {
                case TypePayments.Deposits:
                    return "واریزی";
                case TypePayments.Returned:
                    return "مرجوعی";
                case TypePayments.Sale:
                    return "فروش";
                case TypePayments.DifferentReturns:
                    return "مرجوعی متفرقه";
                case TypePayments.DifferentSale:
                    return "فروش متفرقه";
                case TypePayments.DifferentGoldReturns:
                    return "مرجوعی طلا متفرقه";
                case TypePayments.DifferentGoldSale:
                    return "فروش طلا متفرقه";
                case null:
                    return "";
                default:
                    throw new Exception("TypePayments type not found");
            }
        }

        /// <summary>
        /// عنوان نوع متفرقه
        /// </summary>
        /// <param name="differentType">نوع متفرقه</param>
        /// <returns>عنوان نوع متفرقه</returns>
        public static string GetTitle(DifferentType? differentType)
        {
            switch (differentType)
            {
                case DifferentType.GiftCart:
                    return "کارت هدیه";
                case DifferentType.Check:
                    return "مرجوعی";
                case DifferentType.CheckNotRegistered:
                    return "فروش";
                case DifferentType.LeatherOne:
                    return "چرم یک دور";
                case DifferentType.LeatherTow:
                    return "چرم دو دور";
                case DifferentType.SimpleGold:
                    return "طلا ساده";
                case DifferentType.Others:
                    return "سایر";
                case null:
                    return "";
                default:
                    throw new Exception("DifferentType type not found");
            }
        }

        /// <summary>
        /// دریافت عنوان نوع گیفت 
        /// </summary>
        /// <param name="typePayments">نوع گیفت</param>
        /// <returns>عنوان نوع گیفت</returns>
        public static string GetTitle(GiftType? giftType)
        {
            switch (giftType)
            {
                case GiftType.Cart:
                    return "کارت هدیه";
                case GiftType.Check:
                    return "بن خرید";
                case GiftType.CheckNotRegistered:
                    return "بن خرید بدون ثبت";
                case GiftType.CartNotRegistered:
                    return "کارت هدیه بدون ثبت";
                case GiftType.UnregisterFivePercentCard:
                    return "کارت %5 بدون ثبت";
                case GiftType.FivePercentCard:
                    return "کارت %5 ";
                case null:
                    return "";
                default:
                    throw new Exception("GiftType type not found");
            }
        }

        /// <summary>
        /// </summary>
        /// دریافت عنوان نوع فایل پرسنل
        /// <param name="orderType">نوع فایل</param>
        /// <returns>عنوان نوع فایل</returns>
        public static string GetTitle(PersonFileType fileType)
        {
            switch (fileType)
            {
                case PersonFileType.Image:
                    return "تصویر";
                case PersonFileType.PDF:
                    return "PDF";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// </summary>
        /// دریافت عنوان نوع فایل پرسنل
        /// <param name="orderType">نوع فایل</param>
        /// <returns>عنوان نوع فایل</returns>
        public static string GetTitle(PersonFileCategory category)
        {
            switch (category)
            {
                case PersonFileCategory.ContractForm:
                    return "فرم قرار داد";
                case PersonFileCategory.Medical:
                    return "پزشکی";
                case PersonFileCategory.PersonalPhoto:
                    return "عکس پرسنلی";
                case PersonFileCategory.Other:
                    return "سایر";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// </summary>
        /// دریافت عنوان نوع پرسنل
        /// <param name="orderType">نوع پرسنل</param>
        /// <returns>عنوان نوع پرسنل</returns>
        public static string GetTitle(PersonType personType)
        {
            switch (personType)
            {
                case PersonType.CentralOffice:
                    return "دفتر مرکزی";
                case PersonType.Branch:
                    return "شعبه";
                case PersonType.Workshop:
                    return "کارگاه";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// </summary>
        /// دریافت عنوان وضعیت سفارش ربات
        /// <param name="orderType"> وضعیت سفارش ربات</param>
        /// <returns>عنوان وضعیت سفارش ربات</returns>
        public static object GetTitle(BotOrderStatus botOrderStatus)
        {
            switch (botOrderStatus)
            {
                case BotOrderStatus.None:
                    return "نا مشخص";
                case BotOrderStatus.PendingCall:
                    return "رد تماس";
                case BotOrderStatus.RejectCall:
                    return "Reject Call";
                case BotOrderStatus.PendingPrepayment:
                    return "Pending Prepayment";
                case BotOrderStatus.UnderConstruction:
                    return "در دست ساخت";
                case BotOrderStatus.PendingPayment:
                    return "در انتظار پرداخت";
                case BotOrderStatus.Sent:
                    return "ارسال";
                case BotOrderStatus.Canceled:
                    return "Canceled";
                case BotOrderStatus.ReferredTo:
                    return "Referred To";
                case BotOrderStatus.PendingCustomer:
                    return "در انتظار مشتری";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریافت عنوان نوع شعبه
        /// </summary>
        /// <param name="branchType">نوع شعبه</param>
        /// <returns>عنوان نوع شعبه</returns>
        public static string GetTitle(BranchType branchType)
        {
            switch (branchType)
            {
                case BranchType.Branch:
                    return "شعبه";
                case BranchType.Solicitorship:
                    return "نمایندگی";
                case BranchType.Other:
                    return "سایر";
                case BranchType.CoWorker:
                    return "همکار";

                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// </summary>
        /// دریافت عنوان نوع پیام عمومی
        /// <param name="orderType">نوع پیام عمومی</param>
        /// <returns>عنوان نوع پیام عمومی</returns>
        public static string GetTitle(BotType botType)
        {
            switch (botType)
            {
                case BotType.Text:
                    return "متن";
                case BotType.Image:
                    return "تصویر";
                case BotType.Video:
                    return "ویدیو";
                case BotType.DailyOffer:
                    return "DailyOffer";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// </summary>
        /// دریافت عنوان نوع پیام عمومی
        /// <param name="orderType">نوع پیام عمومی</param>
        /// <returns>عنوان نوع پیام عمومی</returns>
        public static string GetTitle(CrmSex crmSex)
        {
            switch (crmSex)
            {
                case CrmSex.Man:
                    return "آفا";
                case CrmSex.Woman:
                    return "خانم";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریفات عنوان نوع ارسال سفارش
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان نوع ارسال سفارش</returns>
        public static string GetTitle(DeliveryType? deliverType)
        {
            if (deliverType == null) return "";

            switch (deliverType)
            {
                case DeliveryType.DeliveryMan:
                    return "پیک";
                case DeliveryType.Branch:
                    return "شعبه";
                case DeliveryType.KiaPersonnel:
                    return "پرسنل کیا";
                case DeliveryType.Post:
                    return "پست";
                case DeliveryType.Customer:
                    return "حضوری";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریفات عنوان وضعیت سفارش
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان وضعیت سفارش</returns>
        public static string GetTitle(InternalOrderStatus? status)
        {
            switch (status)
            {
                case InternalOrderStatus.Registered:
                    return "ثبت شده";
                case InternalOrderStatus.SendToBranch:
                    return "ارسال به شعبه";
                case InternalOrderStatus.ReadyForDeliver:
                    return "آماده تحویل";
                case InternalOrderStatus.Delivered:
                    return "تحویل شده";
                case InternalOrderStatus.Cancel:
                    return "لغو شده";
                case InternalOrderStatus.PendingCustomer:
                    return "خبر میده";
                case InternalOrderStatus.NoAnswer:
                    return "جواب نداد";
                case InternalOrderStatus.Deleted:
                    return "حذف شده";
                case InternalOrderStatus.SendToOffice:
                    return "ارسال به دفتر مرکزی";
                case InternalOrderStatus.AcceptFromBranch:
                    return "تحویل از شعبه";
                case InternalOrderStatus.AcceptFromOffice:
                    return "تحویل از دفتر مرکزی";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریفات عنوان جنسیت
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان جنسیت</returns>
        public static string GetTitle(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return "جناب آقای";
                case Gender.Female:
                    return "سرکار خانوم";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریفات عنوان نوع مشتری
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان جنسیت</returns>
        public static string GetTitle(UserType userType)
        {
            switch (userType)
            {
                case UserType.KiaPersonnel:
                    return "پرسنل کیا";
                case UserType.Customer:
                    return "مشتری";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریفات عنوان نوع مشتری
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان جنسیت</returns>
        public static string GetTitle(BuyType buyType)
        {
            switch (buyType)
            {
                case BuyType.BuyAttendance:
                    return "خرید در شعبه";
                case BuyType.BuyOnline:
                    return "خرید غیر حضوری";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// نوع خرید مشتری
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان جنسیت</returns>
        public static string GetTitle(BuyTypeOnline buyTypeOnline)
        {
            switch (buyTypeOnline)
            {
                case BuyTypeOnline.Telephonic:
                    return "تلفنی";
                case BuyTypeOnline.Site:
                    return "سایت";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// نوع خرید مشتری
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان جنسیت</returns>
        public static string GetTitle(BuyTypeSubset buyTypeSubset)
        {
            switch (buyTypeSubset)
            {
                case BuyTypeSubset.Order:
                    return "سفارشی";
                case BuyTypeSubset.Repair:
                    return "تعمیری";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریافت عنوان نوع مشتری
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns></returns>
        public static string GetTitle(TradeType tradeType)
        {
            switch (tradeType)
            {
                case TradeType.Buy:
                    return "خرید";
                case TradeType.Sell:
                    return "فروش";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریافت عنوان نوع حواله
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns></returns>
        public static string GetTitle(RemittanceType remittanceType)
        {
            switch (remittanceType)
            {
                case RemittanceType.Weight:
                    return "وزنی";
                case RemittanceType.Money:
                    return "پولی";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریافت عنوان نوع حواله
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns></returns>
        public static string GetTitle(PurchaseType purchaseType)
        {
            switch (purchaseType)
            {
                case PurchaseType.Buy:
                    return "خرید";
                case PurchaseType.Return:
                    return "مرجوع";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دریافت عنوان نوع کاربران ربات سیستم گزارشات فرئش روزانه
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns></returns>
        public static string GetTitle(BotDailyReportUserType botDailyReportUserType)
        {
            switch (botDailyReportUserType)
            {
                case BotDailyReportUserType.None:
                    return "تعریف نشده";
                case BotDailyReportUserType.Branch:
                    return "شعبه";
                case BotDailyReportUserType.Admin:
                    return "ادمین";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        ///گرفتن عنوان نوع سوالات نطر سنجی
        /// </summary>
        /// <param name="surveyQuestionType"></param>
        /// <returns></returns>
        public static string GetTitle(SurveyQuestionType surveyQuestionType)
        {
            switch (surveyQuestionType)
            {
                case SurveyQuestionType.LNH:
                    return "کم-متوسط-زیاد";
                case SurveyQuestionType.BNG:
                    return "بد-متوسط-خوب";
                case SurveyQuestionType.WNG:
                    return "ضعیف-متوسط-خوب";
                case SurveyQuestionType.BNP:
                    return "بد-متوسط-عالی";
                case SurveyQuestionType.WNP:
                    return "ضعیف-متوسط-عالی";
                case SurveyQuestionType.YN:
                    return "بله-خیر";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// گرفتن عنوان نوع جواب های نظر سنجی
        /// </summary>
        /// <param name="surveyValueType"></param>
        /// <returns></returns>
        public static string GetTitle(SurveyAnswerType surveyValueType)
        {
            switch (surveyValueType)
            {
                case SurveyAnswerType.Low:
                    return "کم";
                case SurveyAnswerType.Bad:
                    return "بد";
                case SurveyAnswerType.Weak:
                    return "ضعیف";
                case SurveyAnswerType.Normal:
                    return "متوسط";
                case SurveyAnswerType.High:
                    return "زیاد";
                case SurveyAnswerType.Good:
                    return "خوب";
                case SurveyAnswerType.Perfect:
                    return "عالی";
                case SurveyAnswerType.Yes:
                    return "بله";
                case SurveyAnswerType.No:
                    return "خیر";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// دسته لندی خرج کار
        /// </summary>
        /// <param name="outerWerkCategory"></param>
        /// <returns></returns>
        public static string GetTitle(OuterWerkCategory outerWerkCategory)
        {
            switch (outerWerkCategory)
            {
                case OuterWerkCategory.Stone:
                    return "سنگی";
                case OuterWerkCategory.Leather:
                    return "چرمی";
                case OuterWerkCategory.Both:
                    return "هردو";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// وضعیت دریافت غذا توسط کاربران لاگین شده به سیستم
        /// </summary>
        /// <param name="FoodCensusValue"></param>
        /// <returns></returns>
        public static string GetTitle(FoodStatus foodStatus)
        {
            switch (foodStatus)
            {
                case FoodStatus.Agreement:
                    return "با خوردن غذا موافقم";
                case FoodStatus.Opposition:
                    return "با دریافت غذا مخالفم";
                case FoodStatus.NoIdea:
                    return "نظری ندارم";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        /// وضعیت دریافت غذا توسط کاربران لاگین شده به سیستم
        /// </summary>
        /// <param name="FoodCensusValue"></param>
        /// <returns></returns>
        public static string GetTitle(InstagramPostType instagramPost)
        {
            switch (instagramPost)
            {
                case InstagramPostType.ProductPost:
                    return "محصول";
                case InstagramPostType.AdvicePost:
                    return "اینستاگرام";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        public static string GetTitle(TicketStatus ticketStatus)
        {
            switch (ticketStatus)
            {
                case TicketStatus.New:
                    return "جدید";
                case TicketStatus.Opened:
                    return "باز";
                case TicketStatus.Checking:
                    return "درحال بررسی";
                case TicketStatus.Closed:
                    return "بسته شده";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        public static string GetTitle(LoyalityCardType loyalityCardType)
        {
            switch (loyalityCardType)
            {
                case LoyalityCardType.Bronze:
                    return "برنز";
                case LoyalityCardType.Silver:
                    return "نقره ای";
                case LoyalityCardType.Gold:
                    return "طلایی";
                case LoyalityCardType.Platinum:
                    return "پلاتینوم";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        public static string GetTitle(CustomerCardLevel customerCardLevel)
        {
            switch (customerCardLevel)
            {
                case CustomerCardLevel.None:
                    return "بدون سطح";
                case CustomerCardLevel.Bronze:
                    return "برنز";
                case CustomerCardLevel.Silver:
                    return "نقره ای";
                case CustomerCardLevel.Gold:
                    return "طلایی";
                case CustomerCardLevel.Platinum:
                    return "پلاتینوم";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        public static string GetTitle(LoyalityCardStatus loyalityCardStatus)
        {
            switch (loyalityCardStatus)
            {
                case LoyalityCardStatus.Register:
                    return "ثبت شده";
                case LoyalityCardStatus.publish:
                    return "چاپ شده";
                case LoyalityCardStatus.SendToBranch:
                    return "ارسال به شعبه";
                case LoyalityCardStatus.Reserved:
                    return "تحویل به مشتری";
                case LoyalityCardStatus.Burn:
                    return "سوخته";
                case LoyalityCardStatus.Destroy:
                    return "باطل شده";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        public static string GetTitle(ShowProduct? showProduct)
        {
            switch (showProduct)
            {
                case ShowProduct.Branch:
                    return "شعبه";
                case ShowProduct.SolicitorShip:
                    return "نمایندگی";
                case ShowProduct.Both:
                    return "هردو";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        public static string GetTitle(InventoryType inventoryType)
        {
            switch (inventoryType)
            {
                case InventoryType.Branch:
                    return "شعبه";
                case InventoryType.Customer:
                    return "مشتری";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        public static string GetTitle(TutorialType tutorialType)
        {
            switch (tutorialType)
            {
                case TutorialType.Video:
                    return "ویدیو";
                case TutorialType.Image:
                    return "عکس";
                case TutorialType.Document:
                    return "سند";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        public static string GetTitle(ProductColor? productColor)
        {
            switch (productColor)
            {
                case ProductColor.Gold:
                    return "گلد";
                case ProductColor.Rosegold:
                    return "رزگلد";
                case ProductColor.White:
                    return "سفید";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        public static string GetTitle(ContractSubject contractSubject)
        {
            switch (contractSubject)
            {
                case ContractSubject.Always:
                    return "دائم";
                case ContractSubject.Temporary:
                    return "موقت";
                case ContractSubject.Certain:
                    return "معین";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        /// دریفات عنوان وضعیت سفارش محصولات مصرفی
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان وضعیت سفارش</returns>
        public static string GetTitle(OrderUsableProductStatus orderUsableProductStatus)
        {
            switch (orderUsableProductStatus)
            {
                case OrderUsableProductStatus.Registered:
                    return "ثبت شده";
                case OrderUsableProductStatus.UnderConstruction:
                    return "در حال ساخت";
                case OrderUsableProductStatus.Sent:
                    return "ارسال شده";
                case OrderUsableProductStatus.OutOfConstruction:
                    return "اتمام ساخت";
                case OrderUsableProductStatus.InPreparation:
                    return "در حال آماده سازی";
                case OrderUsableProductStatus.Cancel:
                    return "لغو شده";
                case OrderUsableProductStatus.ReadyForDelivery:
                    return "آماده تحویل";
                case OrderUsableProductStatus.Ordered:
                    return "سفارش داده شده";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        /// <summary>
        ///  دریافت عنوان وضعیت سفارش محصولات مصرفی برای تاییدیه در چاپخانه
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان وضعیت سفارش</returns>
        public static string GetTitle(PrintingHouseOrderStatus printingHouseOrderStatus)
        {
            switch (printingHouseOrderStatus)
            {
                case PrintingHouseOrderStatus.Open:
                    return "باز";
                case PrintingHouseOrderStatus.HalfOpen:
                    return "نیمه باز";
                case PrintingHouseOrderStatus.Closed:
                    return "بسته شده";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }

        public static string GetTitle(CardTransactionStatus cardTransactionStatus)
        {
            switch (cardTransactionStatus)
            {
                case CardTransactionStatus.Waiting:
                    return "تایید نشده";
                case CardTransactionStatus.Agreed:
                    return "تایید شده";
                case CardTransactionStatus.Deny:
                    return "رد شده"; 
                case CardTransactionStatus.Checkout:
                    return "بازبینی";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        /// <summary>
        ///  دریافت عنوان وضعیت محصولات مصرفی در چاپخانه
        /// </summary>
        /// <param name="deliverType"></param>
        /// <returns>عنوان وضعیت سفارش</returns>
        public static string GetTitle(UsableProductStatus usableProductStatus)
        {
            switch (usableProductStatus)
            {
                case UsableProductStatus.Active:
                    return "فعال";
                case UsableProductStatus.DisabledVisible:
                    return "غیرفعال قابل نمایش";
                case UsableProductStatus.DisabledInvisible:
                    return "غیرفعال غیر قابل نمایش";
                default:
                    throw new Exception("مقدار مورد نظر یافت نشد.");
            }
        }
        public static string GetTitle(CardTransactionDescription? cardTransactionDescription)
        {
            switch (cardTransactionDescription)
            {
                case CardTransactionDescription.Accounting:
                    return "بازگشت اعتبار خرید قبلی";
                case CardTransactionDescription.Crm:
                    return "روابط عمومی";
                case CardTransactionDescription.Other:
                    return "غیر پوز";
                default:
                    throw new Exception("عنوان مورد نظر یافت نشد.");
            }
        }


    }


    /// <summary>
    /// نوع کاربر
    /// 0. کاربر سایت
    /// 1. کاربر شعبه
    /// 2. کاربر کارگاه
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// کاربر سایت
        /// </summary>
        User,
        /// <summary>
        /// کاربر شعبه
        /// </summary>
        Branch,
        /// <summary>
        /// کاربر کارگاه
        /// </summary>
        Workshop,
        /// <summary>
        /// خریدار خرج کار
        /// </summary>
        OrderOuterWerk,
        /// <summary>
        /// کاربر چاپخانه
        /// </summary>
        PrintingHouse,
        /// <summary>
        /// مشتری
        /// </summary>
        Customer,
        /// <summary>
        /// پرسنل کیا
        /// </summary>
        KiaPersonnel,

    }
    /// <summary>
    /// نوع ارسال 
    /// </summary>
    public enum DeliveryType
    {
        /// <summary>
        /// پیک
        /// </summary>
        DeliveryMan,
        /// <summary>
        /// شعبه
        /// </summary>
        Branch,
        /// <summary>
        /// پرسنل کیا
        /// </summary>
        KiaPersonnel,
        /// <summary>
        /// پست
        /// </summary>
        Post,
        /// <summary>
        /// حضوری
        /// </summary>
        Customer
    }

    /// <summary>
    /// نوع محصول
    /// 0. دستبند زنجير طلا
    /// 1. گردنبند
    /// 2. گوشواره
    /// 3. انگشتر
    /// 4. آویز ساعت
    /// 5. سنجاق بچگانه
    /// 6. ست
    /// 7. پابند
    /// 8. آویز
    /// 9. النگو
    /// 10. خرج کار
    /// 11. دستبند چرم
    /// 12. دستبند ریلی
    /// 13. دستبند سنگی
    /// 14. پلاک لیزر
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// دستبند زنجير طلا
        /// </summary>
        GoldBracelet,
        /// <summary>
        /// گردنبند
        /// </summary>
        Necklace,
        /// <summary>
        /// گوشواره
        /// </summary>
        Earring,
        /// <summary>
        /// انگشتر
        /// </summary>
        Ring,
        /// <summary>
        /// آویز ساعت
        /// </summary>
        WatchPendent,
        /// <summary>
        /// سنجاق بچگانه
        /// </summary>
        Brooch,
        /// <summary>
        /// ست
        /// </summary>
        Set,
        /// <summary>
        /// پابند
        /// </summary>
        Anklet,
        /// <summary>
        /// آویز
        /// </summary>
        Pendant,
        /// <summary>
        /// النگو
        /// </summary>
        Bangle,
        /// <summary>
        /// خرج کار
        /// </summary>
        OuterWerk,
        /// <summary>
        /// دستبند چرم
        /// </summary>
        LeatherBracelet,
        /// <summary>
        /// دستبند ریلی
        /// </summary>
        RailBracelet,
        /// <summary>
        /// دستبند سنگی
        /// </summary>
        StoneBracelet,
        /// <summary>
        /// پلاک لیزر
        /// </summary>
        Plaque,
        /// <summary>
        /// آویزساعت2 
        /// </summary>
        WatchPendent2,
        /// <summary>
        /// دستبند نخی
        /// </summary>
        CottonBracelet


    }

    /// <summary>
    /// نوع خرج کار
    /// 0. دو حلقه چه کوچک
    /// 1. دو حلقه چه بزرگ
    /// 2. دو طرف حلقه چه
    /// </summary>
    public enum OuterWerkType
    {
        /// <summary>
        /// دو حلقه چه کوچک
        /// </summary>
        TwoLittleRings,
        /// <summary>
        /// دو حلقه چه بزرگ
        /// </summary>
        TwoBigRings,
        /// <summary>
        /// دو طرف حلقه چه
        /// </summary>
        TwoSidesRing,
        /// <summary>
        /// سایز 4
        /// </summary>
        Size4,
        /// <summary>
        /// سایز 6
        /// </summary>
        Size6,
        /// <summary>
        /// سایز 8
        /// </summary>
        Size8,
        /// <summary>
        /// کوچک
        /// </summary>
        Tiny,
        /// <summary>
        /// متوسط
        /// </summary>
        Medium,
        /// <summary>
        /// بزرگ
        /// </summary>
        Big

    }

    /// <summary>
    /// نوع طلا
    /// 0. هردو
    /// 1. مات
    /// 2. براق
    /// </summary>
    public enum GoldType
    {
        /// <summary>
        /// هردو
        /// </summary>
        Both,
        /// <summary>
        /// مات
        /// </summary>
        Matte,
        /// <summary>
        /// براق
        /// </summary>
        Shiny
    }

    /// <summary>
    /// نوع چرم
    /// 0. لوله ای
    /// 1. دوخت
    /// 2. بافت
    /// 3. ریلی
    /// 4. مردانه
    /// 5. رشته ای
    /// </summary>
    public enum LeatherType
    {
        /// <summary>
        /// لوله ای
        /// </summary>
        Piped,
        /// <summary>
        /// دوخت
        /// </summary>
        Sewing,
        /// <summary>
        /// بافت
        /// </summary>
        Texture,
        /// <summary>
        /// ریلی
        /// </summary>
        Rail,
        /// <summary>
        /// مردانه
        /// </summary>
        Masculine,
        /// <summary>
        /// رشته ای
        /// </summary>
        Rope
    }

    /// <summary>
    /// نوع سنگ
    /// 0. براق
    /// 1. رسوبی
    /// 2. مروارید
    /// 3. اتمی
    /// 4. سنگ دستبند چرمی
    /// </summary>
    public enum StoneType
    {
        /// <summary>
        /// براق
        /// </summary>
        Transparent,
        /// <summary>
        /// رسوبی
        /// </summary>
        Sedimentary,
        /// <summary>
        /// مروارید
        /// </summary>
        Pearl,
        /// <summary>
        /// اتمی
        /// </summary>
        Atomic,
        /// <summary>
        /// سنگ دستبند چرمی
        /// </summary>
        LeatherBraceletStone,
        /// <summary>
        /// دستبند سنگی
        /// </summary>
        BraceletStone
    }

    /// <summary>
    /// نوع فایل
    /// 0. زمینه سفید
    /// 1. سفارش
    /// 2. ربات
    /// 3. سایر
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// زمینه سفید
        /// </summary>
        WhiteBack,
        /// <summary>
        /// سفارش
        /// </summary>
        Order,
        /// <summary>
        /// ربات
        /// </summary>
        Bot,
        /// <summary>
        /// سایر
        /// </summary>
        Other,
        /// <summary>
        /// مدل
        /// </summary>
        ModelImage,
        /// <summary>
        /// دوربین
        /// </summary>
        Camera
    }

    /// <summary>
    /// نوع سفارش
    /// 0. سریع
    /// 1. شخصی سازی شده
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// سریع
        /// </summary>
        None,
        /// <summary>
        /// شخصی سازی شده
        /// </summary>
        Personalization,
        /// <summary>
        /// سفارشی
        /// </summary>
        Customize,
        /// <summary>
        /// تعمیری
        /// </summary>
        Repair

    }
    /// <summary>
    /// نوع محصول سفارشی
    /// </summary>
    public enum OrderTypeForm
    {
        /// <summary>
        /// سفارش ساخت برای محصولات تعریف شده
        /// </summary>
        DefineProduct,
        /// <summary>
        /// سفارش ساخت برای محصولات تعریف نشده
        /// </summary>
        UnDefineProduct,
        /// <summary>
        /// سفارش ساخت برای محصولات 
        /// </summary>
        DesignProduct,
        /// <summary>
        /// تعمیری
        /// </summary>
        RepairProduct,
    }

    /// <summary>
    /// وضعیت سفارش
    /// 0. عادی
    /// 1. معلق
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// عادی
        /// </summary>
        Normal,
        /// <summary>
        /// معلق
        /// </summary>
        Suspension
    }
    /// <summary>
    /// وضعیت گیفت
    /// 0. ثبت شده
    /// 1. ارسال شده به چاپخانه
    /// 2. تحویل از چاپخانه
    /// 3. فروخته شده به شعبه
    /// 4. فروخته شده به مشتری
    /// 5. استفاده شده
    /// 6. لغو شده
    /// 7. سوزاندن
    /// </summary>
    public enum GiftStatus
    {
        /// <summary>
        /// ثبت شده
        /// </summary>
        Registered,
        /// <summary>
        /// ارسال شده به چاپخانه
        /// </summary>
        PrintingHouse,
        /// <summary>
        /// تحویل از چاپخانه
        /// </summary>
        DeliveryFromPrintingHouse,
        /// <summary>
        /// فروخته شده به شعبه/شرکت
        /// </summary>
        SoldToTheBranch,
        /// <summary>
        /// فروخته شده به مشتری
        /// </summary>
        SoldToTheCustomer,
        /// <summary>
        /// استفاده شده
        /// </summary>
        Used,
        /// <summary>
        /// لغو شده
        /// </summary>
        Cancel,
        /// <summary>
        /// سوزاندن
        /// </summary>
        Burn
    }

    /// <summary>
    /// نوع گیفت
    /// 0. کارت هدیه
    /// 1. بن خرید
    /// 2. بن خرید بدون ثبت
    public enum GiftType
    {
        /// <summary>
        /// کارت هدیه
        /// </summary>
        Cart,
        /// <summary>
        /// بن خرید
        /// </summary>
        Check,
        /// <summary>
        /// بن خرید بدون ثبت 
        /// </summary>
        CheckNotRegistered,
        /// <summary>
        /// کارت هدیه بدون ثبت
        /// </summary>
        CartNotRegistered,
        /// <summary>
        /// کارت تخفیف %5 بدون ثبت
        /// </summary>
        UnregisterFivePercentCard,
        /// <summary>
        /// کارت تخفیف 5 درصد
        /// </summary>
        FivePercentCard
    }

    /// <summary>
    /// وضعیت کالای سفارش
    /// 0. ثبت شده
    /// 1. ارسال شده به کارگاه
    /// 2. در حال ساخت
    /// 3. اتمام ساخت
    /// 4. در حال آماده سازی
    /// 5. آماده تحویل
    /// 6. ارسال شده
    /// 7. کسری
    /// 8. سفارش مجدد
    /// 9. لغو شده
    /// 10. ارسال شده به کارگاه دوم/مونتاژ
    /// 11. در حال ساخت در کارگاه دوم/مونتاژ
    /// 12. اتمام ساخت در کارگاه دوم/مونتاژ
    /// </summary>
    public enum OrderDetailStatus
    {
        /// <summary>
        /// ثبت شده
        /// </summary>
        Registered,
        /// <summary>
        /// ارسال شده به کارگاه
        /// </summary>
        InWorkshop,
        /// <summary>
        /// در حال ساخت
        /// </summary>
        UnderConstruction,
        /// <summary>
        /// اتمام ساخت
        /// </summary>
        OutOfConstruction,
        /// <summary>
        /// در حال آماده سازی
        /// </summary>
        InPreparation,
        /// <summary>
        /// آماده تحویل
        /// </summary>
        ReadyForDelivery,
        /// <summary>
        /// ارسال شده
        /// </summary>
        Sent,
        /// <summary>
        /// کسری
        /// </summary>
        Shortage,
        /// <summary>
        /// سفارش مجدد
        /// </summary>
        ShortageOrder,
        /// <summary>
        /// لغو شده
        /// </summary>
        Cancel,
        /// <summary>
        /// ارسال شده به کارگاه دوم/مونتاژ
        /// </summary>
        InWorkshop2,
        /// <summary>
        /// در حال ساخت در کارگاه دوم/مونتاژ
        /// </summary>
        UnderConstruction2,
        /// <summary>
        /// اتمام ساخت در کارگاه دوم/مونتاژ
        /// </summary>
        OutOfConstruction2,
        /// <summary>
        /// سفارش داده شده
        /// </summary>
        Ordered,
    }

    /// <summary>
    /// شکل سنگ
    /// 0. دایره
    /// 1. مارکیز
    /// 2. اشک
    /// 3. مثلث
    /// 4. مربع
    /// 5. مستطیل
    /// 6. بیضی
    /// 7. سایر
    /// </summary>
    public enum StoneShape
    {
        /// <summary>
        /// دایره
        /// </summary>
        Circle,
        /// <summary>
        /// مارکیز
        /// </summary>
        Marquise,
        /// <summary>
        /// اشک
        /// </summary>
        Tear,
        /// <summary>
        /// مثلث
        /// </summary>
        Triangle,
        /// <summary>
        /// مربع
        /// </summary>
        Square,
        /// <summary>
        /// مستطیل
        /// </summary>
        Rectangle,
        /// <summary>
        /// بیضی
        /// </summary>
        Oval,
        /// <summary>
        /// سایر
        /// </summary>
        Other
    }

    /// <summary>
    /// نوع موقعیت جغرافیایی
    /// 0. استان
    /// 1. شهر
    /// </summary>
    public enum LocationType
    {
        /// <summary>
        /// استان
        /// </summary>
        Province,
        /// <summary>
        /// شهر
        /// </summary>
        City
    }

    /// <summary>
    /// جنسیت
    /// </summary>
    public enum Sex
    {
        /// <summary>
        /// هردو
        /// </summary>
        Both,
        /// <summary>
        /// مردانه
        /// </summary>
        Man,
        /// <summary>
        /// زنانه
        /// </summary>
        Woman,
        /// <summary>
        /// کودک
        /// </summary>    
        Kid
    }

    /// <summary>
    /// وضعیت روز در تقویم
    /// </summary>
    public enum CalendarStatus
    {
        /// <summary>
        /// خالی
        /// </summary>
        None,
        /// <summary>
        /// پیش نویس
        /// </summary>
        Draft,
        /// <summary>
        /// ثبت نهایی شده
        /// </summary>
        Submit
    }

    /// <summary>
    /// وضعیت روز در تقویم
    /// </summary>
    public enum BranchesPaymentsStatus
    {
        /// <summary>
        /// خالی
        /// </summary>
        None,
        /// <summary>
        /// پیش نویس
        /// </summary>
        Draft,
        /// <summary>
        /// ثبت نهایی شده
        /// </summary>
        Submit
    }
    /// <summary>
    /// نوع پرداخت
    /// 0. واریزی
    /// 1. مرجوعی
    /// 2. فروش
    /// 3. مرجوعی متفرقه
    /// 4. طلا متفرقه
    /// </summary>
    public enum TypePayments
    {
        /// <summary>
        /// واریزی
        /// </summary>
        Deposits,
        /// <summary>
        /// مرجوعی
        /// </summary>
        Returned,
        /// <summary>
        /// فروش
        /// </summary>
        Sale,
        /// <summary>
        /// مرجوعی متفرقه
        /// </summary>
        DifferentReturns,
        /// <summary>
        /// فروش متفرقه
        /// </summary>
        DifferentSale,
        /// <summary>
        /// مرجوعی طلا متفرقه
        /// </summary>
        DifferentGoldReturns,
        /// <summary>
        /// فروش طلا متفرقه
        /// </summary>
        DifferentGoldSale,
    }

    /// <summary>
    /// نوع متفرقه
    /// 0. کارت هدیه
    /// 1. بن خرید
    /// 2. بن خرید بدون ثبت نام
    /// 3. چرم یک دور
    /// 4. چرم دو دور
    /// 5. طلا ساده
    /// </summary>
    public enum DifferentType
    {
        /// <summary>
        /// کارت هدیه
        /// </summary>
        GiftCart,
        /// <summary>
        /// بن خرید
        /// </summary>
        Check,
        /// <summary>
        /// بن خرید بدون ثبت نام
        /// </summary>
        CheckNotRegistered,
        /// <summary>
        /// چرم یک دور
        /// </summary>
        LeatherOne,
        /// <summary>
        /// چرم دو دور
        /// </summary>
        LeatherTow,
        /// <summary>
        /// طلا ساده
        /// </summary>
        SimpleGold,
        /// <summary>
        /// طلا ساده
        /// </summary>
        Others

    }

    /// <summary>
    /// نوع فایل پرسنل
    /// 0. تصویر
    /// 1. PDF
    /// </summary>
    public enum PersonFileType
    {
        /// <summary>
        /// تصویر
        /// </summary>
        Image,
        /// <summary>
        /// PDF
        /// </summary>
        PDF
    }

    /// <summary>
    /// مقطع تحصیلی
    /// 0. سیکل
    /// 1. دیپلم
    /// 2. کاردانی
    /// 3. کارشناسی
    /// 4. کارشناسی ارشد
    /// 5. دکترا
    /// </summary>
    public enum Education
    {
        /// <summary>
        /// سیکل
        /// </summary>
        Pre_highSchool = 0,
        /// <summary>
        /// دیپلم
        /// </summary>
        Diploma = 2,
        /// <summary>
        /// کاردانی
        /// </summary>
        AssociatedDegree = 3,
        /// <summary>
        /// کارشناسی 
        /// </summary>
        Bachelor = 4,
        /// <summary>
        /// کارشناسی ارشد
        /// </summary>
        Master = 5,
        /// <summary>
        /// دکترا
        /// </summary>
        PhD = 6
    }

    /// <summary>
    /// دسته بندی فایل پرسنل
    /// 0. عکس پرسنلی
    /// 1. فرم قرارداد
    /// </summary>
    public enum PersonFileCategory
    {
        /// <summary>
        /// عکس پرسنلی
        /// </summary>
        PersonalPhoto,
        /// <summary>
        /// فرم قرارداد
        /// </summary>
        ContractForm,
        /// <summary>
        /// پزشکی
        /// </summary>
        Medical,
        /// <summary>
        /// سایر
        /// </summary>
        Other,
    }

    /// <summary>
    /// نوع کاربر توکن
    /// 0. اپلیکیشن
    /// 1. موبایل
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// اپلیکیشن
        /// </summary>
        Application,
        /// <summary>
        /// موبایل
        /// </summary>
        Mobile
    }

    /// <summary>
    /// نوع پرسنلی
    /// 0. دفتر مرکزی
    /// 1. شعبه
    /// 2. کارگاه 
    /// </summary>
    public enum PersonType
    {
        /// <summary>
        /// دفتر مرکزی
        /// </summary>
        CentralOffice,
        /// <summary>
        /// شعبه
        /// </summary>
        Branch,
        /// <summary>
        /// کارگاه
        /// </summary>
        Workshop
    }

    /// <summary>
    /// وضعیت سفارش ربات
    /// 0. هیچ
    /// 1. در انتظار تماس
    /// 2. رد تماس
    /// 3. در انتظار پیش پرداخت
    /// 4. در دست ساخت
    /// 5. در انتظار پرداخت
    /// 6. ارسال
    /// 7. لغو شده
    /// 8. اشاره به
    /// 9. در انتظار مشتری
    /// </summary>
    public enum BotOrderStatus
    {
        /// <summary>
        /// نا مشخص
        /// </summary>
        None,
        /// <summary>
        /// در انتظار تماس
        /// </summary>
        PendingCall,
        /// <summary>
        /// رد تماس
        /// </summary>
        RejectCall,
        /// <summary>
        /// در انتظار پیش پرداخت
        /// </summary>
        PendingPrepayment,
        /// <summary>
        /// در دست ساخت
        /// </summary>
        UnderConstruction,
        /// <summary>
        /// در انتظار پرداخت
        /// </summary>
        PendingPayment,
        /// <summary>
        /// ارسال
        /// </summary>
        Sent,
        /// <summary>
        /// لغو شده
        /// </summary>
        Canceled,
        /// <summary>
        /// اشاره به
        /// </summary>
        ReferredTo,
        /// <summary>
        /// در انتظار مشتری
        /// </summary>
        PendingCustomer
    }

    /// <summary>
    /// نوع فایل ربات
    /// 0. متن
    /// 1. تصویر 
    /// 2. ویدیو
    /// 3. DailyOffer
    /// </summary>
    public enum BotType
    {
        /// <summary>
        /// متن
        /// </summary>
        Text,
        /// <summary>
        /// تصویر
        /// </summary>
        Image,
        /// <summary>
        /// ویدیو
        /// </summary>
        Video,
        /// <summary>
        /// DailyOffer
        /// </summary>
        DailyOffer,
    }

    /// <summary>
    /// نوع شعب
    /// 0. شعب
    /// 1. نمایندگی 
    /// 2. سایر 
    /// 3. همکار 
    /// </summary>
    public enum BranchType
    {
        /// <summary>
        /// شعب
        /// </summary>
        Branch,
        /// <summary>
        /// نمایندگی
        /// </summary>
        Solicitorship,
        /// <summary>
        /// سایر
        /// </summary>
        Other,
        /// <summary>
        /// همکار
        /// </summary>
        CoWorker
    }

    /// <summary>
    /// نوع پاسخ
    /// 0. دارم
    /// 1. ندارم
    /// </summary>
    public enum AnswerType
    {
        /// <summary>
        /// دارم
        /// </summary>
        Have,
        /// <summary>
        /// ندارم
        /// </summary>
        Donthave
    }

    /// <summary>
    /// نوع فاکتور سیستم مشتری
    /// </summary>
    public enum CrmInvoiceType
    {
        /// <summary>
        /// خرید
        /// </summary>
        Purchase,
        /// <summary>
        /// مرجوعی
        /// </summary>
        Revocation
    }

    /// <summary>
    /// جنسیت
    /// </summary>
    public enum CrmSex
    {
        /// <summary>
        /// آقا
        /// </summary>
        Man,
        /// <summary>
        /// خانم
        /// </summary>
        Woman
    }
    /// <summary>
    /// وضعیت سفارشات 
    /// </summary>
    public enum InternalOrderStatus
    {
        /// <summary>
        /// آماده تحویل
        /// </summary>
        ReadyForDeliver,
        /// <summary>
        /// تحویل شده
        /// </summary>
        Delivered,
        /// <summary>
        /// لغو شده
        /// </summary>
        Cancel,
        /// <summary>
        /// خبر میده
        /// </summary>
        PendingCustomer,
        /// <summary>
        /// پاسخ نداد
        /// </summary>
        NoAnswer,
        /// <summary>
        /// ثبت شده
        /// </summary>
        Registered,
        /// <summary>
        /// حذف شده
        /// </summary>
        Deleted,
        /// <summary>
        /// ارسال به شعبه
        /// </summary>
        SendToBranch,
        /// <summary>
        /// ارسال به دفتر مرکزی 
        /// </summary>
        SendToOffice,
        /// <summary>
        /// تحویل از شعبه
        /// </summary>
        AcceptFromBranch,
        /// <summary>
        /// تحویل از دفتر مرکزی
        /// </summary>
        AcceptFromOffice,
        /// <summary>
        /// پردازش
        /// </summary>
        Processing
    }
    /// <summary>
    /// جنسیت
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// مرد
        /// </summary>
        Male,
        /// <summary>
        /// زن
        /// </summary>
        Female
    }
    /// <summary>
    /// نوع پیام
    /// </summary>
    public enum SmsCategoryType
    {
        /// <summary>
        ///چندانتخابی به همراه توضیحات مرتبط
        /// </summary>
        MultiChoice,
        /// <summary>
        ///تک انتخابی بدون توضیحات مرتبط 
        /// </summary>
        SingleChoice,
        /// <summary>
        /// تشریحی
        /// </summary>
        Descriptive,
        /// <summary>
        /// پیام آزاد
        /// </summary>
        FreeMessage,
    }
    /// <summary>
    /// نوع پرسش
    /// </summary>
    public enum CrmQuestionType
    {
        /// <summary>
        /// بله/خیر
        /// </summary>
        YesNo,
        /// <summary>
        /// چندانتخابی
        /// </summary>
        MultiChoice,
        /// <summary>
        ///تک انتخابی
        /// </summary>
        SingleChoice,
        /// <summary>
        /// تشریحی
        /// </summary>
        Descriptive,
    }
    public enum PhotographyStatus
    {
        /// <summary>
        /// ارسال به عکاسی زمینه سفید
        /// </summary>
        ToStudio1,
        /// <summary>
        /// ارسال به عکاسی مدل
        /// </summary>
        ToStudio2,
        /// <summary>
        /// تحویل از عکاسی زمینه سفید
        /// </summary>
        FromStudio1,
        /// <summary>
        /// تحویل از عکاسی مدل
        /// </summary>
        FromStudio2,

    }
    /// <summary>
    /// نوع خرید مشتری
    /// </summary>
    public enum BuyType
    {
        /// <summary>
        /// خرید حضوری
        /// </summary>
        BuyAttendance,
        /// <summary>
        /// خرید آنلاین
        /// </summary>
        BuyOnline,
    }
    /// <summary>
    /// زیرمجموعه نوع خرید
    /// </summary>
    public enum BuyTypeSubset
    {
        /// <summary>
        /// سفارشی
        /// </summary>
        Order,
        /// <summary>
        /// تعمیری
        /// </summary>
        Repair,
    }
    /// <summary>
    /// نوع خرید آنلاین
    /// </summary>
    public enum BuyTypeOnline
    {

        /// <summary>
        /// تلفنی
        /// </summary>
        Telephonic,
        /// <summary>
        /// سایت
        /// </summary>
        Site,
        /// <summary>
        /// حضوری شعبه
        /// </summary>
        None
    }

    public enum TradeType
    {
        /// <summary>
        /// خرید
        /// </summary>
        Buy,
        /// <summary>
        /// فروش
        /// </summary>
        Sell,
    }

    public enum RemittanceType
    {
        /// <summary>
        /// خرید
        /// </summary>
        Weight,
        /// <summary>
        /// فروش
        /// </summary>
        Money,
    }

    public enum PurchaseType
    {
        /// <summary>
        /// خرید
        /// </summary>
        Buy,
        /// <summary>
        /// مرجوع
        /// </summary>
        Return,
    }

    /// <summary>
    /// نوع کاربر سیستم گزارش روزانه که ربات را استارت کرده اند
    /// بدون وضعیت برای کسانی هستند که تازه ربات را استارت کرده اند
    /// شعبه فقط می توااند گزارشات شعبه خود را مشاهده کنند
    /// ادمین برای کسانی که همه گزارشات برای آن ها ارسال میشود
    /// </summary>
    public enum BotDailyReportUserType
    {
        None,
        Branch,
        Admin
    }
    /// <summary>
    /// نوع کارت مشتریان وفادار:
    /// 1.نقرهای
    /// 2.طلایی
    /// 3.پلاتینوم
    /// </summary>
    public enum LoyalityCardType
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }
    /// <summary>
    /// 
    /// </summary>
    public enum CustomerCardLevel
    {
        None,
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    public enum LoyalityCardStatus
    {
        Register,
        publish,
        SendToBranch,
        Reserved,
        Burn,
        Destroy,
    }
    /// <summary>
    /// نوع سوال نظر سنجی
    /// 1.کم-متوسط-زیاد
    /// 2.بد-متوسط-خوب
    /// 3.ضعیف-متوسط-خوب
    /// 4.بد-متوسط-عالی
    /// 5.ضعیف-متوسط-عالی
    /// 6.بله-خیر
    /// </summary>
    public enum SurveyQuestionType
    {
        LNH,
        BNG,
        WNG,
        BNP,
        WNP,
        YN
    }
    /// <summary>
    /// نوع سوال نظر سنجی
    /// 1.کم
    /// 2.بد
    /// 3.ضعیف
    /// 4.متوسط
    /// 5.زیاد
    /// 6.خوب
    /// 7.عالی
    /// 8.بله
    /// 9.خیر
    /// </summary>
    public enum SurveyAnswerType
    {
        Low,
        Bad,
        Weak,
        Normal,
        High,
        Good,
        Perfect,
        Yes,
        No
    }

    public enum PersonPost
    {
        supplies,
        Guard
    }
    /// <summary>
    /// 0.سنگی
    /// 1.چرمی
    /// </summary>
    public enum OuterWerkCategory
    {
        Stone,
        Leather,
        Both
    }

    /// <summary>
    /// وضعیت ثبت غذا توسط کاربران لاگین شده در سیستم 
    /// 0. موافقت حهت دریافت غذا توسط کاربری که در سیستم للاگین شده است
    /// 1.مخالفت حهت دریافت غذا توسط کاربری که در سیستم للاگین شده است
    /// 2.کاربر پاسخی را ثبت نکرده است
    /// </summary>
    public enum FoodStatus
    {
        /// <summary>
        /// موافقت حهت دریافت غذا توسط کاربری که در سیستم للاگین شده است
        /// </summary>
        Agreement,
        /// <summary>
        /// مخالفت حهت دریافت غذا توسط کاربری که در سیستم للاگین شده است
        /// </summary>
        Opposition,
        /// <summary>
        /// کاربر پاسخی را ثبت نکرده است
        /// </summary>
        NoIdea,

    }
    /// <summary>
    /// نوع کاربر توکن
    /// 0. اپلیکیشن
    /// 1. موبایل
    /// </summary>
    public enum InstagramPostType
    {
        /// <summary>
        /// محصول
        /// </summary>
        ProductPost,
        /// <summary>
        /// اینستاگرام
        /// </summary>
        AdvicePost
    }

    public enum TicketStatus
    {
        New,
        Opened,
        Checking,
        Closed
    }

    /// <summary>
    /// وضعیت کالای سفارش
    /// 0. ثبت شده
    /// 1. در حال ساخت
    /// 2. اتمام ساخت
    /// 3. در حال آماده سازی   
    /// 4. آماده تحویل
    /// 5. ارسال شده
    /// 6. لغو شده
    /// 7. سفارش داده شده
    /// </summary>
    public enum OrderUsableProductStatus
    {
        /// <summary>
        /// ثبت شده
        /// </summary>
        Registered,
        /// <summary>
        /// آماده تحویل
        /// </summary>
        ReadyForDelivery,
        /// <summary>
        /// ارسال شده
        /// </summary>
        Sent,
        /// <summary>
        /// در حال ساخت
        /// </summary>
        UnderConstruction,
        /// <summary>
        /// اتمام ساخت
        /// </summary>
        OutOfConstruction,
        /// <summary>
        /// در حال آماده سازی
        /// </summary>
        InPreparation,
        /// <summary>
        /// لغو شده
        /// </summary>
        Cancel,
        /// <summary>
        /// سفارش داده شده
        /// </summary>
        Ordered,
    }
    public enum PrintingHouseOrderStatus
    {
        /// <summary>
        /// باز
        /// </summary>
        Open,
        /// <summary>
        /// نیمه باز
        /// </summary>
        HalfOpen,
        /// <summary>
        /// بسته شده
        /// </summary>
        Closed,
    }
    public enum UsableProductStatus
    {
        Active,
        DisabledVisible,
        DisabledInvisible,
    }
    public enum ShowProduct
    {
        Branch,
        SolicitorShip,
        Both,
    }
    public enum InventoryType
    {
        Branch,
        Customer
    }
    public enum TutorialType
    {
        Video,
        Image,
        Document
    }

    public enum ProductColor
    {
        Gold,
        Rosegold,
        White
    }

    public enum ContractSubject
    {
        Always,
        Temporary,
        Certain
    }

    public enum CardTransactionStatus
    {
        Waiting = 1,
        Agreed = 2,
        Deny = 3,
        Checkout = 4
    }
    public enum CardTransactionType
    {
        Charge,
        Debit
    }

    public enum CardTransactionDescription
    {
        /// <summary>
        /// بازگشت اعتبار خرید قبلی
        /// </summary>
        Accounting,
        /// <summary>
        /// روابط عمومی
        /// </summary>
        Crm,
        /// <summary>
        /// غیر پوز
        /// </summary>
        Other
    }

}

