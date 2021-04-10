
using KiaGallery.Model;

namespace KiaGallery.Web.Models
{
    /// <summary>
    /// مالی شعب
    /// </summary>
    public class FinancialBranch
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// ترتیب
        /// </summary>
        public int order { get; set; }
        /// <summary>
        /// نام شعبه
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// بدهی حساب طلا
        /// </summary>
        public float goldDebt { get; set; }
        /// <summary>
        /// بدهی حساب ریال
        /// </summary>
        public long rialDebt { get; set; }
        /// <summary>
        /// نوع شعبه
        /// </summary>
        public BranchType branchType { get; set; }
        /// <summary>
        /// اعتبار حساب طلا
        /// </summary>
        public float goldCredit { get; set; }
        /// <summary>
        /// وزن طلا سبد خرید
        /// </summary>
        public double? goldWeightCart { get; set; }
        public float? inPreprationDebt { get; set; }
        public float? SumDebt { get; set; }

    }
}