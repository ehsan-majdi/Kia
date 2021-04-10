namespace KiaGallery.Crm.App.Model
{
    /// <summary>
    /// کلاس نمایشی مشتری
    /// </summary>
    public class CustomerViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// بارکد کارت اسکن شده
        /// </summary>
        public string barcode { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string lastName { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string nationalityCode { get; set; }
        /// <summary>
        /// تلفن همراه
        /// </summary>
        public string mobileNo { get; set; }
        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public string birthDate { get; set; }
        /// <summary>
        /// تاریخ ازدواج
        /// </summary>
        public string weddingDate { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public int sex { get; set; }
        /// <summary>
        /// عنوان جنسیت
        /// </summary>
        public string sexTitle { get; set; }
        /// <summary>
        /// ترازمالی
        /// </summary>
        public int balance { get; set; }
    }
}
