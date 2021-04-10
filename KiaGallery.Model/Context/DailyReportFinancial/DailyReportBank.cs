using System.ComponentModel.DataAnnotations.Schema;

namespace KiaGallery.Model.Context.DailyReportFinancial
{
    /// <summary>
    /// بانک گزارش روزانه
    /// </summary>
    [Table(name: "DailyReportBank", Schema = "drf")]
    public class DailyReportBank
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف گزارش
        /// </summary>
        public int DailyReportId { get; set; }
        /// <summary>
        /// ردیف بانک
        /// </summary>
        public int BankId { get; set; }
        /// <summary>
        /// مبلغ ورودی
        /// </summary>
        public long Entry { get; set; }
        /// <summary>
        /// مبلغ خروجی
        /// </summary>
        public long Exit { get; set; }

        /// <summary>
        /// گزارش
        /// </summary>
        public virtual DailyReport DailyReport { get; set; }
        /// <summary>
        /// بانک
        /// </summary>
        public virtual Bank Bank { get; set; }
    }
}
