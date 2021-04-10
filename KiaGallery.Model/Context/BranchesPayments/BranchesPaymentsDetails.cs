

using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.BranchesPayments
{
    /// <summary>
    /// جزئیات پرداخت شعب
    /// </summary>
    public class BranchesPaymentsDetails
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ردیف پرداخت شعب
        /// </summary>
        public int BranchesPaymentsId { get; set; }
        /// <summary>
        /// اجرت طلا
        /// </summary>
        public long? GoldWage { get; set; }
        /// <summary>
        /// وزن طلا
        /// </summary>
        public float? GoldWeights { get; set; }
        /// <summary>
        /// مبلغ 
        /// </summary>
        public long? Amount { get; set; }
        /// <summary>
        /// تعداد محصول متفرقه (چرم ، گیفت)
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        [MaxLength(200)]
        public string Title { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        [MaxLength(200)]
        public string Code { get; set; }
        /// <summary>
        ///  پرداخت شعب
        /// </summary>
        public BranchesPayments BranchesPayments { get; set; }
    }
}
