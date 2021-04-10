using KiaGallery.Model.Context.DailyReportFinancial;
using KiaGallery.Model.Context.Order;
using KiaGallery.Model.Context.Post;
using KiaGallery.Model.Context.Gift;
using KiaGallery.Model.Context.StoneTable;
using System.Data.Entity;
using KiaGallery.Model.Context.Salary;
using KiaGallery.Model.Context.Bot;
using KiaGallery.Model.Context.BranchesPayments;
using KiaGallery.Model.Context.BotDailyReport;
using KiaGallery.Model.Context.InternalOrder;
using KiaGallery.Model.Context.FileManagement;

namespace KiaGallery.Model.Context
{
    /// <summary>
    /// کلاس برای ارتباط با دیتابیس گالری کیا
    /// </summary>
    public class KiaGalleryContext : DbContext
    {
        public KiaGalleryContext() : base("KiaGallery")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<KiaGalleryContext>());
        }

        // Core
        /// <summary>
        /// کاربران
        /// </summary>
        public DbSet<User> User { get; set; }
        /// <summary>
        /// دسترسی
        /// </summary>
        public DbSet<Role> Role { get; set; }
        /// <summary>
        /// تنظیمات
        /// </summary>
        public DbSet<Settings> Settings { get; set; }
        /// <summary>
        /// پیام ها
        /// </summary>
        public DbSet<Message> Message { get; set; }
        /// <summary>
        /// پاسخ استعلام محصول
        /// </summary>
        public DbSet<InquiryProductReply> InquiryProductReply { get; set; }
        /// <summary>
        /// استعلام محصول
        /// </summary>
        public DbSet<InquiryProduct> InquiryProduct { get; set; }
        /// <summary>
        /// چرم
        /// </summary>
        public DbSet<Leather> Leather { get; set; }
        /// <summary>
        /// سنگ
        /// </summary>
        public DbSet<Stone> Stone { get; set; }
        /// <summary>
        /// شهر
        /// </summary>
        public DbSet<Location> Location { get; set; }
        /// <summary>
        /// شعب
        /// </summary>
        public DbSet<Branch> Branch { get; set; }
        /// <summary>
        /// کارگاه
        /// </summary>
        public DbSet<Workshop> Workshop { get; set; }
        /// <summary>
        /// محصولات
        /// </summary>
        public DbSet<Product> Product { get; set; }
        /// <summary>
        /// محصولات مرتبط
        /// </summary>
        public DbSet<RelatedProduct> RelatedProduct { get; set; }
        /// <summary>
        /// محصولات ست
        /// </summary>
        public DbSet<SetProduct> SetProduct { get; set; }
        /// <summary>
        /// فایل محصولات
        /// </summary>
        public DbSet<ProductFile> ProductFile { get; set; }
        /// <summary>
        /// چرم محصول
        /// </summary>
        public DbSet<ProductLeather> ProductLeather { get; set; }
        /// <summary>
        /// سنگ محصول
        /// </summary>
        public DbSet<ProductStone> ProductStone { get; set; }
        /// <summary>
        /// سایز
        /// </summary>
        public DbSet<Size> Size { get; set; }
        /// <summary>
        /// مقادیر سایز
        /// </summary>
        public DbSet<SizeValue> SizeValue { get; set; }
        /// <summary>
        /// سایز اشکال
        /// </summary>
        public DbSet<ShapeSize> ShapeSize { get; set; }

        // Order
        /// <summary>
        /// سبد خرید
        /// </summary>
        public DbSet<Cart> Cart { get; set; }
        /// <summary>
        /// سنگ محصول سبد خرید
        /// </summary>
        public DbSet<CartProductStone> CartProductStone { get; set; }
        /// <summary>
        /// چرم محصول سبد خرید
        /// </summary>
        public DbSet<CartProductLeather> CartProductLeather { get; set; }
        /// <summary>
        /// سفارش
        /// </summary>
        public DbSet<Order.Order> Order { get; set; }
        /// <summary>
        /// سوابق سفارش
        /// </summary>
        public DbSet<OrderLog> OrderLog { get; set; }
        /// <summary>
        /// سفارش کارگاه
        /// </summary>
        public DbSet<WorkshopOrder> WorkshopOrder { get; set; }
        /// <summary>
        /// جزئیات سفارش
        /// </summary>
        public DbSet<OrderDetail> OrderDetail { get; set; }
        /// <summary>
        /// سوابق جزئیات سفارش
        /// </summary>
        public DbSet<OrderDetailLog> OrderDetailLog { get; set; }
        /// <summary>
        /// سنگ های محصولات سفارش
        /// </summary>
        public DbSet<OrderDetailStone> OrderDetailStone { get; set; }
        /// <summary>
        /// چرم های محصولات سفارش
        /// </summary>
        public DbSet<OrderDetailLeather> OrderDetailLeather { get; set; }
        /// <summary>
        /// جزئیات علت های سابقه
        /// </summary>
        public DbSet<OrderDetailLogReason> OrderDetailLogReason { get; set; }
        /// <summary>
        /// لیست محصولات مورد علاقه شعبه
        /// </summary>
        public DbSet<FavouritesProduct> FavouritesProduct { get; set; }

        // Daily Report Financial
        /// <summary>
        /// نوکن ها
        /// </summary>
        public DbSet<AppToken> Token { get; set; }
        /// <summary>
        /// بانک
        /// </summary>
        public DbSet<Bank> Bank { get; set; }
        /// <summary>
        /// ارز
        /// </summary>
        public DbSet<Currency> Currency { get; set; }
        /// <summary>
        /// گزارش روزانه
        /// </summary>
        public DbSet<DailyReport> DailyReport { get; set; }
        /// <summary>
        /// بانک های گزارش روزانه
        /// </summary>
        public DbSet<DailyReportBank> DailyReportBank { get; set; }
        /// <summary>
        /// واحد های پول گزارش روزانه
        /// </summary>
        public DbSet<DailyReportCurrency> DailyReportCurrency { get; set; }
        /// <summary>
        /// سوابق گزارش روزانه
        /// </summary>
        public DbSet<DailyReportLog> DailyReportLog { get; set; }
        /// <summary>
        /// تقویم شعبه
        /// </summary>
        public DbSet<BranchCalendar> BranchCalendar { get; set; }

        // CRM
        /// <summary>
        /// مشتری
        /// </summary>
        public DbSet<Customer> Customer { get; set; }
        /// <summary>
        /// فاکتور های مشتری
        /// </summary>
        public DbSet<CrmInvoice> CrmInvoice { get; set; }
        /// <summary>
        /// جزئیات فاکتور های مشتری
        /// </summary>
        public DbSet<CrmInvoiceDetail> CrmInvoiceDetail { get; set; }
        /// <summary>
        /// تنظیمات تخفیفات فاکتور مشتری
        /// </summary>
        public DbSet<CrmDiscountSetting> CrmDiscountSetting { get; set; }

        // Post
        /// <summary>
        /// مرسوله پست شده
        /// </summary>
        public DbSet<PostItem> PostItem { get; set; }

        // StoneTable
        /// <summary>
        /// سنگ های به اتمام رسیده
        /// </summary>
        public DbSet<StoneOutOfStock> StoneOutOfStock { get; set; }

        /// <summary>
        /// یاداشت شعبه
        /// </summary>
        public DbSet<BranchNote> BranchNote { get; set; }
        /// <summary>
        /// گیفت
        /// </summary>
        public DbSet<Gift.Gift> Gift { get; set; }
        /// <summary>
        /// سوابق گیفت
        /// </summary>
        public DbSet<GiftLog> GiftLog { get; set; }
        /// <summary>
        /// خرج کار محصول
        /// </summary>
        public DbSet<ProductOuterWerk> ProductOuterWerk { get; set; }
        /// <summary>
        /// پرداخت شعب
        /// </summary>
        public DbSet<BranchesPayments.BranchesPayments> BranchesPayments { get; set; }
        /// <summary>
        /// تاریخچه وضعیت فاکتور شعب
        /// </summary>
        public DbSet<BranchesPayments.BranchesPaymentsLog> BranchesPaymentsLog { get; set; }
        /// <summary>
        /// کاربران ربات فاکتور پرداخت شعب 
        /// </summary>
        public DbSet<BranchesPayments.UserData> UserData { get; set; }
        /// <summary>
        /// پرسنل
        /// </summary>
        public DbSet<Person> Person { get; set; }
        /// <summary>
        /// حقوق دستمزد
        /// </summary>
        public DbSet<Salary.Salary> Salary { get; set; }
        /// <summary>
        /// فایل پرسنل
        /// </summary>
        public DbSet<PersonFile> PersonFile { get; set; }
        /// <summary>
        /// سفارشات ربات
        /// </summary>
        public DbSet<BotOrder> BotOrder { get; set; }
        /// <summary>
        /// تنظیمات ربات
        /// </summary>
        public DbSet<BotSettings> BotSettings { get; set; }
        /// <summary>
        /// اخبار ربات
        /// </summary>
        public DbSet<BotNews> BotNews { get; set; }
        /// <summary>
        /// پیام عمومی ربات
        /// </summary>
        public DbSet<Broadcast> BotBroadcast { get; set; }
        /// <summary>
        /// اینستاگرام ربات
        /// </summary>
        public DbSet<Instagram> BotInstagram { get; set; }
        /// <summary>
        /// پیام ربات
        /// </summary>
        public DbSet<BotMessage> BotMessage { get; set; }
        /// <summary>
        /// پاسخ پیام ربات
        /// </summary>
        public DbSet<Replay> BotReplay { get; set; }
        /// <summary>
        /// کاربران ربات
        /// </summary>
        public DbSet<BotUserData> BotUserData { get; set; }
        /// <summary>
        /// لاگ سفارشات 
        /// </summary>
        public DbSet<BotOrderLog> BotOrderLog { get; set; }
        /// <summary>
        /// سنگ های جزئیات محصول سفارش ربات
        /// </summary>
        public DbSet<BotOrderStone> BotOrderStone { get; set; }
        /// <summary>
        /// چرم های محصول ربات سفارش
        /// </summary>
        public DbSet<BotOrderLeather> BotOrderLeather { get; set; }
        /// <summary>
        /// شرکت
        /// </summary>
        public DbSet<Company> Company { get; set; }
        /// <summary>
        /// پیام های ربات پرداخت شعب
        /// </summary>
        public DbSet<BranchesPaymentsMessage> BranchesPaymentsMessage { get; set; }
        /// <summary>
        /// پیام های ربات پرداخت شعب
        /// </summary>
        public DbSet<BranchesPaymentsSendMessage> BranchesPaymentsSendMessage { get; set; }
        /// <summary>
        ///  جزئیات ربات پرداخت شعب
        /// </summary>
        public DbSet<BranchesPaymentsDetails> BranchesPaymentsDetails { get; set; }
        /// <summary>
        /// تنظیمات ربات پرداخت شعب
        /// </summary>
        public DbSet<BranchesPaymentsSettings> BranchesPaymentsSettings { get; set; }

        /// <summary>
        /// پیام های ربات گزارشات شعب
        /// </summary>
        public DbSet<DailyReportMessage> DailyReportMessage { get; set; }

        /// <summary>
        /// کاربران ربات گزارشات شعب
        /// </summary>
        public DbSet<BotDailyReportUserData> BotDailyReportUserData { get; set; }
        /// <summary>
        /// تنظیمات ربات گزارشات شعب
        /// </summary>
        public DbSet<DailyReportSettings> DailyReportSettings { get; set; }
        /// <summary>
        /// توکن کاربر
        /// </summary>
        public DbSet<UserToken> UserToken { get; set; }
        /// <summary>
        /// سفارشات داخلی
        /// </summary>
        public DbSet<InternalOrder.InternalOrder> InternalOrder { get; set; }
        /// <summary>
        ///جزییات سفارشات داخلی 
        /// </summary>
        public DbSet<InternalOrder.InternalOrderDetail> InternalOrderDetail { get; set; }
        /// <summary>
        /// سنگ هایی که هر مشتری برای سفارش خود انتخاب کرده است
        /// </summary>
        public DbSet<InternalOrderDetailStone> InternalOrderDetailStone { get; set; }
        /// <summary>
        /// چرم هایی که هر مشتری برای سفارش خود انتخاب کرده است
        /// </summary>
        public DbSet<InternalOrderDetailLeather> InternalOrderDetailLeather { get; set; }
        /// <summary>
        /// سوابق وضعیت سفارشات داخلی مربوط به هر مشتری 
        /// </summary>
        public DbSet<InternalOrderStatusLog> InternalOrderStatusLog { get; set; }
        /// <summary>
        /// پیامک
        /// </summary>
        public DbSet<Sms> Sms { get; set; }
        /// <summary>
        /// حساب بانکی
        /// </summary>
        public DbSet<BankAccount> BankAccount { get; set; }
        /// <summary>
        /// جزیات حساب های بانکی
        /// </summary>
        public DbSet<BankAccountDetail> BankAccountDetail { get; set; }
        /// <summary>
        /// سوالات سیستم مدیریت ارتباط با مشتری
        /// </summary>
        public DbSet<CrmQuestion> CrmQuestion { get; set; }
        /// <summary>
        /// مقدار سوالات سیستم مدیریت ارتباط با مشتری
        /// </summary>
        public DbSet<CrmQuestionValue> CrmQuestionValue { get; set; }
        /// <summary>
        /// دسته بندی سوالات
        /// </summary>
        public DbSet<CategoryQuestion> CategoryQuestion { get; set; }
        /// <summary>
        /// مشتری
        /// </summary>
        public DbSet<CrmCustomer> CrmCustomer { get; set; }
        /// <summary>
        /// جواب سوالات سیستم مدیریت ارتباط با مشتری
        /// </summary>
        public DbSet<CrmCustomerAnswer> CrmCustomerAnswer { get; set; }
        /// <summary>
        /// مدیریت عکاسی
        /// </summary>
        public DbSet<PhotographyManage> PhotographyManage { get; set; }
        /// <summary>
        /// موجودی طلا
        /// </summary>
        public DbSet<GoldBalance> GoldBalance { get; set; }
        /// <summary>
        /// طلای کارگاه
        /// </summary>
        public DbSet<WorkShopGold> WorkShopGold { get; set; }
        /// <summary>
        /// طلای شعب
        /// </summary>
        public DbSet<BranchGold> BranchGold { get; set; }
        /// <summary>
        /// مشتری وفادار
        /// </summary>
        public DbSet<CustomerFactor> CustomerFactor { get; set; }
        /// <summary>
        /// وفاداری مشتریان
        /// </summary>
        public DbSet<CustomerLoyality> CustomerLoyality { get; set; }
        /// <summary>
        /// محدوده آدرس مشتریان
        /// </summary>
        public DbSet<CustomerLocation> CustomerLocation { get; set; }
        /// <summary>
        /// دسته بندی اس ام اس
        /// </summary>
        public DbSet<SmsCategory> SmsCategory { get; set; }
        /// <summary>
        /// ایجاد پیام
        /// </summary>
        public DbSet<CreateMessage> CreateMessage { get; set; }
        /// <summary>
        /// متن اس ام اس
        /// </summary>
        public DbSet<SmsText> SmsText { get; set; }
        /// <summary>
        /// فاکتور شعب
        /// </summary>
        public DbSet<BranchFactor> BranchFactor { get; set; }
        /// <summary>
        /// فایل های مارکیز
        /// </summary>
        public DbSet<MarquisFile> MarquisFile { get; set; }
        /// <summary>
        /// جزئیات فایل های مارکیز
        /// </summary>
        public DbSet<MarquisFileDetail> MarquisFileDetail { get; set; }
        /// <summary>
        /// متن پیش فرض شرح وظایف پرسنل
        /// </summary>
        public DbSet<PersonJobDescriptionTemplate> PersonJobDescriptionTemplate { get; set; }
        /// <summary>
        /// ردیف درج اعضای گزارش روزانه
        /// </summary>
        public DbSet<CategoryInventoryReportMember> CategoryInventoryReportMember { get; set; }
        /// <summary>
        /// ثبت جزئیات موجودی اعم از وزن و مقدار
        /// </summary>
        public DbSet<InventoryDetail> InventoryDetail { get; set; }
        /// <summary>
        /// جواب نظر سنجی مشتریان
        /// </summary>
        public DbSet<SurveyCustomerAnswer> SurveyCustomerAnswer { get; set; }
        /// <summary>
        /// سوالات نظر سنجی
        /// </summary>
        public DbSet<SurveyQuestion> SurveyQuestion { get; set; }
        /// <summary>
        /// نظرسنجی
        /// </summary>
        public DbSet<CustomerSurvey> CustomerSurvey { get; set; }
        /// <summary>
        /// محصولات مصرفی
        /// </summary>
        public DbSet<UsableProduct> UsableProduct { get; set; }
        /// <summary>
        /// فایل های محصولات مصرفی
        /// </summary>
        public DbSet<UsableProductFile> UsableProductFile { get; set; }
        /// <summary>
        /// دسته بندی محصول مصرفی
        /// </summary>
        public DbSet<CategoryUsableProduct> CategoryUsableProduct { get; set; }
        /// <summary>
        /// سفارش محصولات مصرفی
        /// </summary>
        public DbSet<OrderUsableProduct> OrderUsableProduct { get; set; }
        /// <summary>
        /// جزئیات سفارش محصول مصرفی
        /// </summary>
        public DbSet<OrderUsableProductDetail> OrderUsableProductDetail { get; set; }
        /// <summary>
        /// سوابق سفارش محصول مصرفی
        /// </summary>
        public DbSet<OrderUsableProductLog> OrderUsableProductLog { get; set; }
        /// <summary>
        /// چاپخانه
        /// </summary>
        public DbSet<PrintingHouse> PrintingHouse { get; set; }
        /// <summary>
        /// ارسال فکس سفارش محصولات مصرفی
        /// </summary>
        public DbSet<FaxOrderUsableProduct> FaxOrderUsableProduct { get; set; }
        /// <summary>
        /// سبد خرید محصول چاپخانه یا محصول مصرفی
        /// </summary>
        public DbSet<UsableProductCart> UsableProductCart { get; set; }
        /// <summary>
        /// جدول محصولات شناسنامه دار
        /// </summary>
        public DbSet<LicencedProduct> LicencedProduct { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<BotGoldReportUserData> BotGoldReportUserData { get; set; }
        /// <summary>
        /// جدول سرشماری غذا
        /// </summary>
        public DbSet<FoodCensus> FoodCensus { get; set; }
        /// <summary>
        /// ثبت وضعیت دریافت غذا توسط کاربر لاگین شده در سیستم
        /// </summary>
        public DbSet<FoodRegistration> FoodRegistration { get; set; }
        /// <summary>
        /// تنظیمات مربوط به بخش غذا
        /// </summary>
        public DbSet<FoodSetting> FoodSetting { get; set; }
        /// <summary>
        /// تصاویر محصولات که قرار است در اینستاگرام منتشر شود
        /// </summary>
        public DbSet<InstagramPost> InstagramPost { get; set; }
        /// <summary>
        /// تیکت
        /// </summary>
        public DbSet<Ticket> Ticket { get; set; }
        /// <summary>
        /// پیام های تیکت
        /// </summary>
        public DbSet<TicketMessage> TicketMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<Department> Department { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<DepartmentPerson> DepartmentPerson { get; set; }
        /// <summary>
        /// کالکشن محصول
        /// </summary>
        public DbSet<ProductCollection> ProductCollection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<LoyalityCard> LoyalityCard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<LoyalityCardLog> LoyalityCardLog { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<CustomerCreditLog> CustomerCreditLog { get; set; }
        /// <summary>
        /// اعلان
        /// </summary>
        public DbSet<Notification> Notification { get; set; }
        /// <summary>
        /// موجودی شعب
        /// </summary>
        public DbSet<BranchInventory> BranchInventory { get; set; }
        /// <summary>
        /// آموزش ها
        /// </summary>
        public DbSet<Tutorial> Tutorial { get; set; }
        /// <summary>
        /// تگ کارگاه
        /// </summary>
        public DbSet<WorkshopTag> WorkshopTag { get; set; }
        /// <summary>
        /// کارت مشتری
        /// </summary>
        public DbSet<CustomerCard> CustomerCard { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<UserInfo> UserInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<CardTransaction> CardTransaction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbSet<CustomerFactorProductCode> CustomerFactorProductCode { get; set; }
        /// <summary>
        /// جدول فاکتور حقوقی یا شرکتی
        /// </summary>
        public DbSet<CompanyInvoice> CompanyInvoices { get; set; }
        /// <summary>
        /// جدول جزئیات فاکتور حقوقی یا شرکتی
        /// </summary>
        public DbSet<CompanyInvoiceDetail> CompanyInvoiceDetails { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sms>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Sms>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CardTransaction>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CardTransaction>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CardTransaction>().HasRequired(x => x.UserInfo).WithMany().HasForeignKey(x => x.UserInfoId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CardTransaction>().HasRequired(x => x.Person).WithMany().HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerFactorProductCode>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerFactorProductCode>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerFactorProductCode>().HasRequired(x => x.UserInfo).WithMany().HasForeignKey(x => x.UserInfoId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerFactorProductCode>().HasRequired(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).WillCascadeOnDelete(false);


            modelBuilder.Entity<WorkshopTag>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<WorkshopTag>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<WorkshopTag>().HasRequired(x => x.Workshop).WithMany().HasForeignKey(x => x.WorkshopId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerCard>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerCard>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerCard>().HasRequired(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);

            modelBuilder.Entity<UserInfo>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<UserInfo>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonJobDescriptionTemplate>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<PersonJobDescriptionTemplate>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SmsCategory>().HasMany(x => x.SmsTextList).WithRequired(x => x.SmsCategory).HasForeignKey(x => x.SmsCategoryId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBranchFactorList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBranchFactorList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateSmsCategoryList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifySmsCategoryList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateSmsTextList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifySmsTextList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerLoyality>().HasMany(x => x.CustomerFactorList).WithRequired(x => x.CustomerLoyality).HasForeignKey(x => x.CustomerLoyalityId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerLoyality>().HasOptional(x => x.CustomerLocation).WithMany().HasForeignKey(x => x.CustomerLocationId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateCompanyInvoiceList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCompanyInvoiceList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateInnovationMessageList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyInnovationMessageList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateCustomerLoyalityList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCustomerLoyalityList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateCustomerFactorList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCustomerFactorList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateBranchGoldList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBranchGoldList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateCategoryInventoryReportList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCategoryInventoryReportList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateInventoryDetailList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyInventoryDetailList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateWorkShopGoldList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyWorkShopGoldList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Branch>().HasMany(x => x.CustomerFactorList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Workshop>().HasMany(x => x.WorkShopGoldList).WithRequired(x => x.Workshop).HasForeignKey(x => x.WorkshopId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateGoldBalanceList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyGoldBalanceList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateUsableProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyUsableProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateUsableProductFileList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyUsableProductFileList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCategoryUsableProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCategoryUsableProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreatePrintingHouseList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyPrintingHouseList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCategoryUsableProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderUsableProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyOrderUsableProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderUsableProductDetailList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyOrderUsableProductDetailList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateFaxOrderUsableProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateUsableProductCartList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyUsableProductCartList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Branch>().HasMany(x => x.BranchGoldList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.OrderUsableProductList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>().HasMany(x => x.PhotographyManageList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreatePhotographyManageList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyPhotographyManageList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<SurveyQuestion>().HasMany(x => x.SurveyCustomerAnswerList).WithRequired(x => x.SurveyQuestion).HasForeignKey(x => x.SurveyQuestionId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SurveyQuestion>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SurveyQuestion>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerSurvey>().HasOptional(x => x.CustomerFactor).WithMany(x => x.CustomerSurveyList).HasForeignKey(x => x.CustomerFactorId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerSurvey>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SurveyCustomerAnswer>().HasRequired(x => x.CustomerSurvey).WithMany(x => x.SurveyCustomerAnswerList).HasForeignKey(x => x.CustomerSurveyId).WillCascadeOnDelete(false);
            modelBuilder.Entity<SurveyCustomerAnswer>().HasRequired(x => x.SurveyQuestion).WithMany().HasForeignKey(x => x.SurveyQuestionId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CrmQuestion>().HasMany(x => x.CrmQuestionValueList).WithRequired(x => x.CrmQuestion).HasForeignKey(x => x.CrmQuestionId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CategoryQuestion>().HasMany(x => x.CrmQuestionList).WithRequired(x => x.CategoryQuestion).HasForeignKey(x => x.CategoryQuestionId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CrmCustomer>().HasMany(x => x.CrmCustomerAnswerList).WithRequired(x => x.CrmCustomer).HasForeignKey(x => x.CrmCustomerId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCrmQuestionList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCrmQuestionList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCategoryQuestionList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCategoryQuestionList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CrmQuestionValueList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCrmCustomerAnswerList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCrmCustomerAnswerList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCrmCustomerList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCrmCustomerList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateFoodCensusList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyFoodCensusList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateFoodRegistrationList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyFoodRegistrationList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<FoodRegistration>().HasRequired(x => x.User).WithMany().HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<FoodRegistration>().HasRequired(x => x.FoodCensus).WithMany(x => x.FoodRegistrationList).HasForeignKey(x => x.FoodCensusId).WillCascadeOnDelete(false);

            modelBuilder.Entity<BankAccount>().HasMany(x => x.BankAccountDetailList).WithRequired(x => x.BankAccount).HasForeignKey(x => x.BankAccountId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BankAccount>().HasMany(x => x.BankAccountDetailList).WithRequired(x => x.BankAccount).HasForeignKey(x => x.BankAccountId).WillCascadeOnDelete(true);

            modelBuilder.Entity<User>().HasMany(x => x.BankAccountDetailList).WithOptional(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateBankAccountList).WithOptional(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBankAccountList).WithOptional(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateInternalOrderList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyInternalOrderList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.InternalOrderStatusLogList).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Person>().HasMany(x => x.InternalOrderList).WithOptional(x => x.CreatePerson).HasForeignKey(x => x.CreatePersonId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.InternalOrderDetailList).WithOptional(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            //modelBuilder.Entity<Stone>().HasMany(x => x.InternalOrderDetailStonesList).WithOptional(x => x.Stone).HasForeignKey(x => x.StoneId).WillCascadeOnDelete(false);
            //modelBuilder.Entity<Leather>().HasMany(x => x.InternalOrderDetailLeathersList).WithOptional(x => x.Leather).HasForeignKey(x => x.LeatherId).WillCascadeOnDelete(false);
            modelBuilder.Entity<InternalOrder.InternalOrder>().HasMany(x => x.InternalOrderStatusLogList).WithRequired(x => x.InternalOrder).HasForeignKey(x => x.InternalOrderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<InternalOrder.InternalOrder>().HasMany(x => x.InternalOrderDetailList).WithRequired(x => x.InternalOrder).HasForeignKey(x => x.InternalOrderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<InternalOrderDetail>().HasMany(x => x.InternalOrderDetailStonesList).WithRequired(x => x.InternalOrderDetail).HasForeignKey(x => x.InternalOrderDetailId).WillCascadeOnDelete(false);
            modelBuilder.Entity<InternalOrderDetail>().HasMany(x => x.InternalOrderDetailLeathersList).WithRequired(x => x.InternalOrderDetail).HasForeignKey(x => x.InternalOrderDetailId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.SentSmsList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.RoleList).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.TokenList).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.DailyReportLogList).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.BranchesPaymentsLogList).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.MessageList).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.UserTokenList).WithRequired(x => x.User).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateUserList).WithOptional(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyUserList).WithOptional(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateSettingsList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifySettingsList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateFoodSettingsList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyFoodSettingsList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateMessageList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyMessageList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateInquiryProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateInquiryProductReplyList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateLeatherList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyLeatherList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateStoneList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyStoneList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateLocationList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyLocationList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBranchList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBranchList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateWorkshopList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyWorkshopList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateRelatedProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyRelatedProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateSetProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifySetProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateProductFileList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyProductFileList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateSizeList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifySizeList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateSizeValueList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifySizeValueList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateShapeSizeList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyShapeSizeList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCartList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCartList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyOrderList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderDetailList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyOrderDetailList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderLogList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderDetailLogList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateWorkshopOrderList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyWorkshopOrderList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderDetailLogReasonList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyOrderDetailLogReasonList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreatePostItemList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyPostItemList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateFavouritesProductList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyFavouritesProductList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateStoneOutOfStockList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyStoneOutOfStockList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBankList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBankList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBranchCalendarList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBranchCalendarList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCurrencyList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCurrencyList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBranchNoteList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBranchNoteList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateGiftLogList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateOrderUsableProductLogList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBranchesPaymentsList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBranchesPaymentsList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyUserDataList).WithOptional(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreatePersonList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyPersonList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateSalaryList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifySalaryList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBranchesPaymentsSettingsList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBranchesPaymentsSettingsList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateDailyReportSettingsList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyDailyReportSettingsList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCustomerList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCustomerList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateInvoiceList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyInvoiceList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateInvoiceDetailList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyInvoiceDetailList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateCrmDiscountSettingList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyCrmDiscountSettingList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<User>().HasMany(x => x.CreateBotBroadcastList).WithRequired(x => x.User).HasForeignKey(x => x.SubmitedUser).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBotMessageList).WithRequired(x => x.User).HasForeignKey(x => x.SubmitedUser).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBotNewsList).WithRequired(x => x.User).HasForeignKey(x => x.SubmitedUser).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBotOrderLogList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateBotSettingsList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyBotSettingsList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.CreateInstagramPostList).WithRequired(x => x.CreateUser).HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(x => x.ModifyInstagramPostList).WithRequired(x => x.ModifyUser).HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Location>().HasMany(x => x.ChildList).WithOptional(x => x.Parent).HasForeignKey(x => x.ParentId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Location>().HasMany(x => x.PostItemList).WithRequired(x => x.City).HasForeignKey(x => x.CityId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Location>().HasMany(x => x.PostItemList).WithRequired(x => x.City).HasForeignKey(x => x.CityId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Branch>().HasMany(x => x.UserList).WithOptional(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.InternalOrderList).WithOptional(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Branch>().HasMany(x => x.BankList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.DailyReportList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.CartList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.OrderList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.MessageList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.FavouritesProductList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.BranchCalendarList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.BranchNoteList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.GiftShoppingList).WithOptional(x => x.BranchShopping).HasForeignKey(x => x.BranchIdShopping).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.GiftReceiverList).WithOptional(x => x.BranchReceiver).HasForeignKey(x => x.BranchIdReceiverCustomer).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.BranchesPaymentsList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.UserDataList).WithOptional(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.PersonList).WithOptional(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.InquiryProductList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Branch>().HasMany(x => x.InvoiceList).WithRequired(x => x.Branch).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Company>().HasMany(x => x.GiftShoppingList).WithOptional(x => x.CompanyShopping).HasForeignKey(x => x.CompanyIdShopping).WillCascadeOnDelete(false);

            modelBuilder.Entity<BranchCalendar>().HasMany(x => x.DailyReportList).WithRequired(x => x.BranchCalendar).HasForeignKey(x => x.BranchCalendarId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Bank>().HasMany(x => x.DailyReportBankList).WithRequired(x => x.Bank).HasForeignKey(x => x.BankId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Currency>().HasMany(x => x.DailyReportCurrencyList).WithRequired(x => x.Currency).HasForeignKey(x => x.CurrencyId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Workshop>().HasMany(x => x.ProductList).WithRequired(x => x.Workshop).HasForeignKey(x => x.WorkshopId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Workshop>().HasMany(x => x.ProductList2).WithOptional(x => x.Workshop2).HasForeignKey(x => x.WorkshopId2).WillCascadeOnDelete(false);
            modelBuilder.Entity<Workshop>().HasMany(x => x.UserList).WithOptional(x => x.Workshop).HasForeignKey(x => x.WorkshopId).WillCascadeOnDelete(false);
            modelBuilder.Entity<PrintingHouse>().HasMany(x => x.UserList).WithOptional(x => x.PrintingHouse).HasForeignKey(x => x.PrintingHouseId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>().HasMany(x => x.ProductFileList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.ProductStoneList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.ProductLeatherList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.CartList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.OrderDetailList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.FavouritesProductList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.SourceRelatedProduct).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.RelatedProduct).WithRequired(x => x.RelatedTo).HasForeignKey(x => x.RelatedToId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.SourceSetProduct).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.SetProduct).WithRequired(x => x.SetTo).HasForeignKey(x => x.SetToId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.ProductOuterWerkList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.BotOrderList).WithRequired(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasMany(x => x.InquiryProductList).WithOptional(x => x.Product).HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasOptional(x => x.WorkshopTag).WithMany().HasForeignKey(x => x.WorkshopTagId).WillCascadeOnDelete(false);

            modelBuilder.Entity<LicencedProduct>().HasOptional(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Stone>().HasMany(x => x.ProductStoneList).WithRequired(x => x.Stone).HasForeignKey(x => x.StoneId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Stone>().HasMany(x => x.CartProductStoneList).WithOptional(x => x.Stone).HasForeignKey(x => x.StoneId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Stone>().HasMany(x => x.OrderDetailStoneList).WithOptional(x => x.Stone).HasForeignKey(x => x.StoneId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Stone>().HasMany(x => x.StoneOutOfStockList).WithRequired(x => x.Stone).HasForeignKey(x => x.StoneId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Leather>().HasMany(x => x.ProductLeatherList).WithRequired(x => x.Leather).HasForeignKey(x => x.LeatherId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Leather>().HasMany(x => x.CartProductLeatherList).WithOptional(x => x.Leather).HasForeignKey(x => x.LeatherId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Leather>().HasMany(x => x.OrderDetailLeatherList).WithOptional(x => x.Leather).HasForeignKey(x => x.LeatherId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Size>().HasMany(x => x.SizeValueList).WithRequired(x => x.Size).HasForeignKey(x => x.SizeId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Size>().HasMany(x => x.ProductList).WithOptional(x => x.Size).HasForeignKey(x => x.SizeId).WillCascadeOnDelete(false);

            modelBuilder.Entity<SizeValue>().HasMany(x => x.ProductList).WithOptional(x => x.NormalSizeValue).HasForeignKey(x => x.NormalSizeValueId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Cart>().HasMany(x => x.CartProductStoneList).WithRequired(x => x.Cart).HasForeignKey(x => x.CartId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Cart>().HasMany(x => x.CartProductLeatherList).WithRequired(x => x.Cart).HasForeignKey(x => x.CartId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Order.Order>().HasMany(x => x.WorkshopOrderList).WithRequired(x => x.Order).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Order.Order>().HasMany(x => x.OrderLogList).WithRequired(x => x.Order).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Order.Order>().HasMany(x => x.OrderDetailList).WithRequired(x => x.Order).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderDetail>().HasMany(x => x.OrderDetailLogList).WithRequired(x => x.OrderDetail).HasForeignKey(x => x.OrderDetailId).WillCascadeOnDelete(false);
            modelBuilder.Entity<OrderDetail>().HasMany(x => x.OrderDetailStoneList).WithRequired(x => x.OrderDetail).HasForeignKey(x => x.OrderDetailId).WillCascadeOnDelete(false);
            modelBuilder.Entity<OrderDetail>().HasMany(x => x.OrderDetailLeatherList).WithRequired(x => x.OrderDetail).HasForeignKey(x => x.OrderDetailId).WillCascadeOnDelete(false);
            modelBuilder.Entity<OrderDetail>().HasMany(x => x.RelatedOrderDetailList).WithOptional(x => x.RelatedOrderDetail).HasForeignKey(x => x.RelatedOrderDetailId).WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderDetailLogReason>().HasMany(x => x.OrderDetailLogList).WithOptional(x => x.OrderDetailLogReason).HasForeignKey(x => x.OrderDetailLogReasonId).WillCascadeOnDelete(false);

            modelBuilder.Entity<WorkshopOrder>().HasMany(x => x.OrderDetailList).WithOptional(x => x.WorkshopOrder).HasForeignKey(x => x.WorkshopOrderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<WorkshopOrder>().HasMany(x => x.OrderDetailList2).WithOptional(x => x.WorkshopOrder2).HasForeignKey(x => x.WorkshopOrderId2).WillCascadeOnDelete(false);

            modelBuilder.Entity<DailyReport>().HasMany(x => x.DailyReportBankList).WithRequired(x => x.DailyReport).HasForeignKey(x => x.DailyReportId).WillCascadeOnDelete(false);
            modelBuilder.Entity<DailyReport>().HasMany(x => x.DailyReportCurrencyList).WithRequired(x => x.DailyReport).HasForeignKey(x => x.DailyReportId).WillCascadeOnDelete(false);
            modelBuilder.Entity<DailyReport>().HasMany(x => x.DailyReportLogList).WithRequired(x => x.DailyReport).HasForeignKey(x => x.DailyReportId).WillCascadeOnDelete(false);

            modelBuilder.Entity<BotGoldReportUserData>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ShapeSize>().HasMany(x => x.ProductStoneList).WithOptional(x => x.ShapeSize).HasForeignKey(x => x.ShapeSizeId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ShapeSize>().HasMany(x => x.StoneOutOfStockList).WithRequired(x => x.ShapeSize).HasForeignKey(x => x.ShapeSizeId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>().HasMany(x => x.SalaryList).WithRequired(x => x.Person).HasForeignKey(x => x.PersonId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Person>().HasMany(x => x.PersonFileList).WithRequired(x => x.Person).HasForeignKey(x => x.PersonId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Person>().HasOptional(x => x.Job).WithMany().HasForeignKey(x => x.JobId).WillCascadeOnDelete(false);

            modelBuilder.Entity<BotMessage>().HasMany(x => x.MessageList).WithRequired(x => x.ReplayMessage).HasForeignKey(x => x.ReplayMessageId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Replay>().HasMany(x => x.MessageList).WithRequired(x => x.Replay).HasForeignKey(x => x.ReplayId).WillCascadeOnDelete(false);

            modelBuilder.Entity<BotOrder>().HasMany(x => x.BotOrderStoneList).WithRequired(x => x.BotOrder).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BotOrder>().HasMany(x => x.BotOrderLeatherList).WithRequired(x => x.BotOrder).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BotOrder>().HasMany(x => x.BotOrderLogList).WithRequired(x => x.BotOrder).HasForeignKey(x => x.OrderId).WillCascadeOnDelete(false);

            modelBuilder.Entity<BotUserData>().HasMany(x => x.BotOrderList).WithRequired(x => x.BotUserData).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<UserData>().HasMany(x => x.SendMessageList).WithRequired(x => x.UserData).HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BranchesPayments.BranchesPayments>().HasMany(x => x.SendMessageList).WithRequired(x => x.BranchesPayments).HasForeignKey(x => x.BranchesPaymentsId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BranchesPayments.BranchesPayments>().HasMany(x => x.BranchesPaymentsDetails).WithRequired(x => x.BranchesPayments).HasForeignKey(x => x.BranchesPaymentsId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BranchesPayments.BranchesPayments>().HasMany(x => x.BranchesPaymentsLogList).WithRequired(x => x.BranchesPayments).HasForeignKey(x => x.BranchPaymentsId).WillCascadeOnDelete(false);


            modelBuilder.Entity<Customer>().HasMany(x => x.InvoiceList).WithRequired(x => x.Customer).HasForeignKey(x => x.CustomerId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CrmInvoice>().HasMany(x => x.InvoiceDetailList).WithRequired(x => x.Invoice).HasForeignKey(x => x.InvoiceId).WillCascadeOnDelete(false);

            modelBuilder.Entity<MarquisFile>().HasRequired(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<MarquisFile>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<MarquisFile>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<MarquisFileDetail>().HasRequired(x => x.MarquisFile).WithMany(x => x.MarquisFileDetailList).HasForeignKey(x => x.MarquisFileId).WillCascadeOnDelete(false);

            modelBuilder.Entity<UsableProduct>().HasMany(x => x.OrderUsableProductDetailList).WithRequired(x => x.UsableProduct).HasForeignKey(x => x.UsableProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<UsableProduct>().HasMany(x => x.UsableProductFileList).WithRequired(x => x.UsableProduct).HasForeignKey(x => x.UsableProductId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CategoryUsableProduct>().HasMany(x => x.UsableProductList).WithRequired(x => x.CategoryUsableProduct).HasForeignKey(x => x.CategoryUsableProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CategoryUsableProduct>().HasOptional(x => x.Parent).WithMany(x => x.ChildList).HasForeignKey(x => x.ParentId).WillCascadeOnDelete(false);
            modelBuilder.Entity<PrintingHouse>().HasMany(x => x.UsableProductList).WithRequired(x => x.PrintingHouse).HasForeignKey(x => x.PrintingHouseId).WillCascadeOnDelete(false);
            modelBuilder.Entity<OrderUsableProduct>().HasMany(x => x.OrderUsableProductDetailList).WithRequired(x => x.OrderUsableProduct).HasForeignKey(x => x.OrderUsableProductId).WillCascadeOnDelete(false);
            modelBuilder.Entity<UsableProduct>().HasMany(x => x.UsableProductCartList).WithRequired(x => x.UsableProduct).HasForeignKey(x => x.UsableProductId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Ticket>().HasOptional(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Ticket>().HasOptional(x => x.FromUser).WithMany().HasForeignKey(x => x.FromUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Ticket>().HasOptional(x => x.ToUser).WithMany().HasForeignKey(x => x.ToUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Ticket>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Ticket>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Department>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentPerson>().HasRequired(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).WillCascadeOnDelete(false);
            modelBuilder.Entity<DepartmentPerson>().HasRequired(x => x.User).WithMany().HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<DepartmentPerson>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<DepartmentPerson>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<TicketMessage>().HasRequired(x => x.Ticket).WithMany(x => x.TicketMessagesList).HasForeignKey(x => x.TicketId).WillCascadeOnDelete(false);
            modelBuilder.Entity<TicketMessage>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<TicketMessage>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCollection>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ProductCollection>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Product>().HasOptional(x => x.ProductCollection).WithMany().HasForeignKey(x => x.ProductCollectionId).WillCascadeOnDelete(false);


            modelBuilder.Entity<LoyalityCard>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<LoyalityCard>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<LoyalityCardLog>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<LoyalityCardLog>().HasRequired(x => x.LoyalityCard).WithMany(x => x.LogList).HasForeignKey(x => x.LoyalityCardId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CustomerLoyality>().HasOptional(x => x.LoyalityCard).WithMany(x => x.CustomerList).HasForeignKey(x => x.LoyalityCardId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomerCreditLog>().HasRequired(x => x.CustomerLoyality).WithMany().HasForeignKey(x => x.CustomerId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Notification>().HasRequired(x => x.Ticket).WithMany(x => x.NotificationList).HasForeignKey(x => x.TicketId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Notification>().HasRequired(x => x.User).WithMany().HasForeignKey(x => x.UserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Notification>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BranchInventory>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BranchInventory>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<BranchInventory>().HasRequired(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Tutorial>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Tutorial>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);

            modelBuilder.Entity<CompanyInvoice>().HasRequired(x => x.CreateUser).WithMany().HasForeignKey(x => x.CreateUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CompanyInvoice>().HasRequired(x => x.ModifyUser).WithMany().HasForeignKey(x => x.ModifyUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CompanyInvoice>().HasRequired(x => x.Branch).WithMany(x => x.CompanyInvoiceList).HasForeignKey(x => x.BranchId).WillCascadeOnDelete(false);
            modelBuilder.Entity<CompanyInvoice>().HasMany(x => x.CompanyInvoiceDetailList).WithRequired(x => x.CompanyInvoice).HasForeignKey(x => x.CompanyInvoiceId).WillCascadeOnDelete(false);
        }
    }
}
