using KiaGallery.Model.Context.Bot;
using KiaGallery.Model.Context.BotDailyReport;
using KiaGallery.Model.Context.BranchesPayments;
using KiaGallery.Model.Context.DailyReportFinancial;
using KiaGallery.Model.Context.Gift;
using KiaGallery.Model.Context.Order;
using KiaGallery.Model.Context.Post;
using KiaGallery.Model.Context.StoneTable;
using KiaGallery.Model.Context.InternalOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// کاربر
    /// </summary>
    [Table("UserProfile")]
    public class User
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public User()
        {
            CreateBranchFactorList = new List<BranchFactor>();
            ModifyBranchFactorList = new List<BranchFactor>();
            CreateSmsTextList = new List<SmsText>();
            ModifySmsTextList = new List<SmsText>();
            CreateSmsCategoryList = new List<SmsCategory>();
            ModifySmsCategoryList = new List<SmsCategory>();

            CreateCustomerLoyalityList = new List<CustomerLoyality>();
            ModifyCustomerLoyalityList = new List<CustomerLoyality>();

            CreateCustomerFactorList = new List<CustomerFactor>();
            ModifyCustomerFactorList = new List<CustomerFactor>();

            CreateCrmQuestionList = new List<CrmQuestion>();
            ModifyCrmQuestionList = new List<CrmQuestion>();
            CreateCrmCustomerList = new List<CrmCustomer>();
            ModifyCrmCustomerList = new List<CrmCustomer>();
            CreateCrmCustomerAnswerList = new List<CrmCustomerAnswer>();
            ModifyCrmCustomerAnswerList = new List<CrmCustomerAnswer>();
            CreateCategoryQuestionList = new List<CategoryQuestion>();
            ModifyCategoryQuestionList = new List<CategoryQuestion>();
            CrmQuestionValueList = new List<CrmQuestionValue>();

            CreateBranchGoldList = new List<BranchGold>();
            ModifyBranchGoldList = new List<BranchGold>();

            CreateWorkShopGoldList = new List<WorkShopGold>();
            ModifyWorkShopGoldList = new List<WorkShopGold>();

            CreateGoldBalanceList = new List<GoldBalance>();
            ModifyGoldBalanceList = new List<GoldBalance>();

            CreatePhotographyManageList = new List<PhotographyManage>();
            ModifyPhotographyManageList = new List<PhotographyManage>();

            CreateCrmCustomerAnswerList = new List<CrmCustomerAnswer>();
            ModifyCrmCustomerAnswerList = new List<CrmCustomerAnswer>();

            CreateCrmCustomerList = new List<CrmCustomer>();
            ModifyCrmCustomerList = new List<CrmCustomer>();

            CreateCrmQuestionList = new List<CrmQuestion>();
            ModifyCrmQuestionList = new List<CrmQuestion>();

            CrmQuestionValueList = new List<CrmQuestionValue>();
            BankAccountDetailList = new List<BankAccountDetail>();

            CreateBankAccountList = new List<BankAccount>();
            ModifyBankAccountList = new List<BankAccount>();
            CreateInternalOrderList = new List<InternalOrder.InternalOrder>();
            ModifyInternalOrderList = new List<InternalOrder.InternalOrder>();
            InternalOrderLogList = new List<InternalOrderLog>();
            InternalOrderStatusLogList = new List<InternalOrderStatusLog>();
            SentSmsList = new List<SentSms>();
            UserTokenList = new List<UserToken>();
            RoleList = new List<Role>();
            TokenList = new List<AppToken>();
            DailyReportLogList = new List<DailyReportLog>();
            MessageList = new List<Message>();
            CreateUserList = new List<User>();
            ModifyUserList = new List<User>();
            CreateSettingsList = new List<Settings>();
            ModifySettingsList = new List<Settings>();
            CreateMessageList = new List<Message>();
            ModifyMessageList = new List<Message>();
            CreateLeatherList = new List<Leather>();
            ModifyLeatherList = new List<Leather>();
            CreateStoneList = new List<Stone>();
            ModifyStoneList = new List<Stone>();
            CreateLocationList = new List<Location>();
            ModifyLocationList = new List<Location>();
            CreateBranchList = new List<Branch>();
            ModifyBranchList = new List<Branch>();
            CreateWorkshopList = new List<Workshop>();
            ModifyWorkshopList = new List<Workshop>();
            CreateProductList = new List<Product>();
            ModifyProductList = new List<Product>();
            CreateRelatedProductList = new List<RelatedProduct>();
            ModifyRelatedProductList = new List<RelatedProduct>();
            CreateSetProductList = new List<SetProduct>();
            ModifySetProductList = new List<SetProduct>();
            CreateProductFileList = new List<ProductFile>();
            ModifyProductFileList = new List<ProductFile>();
            CreateSizeList = new List<Size>();
            ModifySizeList = new List<Size>();
            CreateSizeValueList = new List<SizeValue>();
            ModifySizeValueList = new List<SizeValue>();
            CreateShapeSizeList = new List<ShapeSize>();
            ModifyShapeSizeList = new List<ShapeSize>();
            CreateCartList = new List<Cart>();
            ModifyCartList = new List<Cart>();
            CreateOrderList = new List<Order.Order>();
            ModifyOrderList = new List<Order.Order>();
            CreateOrderDetailList = new List<OrderDetail>();
            ModifyOrderDetailList = new List<OrderDetail>();
            CreateOrderLogList = new List<OrderLog>();
            CreateOrderDetailLogList = new List<OrderDetailLog>();
            CreateWorkshopOrderList = new List<WorkshopOrder>();
            ModifyWorkshopOrderList = new List<WorkshopOrder>();
            CreateOrderDetailLogReasonList = new List<OrderDetailLogReason>();
            ModifyOrderDetailLogReasonList = new List<OrderDetailLogReason>();
            CreateFavouritesProductList = new List<FavouritesProduct>();
            ModifyFavouritesProductList = new List<FavouritesProduct>();
            CreatePostItemList = new List<PostItem>();
            ModifyPostItemList = new List<PostItem>();
            CreateStoneOutOfStockList = new List<StoneOutOfStock>();
            ModifyStoneOutOfStockList = new List<StoneOutOfStock>();
            CreateBankList = new List<Bank>();
            ModifyBankList = new List<Bank>();
            CreateBranchCalendarList = new List<BranchCalendar>();
            ModifyBranchCalendarList = new List<BranchCalendar>();
            CreateCurrencyList = new List<Currency>();
            ModifyCurrencyList = new List<Currency>();
            CreateBranchNoteList = new List<BranchNote>();
            ModifyBranchNoteList = new List<BranchNote>();
            CreateGiftLogList = new List<GiftLog>();
            CreateOrderUsableProductLogList = new List<OrderUsableProductLog>();
            CreateBranchesPaymentsList = new List<BranchesPayments.BranchesPayments>();
            ModifyBranchesPaymentsList = new List<BranchesPayments.BranchesPayments>();
            ModifyUserDataList = new List<BranchesPayments.UserData>();
            CreatePersonList = new List<Salary.Person>();
            ModifyPersonList = new List<Salary.Person>();
            CreateSalaryList = new List<Salary.Salary>();
            ModifySalaryList = new List<Salary.Salary>();
            CreateBotBroadcastList = new List<Broadcast>();
            CreateBotMessageList = new List<BotMessage>();
            CreateBotNewsList = new List<BotNews>();
            CreateBotOrderLogList = new List<BotOrderLog>();
            CreateBotSettingsList = new List<BotSettings>();
            ModifyBotSettingsList = new List<BotSettings>();
            CreateBranchesPaymentsSettingsList = new List<BranchesPaymentsSettings>();
            ModifyBranchesPaymentsSettingsList = new List<BranchesPaymentsSettings>();
            CreateDailyReportSettingsList = new List<DailyReportSettings>();
            ModifyDailyReportSettingsList = new List<DailyReportSettings>();
            CreateInquiryProductReplyList = new List<InquiryProductReply>();
            CreateInquiryProductList = new List<InquiryProduct>();
            CreateCustomerList = new List<Customer>();
            ModifyCustomerList = new List<Customer>();
            CreateInvoiceList = new List<CrmInvoice>();
            ModifyInvoiceList = new List<CrmInvoice>();
            CreateInvoiceDetailList = new List<CrmInvoiceDetail>();
            ModifyInvoiceDetailList = new List<CrmInvoiceDetail>();
            CreateCrmDiscountSettingList = new List<CrmDiscountSetting>();
            ModifyCrmDiscountSettingList = new List<CrmDiscountSetting>();
            CreateCategoryInventoryReportList = new List<CategoryInventoryReportMember>();
            ModifyCategoryInventoryReportList = new List<CategoryInventoryReportMember>();
            CreateInventoryDetailList = new List<InventoryDetail>();
            ModifyInventoryDetailList = new List<InventoryDetail>();
            CreateInnovationMessageList = new List<CreateMessage>();
            ModifyInnovationMessageList = new List<CreateMessage>();
            CreateUsableProductList = new List<UsableProduct>();
            ModifyUsableProductList = new List<UsableProduct>();
            CreateUsableProductFileList = new List<UsableProductFile>();
            ModifyUsableProductFileList = new List<UsableProductFile>();
            CreateCategoryUsableProductList = new List<CategoryUsableProduct>();
            ModifyCategoryUsableProductList = new List<CategoryUsableProduct>();
            CreatePrintingHouseList = new List<PrintingHouse>();
            ModifyPrintingHouseList = new List<PrintingHouse>();
            CreateOrderUsableProductList = new List<OrderUsableProduct>();
            ModifyOrderUsableProductList = new List<OrderUsableProduct>();
            CreateOrderUsableProductDetailList = new List<OrderUsableProductDetail>();
            ModifyOrderUsableProductDetailList = new List<OrderUsableProductDetail>();
            CreateFaxOrderUsableProductList = new List<FaxOrderUsableProduct>();
            CreateUsableProductCartList = new List<UsableProductCart>();
            ModifyUsableProductCartList = new List<UsableProductCart>();
            CreateInstagramPostList = new List<InstagramPost>();
            ModifyInstagramPostList = new List<InstagramPost>();
            BranchesPaymentsLogList = new List<BranchesPayments.BranchesPaymentsLog>();
            CreateCompanyInvoiceList = new List<CompanyInvoice>();
            ModifyCompanyInvoiceList = new List<CompanyInvoice>();
        }

        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int? BranchId { get; set; }
        /// <summary>
        /// ردیف کارگاه
        /// </summary>
        public int? WorkshopId { get; set; }
        /// <summary>
        /// ردیف چاپخانه
        /// </summary>
        public int? PrintingHouseId { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        [MaxLength(50)]
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [MaxLength(50)]
        public string LastName { get; set; }
        /// <summary>
        /// رنگ
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// نام فایل
        /// </summary>
        [MaxLength(255)]
        public string FileName { get; set; }
        /// <summary>
        /// شماره تلفن
        /// </summary>
        [MaxLength(14)]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// نام کاربری
        /// </summary>
        [MaxLength(50)]
        [Index("Username", IsUnique = true)]
        public string Username { get; set; }
        /// <summary>
        /// کلید ساخت گذرواژه
        /// </summary>
        [MaxLength(255)]
        public string Salt { get; set; }
        /// <summary>
        /// گذرواژه
        /// </summary>
        [MaxLength(1000)]
        public string Password { get; set; }
        /// <summary>
        /// نوع کاربر
        /// </summary>
        public UserType UserType { get; set; }
        /// <summary>
        /// وضعیت
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// کد تایید برای تغییر رمز عبور
        /// </summary>
        public string ConfirmationCode { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int? CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int? ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }
        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        [NotMapped]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        /// <summary>
        /// محل کاربر
        /// </summary>
        [NotMapped]
        public string UserPlace { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// کارگاه
        /// </summary>
        public virtual Workshop Workshop { get; set; }
        /// <summary>
        /// چاپخانه
        /// </summary>
        public virtual PrintingHouse PrintingHouse { get; set; }
        /// <summary>
        /// لیست دسترسی های کاربر
        /// </summary>
        public virtual List<Role> RoleList { get; set; }
        /// <summary>
        /// لیست توکن های صادر شده برای کاربر
        /// </summary>
        public virtual List<AppToken> TokenList { get; set; }
        /// <summary>
        /// لیست گزارشات کاربر
        /// </summary>
        public virtual List<DailyReportLog> DailyReportLogList { get; set; }
        /// <summary>
        /// لیست گزارشات فاکتور شعب
        /// </summary>
        public virtual List<BranchesPaymentsLog> BranchesPaymentsLogList { get; set; }
        /// <summary>
        /// لیست پیام های کاربر
        /// </summary>
        public virtual List<Message> MessageList { get; set; }
        /// <summary>
        /// لیست توکن های کاربر
        /// </summary>
        public virtual List<UserToken> UserTokenList { get; set; }

        /// <summary>
        /// لیست کاربران ایجاد شده
        /// </summary>
        public virtual List<User> CreateUserList { get; set; }
        /// <summary>
        /// لیست کاربران ویرایش شده
        /// </summary>
        public virtual List<User> ModifyUserList { get; set; }
        /// <summary>
        /// لیست تنظیمات ایجاد شده
        /// </summary>
        public virtual List<Settings> CreateSettingsList { get; set; }
        /// <summary>
        /// لیست تنظیمات ویرایش شده
        /// </summary>
        public virtual List<Settings> ModifySettingsList { get; set; }
        /// <summary>
        /// لیست پیام های ایجاد شده
        /// </summary>
        public virtual List<Message> CreateMessageList { get; set; }
        /// <summary>
        /// لیست پیام های ویرایش شده
        /// </summary>
        public virtual List<Message> ModifyMessageList { get; set; }
        /// <summary>
        /// لیست چرم های ایجاد شده
        /// </summary>
        public virtual List<Leather> CreateLeatherList { get; set; }
        /// <summary>
        /// لیست چرم های ویرایش شده
        /// </summary>
        public virtual List<Leather> ModifyLeatherList { get; set; }
        /// <summary>
        /// لیست سنگ های ایجاد شده
        /// </summary>
        public virtual List<Stone> CreateStoneList { get; set; }
        /// <summary>
        /// لیست سنگ های ویرایش شده
        /// </summary>
        public virtual List<Stone> ModifyStoneList { get; set; }
        /// <summary>
        /// لیست شهر های ایجاد شده
        /// </summary>
        public virtual List<Location> CreateLocationList { get; set; }
        /// <summary>
        /// لیست شهر های ویرایش شده
        /// </summary>
        public virtual List<Location> ModifyLocationList { get; set; }
        /// <summary>
        /// لیست شعبه های ایجاد شده
        /// </summary>
        public virtual List<Branch> CreateBranchList { get; set; }
        /// <summary>
        /// لیست شعبه های ویرایش شده
        /// </summary>
        public virtual List<Branch> ModifyBranchList { get; set; }
        /// <summary>
        /// لیست کارگا های ایجاد شده
        /// </summary>
        public virtual List<Workshop> CreateWorkshopList { get; set; }
        /// <summary>
        /// لیست کارگا های ویرایش شده
        /// </summary>
        public virtual List<Workshop> ModifyWorkshopList { get; set; }
        /// <summary>
        /// لیست محصولات ایجاد شده
        /// </summary>
        public virtual List<Product> CreateProductList { get; set; }
        /// <summary>
        /// لیست محصولات ویرایش شده
        /// </summary>
        public virtual List<Product> ModifyProductList { get; set; }
        /// <summary>
        /// لیست محصولات مرتبط ایجاد شده
        /// </summary>
        public virtual List<RelatedProduct> CreateRelatedProductList { get; set; }
        /// <summary>
        /// لیست محصولات مرتبط ویرایش شده
        /// </summary>
        public virtual List<RelatedProduct> ModifyRelatedProductList { get; set; }
        /// <summary>
        /// لیست محصولات ست ایجاد شده
        /// </summary>
        public virtual List<SetProduct> CreateSetProductList { get; set; }
        /// <summary>
        /// لیست محصولات ست ویرایش شده
        /// </summary>
        public virtual List<SetProduct> ModifySetProductList { get; set; }
        /// <summary>
        /// لیست فایل های محصول ایجاد شده
        /// </summary>
        public virtual List<ProductFile> CreateProductFileList { get; set; }
        /// <summary>
        /// لیست فایل های محصول ویرایش شده
        /// </summary>
        public virtual List<ProductFile> ModifyProductFileList { get; set; }
        /// <summary>
        /// لیست سایز های ایجاد شده
        /// </summary>
        public virtual List<Size> CreateSizeList { get; set; }
        /// <summary>
        /// لیست سایز های ویرایش شده
        /// </summary>
        public virtual List<Size> ModifySizeList { get; set; }
        /// <summary>
        /// لیست مفادیر سایز های ایجاد شده
        /// </summary>
        public virtual List<SizeValue> CreateSizeValueList { get; set; }
        /// <summary>
        /// لیست مفادیر سایز های ویرایش شده
        /// </summary>
        public virtual List<SizeValue> ModifySizeValueList { get; set; }
        /// <summary>
        /// لیست مفادیر سایز های اشکال ایجاد شده
        /// </summary>
        public virtual List<ShapeSize> CreateShapeSizeList { get; set; }
        /// <summary>
        /// لیست مفادیر سایز های اشکال ویرایش شده
        /// </summary>
        public virtual List<ShapeSize> ModifyShapeSizeList { get; set; }
        /// <summary>
        /// لیست سبد خرید ایجاد شده
        /// </summary>
        public virtual List<Cart> CreateCartList { get; set; }
        /// <summary>
        /// لیست سبد خرید ویرایش شده
        /// </summary>
        public virtual List<Cart> ModifyCartList { get; set; }
        /// <summary>
        /// لیست سفارشات ایجاد شده
        /// </summary>
        public virtual List<Order.Order> CreateOrderList { get; set; }
        /// <summary>
        /// لیست سفارشات ویرایش شده
        /// </summary>
        public virtual List<Order.Order> ModifyOrderList { get; set; }
        /// <summary>
        /// لیست جزئیات سفارش ایجاد شده
        /// </summary>
        public virtual List<OrderDetail> CreateOrderDetailList { get; set; }
        /// <summary>
        /// لیست جزئیات سفارش ویرایش شده
        /// </summary>
        public virtual List<OrderDetail> ModifyOrderDetailList { get; set; }
        /// <summary>
        /// لیست سوابق های سفارش ایجاد شده
        /// </summary>
        public virtual List<OrderLog> CreateOrderLogList { get; set; }
        /// <summary>
        /// لیست سوابق های محصولات داخل سفارش ایجاد شده
        /// </summary>
        public virtual List<OrderDetailLog> CreateOrderDetailLogList { get; set; }
        /// <summary>
        /// لیست سفارشات شعب ایجاد شده
        /// </summary>
        public virtual List<WorkshopOrder> CreateWorkshopOrderList { get; set; }
        /// <summary>
        /// لیست سفارشات شعب ویرایش شده
        /// </summary>
        public virtual List<WorkshopOrder> ModifyWorkshopOrderList { get; set; }
        /// <summary>
        /// لیست علت های محصولات درون سفارش ایجاد شده
        /// </summary>
        public virtual List<OrderDetailLogReason> CreateOrderDetailLogReasonList { get; set; }
        /// <summary>
        /// لیست علت های محصولات درون سفارش ویرایش شده
        /// </summary>
        public virtual List<OrderDetailLogReason> ModifyOrderDetailLogReasonList { get; set; }
        /// <summary>
        /// لیست رکورد های ایجاد شده برای بسته های ارسالی توسط پست
        /// </summary>
        public virtual List<PostItem> CreatePostItemList { get; set; }
        /// <summary>
        /// لیست رکورد های ویرایش شده برای بسته های ارسالی توسط پست
        /// </summary>
        public virtual List<PostItem> ModifyPostItemList { get; set; }
        /// <summary>
        /// لیست رکورد های ایجاد شده برای برای محصولات مورد علاقه شعبه
        /// </summary>
        public virtual List<FavouritesProduct> CreateFavouritesProductList { get; set; }
        /// <summary>
        /// لیست رکورد های ویرایش شده برای محصولات مورد علاقه شعبه
        /// </summary>
        public virtual List<FavouritesProduct> ModifyFavouritesProductList { get; set; }
        /// <summary>
        /// لیست محصولات ایجاد شده برای خارج از دسترس بودن سنگ
        /// </summary>
        public virtual List<StoneOutOfStock> CreateStoneOutOfStockList { get; set; }
        /// <summary>
        /// لیست محصولات ویرایش شده برای خارج از دسترس بودن سنگ
        /// </summary>
        public virtual List<StoneOutOfStock> ModifyStoneOutOfStockList { get; set; }
        /// <summary>
        /// لیست بانک های ایجاد شده برای گزارش روزانه شعبه
        /// </summary>
        public virtual List<Bank> CreateBankList { get; set; }
        /// <summary>
        /// لیست بانک های ویرایش شده برای گزارش روزانه شعبه
        /// </summary>
        public virtual List<Bank> ModifyBankList { get; set; }
        /// <summary>
        /// لیست تقویم شعبه ایجاد شده برای گزارش روزانه شعبه
        /// </summary>
        public virtual List<BranchCalendar> CreateBranchCalendarList { get; set; }
        /// <summary>
        /// لیست تقویم شعبه ویرایش شده برای گزارش روزانه شعبه
        /// </summary>
        public virtual List<BranchCalendar> ModifyBranchCalendarList { get; set; }
        /// <summary>
        /// لیست ارز های ایجاد شده برای گزارش روزانه شعبه
        /// </summary>
        public virtual List<Currency> CreateCurrencyList { get; set; }
        /// <summary>
        /// لیست ارز های ویرایش شده برای گزارش روزانه شعبه
        /// </summary>
        public virtual List<Currency> ModifyCurrencyList { get; set; }

        /// <summary>
        /// لیست یاداشت شعبه ایجاد شده
        /// </summary>
        public virtual List<BranchNote> CreateBranchNoteList { get; set; }
        /// <summary>
        /// لیست یاداشت شعبه ویرایش شده
        /// </summary>
        public virtual List<BranchNote> ModifyBranchNoteList { get; set; }
        /// <summary>
        /// لیست سوابق گیفت ایجاد شده
        /// </summary>
        public virtual List<GiftLog> CreateGiftLogList { get; set; }
        /// <summary>
        /// لیست سوابق سفارش محصولات مصرفی
        /// </summary>
        public virtual List<OrderUsableProductLog> CreateOrderUsableProductLogList { get; set; }
        /// <summary>
        /// لیست پرداخت شعب ویرایش شده
        /// </summary>
        public virtual List<BranchesPayments.BranchesPayments> ModifyBranchesPaymentsList { get; set; }
        /// <summary>
        /// لیست پرداخت شعب ایجاد شده
        /// </summary>
        public virtual List<BranchesPayments.BranchesPayments> CreateBranchesPaymentsList { get; set; }
        /// <summary>
        /// لیست کاربران ویراش شده ربات پرداخت شعب   
        /// </summary>
        public virtual List<BranchesPayments.UserData> ModifyUserDataList { get; set; }

        /// <summary>
        /// لیست پرسنل ایجاد شده
        /// </summary>
        public virtual List<Salary.Person> CreatePersonList { get; set; }
        /// <summary>
        /// لیست پرسنل ویرایش شده
        /// </summary>
        public virtual List<Salary.Person> ModifyPersonList { get; set; }
        /// <summary>
        /// لیست پرسنل ایجاد شده
        /// </summary>
        public virtual List<Salary.Salary> CreateSalaryList { get; set; }
        /// <summary>
        /// لیست پرسنل ویرایش شده
        /// </summary>
        public virtual List<Salary.Salary> ModifySalaryList { get; set; }
        /// <summary>
        /// لیست پیام عمومی ربات
        /// </summary>
        public virtual List<Bot.Broadcast> CreateBotBroadcastList { get; set; }
        /// <summary>
        /// لیست پیام ربات
        /// </summary>
        public virtual List<BotMessage> CreateBotMessageList { get; set; }
        /// <summary>
        /// لیست اخبار ربات
        /// </summary>
        public virtual List<BotNews> CreateBotNewsList { get; set; }
        /// <summary>
        /// لیست سفارشات ربات
        /// </summary>
        public virtual List<BotOrderLog> CreateBotOrderLogList { get; set; }
        /// <summary>
        /// لیست تنظیمات ربات
        /// </summary>
        public virtual List<BotSettings> CreateBotSettingsList { get; set; }
        /// <summary>
        ///  لیست اخبار ربات ویرایش شده
        /// </summary>
        public virtual List<BotSettings> ModifyBotSettingsList { get; set; }

        /// <summary>
        /// لیست تنظیمات ربات مالی شعب ایجاد شده
        /// </summary>
        public virtual List<BranchesPaymentsSettings> CreateBranchesPaymentsSettingsList { get; set; }
        /// <summary>
        /// لیست تنظیمات ربات مالی شعب ویرایش شده
        /// </summary>
        public virtual List<BranchesPaymentsSettings> ModifyBranchesPaymentsSettingsList { get; set; }

        /// <summary>
        /// لیست تنظیمات ربات مالی شعب ایجاد شده
        /// </summary>
        public virtual List<DailyReportSettings> CreateDailyReportSettingsList { get; set; }
        /// <summary>
        /// لیست تنظیمات ربات مالی شعب ویرایش شده
        /// </summary>
        public virtual List<DailyReportSettings> ModifyDailyReportSettingsList { get; set; }
        /// <summary>
        /// لیست پاسخ ایجاد شده پیام ها
        /// </summary>
        public virtual List<InquiryProductReply> CreateInquiryProductReplyList { get; set; }
        /// <summary>
        /// لیست استعلام محصول
        /// </summary>
        public virtual List<InquiryProduct> CreateInquiryProductList { get; set; }
        /// <summary>
        /// لیست مشتری ایجاد شده
        /// </summary>
        public virtual List<Customer> CreateCustomerList { get; set; }
        /// <summary>
        /// لیست مشتری ویرایش شده
        /// </summary>
        public virtual List<Customer> ModifyCustomerList { get; set; }
        /// <summary>
        /// لیست فاکتور ایجاد شده
        /// </summary>
        public virtual List<CrmInvoice> CreateInvoiceList { get; set; }
        /// <summary>
        /// لیست فاکتور ویرایش شده
        /// </summary>
        public virtual List<CrmInvoice> ModifyInvoiceList { get; set; }
        /// <summary>
        /// لیست جزئیات فاکتور ایجاد شده
        /// </summary>
        public virtual List<CrmInvoiceDetail> CreateInvoiceDetailList { get; set; }
        /// <summary>
        /// لیست جزئیات فاکتور ویرایش شده
        /// </summary>
        public virtual List<CrmInvoiceDetail> ModifyInvoiceDetailList { get; set; }
        /// <summary>
        /// لیست تنظیمات تخفیفات مشتری ایجاد شده
        /// </summary>
        public virtual List<CrmDiscountSetting> CreateCrmDiscountSettingList { get; set; }
        /// <summary>
        /// لیست تنظیمات تخفیفات مشتری ویرایش شده
        /// </summary>
        public virtual List<CrmDiscountSetting> ModifyCrmDiscountSettingList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده سفارش داخلی
        /// </summary>
        public virtual List<InternalOrder.InternalOrder> CreateInternalOrderList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده سفارش داخلی
        /// </summary>
        public virtual List<InternalOrder.InternalOrder> ModifyInternalOrderList { get; set; }
        /// <summary>
        /// لیست سوابق سفارش مربوط به هر مشتری
        /// </summary>
        public virtual List<InternalOrderLog> InternalOrderLogList { get; set; }
        /// <summary>
        /// لیست سوابق وضعیت سفارش داخلی
        /// </summary>
        public virtual List<InternalOrderStatusLog> InternalOrderStatusLogList { get; set; }
        /// <summary>
        /// لیست پیام ها
        /// </summary>
        public virtual List<SentSms> SentSmsList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده حساب های بانکی
        /// </summary>
        public virtual List<BankAccount> CreateBankAccountList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده حساب های بانکی
        /// </summary>
        public virtual List<BankAccount> ModifyBankAccountList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual List<BankAccountDetail> BankAccountDetailList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده سوالات
        /// </summary>
        public virtual List<CrmQuestion> CreateCrmQuestionList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده سوالات
        /// </summary>
        public virtual List<CrmQuestion> ModifyCrmQuestionList { get; set; }
        /// <summary>
        /// لیست مقادیر سوالات
        /// </summary>
        public virtual List<CrmQuestionValue> CrmQuestionValueList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده دسته بندی سوالات
        /// </summary>
        public virtual List<CategoryQuestion> CreateCategoryQuestionList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده دسته بندی سوالات
        /// </summary>
        public virtual List<CategoryQuestion> ModifyCategoryQuestionList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده مشتری
        /// </summary>
        public virtual List<CrmCustomer> CreateCrmCustomerList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده مشتری
        /// </summary>
        public virtual List<CrmCustomer> ModifyCrmCustomerList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده پاسخ مشتری
        /// </summary>
        public virtual List<CrmCustomerAnswer> CreateCrmCustomerAnswerList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده پاسخ مشتری
        /// </summary>
        public virtual List<CrmCustomerAnswer> ModifyCrmCustomerAnswerList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده مدیریت عکس
        /// </summary>
        public virtual List<PhotographyManage> CreatePhotographyManageList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده مدیریت عکس
        /// </summary>
        public virtual List<PhotographyManage> ModifyPhotographyManageList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده خرید و فروش طلا 
        /// </summary>
        public virtual List<GoldBalance> CreateGoldBalanceList { get; set; }
        /// <summary>
        ///لیست ویرایش کننده خرید و فروش طلا 
        /// </summary>
        public virtual List<GoldBalance> ModifyGoldBalanceList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده طلای کارگاه ها
        /// </summary>
        public virtual List<WorkShopGold> CreateWorkShopGoldList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده طلای کارگاه ها
        /// </summary>
        public virtual List<WorkShopGold> ModifyWorkShopGoldList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده طلای شعبه ها
        /// </summary>
        public virtual List<BranchGold> CreateBranchGoldList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده طلای شعبه ها
        /// </summary>
        public virtual List<BranchGold> ModifyBranchGoldList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده مشتریان وفادار 
        /// </summary>
        public virtual List<CustomerFactor> CreateCustomerFactorList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده مشتریان وفادار 
        /// </summary>
        public virtual List<CustomerFactor> ModifyCustomerFactorList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده وفاداری مشتریان 
        /// </summary>
        public virtual List<CustomerLoyality> CreateCustomerLoyalityList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده وفاداری مشتریان 
        /// </summary>
        public virtual List<CustomerLoyality> ModifyCustomerLoyalityList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده مشتریان وفادار 
        /// </summary>
        public virtual List<SmsText> CreateSmsTextList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده مشتریان وفادار 
        /// </summary>
        public virtual List<SmsText> ModifySmsTextList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده وفاداری مشتریان 
        /// </summary>
        public virtual List<SmsCategory> CreateSmsCategoryList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده وفاداری مشتریان 
        /// </summary>
        public virtual List<SmsCategory> ModifySmsCategoryList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده فاکتور شعب
        /// </summary>
        public virtual List<BranchFactor> CreateBranchFactorList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده فاکتور شعب
        /// </summary>
        public virtual List<BranchFactor> ModifyBranchFactorList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده عناوین گزارش روزانه
        /// </summary>
        public virtual List<CategoryInventoryReportMember> CreateCategoryInventoryReportList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده عناوین گزارش روزانه
        /// </summary>
        public virtual List<CategoryInventoryReportMember> ModifyCategoryInventoryReportList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده جزئیات گزارش روزانه
        /// </summary>
        public virtual List<InventoryDetail> CreateInventoryDetailList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده جزئیات گزارش روزانه
        /// </summary>
        public virtual List<InventoryDetail> ModifyInventoryDetailList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده پیام های ایجاده
        /// </summary>
        public virtual List<CreateMessage> CreateInnovationMessageList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده پیام های ایجاد شده
        /// </summary>
        public virtual List<CreateMessage> ModifyInnovationMessageList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده محصول مصرفی
        /// </summary>
        public virtual List<UsableProduct> CreateUsableProductList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده محصول مصرفی
        /// </summary>
        public virtual List<UsableProduct> ModifyUsableProductList { get; set; }
        /// <summary>
        /// لیست ایجادکننده سفارش محصولات مصرفی
        /// </summary>
        public virtual List<OrderUsableProduct> CreateOrderUsableProductList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده سفارش محصولات مصرفی
        /// </summary>
        public virtual List<OrderUsableProduct> ModifyOrderUsableProductList { get; set; }
        /// <summary>
        /// لیست ایجادکننده دسته بندی محصولات مصرفی
        /// </summary>
        public virtual List<CategoryUsableProduct> CreateCategoryUsableProductList { get; set; }
        /// <summary>
        /// لیست ایجادکننده فایل محصولات مصرفی
        /// </summary>
        public virtual List<UsableProductFile> CreateUsableProductFileList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده فایل محصولات مصرفی
        /// </summary>
        public virtual List<UsableProductFile> ModifyUsableProductFileList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده دسته بندی  محصولات مصرفی
        /// </summary>
        public virtual List<CategoryUsableProduct> ModifyCategoryUsableProductList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده چاپخانه
        /// </summary>
        public virtual List<PrintingHouse> CreatePrintingHouseList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده چاپخانه
        /// </summary>
        public virtual List<PrintingHouse> ModifyPrintingHouseList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده جزئیات سفارش محصولات مصرفی
        /// </summary>
        public virtual List<OrderUsableProductDetail> CreateOrderUsableProductDetailList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده جزئیات سفارش محصولات مصرفی
        /// </summary>
        public virtual List<OrderUsableProductDetail> ModifyOrderUsableProductDetailList { get; set; }
        /// <summary>
        /// لیست ایجادکننده ارسال فکس سفارش محصولات مصرفی
        /// </summary>
        public virtual List<FaxOrderUsableProduct> CreateFaxOrderUsableProductList { get; set; }
        /// <summary>
        /// لیست ایجادکننده سبد خرید برای محصولات مصرفی یا محصول چاپخانه
        /// </summary>
        public virtual List<UsableProductCart> CreateUsableProductCartList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده سبد خرید برای محصولات مصرفی یا محصول چاپخانه
        /// </summary>
        public virtual List<UsableProductCart> ModifyUsableProductCartList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده سرشماری غذا برای استعلام تعداد افراد ثبت شده برای دریافت غذا
        /// </summary>
        public virtual List<FoodCensus> CreateFoodCensusList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده سرشماری غذا برای استعلام تعداد افراد ثبت شده برای دریافت غذا
        /// </summary>
        public virtual List<FoodCensus> ModifyFoodCensusList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده غذا برای ثبت اینکه کاربر سیستم غذا میخورد یا خیر
        /// </summary>
        public virtual List<FoodRegistration> CreateFoodRegistrationList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده غذا برای ثبت اینکه کاربر سیستم غذا میخورد یا خیر
        /// </summary>
        public virtual List<FoodRegistration> ModifyFoodRegistrationList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده تنظیمات برای غذا
        /// </summary>
        public virtual List<FoodSetting> CreateFoodSettingsList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده تنظیمات برای غذا
        /// </summary>
        public virtual List<FoodSetting> ModifyFoodSettingsList { get; set; }
        /// <summary>
        /// لیست ایجاد کننده مدل تصاویر محصولات برای انتخاب و سفارش محصول توسط شعب 
        /// </summary>
        public virtual List<InstagramPost> CreateInstagramPostList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده مدل تصاویر محصولات برای انتخاب و سفارش محصول توسط شعب 
        /// </summary>
        public virtual List<InstagramPost> ModifyInstagramPostList { get; set; }
        /// <summary>
        /// لیست ایجادکننده فاکتور حقوقی یا شرکتی
        /// </summary>
        public virtual List<CompanyInvoice> CreateCompanyInvoiceList { get; set; }
        /// <summary>
        /// لیست ویرایش کننده فاکتور حقوقی یا شرکتی
        /// </summary>
        public virtual List<CompanyInvoice> ModifyCompanyInvoiceList { get; set; }
    }
}
