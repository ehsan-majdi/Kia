using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.DailyReportFinancial
{
    /// <summary>
    /// ارز های گزارش روزانه
    /// </summary>
    [Table(name: "DailyReportCurrency", Schema = "drf")]
    public class DailyReportCurrency
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف گزارش روزانه
        /// </summary>
        public int DailyReportId { get; set; }
        /// <summary>
        /// ردیف ارز
        /// </summary>
        public int CurrencyId { get; set; }
        /// <summary>
        /// مقدار
        /// </summary>
        public decimal Value { get; set; }
        /// <summary>
        /// ارزش ریالی
        /// </summary>
        public long RialValue { get; set; }
        /// <summary>
        /// ارزش ریالی وارد شده
        /// </summary>
        public long RialEntry { get; set; }
        /// <summary>
        /// ارزش ریالی خارج شده
        /// </summary>
        public long RialExit { get; set; }

        /// <summary>
        /// گزارش روزانه
        /// </summary>
        public virtual DailyReport DailyReport { get; set; }
        /// <summary>
        /// ارز
        /// </summary>
        public virtual Currency Currency { get; set; }
    }
}
