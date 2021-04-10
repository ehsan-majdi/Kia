
namespace KiaGallery.Model.Context.BranchesPayments
{
    /// <summary>
    /// محصولات پرداخت شعب
    /// </summary>
    public class BranchesPaymentsProduct
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
        ///  پرداخت شعب
        /// </summary>
        public BranchesPayments BranchesPayments { get; set; }
    }
}
